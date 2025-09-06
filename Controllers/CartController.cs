using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UShop.Data;
using UShop.Models;

namespace UShop.Controllers
{
	[Authorize] // only logged-in users can use the cart
	public class CartController : Controller
	{
		private readonly UShopDBContext _context;

		public CartController(UShopDBContext context)
		{
			_context = context;
		}

		// GET: Cart
		public async Task<IActionResult> Index()
		{
			var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			var cart = await _context.Carts
				 .Include(c => c.Items)
					  .ThenInclude(i => i.Product)
				 .FirstOrDefaultAsync(c => c.CustomerId == customerId);

			if (cart == null)
			{
				cart = new Cart { CustomerId = customerId };
				_context.Carts.Add(cart);
				await _context.SaveChangesAsync();
			}

			return View(cart);
		}

		// POST: Cart/Add
		[HttpPost]
		public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
		{
			var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var cart = await _context.Carts
				 .Include(c => c.Items)
				 .FirstOrDefaultAsync(c => c.CustomerId == customerId);

			if (cart == null)
			{
				cart = new Cart { CustomerId = customerId };
				_context.Carts.Add(cart);
				await _context.SaveChangesAsync();
			}

			var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

			if (existingItem != null)
			{
				existingItem.Quantity += quantity;
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
			return RedirectToAction(nameof(Index));
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
			var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var cart = await _context.Carts
				 .Include(c => c.Items).ThenInclude(i => i.Product)
				 .Include(c => c.Customer).ThenInclude(c => c.CreditCard)
				 .FirstOrDefaultAsync(c => c.CustomerId == customerId);

			if (cart == null || !cart.Items.Any())
			{
				TempData["Error"] = "Your cart is empty!";
				return RedirectToAction(nameof(Index));
			}

			var order = new Order
			{
				CustomerId = customerId,
				Customer = cart.Customer,
				Items = cart.Items.ToList(),
				OrderDate = DateTime.Now
			};

			return View(order);
		}

		// POST: Cart/Checkout
		[HttpPost]
		public async Task<IActionResult> Checkout(Order order)
		{

			// Get the logged-in user ID
			var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

			var cart = await _context.Carts
				 .Include(c => c.Items).ThenInclude(i => i.Product)
				 .Include(c => c.Customer).ThenInclude(c => c.CreditCard)
				 .FirstOrDefaultAsync(c => c.CustomerId == customerId);

			if (cart == null || !cart.Items.Any())
			{
				TempData["Error"] = "Your cart is empty!";
				return RedirectToAction(nameof(Index));
			}

			// Ensure PaymentMethod has a valid value
			if (order.PaymentMethod == 0)
				order.PaymentMethod = PaymentMethod.CashOnDelivery;

			// Handle Credit Card if chosen
			if (order.PaymentMethod == PaymentMethod.CreditCard && order.Customer?.CreditCard != null)
			{
				var customer = cart.Customer;
				if (customer != null)
				{
					if (customer.CreditCard == null)
					{
						customer.CreditCard = order.Customer.CreditCard;
					}
					else
					{
						// Update existing card
						customer.CreditCard.CardNumber = order.Customer.CreditCard.CardNumber;
						customer.CreditCard.CardholderName = order.Customer.CreditCard.CardholderName;
						customer.CreditCard.ExpiryMonth = order.Customer.CreditCard.ExpiryMonth;
						customer.CreditCard.ExpiryYear = order.Customer.CreditCard.ExpiryYear;
						customer.CreditCard.CVV = order.Customer.CreditCard.CVV;
					}
					await _context.SaveChangesAsync();
				}
			}

			// Create the order
			var newOrder = new Order
			{
				CustomerId = customerId,
				OrderDate = DateTime.Now,
				Status = OrderStatus.Pending,
				PaymentMethod = order.PaymentMethod,
				Items = cart.Items.Select(i => new Item
				{
					ProductId = i.ProductId,
					Quantity = i.Quantity,
					UnitPrice = i.UnitPrice
				}).ToList()
			};

			_context.Orders.Add(newOrder);

			// Clear cart
			_context.Items.RemoveRange(cart.Items);
			await _context.SaveChangesAsync();

			TempData["Success"] = "Order placed successfully!";
			return RedirectToAction("Details", "Orders", new { id = newOrder.Id });
		}
	}
}
