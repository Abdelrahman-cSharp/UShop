using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShop.Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(150)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(500)]
		public string? Description { get; set; }

		[Column(TypeName = "decimal(18,2)")] 
		public decimal Price { get; set; }

		public int StockQuantity { get; set; }

		public string? ImageUrl { get; set; }

		public int SellerId { get; set; }
		[Required]
		public Seller Seller { get; set; }

		public int CategoryId { get; set; }
		public Category? Category { get; set; }
	}
}
