using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShop.Domain.Entities
{
	public class Order
	{
		[Key]
		public int OrderId { get; set; }

		[Required]
		public int CustomerId { get; set; }

		public Customer Customer { get; set; }

		[Required]
		public DateTime OrderDate { get; set; } = DateTime.UtcNow;

		[Required, MaxLength(50)]
		public string Status { get; set; } = "Pending";
		// e.g., Pending, Processing, Shipped, Delivered
		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

		[Required]
		public decimal TotalAmount => OrderItems.Sum(item => item.Quantity * item.UnitPrice);
	}
}
