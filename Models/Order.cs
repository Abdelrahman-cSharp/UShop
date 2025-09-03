using System.ComponentModel.DataAnnotations;

namespace UShop.Models
{
	public class Order
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int CustomerId { get; set; }

		public Customer Customer { get; set; }

		[Required]
		public DateTime OrderDate { get; set; } = DateTime.UtcNow;

		[Required, MaxLength(50)]
		public OrderStatus Status { get; set; } = OrderStatus.Pending;
		// e.g., Pending, Processing, Shipped, Delivered
		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

		[Required]
		public decimal TotalAmount => OrderItems.Sum(item => item.Quantity * item.UnitPrice);
	}
}
