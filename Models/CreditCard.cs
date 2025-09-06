using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UShop.Models
{
	public class CreditCard
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[RegularExpression(@"^4[0-9]{12,18}$",
		ErrorMessage = "Visa card must start with 4 and be 13–19 digits.")]
		public string CardNumber { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 2)]
		public string CardholderName { get; set; }

		[Required]
		[Range(1, 12)]
		public int ExpiryMonth { get; set; }

		[Required]
		[Range(2025, 2050)]
		public int ExpiryYear { get; set; }

		[Required]
		[RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be 3 digits.")]
		public string CVV { get; set; }

		[ForeignKey("Customer")]
		public int? CustomerId { get; set; }
		[ValidateNever]
		public Customer? Customer { get; set; }

		[ForeignKey("Seller")]
		public int? SellerId { get; set; }
		[ValidateNever]
		public Seller? Seller { get; set; }
	}
}
