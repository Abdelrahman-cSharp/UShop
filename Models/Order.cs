using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShop.Models
{
	public class Order
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int CustomerId { get; set; }

		[ValidateNever]
		public Customer? Customer { get; set; }

		[Required]
		public DateTime OrderDate { get; set; } = DateTime.UtcNow;

		public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

		[Required]
		public OrderStatus Status { get; set; } = OrderStatus.Pending;
		// e.g., Pending, Processing, Shipped, Delivered
		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
		
		[NotMapped]
		public decimal TotalAmount => OrderItems.Sum(item => item.Quantity * item.UnitPrice);
	}
}
