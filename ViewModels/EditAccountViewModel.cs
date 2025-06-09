using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditAccountViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		private readonly Account _originalAccount;

		public event EventHandler<Account> AccountUpdated;

		private Account _account;
		public Account Account
		{
			get => _account;
			set => SetProperty(ref _account, value);
		}

		private string _newPassword;
		public string NewPassword
		{
			get => _newPassword;
			set => SetProperty(ref _newPassword, value);
		}

		private string _confirmPassword;
		public string ConfirmPassword
		{
			get => _confirmPassword;
			set => SetProperty(ref _confirmPassword, value);
		}

		private bool _changePassword;
		public bool ChangePassword
		{
			get => _changePassword;
			set => SetProperty(ref _changePassword, value);
		}

		private ObservableCollection<Staff> _staffList;
		public ObservableCollection<Staff> StaffList
		{
			get => _staffList;
			set => SetProperty(ref _staffList, value);
		}

		private ObservableCollection<string> _roles;
		public ObservableCollection<string> Roles
		{
			get => _roles;
			set => SetProperty(ref _roles, value);
		}

		private ObservableCollection<string> _statuses;
		public ObservableCollection<string> Statuses
		{
			get => _statuses;
			set => SetProperty(ref _statuses, value);
		}

		private string _usernameError;
		public string UsernameError
		{
			get => _usernameError;
			set => SetProperty(ref _usernameError, value);
		}

		private Visibility _usernameErrorVisibility;
		public Visibility UsernameErrorVisibility
		{
			get => _usernameErrorVisibility;
			set => SetProperty(ref _usernameErrorVisibility, value);
		}

		private string _passwordError;
		public string PasswordError
		{
			get => _passwordError;
			set => SetProperty(ref _passwordError, value);
		}

		private Visibility _passwordErrorVisibility;
		public Visibility PasswordErrorVisibility
		{
			get => _passwordErrorVisibility;
			set => SetProperty(ref _passwordErrorVisibility, value);
		}

		private string _confirmPasswordError;
		public string ConfirmPasswordError
		{
			get => _confirmPasswordError;
			set => SetProperty(ref _confirmPasswordError, value);
		}

		private Visibility _confirmPasswordErrorVisibility;
		public Visibility ConfirmPasswordErrorVisibility
		{
			get => _confirmPasswordErrorVisibility;
			set => SetProperty(ref _confirmPasswordErrorVisibility, value);
		}

		private string _staffError;
		public string StaffError
		{
			get => _staffError;
			set => SetProperty(ref _staffError, value);
		}

		private Visibility _staffErrorVisibility;
		public Visibility StaffErrorVisibility
		{
			get => _staffErrorVisibility;
			set => SetProperty(ref _staffErrorVisibility, value);
		}

		private string _roleError;
		public string RoleError
		{
			get => _roleError;
			set => SetProperty(ref _roleError, value);
		}

		private Visibility _roleErrorVisibility;
		public Visibility RoleErrorVisibility
		{
			get => _roleErrorVisibility;
			set => SetProperty(ref _roleErrorVisibility, value);
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public EditAccountViewModel(Account account, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;
			_originalAccount = account;

			Title = $"Sửa tài khoản: {account.Username}";

			Account = new Account
			{
				AccountID = account.AccountID,
				Username = account.Username,
				Password = account.Password,
				StaffID = account.StaffID,
				Role = account.Role,
				Status = account.Status,
				LastLogin = account.LastLogin,
				CreatedDate = account.CreatedDate,
				CreatedBy = account.CreatedBy
			};

			Roles = new ObservableCollection<string> { "Admin", "Staff" };
			Statuses = new ObservableCollection<string> { "Active", "Inactive" };

			SaveCommand = new AsyncRelayCommand(async _ => await SaveAccountAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());

			UsernameErrorVisibility = Visibility.Collapsed;
			PasswordErrorVisibility = Visibility.Collapsed;
			ConfirmPasswordErrorVisibility = Visibility.Collapsed;
			StaffErrorVisibility = Visibility.Collapsed;
			RoleErrorVisibility = Visibility.Collapsed;

			_ = LoadReferenceDataAsync();
		}

		public async Task LoadReferenceDataAsync()
		{
			try
			{
				IsBusy = true;
				var staffList = await _dataService.GetAllStaffsAsync();
				StaffList = new ObservableCollection<Staff>(staffList);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải dữ liệu: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task SaveAccountAsync()
		{
			// Reset error messages
			UsernameError = string.Empty;
			PasswordError = string.Empty;
			ConfirmPasswordError = string.Empty;
			StaffError = string.Empty;
			RoleError = string.Empty;
			UsernameErrorVisibility = Visibility.Collapsed;
			PasswordErrorVisibility = Visibility.Collapsed;
			ConfirmPasswordErrorVisibility = Visibility.Collapsed;
			StaffErrorVisibility = Visibility.Collapsed;
			RoleErrorVisibility = Visibility.Collapsed;

			bool hasError = false;

			// Validation
			if (string.IsNullOrWhiteSpace(Account.Username))
			{
				UsernameError = "Vui lòng nhập tên đăng nhập";
				UsernameErrorVisibility = Visibility.Visible;
				hasError = true;
			}

			if (Account.StaffID <= 0)
			{
				StaffError = "Vui lòng chọn nhân viên";
				StaffErrorVisibility = Visibility.Visible;
				hasError = true;
			}

			if (string.IsNullOrWhiteSpace(Account.Role))
			{
				RoleError = "Vui lòng chọn quyền";
				RoleErrorVisibility = Visibility.Visible;
				hasError = true;
			}

			// Password validation if changing password
			if (!string.IsNullOrWhiteSpace(NewPassword) || !string.IsNullOrWhiteSpace(ConfirmPassword))
			{
				ChangePassword = true;
				if (string.IsNullOrWhiteSpace(NewPassword))
				{
					PasswordError = "Vui lòng nhập mật khẩu mới";
					PasswordErrorVisibility = Visibility.Visible;
					hasError = true;
				}

				if (string.IsNullOrWhiteSpace(ConfirmPassword))
				{
					ConfirmPasswordError = "Vui lòng xác nhận mật khẩu mới";
					ConfirmPasswordErrorVisibility = Visibility.Visible;
					hasError = true;
				}

				if (NewPassword != ConfirmPassword)
				{
					ConfirmPasswordError = "Mật khẩu xác nhận không khớp";
					ConfirmPasswordErrorVisibility = Visibility.Visible;
					hasError = true;
				}
			}

			if (hasError)
			{
				return;
			}

			// Check if username exists (excluding current account)
			bool usernameExists = await _dataService.IsUsernameExistsAsync(Account.Username, Account.AccountID);
			if (usernameExists)
			{
				UsernameError = $"Tên đăng nhập '{Account.Username}' đã tồn tại";
				UsernameErrorVisibility = Visibility.Visible;
				return;
			}

			try
			{
				if (ChangePassword && !string.IsNullOrWhiteSpace(NewPassword))
				{
					Account.Password = NewPassword;
				}

				var success = await _dataService.UpdateAccountAsync(Account);

				if (success)
				{
					AccountUpdated?.Invoke(this, Account);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật tài khoản thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Cập nhật tài khoản thất bại, vui lòng thử lại");
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Đã xảy ra lỗi: {ex.Message}");
			}
		}

		private void CancelAndClose()
		{
			_window.DialogResult = false;
			_window.Close();
		}
	}
}