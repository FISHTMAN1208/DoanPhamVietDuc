using DoanPhamVietDuc.Attributes;
using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
	[Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
	[UsernameValidation]
	public string Username { get; set; }

	[Required(ErrorMessage = "Mật khẩu là bắt buộc")]
	public string Password { get; set; } 

	[Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
	[Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
	public string ConfirmPassword { get; set; }

	[Required(ErrorMessage = "Chọn nhân viên là bắt buộc")]
	public int StaffId { get; set; }

	[Required(ErrorMessage = "Quyền là bắt buộc")]
	[RoleValidation]
	public string Role { get; set; }
}