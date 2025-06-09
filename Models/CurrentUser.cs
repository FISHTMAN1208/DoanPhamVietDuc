using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class CurrentUser
	{
		public Account Account { get; set; }
		public Staff Staff { get; set; }
		public string[] Permissions { get; set; }

		public bool HasPermission(string permission)
		{
			return Permissions?.Contains(permission) ?? false;
		}

		public bool IsAdmin => Account?.Role == "Admin";
		public bool IsStaff => Account?.Role == "Staff";
		public string DisplayName => Staff?.FullName ?? Account?.Username;
	}
}
