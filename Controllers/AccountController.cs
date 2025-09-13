using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UShop.Data;
using UShop.Models;
using UShop.ViewModels;


namespace UshopFront.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UShopDBContext _context;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 UShopDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ------------------------------
        // LOGIN
        // ------------------------------
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["Success"] = "Login successful!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login credentials.");
            return View(model);
        }

        // ------------------------------
        // REGISTER
        // ------------------------------
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                UserType = model.UserType
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.UserType.ToString());

                switch (model.UserType)
                {
                    case UserType.Admin:
                        var admin = new Admin
                        {
                            FullName = model.FullName,
                            Email = model.Email,
                            Description = "New admin account"
                        };
                        _context.Admins.Add(admin);
                        await _context.SaveChangesAsync();
                        user.AdminId = admin.Id;
                        break;

                    case UserType.Customer:
                        var customer = new Customer
                        {
                            FullName = model.FullName,
                            Email = model.Email
                        };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();
                        user.CustomerId = customer.Id;
                        break;

                    case UserType.Seller:
                        var seller = new Seller
                        {
                            FullName = model.FullName,
                            Email = model.Email
                        };
                        _context.Sellers.Add(seller);
                        await _context.SaveChangesAsync();
                        user.SellerId = seller.Id;
                        break;
                }

                await _userManager.UpdateAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = "Registration successful!";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ------------------------------
        // PROFILE
        // ------------------------------
        public async Task<IActionResult> Profile(string? userId = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login");

            var targetUserId = userId ?? currentUser.Id;

            if (targetUserId != currentUser.Id && !User.IsInRole("Admin"))
                return Forbid();

            var viewModel = await GetUserProfileViewModelAsync(targetUserId, currentUser);
            if (viewModel is null) return NotFound();

            return View(viewModel);
        }

        // ------------------------------
        // CREDIT CARD MANAGEMENT
        // ------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCard(string CardNumber, string CardholderName, int ExpiryMonth, int ExpiryYear, string CVV)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["Error"] = "You must be logged in to add a credit card.";
                return RedirectToAction("Login");
            }

            CardNumber = CardNumber?.Replace(" ", "") ?? "";
            if (CardNumber.Length < 13 || CardNumber.Length > 19)
            {
                TempData["Error"] = "Invalid card number.";
                return RedirectToAction("Profile");
            }

            if (ExpiryMonth < 1 || ExpiryMonth > 12 ||
                ExpiryYear < DateTime.Now.Year ||
                (ExpiryYear == DateTime.Now.Year && ExpiryMonth < DateTime.Now.Month))
            {
                TempData["Error"] = "Card has expired.";
                return RedirectToAction("Profile");
            }

            if (string.IsNullOrWhiteSpace(CVV) || CVV.Length is < 3 or > 4)
            {
                TempData["Error"] = "Invalid CVV.";
                return RedirectToAction("Profile");
            }

            try
            {
                var card = new CreditCard
                {
                    CardNumber = CardNumber,
                    CardholderName = CardholderName,
                    ExpiryMonth = ExpiryMonth,
                    ExpiryYear = ExpiryYear,
                    CVV = CVV
                };

                switch (currentUser.UserType)
                {
                    case UserType.Customer:
                        if (currentUser.CustomerId.HasValue)
                            card.CustomerId = currentUser.CustomerId.Value;
                        else
                        {
                            TempData["Error"] = "Customer profile not found.";
                            return RedirectToAction("Profile");
                        }
                        break;

                    case UserType.Seller:
                        if (currentUser.SellerId.HasValue)
                            card.SellerId = currentUser.SellerId.Value;
                        else
                        {
                            TempData["Error"] = "Seller profile not found.";
                            return RedirectToAction("Profile");
                        }
                        break;

                    default:
                        TempData["Error"] = "Only customers and sellers can add credit cards.";
                        return RedirectToAction("Profile");
                }

                _context.CreditCards.Add(card);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Credit card added successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding credit card: " + ex.Message;
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login");

            var card = await _context.CreditCards.FindAsync(id);
            if (card == null)
            {
                TempData["Error"] = "Card not found.";
                return RedirectToAction("Profile");
            }

            bool canDelete = false;
            if (currentUser.UserType == UserType.Customer && card.CustomerId == currentUser.CustomerId)
                canDelete = true;
            else if (currentUser.UserType == UserType.Seller && card.SellerId == currentUser.SellerId)
                canDelete = true;
            else if (User.IsInRole("Admin"))
                canDelete = true;

            if (!canDelete)
            {
                TempData["Error"] = "You don't have permission to delete this card.";
                return RedirectToAction("Profile");
            }

            _context.CreditCards.Remove(card);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Card deleted successfully!";

            return RedirectToAction("Profile");
        }

        // ------------------------------
        // LOGOUT
        // ------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Info"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

        // ------------------------------
        // HELPERS
        // ------------------------------
        private async Task<UserProfileViewModel?> GetUserProfileViewModelAsync(string userId, User currentUser)
        {
            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (targetUser == null) return null;

            var viewModel = new UserProfileViewModel
            {
                UserId = userId,
                UserType = targetUser.UserType,
                Email = targetUser.Email ?? string.Empty,
                IsOwnProfile = currentUser.Id == userId,
                CanEdit = currentUser.Id == userId || User.IsInRole("Admin")
            };

            switch (targetUser.UserType)
            {
                case UserType.Admin:
                    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.User != null && a.User.Id == userId);
                    if (admin != null)
                    {
                        viewModel.Id = admin.Id;
                        viewModel.FullName = admin.FullName;
                        viewModel.Description = admin.Description;
                    }
                    break;

                case UserType.Customer:
                    var customer = await _context.Customers
                        .Include(c => c.Address)
                        .Include(c => c.Orders)
                        .FirstOrDefaultAsync(c => c.User != null && c.User.Id == userId);

                    if (customer != null)
                    {
                        viewModel.Id = customer.Id;
                        viewModel.FullName = customer.FullName;
                        viewModel.PhoneNumber = customer.PhoneNumber;
                        viewModel.Address = customer.Address;

                        viewModel.CreditCards = await _context.CreditCards
                            .Where(cc => cc.CustomerId == customer.Id)
                            .ToListAsync();

                        viewModel.Orders = customer.Orders?.OrderByDescending(o => o.OrderDate).Take(5).ToList();
                        viewModel.OrdersCount = customer.Orders?.Count() ?? 0;
                        viewModel.TotalSpent = customer.Orders?.Sum(o => o.TotalAmount) ?? 0;
                    }
                    break;

                case UserType.Seller:
                    var seller = await _context.Sellers
                        .Include(s => s.Products)
                        .FirstOrDefaultAsync(s => s.User != null && s.User.Id == userId);

                    if (seller != null)
                    {
                        viewModel.Id = seller.Id;
                        viewModel.FullName = seller.FullName;
                        viewModel.PhoneNumber = seller.PhoneNumber;
                        viewModel.ImageUrl = seller.ImageUrl;
                        viewModel.Products = seller.Products?.Take(10).ToList();

                        viewModel.CreditCards = await _context.CreditCards
                            .Where(cc => cc.SellerId == seller.Id)
                            .ToListAsync();

                        viewModel.ProductsCount = seller.Products?.Count() ?? 0;
                    }
                    break;
            }

            return viewModel;
        }
    }
}
