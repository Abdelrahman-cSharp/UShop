using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UShop.Domain.Entities
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

		[Required, MaxLength(50)]
		public string PasswordHash { get; set; } = string.Empty; // store hashed password

		[MaxLength(15)]
		public string? PhoneNumber { get; set; }

		[MaxLength(250)]
		public string? Address { get; set; }

		// Relationships
		public ICollection<Order> Orders { get; set; } = new List<Order>();
	}
}
