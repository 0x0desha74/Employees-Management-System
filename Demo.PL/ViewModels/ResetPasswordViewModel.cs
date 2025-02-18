using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Confirm Password is required")]
		[Compare("NewPassword", ErrorMessage = "Confirm Password Doesn't Match Password")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }

	
	}
}
