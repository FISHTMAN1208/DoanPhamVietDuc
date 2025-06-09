using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoanPhamVietDuc.Models;
using Microsoft.Identity.Client;

namespace DoanPhamVietDuc.Services.AuthenticationService
{
	public interface IAuthenticationService
	{
		Task<AuthResult> LoginAsync(string username, string password);
		Task<AuthResult> RegisterAsync(RegisterRequest request);
		Task LogoutAsync();
		bool IsAuthenticated { get; }
		CurrentUser CurrentUser { get; }
		Task<string[]> GetUserPermissionsAsync(string role);
		event Action<CurrentUser> UserChanged;
	}
}
