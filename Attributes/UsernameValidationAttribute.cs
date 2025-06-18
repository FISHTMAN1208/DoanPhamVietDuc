using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DoanPhamVietDuc.Helpers;
using DoanPhamVietDuc.Services.DialogService;

namespace DoanPhamVietDuc.Attributes
{
	public class UsernameValidationAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
			{
				ErrorMessage = "Tên đăng nhập không được để trống";
				return false;
			}
			if (value is not string username)
			{
				return false;
			}

			//Ktra độ dài
			if (username.Length < 3 || username.Length > 50)
			{
				ErrorMessage = "Tên đăng nhập phải từ 3-50 ký tự";
				return false;
			}

			//Chỉ cho phép chữ cái, số, dấu gạch
			if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
			{
				ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới";
				return false;
			}

			if (!char.IsLetter(username[0]))
			{
				ErrorMessage = "Tên đăng nhập phải bắt đầu bằng chữ cái";
				return false;
			}

			return true;
		}
	}

	public class RoleValidationAttribute : ValidationAttribute
	{
		private static readonly string[] ValidRoles = { "Admin", "Staff" };

		public override bool IsValid(object value)
		{
			if (value is not string role)
			{
				return false;
			}

			if (!ValidRoles.Contains(role))
			{
				ErrorMessage = $"Role phải là một trong các giá trị: {string.Join(", ", ValidRoles)}";
				return false;
			}
			return true;
		}
	}
}