using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UShop.Models;

namespace UShop.Data;


public class UShopDBContext : IdentityDbContext<User>
{

	public UShopDBContext(DbContextOptions<UShopDBContext> options)
	: base(options)
	{
	}

	public DbSet<Seller> Sellers { get; set; }
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Admin> Admins { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		
	}
}

