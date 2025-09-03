using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UShop.Models
{
	public class User : IdentityUser
	{
		[Key]
		public int Id { get; set; }

		[Required, MaxLength(100)]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Required, MaxLength(50)]
		public string PasswordHash { get; set; } = string.Empty;

		[Required]
		public UserType UserType { get; set; } // Admin or Customer

		// Navigation properties
		public Admin? Admin { get; set; }
		public Customer? Customer { get; set; }
	}
}
