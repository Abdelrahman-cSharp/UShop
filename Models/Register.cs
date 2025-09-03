using System.ComponentModel.DataAnnotations;

namespace UShop.Models
{
	public class Register
	{
		public string FullName { get; set; }
		[Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
		[EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
		[Display(Name = "البريد الإلكتروني")]
		public string Email { get; set; }

		[Required(ErrorMessage = "كلمة المرور مطلوبة")]
		[StringLength(100, ErrorMessage = "كلمة المرور يجب أن تكون على الأقل {2} أحرف", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "كلمة المرور")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "تأكيد كلمة المرور")]
		[Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقتين")]
		public string ConfirmPassword { get; set; }
	}
}
