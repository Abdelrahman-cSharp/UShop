using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Stripe;
using UShop.Data;
using UShop.Models;

var builder = WebApplication.CreateBuilder(args);

// Add MVC controllers with views
builder.Services.AddControllersWithViews();

// Stripe Configurations
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Add DbContext (your application context)
builder.Services.AddDbContext<UShopDBContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity (using EF Core with your DbContext)
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireUppercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<UShopDBContext>()
.AddDefaultTokenProviders();


var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
//--------------TO BE DELETED-----testing without login------------------------
//app.Use(async (context, next) =>
//{
//	// Fake login user with ID = 1
//	var claims = new List<Claim>
//	 {
//		  new Claim(ClaimTypes.NameIdentifier, "1"),
//		  new Claim(ClaimTypes.Name, "TestUser"),
//		  new Claim(ClaimTypes.Email, "test@example.com")
//	 };

//	var identity = new ClaimsIdentity(claims, "FakeAuth");
//	context.User = new ClaimsPrincipal(identity);

//	await next();
//});

//-------------------------------------------
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


// Default route
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();


app.Run();
