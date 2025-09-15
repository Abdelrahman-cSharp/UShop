using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UShop.Data;
using UShop.Models;

namespace UShop.Controllers
{
    [Authorize] // only logged-in users can use the cart
    public class CartController : Controller
    {
        private readonly UShopDBContext _context;
        private readonly UserManager<User> _userManager;

        public CartController(UShopDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // helper to get the current logged-in customer's id
        private async Task<int?> GetCustomerIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.CustomerId;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null)
                return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId.Value };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        // POST: Cart/Add
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null)
                return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId.Value };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                if (existingItem.Product == null)
                    return BadRequest("Product not found.");

                // Check if adding the quantity exceeds stock
                if (existingItem.Quantity + quantity > existingItem.Product.StockQuantity)
                {
                    existingItem.Quantity = existingItem.Product.StockQuantity;
                }
                else
                {
                    existingItem.Quantity += quantity;
                }
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                if (product != null)
                {
                    cart.Items.Add(new Item
                    {
                        ProductId = product.Id,
                        Quantity = quantity,
                        UnitPrice = product.Price
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // POST: Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null)
                return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items).ThenInclude(i => i.Product)
                .Include(c => c.Customer).ThenInclude(c => c.CreditCards)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }

            var order = new Order
            {
                CustomerId = customerId.Value,
                Customer = cart.Customer,
                Items = cart.Items.ToList(),
                OrderDate = DateTime.Now
            };

            return View(order);
        }
        
        [HttpGet]
        public async Task<IActionResult> BuyNow(int productId, int quantity = 1)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null)
                return Unauthorized();

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            var customer = await _context.Customers
                .Include(c => c.CreditCards)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            var item = new Item
            {
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = product.Price,
                Product = product
            };

            var order = new Order
            {
                CustomerId = customerId.Value,
                Customer = customer,
                Items = new List<Item> { item },
                OrderDate = DateTime.Now
            };

            return View(order);
        }

    }
}
