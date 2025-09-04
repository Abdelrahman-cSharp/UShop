using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShop.Models
{
	public class Customer
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

		[MaxLength(250)]
		public string? Address { get; set; }

		// Relationships
		[ValidateNever]
		public Cart? Cart { get; set; }

		[ValidateNever]
		public CreditCard? CreditCards { get; set; }

		public ICollection<Order> Orders { get; set; } = new List<Order>();
	}
}
