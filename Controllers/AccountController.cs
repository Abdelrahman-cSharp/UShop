using Microsoft.AspNetCore.Mvc;
using UShop.Models;

namespace UshopFront.Controllers
{
	public class AccountController : Controller
	{
		// GET: /Account/Login
		public IActionResult Login()
		{
			return View();
		}

		// POST: /Account/Login
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Login(Login model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Placeholder login check
			if (model.Email == "admin@example.com" && model.Password == "123")
			{
				TempData["Message"] = "Login successful!";
				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError(string.Empty, "Invalid login credentials.");
			return View(model);
		}

		// GET: /Account/Register
		public IActionResult Register()
		{
			return View();
		}

		// POST: /Account/Register
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Register(Register model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Fake success message (no database yet)
			TempData["Message"] = "Registration successful! Please login.";
			return RedirectToAction("Login");
		}
	}
}
