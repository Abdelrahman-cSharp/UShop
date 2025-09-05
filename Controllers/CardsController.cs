using UShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace UShop.Controllers
{
    public static class DB
    {
        // --- Static demo data ---
        public static Customer Customer = new Customer
        {
            Id = 1,
            FullName = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Address = "123 Street",
            CreditCard = new CreditCard
            {
                Id = 1,
                CardNumber = "4111111111111111",
                CardholderName = "John Doe",
                ExpiryMonth = 12,
                ExpiryYear = 2026,
                CVV = "123",
                CustomerId = 1
            }
        };
    }

    public class CreditCardController : Controller
    {
        // Show customer + card
        public IActionResult Index()
        {
            return View(DB.Customer);
        }

        // GET: Create
        public IActionResult Create()
        {
            if (DB.Customer.CreditCard != null)
                return RedirectToAction(nameof(Index)); // only one card per customer
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreditCard card)
        {
            card.CustomerId = DB.Customer.Id;
            card.Id = 1;
            DB.Customer.CreditCard = card;
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            ModelState.Remove("CustomerId"); // ignore it
            if (DB.Customer.CreditCard == null || DB.Customer.CreditCard.Id != id)
                return RedirectToAction(nameof(Index));

            return View(DB.Customer.CreditCard);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CreditCard card)
        {
            if (DB.Customer.CreditCard == null || DB.Customer.CreditCard.Id != id)
                return RedirectToAction(nameof(Index));

            DB.Customer.CreditCard.CardNumber = card.CardNumber;
            DB.Customer.CreditCard.CardholderName = card.CardholderName;
            DB.Customer.CreditCard.ExpiryMonth = card.ExpiryMonth;
            DB.Customer.CreditCard.ExpiryYear = card.ExpiryYear;
            DB.Customer.CreditCard.CVV = card.CVV;
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            if (DB.Customer.CreditCard == null || DB.Customer.CreditCard.Id != id)
                return RedirectToAction(nameof(Index));

            return View(DB.Customer.CreditCard);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (DB.Customer.CreditCard != null && DB.Customer.CreditCard.Id == id)
            {
                DB.Customer.CreditCard = null;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}