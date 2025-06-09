using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Enums
{
	public enum UserRole
	{
		Admin = 1,
		Staff = 2
	}

	public static class UserRoles
	{
		public const string Admin = "Admin";
		public const string Staff = "Staff";

		public static readonly string[] AllRoles = {Admin, Staff};
		
		public static bool IsValid(string role)
		{
			return AllRoles.Contains(role);
		}

		public static UserRole ParseRole(string role)
		{
			return role switch
			{
				Admin => UserRole.Admin,
				Staff => UserRole.Staff,
				_ => throw new ArgumentException($"Lỗi role {role}")
			};
		}

		public static string GetDisplayName(string role)
		{
			return role switch
			{
				Admin => "Quản trị viên",
				Staff => "Nhân viên",
				_ => "Không xác định"
			};
		}
	}
}
