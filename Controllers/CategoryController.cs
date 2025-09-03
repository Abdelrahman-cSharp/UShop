using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UShop.Data;
using UShop.Models;

namespace UShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly UShopDBContext _context;

        // Dependency Injection: The controller receives the database context.
        public CategoryController(UShopDBContext context)
        {
            _context = context;
        }

        // GET: Category
        // Displays a list of all categories.
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                // Log the error and show user-friendly message
                ViewBag.ErrorMessage = $"An error occurred while loading categories: {ex.Message}";
                return View(new List<Category>());
            }
        }

        // GET: Category/Create
        // Displays the category creation form.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // Handles the form submission to create a new category.
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents Cross-Site Request Forgery (CSRF) attacks.
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync(); // Saves changes to the database.
                    TempData["SuccessMessage"] = "Category created successfully!";
                    return RedirectToAction(nameof(Index)); // Redirects to the list of categories.
                }
                return View(category);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred while creating the category: {ex.Message}";
                return View(category);
            }
        }

        // GET: Category/Details/5
        // Displays the details of a single category
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred while loading category details: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Category/Edit/5
        // Displays the category editing form.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Return 404 if no ID is provided.
            }

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(); // Return 404 if the category is not found.
                }
                return View(category);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred while loading the category for editing: {ex.Message}";
                return View("Error");
            }
        }

        // POST: Category/Edit/5
        // Handles the form submission to update a category.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Category updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred while updating the category: {ex.Message}";
                return View(category);
            }
        }

        // GET: Category/Delete/5
        // Displays the category deletion confirmation page.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred while loading the category for deletion: {ex.Message}";
                return View("Error");
            }
        }

        // POST: Category/Delete/5
        // Handles the confirmation and deletion of a category.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category != null)
                {
                    // Check if category has products
                    if (category.Products.Any())
                    {
                        TempData["ErrorMessage"] = "Cannot delete category because it contains products. Please delete or reassign the products first.";
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Category deleted successfully!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the category: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Helper method to check if a category exists.
        private bool CategoryExists(int id)
        {
            try
            {
                return _context.Categories.Any(e => e.Id == id);
            }
            catch
            {
                return false;
            }
        }
    }
}