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
        public string? Description { get; set; } // For Admin
        public string? ImageUrl { get; set; } // For Seller
        public bool IsOwnProfile { get; set; }
        public bool CanEdit { get; set; }

        // Address (Customer only)
        public Address? Address { get; set; }

        // Credit Cards (Customer & Seller)
        public List<CreditCard>? CreditCards { get; set; }

        // Orders (Customer only)
        public List<Order>? Orders { get; set; }

        // Products (Seller only)
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

        public string ProfileImageUrl => UserType == UserType.Seller && !string.IsNullOrEmpty(ImageUrl)
            ? ImageUrl
            : "/images/avatars/default.png";
    }
}