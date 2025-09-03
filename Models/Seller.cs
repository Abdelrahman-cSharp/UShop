using System.ComponentModel.DataAnnotations;

namespace UShop.Models
{
	public class Seller
	{
		[Key]
		public int Id { get; set; }
		[Required, MaxLength(100)]
		public string FullName { get; set; } = string.Empty;
		[Required, MaxLength(100)]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;
		[MaxLength(15)]
		public string? PhoneNumber { get; set; }
		public ICollection<Product> Products { get; set; } = new List<Product>();
	}
}
