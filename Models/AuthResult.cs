using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class AuthResult
	{
		public bool IsSuccess { get; set; }
		public string ErrorMessage { get; set; }
		public Account User { get; set; }

		public static AuthResult Success(Account user) => new() { IsSuccess = true, User = user };
		public static AuthResult Failed(string error) => new() { IsSuccess = false, ErrorMessage = error };
	}

}
