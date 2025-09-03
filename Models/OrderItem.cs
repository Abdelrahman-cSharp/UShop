using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShop.Models
{
	public class OrderItem
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int OrderId { get; set; }

		[ForeignKey(nameof(OrderId))]
		public Order Order { get; set; } = null!;

		[Required]
		public int ProductId { get; set; }

		[Required]
		public int Quantity { get; set; }

		[Required]
		[Column(TypeName = "decimal(18,2)")]
		public decimal UnitPrice { get; set; }

		[NotMapped]
		public decimal TotalPrice => Quantity * UnitPrice;
	}
}
