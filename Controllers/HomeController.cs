using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UShop.Data;
using UShop.Models;

public class HomeController : Controller
{
    private readonly UShopDBContext _context;

    public HomeController(UShopDBContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories ?? new List<Category>());
        }
        catch (Exception ex)
        {
            // Log the exception
            // You can use ILogger here if configured
            // For now, return empty list
            return View(new List<Category>());
        }
    }

    public IActionResult Deals()
    {
        return View();
    }

    public async Task<IActionResult> Categories()
    {
        try
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories ?? new List<Category>());
        }
        catch (Exception ex)
        {
            // Log the exception
            return View(new List<Category>());
        }
    }
}