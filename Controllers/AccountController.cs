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
		public IActionResult Login(Login model)
		{
			if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Password))
			{
				ViewBag.Error = "FullName and Password are required.";
				return View(model);
			}

			// Placeholder login check
			if (model.FullName == "admin" && model.Password == "123")
			{
				TempData["Message"] = "Login successful!";
				return RedirectToAction("Index", "Home");
			}

			ViewBag.Error = "Invalid login credentials.";
			return View(model);
		}

		// GET: /Account/Register
		public IActionResult Register()
		{
			return View();
		}

		// POST: /Account/Register
		[HttpPost]
		public IActionResult Register(Register model)
		{
			if (string.IsNullOrEmpty(model.FullName) ||
				 string.IsNullOrEmpty(model.Password) ||
				 string.IsNullOrEmpty(model.Email))
			{
				ViewBag.Error = "All fields are required.";
				return View(model);
			}

			// Fake success message (no database yet)
			TempData["Message"] = "Registration successful! Please login.";
			return RedirectToAction("Login");
		}
	}
}
