using Microsoft.AspNetCore.Mvc;
using UShop.Models;

namespace UShop.Controllers
{
	public class CheckoutController : Controller
	{
		private static readonly List<Order> Orders = new();

		[HttpGet]
		public IActionResult Index()
		{
			var order = new Order
			{
				Customer = new Customer(),
				Items = new List<Item>
				{

				}
			};
			return View(order);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Place(Order order, CreditCard creditCard)
		{

			if (order.Items == null || !order.Items.Any())
			{
				ModelState.AddModelError(string.Empty, "Please add items to your order before checkout.");
				return View("Index", order);
			}
			if (string.IsNullOrEmpty(order.Customer?.FullName) || string.IsNullOrEmpty(order.Customer?.Email))
			{
				ModelState.AddModelError("Customer.FullName", "Full Name and Email are required.");
				return View("Index", order);
			}

			order.OrderDate = DateTime.UtcNow;
			order.Status = OrderStatus.Pending;
			order.Id = Orders.Any() ? Orders.Max(o => o.Id) + 1 : 1;

			// Handle credit card payment details if selected
			if (order.PaymentMethod == PaymentMethod.CreditCard)
			{

				if (creditCard == null || !TryValidateModel(creditCard))
				{
					ModelState.AddModelError(string.Empty, "Credit card details are required for this payment method.");
					return View("Index", order);
				}

				// Link the credit card to the customer
				order.Customer.CreditCard = creditCard;
				order.Customer.CreditCard.CustomerId = order.CustomerId;
			}

			Orders.Add(order);

			TempData["OrderId"] = order.Id;
			return RedirectToAction(nameof(Confirmation), new { id = order.Id });
		}

		[HttpGet]
		public IActionResult Confirmation(int id)
		{
			var order = Orders.FirstOrDefault(o => o.Id == id);
			if (order == null)
			{
				return NotFound();
			}
			return View(order);
		}
	}
}