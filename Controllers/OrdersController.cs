using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UShop.Data;
using UShop.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly UShopDBContext _context;
        private readonly UserManager<User> _userManager;


        public OrdersController(UShopDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var orders = await GetOrdersOfUser(user).ToListAsync();
            return View(orders);
        }

        private IQueryable<Order> GetOrdersOfUser(User user)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items).ThenInclude(i => i.Product);

            if (User.IsInRole(Roles.Admin))
            {
                return query;
            }
            if (User.IsInRole(Roles.Customer) && user.CustomerId.HasValue)
            {
                return query.Where(o => o.CustomerId == user.CustomerId);
            }
            if (User.IsInRole(Roles.Seller) && user.SellerId.HasValue)
            {
                return query.Where(o => o.Items.Any(i => i.Product.SellerId == user.SellerId));
            }

            return query.Where(o => false);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            IQueryable<Order> query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product);

            if (User.IsInRole(Roles.Customer) && user.CustomerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == user.CustomerId);
            }
            else if (User.IsInRole(Roles.Seller) && user.SellerId.HasValue)
            {
                query = query.Where(o => o.Items.Any(i => i.Product!.SellerId == user.SellerId));
            }
            else
            {
                return Forbid(); 
            }

            var order = await query.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            if(User.IsInRole(Roles.Seller) && user.SellerId.HasValue)
            {
                order.Items = order.Items
                    .Where(i => i.Product!.SellerId == user.SellerId)
                    .ToList();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            // Customers dropdown
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName");
            // Products dropdown (for order items)
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, List<int> productIds, List<int> quantities)
        {
            if (ModelState.IsValid)
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var sellerSet = new HashSet<Seller>();

                if (productIds != null && quantities != null && productIds.Count == quantities.Count)
                {
                    for (int i = 0; i < productIds.Count; i++)
                    {
                        if (quantities[i] <= 0)
                        {
                            ModelState.AddModelError("", "Quantity must be greater than 0");
                            continue;
                        }

                        var product = await _context.Products
                            .Include(p => p.Seller)
                            .FirstOrDefaultAsync(p => p.Id == productIds[i]);

                        if (product != null)
                        {
                            var orderItem = new Item
                            {
                                OrderId = order.Id,
                                ProductId = product.Id,
                                Quantity = quantities[i],
                                UnitPrice = product.Price
                            };
                            _context.Items.Add(orderItem);

                            if (product.Seller != null)
                                sellerSet.Add(product.Seller);
                        }
                    }
                }

                // Assign all unique sellers for the order
                order.Sellers = sellerSet.ToList();

                order.Statuses = sellerSet.Select(s => OrderStatus.Pending).ToList();


                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", order.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                  .Include(o => o.Items)
                  .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", order.CustomerId);

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Orders.Any(o => o.Id == order.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", order.CustomerId);
            return View(order);
        }

        // Helper method for status validation
        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Define valid status transitions
            return currentStatus switch
            {
                OrderStatus.Pending => newStatus == OrderStatus.Processing || newStatus == OrderStatus.Cancelled,
                OrderStatus.Processing => newStatus == OrderStatus.Shipped || newStatus == OrderStatus.Cancelled,
                OrderStatus.Shipped => newStatus == OrderStatus.Delivered,
                OrderStatus.Delivered => false, // No transitions from delivered
                OrderStatus.Cancelled => false, // No transitions from cancelled
                _ => false
            };
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                  .Include(o => o.Customer)
                  .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                  .Include(o => o.Items)
                  .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                // Delete related order items first
                _context.Items.RemoveRange(order.Items);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Orders/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders
                 .Include(o => o.Customer)
                 .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Check if the order can be cancelled
            if (!CanOrderBeCancelled(order.Status))
            {
                TempData["Error"] = "This order cannot be cancelled. It may have already been shipped or delivered.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Update order status to cancelled
            order.Status = OrderStatus.Cancelled;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Order has been cancelled successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while cancelling the order. Please try again.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // Helper method to check if order can be cancelled
        private bool CanOrderBeCancelled(OrderStatus status)
        {
            // Only allow cancellation for Pending and Processing orders
            return status == OrderStatus.Pending || status == OrderStatus.Processing;
        }

        // Optional: GET method to show cancellation confirmation page
        [HttpGet]
        public async Task<IActionResult> CancelConfirmation(int id)
        {
            var order = await _context.Orders
                 .Include(o => o.Customer)
                 .Include(o => o.Items)
                      .ThenInclude(oi => oi.Product)
                 .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (!CanOrderBeCancelled(order.Status))
            {
                TempData["Error"] = "This order cannot be cancelled.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(order);
        }

        private async Task<int?> GetCustomerIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user?.CustomerId;
        }

        // POST: Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.Sellers)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole(Roles.Admin);
            var isSeller = User.IsInRole(Roles.Seller) &&
                           user?.SellerId != null &&
                           order.Sellers.Any(s => s.Id == user.SellerId);

            if (!isAdmin && !isSeller)
                return Forbid(); // not allowed

            if (isSeller)
            {
                // Find the index of the current seller
                var index = order.Sellers.ToList().FindIndex(s => s.Id == user!.SellerId);
                if (index >= 0)
                {
                    // Validate status transition for this seller's current status
                    if (!IsValidStatusTransition(order.Statuses[index], newStatus))
                    {
                        TempData["Error"] = "Invalid status transition";
                        return RedirectToAction(nameof(Details), new { id });
                    }

                    // Update only this seller's status
                    order.Statuses[index] = newStatus;
                }
            }
            else if (isAdmin)
            {
                // Admin can update the overall status if you want, or all statuses
                for (int i = 0; i < order.Statuses.Count; i++)
                {
                    if (IsValidStatusTransition(order.Statuses[i], newStatus))
                        order.Statuses[i] = newStatus;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Order status updated to {newStatus}";
            return RedirectToAction(nameof(Details), new { id });
        }


    }
}