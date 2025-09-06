using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UShop.Data;
using UShop.Models;

namespace UShop.Controllers
{
	public class CheckoutController : Controller
    {
        private readonly UShopDBContext _context;
        private readonly UserManager<User> _userManager;

        public CheckoutController(UShopDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            Customer? customer = null;

            if (user != null)
            {
                customer = await _context.Customers
                    .Include(c => c.CreditCard)
                    .FirstOrDefaultAsync(c => c.Email == user.Email);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        FullName = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        Address = string.Empty
                    };

                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Guest checkout
                customer = new Customer
                {
                    FullName = "Guest",
                    Email = string.Empty,
                    Address = string.Empty
                };
            }

            var order = new Order
            {
                Customer = customer, // ✅ never null
                Items = new List<Item>(),
                PaymentMethod = PaymentMethod.CashOnDelivery
            };

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Place(Order order, CreditCard creditCard, bool? UseSavedCard)
        {
            var user = await _userManager.GetUserAsync(User);

            Customer? customer = null;

            if (user != null)
            {
                customer = await _context.Customers
                    .Include(c => c.CreditCard)
                    .FirstOrDefaultAsync(c => c.Email == user.Email);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        FullName = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        Address = string.Empty
                    };

                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Guest checkout
                customer = new Customer
                {
                    FullName = order.Customer?.FullName ?? "Guest",
                    Email = order.Customer?.Email ?? string.Empty,
                    Address = order.Customer?.Address ?? string.Empty
                };
            }

            // Validate customer profile
            if (string.IsNullOrEmpty(customer.FullName) ||
                string.IsNullOrEmpty(customer.Email) ||
                string.IsNullOrEmpty(customer.Address))
            {
                TempData["IncompleteProfile"] = "Please complete your profile information before checkout.";
                return RedirectToAction("Edit", "Customer", new { id = customer.Id });
            }

            // Payment
            if (order.PaymentMethod == PaymentMethod.CreditCard)
            {
                if (UseSavedCard == true && customer.CreditCard != null)
                {
                    order.Customer.CreditCard = customer.CreditCard;
                }
                else
                {
                    if (!TryValidateModel(creditCard))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid credit card details.");
                        return View("Index", order);
                    }

                    creditCard.CustomerId = customer.Id;
                    _context.CreditCards.Add(creditCard);
                    customer.CreditCard = creditCard;
                }
            }

            order.CustomerId = customer.Id;
            order.Customer = customer;
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
