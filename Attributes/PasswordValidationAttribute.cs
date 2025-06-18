using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DoanPhamVietDuc.Attributes
{
	public class PasswordValidationAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if (value is not string password)
			{
				ErrorMessage = "Mật khẩu không được để trống";
				return false;
			}

			if (password.Length < 8)
			{
				ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự";
				return false;
			}

			if (!Regex.IsMatch(password, @"[A-Z]"))
			{
				ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ hoa";
				return false;
			}

			if (!Regex.IsMatch(password, @"[a-z]"))
			{
				ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ thường";
				return false;
			}

			if (!Regex.IsMatch(password, @"[0-9]"))
			{
				ErrorMessage = "Mật khẩu phải chứa ít nhất một số";
				return false;
			}

			if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]"))
			{
				ErrorMessage = "Mật khẩu phải chứa ít nhất một ký tự đặc biệt";
				return false;
			}
			if (password.Any(char.IsWhiteSpace))
			{
				ErrorMessage = "Mật khẩu không được chứa khoảng trắng";
				return false;
			}

			return true;
		}
	}
}