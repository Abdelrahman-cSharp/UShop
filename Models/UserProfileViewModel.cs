
namespace UShop.Models
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public int Id { get; set; }
        public UserType UserType { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }

        // Stores the relative URL of the saved profile image in /wwwroot/images/avatars
        public string? ImageUrl { get; set; }

        // Used only for uploading a new image via a form
        public IFormFile? ImageFile { get; set; }

        public bool IsOwnProfile { get; set; }

        public Address? Address { get; set; }

        public List<CreditCard>? CreditCards { get; set; }

        public List<Order>? Orders { get; set; }

        public List<Product>? Products { get; set; }

        // Statistics
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
        public int ProductsCount { get; set; }
        public decimal TotalRevenue { get; set; }

        // Helper methods
        public string GetUserTypeDisplayName() => UserType.ToString();
        public string GetUserTypeBadgeClass() => UserType switch
        {
            UserType.Admin => "bg-danger",
            UserType.Seller => "bg-warning",
            UserType.Customer => "bg-success",
            _ => "bg-secondary"
        };

        // ✅ Always show the uploaded image if one exists, otherwise default
        public string ProfileImageUrl =>
            !string.IsNullOrEmpty(ImageUrl)
                ? ImageUrl
                : "/images/avatars/default.svg";
    }
}
