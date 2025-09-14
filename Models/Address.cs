using System.ComponentModel.DataAnnotations;


namespace UShop.Models
{
    public class Address 
    {
        public int Id { get; set; }
        [Required]
        public string Street { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = string.Empty;
        public int CustomerId { get; set; }
    }

    public class EditProfileViewModel
    {
        public string UserId { get; set; } = string.Empty; // مفقود
        public UserType UserType { get; set; }
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public Address? Address { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? CurrentImageUrl { get; set; }
        public bool CanEdit { get; set; }
        public bool IsOwnProfile { get; set; }
    }
}