using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UShop.Models
{
	public class User : IdentityUser
	{
		[Required]
		public UserType UserType { get; set; } // Admin or Customer

		// Navigation properties
		public int? AdminId { get; set; }
		public int? CustomerId { get; set; }
		public int? SellerId { get; set; }
	}
}
