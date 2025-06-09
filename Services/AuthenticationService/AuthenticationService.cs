using DoanPhamVietDuc.Constants;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;

namespace DoanPhamVietDuc.Services.AuthenticationService
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IDataService _dataService;
		private CurrentUser _currentUser;

		public bool IsAuthenticated => _currentUser != null;
		public CurrentUser CurrentUser => _currentUser;

		public event Action<CurrentUser> UserChanged;

		public AuthenticationService(IDataService dataService)
		{
			_dataService = dataService;
		}

		public async Task<AuthResult> LoginAsync(string username, string password)
		{
			try
			{
				System.Diagnostics.Debug.WriteLine($"LoginAsync: Username={username}, Password={(string.IsNullOrEmpty(password) ? "empty" : "not empty")}");
				if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
				{
					return AuthResult.Failed("Vui lòng nhập đầy đủ thông tin đăng nhập");
				}

				var account = await _dataService.GetAccountByUsernameAsync(username);
				System.Diagnostics.Debug.WriteLine($"LoginAsync: Found account - Username={account?.Username}, Password={account?.Password}, InputPassword={password}");
				if (account == null)
				{
					return AuthResult.Failed("Tài khoản không tồn tại");
				}
				if (account == null)
				{
					return AuthResult.Failed("Tài khoản không tồn tại");
				}

				if (account.Status != "Active")
				{
					return AuthResult.Failed("Tài khoản đã bị khóa");
				}

				// So sánh trực tiếp mật khẩu plaintext
				bool passwordMatch = account.Password == password;
				System.Diagnostics.Debug.WriteLine($"LoginAsync: Username={username}, PasswordMatch={passwordMatch}, Password={account.Password}");
				if (!passwordMatch)
				{
					return AuthResult.Failed("Mật khẩu không chính xác");
				}

				var staff = await _dataService.GetStaffByIdAsync(account.StaffID);
				if (staff == null)
				{
					return AuthResult.Failed("Không tìm thấy thông tin nhân viên");
				}

				_currentUser = new CurrentUser
				{
					Account = account,
					Staff = staff,
					Permissions = await GetUserPermissionsAsync(account.Role)
				};

				await _dataService.UpdateLastLoginAsync(account.AccountID);
				UserChanged?.Invoke(_currentUser);

				return AuthResult.Success(account);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"LoginAsync: Exception - {ex.Message}");
				return AuthResult.Failed($"Lỗi đăng nhập: {ex.Message}");
			}
		}

		public async Task<AuthResult> RegisterAsync(RegisterRequest request)
		{
			try
			{
				System.Diagnostics.Debug.WriteLine("RegisterAsync: Bắt đầu validation");

				if (string.IsNullOrWhiteSpace(request.Username) ||
					string.IsNullOrWhiteSpace(request.Password) ||
					string.IsNullOrWhiteSpace(request.ConfirmPassword))
				{
					return AuthResult.Failed("Vui lòng nhập đầy đủ thông tin đăng ký");
				}

				if (request.Password != request.ConfirmPassword)
				{
					return AuthResult.Failed("Mật khẩu xác nhận không khớp");
				}

				// Kiểm tra username đã tồn tại
				var existingAccount = await _dataService.GetAccountByUsernameAsync(request.Username);
				if (existingAccount != null)
				{
					return AuthResult.Failed("Tên đăng nhập đã tồn tại");
				}

				// Kiểm tra Staff tồn tại
				var existingStaff = await _dataService.GetStaffByIdAsync(request.StaffId);
				if (existingStaff == null)
				{
					return AuthResult.Failed("Nhân viên không tồn tại");
				}

				// Kiểm tra Staff đã có account chưa
				var existingStaffAccount = await _dataService.GetAccountByStaffIdAsync(request.StaffId);
				if (existingStaffAccount != null)
				{
					return AuthResult.Failed("Nhân viên này đã có tài khoản");
				}

				System.Diagnostics.Debug.WriteLine("RegisterAsync: Validation thành công, tạo account");

				// Tạo account mới với mật khẩu plaintext
				var newAccount = new Account
				{
					Username = request.Username,
					Password = request.Password,
					StaffID = request.StaffId,
					Role = request.Role,
					Status = "Active",
					CreatedDate = DateTime.Now,
					CreatedBy = _currentUser?.DisplayName ?? "System"
				};

				System.Diagnostics.Debug.WriteLine($"RegisterAsync: Tạo account - Username={newAccount.Username}, StaffID={newAccount.StaffID}, Role={newAccount.Role}");

				// LƯU VÀO DATABASE
				bool success = await _dataService.AddAccountAsync(newAccount);

				if (!success)
				{
					return AuthResult.Failed("Lỗi khi lưu tài khoản vào database");
				}

				System.Diagnostics.Debug.WriteLine("RegisterAsync: Lưu account thành công");
				return AuthResult.Success(newAccount);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"RegisterAsync: Exception - {ex.Message}");
				return AuthResult.Failed($"Lỗi tạo tài khoản: {ex.Message}");
			}
		}

		public async Task LogoutAsync()
		{
			_currentUser = null;
			UserChanged?.Invoke(null);
			await Task.CompletedTask;
		}

		public async Task<string[]> GetUserPermissionsAsync(string role)
		{
			return await Task.FromResult(RolePermissions.GetPermissions(role));
		}
	}
}