using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShop.Domain.Entities
{
	public class OrderItem
	{
		[Key]
		public int OrderItemId { get; set; }

		[Required]
		public int OrderId { get; set; }

		[ForeignKey(nameof(OrderId))]
		public Order Order { get; set; } = null!;

		[Required]
		public int ProductId { get; set; }

		[Required]
		public int Quantity { get; set; }

		[Required]
		public decimal UnitPrice { get; set; }

		[NotMapped]
		public decimal Subtotal => Quantity * UnitPrice;
	}
}
