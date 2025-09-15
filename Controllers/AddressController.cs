using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UShop.Data;
using UShop.Models;

namespace UShop.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly UShopDBContext _context;

        public AddressController(UserManager<User> userManager, UShopDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // POST: /Address/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(string Street, string City, string Country)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            if (currentUser.UserType != UserType.Customer || !currentUser.CustomerId.HasValue)
            {
                TempData["Error"] = "Only customers can update addresses.";
                return RedirectToAction("Profile", "Account");
            }

            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == currentUser.CustomerId.Value);

            if (customer == null)
            {
                TempData["Error"] = "Customer not found.";
                return RedirectToAction("Profile", "Account");
            }

            // create or update address
            if (customer.Address == null)
            {
                customer.Address = new Address
                {
                    Street = Street,
                    City = City,
                    Country = Country
                };
                _context.Addresses.Add(customer.Address);
            }
            else
            {
                customer.Address.Street = Street;
                customer.Address.City = City;
                customer.Address.Country = Country;
                _context.Update(customer.Address);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Address saved successfully!";
            return RedirectToAction("Profile", "Account");
        }

        // Optional: Delete Address
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.CustomerId == null)
                return RedirectToAction("Profile", "Account");

            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == currentUser.CustomerId);

            if (customer?.Address != null)
            {
                _context.Addresses.Remove(customer.Address);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Address removed.";
            }

            return RedirectToAction("Profile", "Account");
        }
    }
}
