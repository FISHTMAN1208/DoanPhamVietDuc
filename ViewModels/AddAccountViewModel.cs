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
	public class AddAccountViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;
		private readonly Window _window;

		public event EventHandler<Account> AccountSaved;

		private Account _account;
		public Account Account
		{
			get => _account;
			set => SetProperty(ref _account, value);
		}

		private string _password;
		public string Password
		{
			get => _password;
			set => SetProperty(ref _password, value);
		}

		private string _confirmPassword;
		public string ConfirmPassword
		{
			get => _confirmPassword;
			set => SetProperty(ref _confirmPassword, value);
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

		public AddAccountViewModel(IDialogService dialogService, IDataService dataService, Window window)
		{
			_dialogService = dialogService;
			_dataService = dataService;
			_window = window;

			Title = "Thêm tài khoản mới";

			Account = new Account
			{
				CreatedDate = DateTime.Now,
				Status = "Active"
			};

			Roles = new ObservableCollection<string> { "Admin", "Staff" };
			Statuses = new ObservableCollection<string> { "Active", "Inactive" };

			SaveCommand = new AsyncRelayCommand(async _ => await SaveAccountAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());

			UsernameErrorVisibility = Visibility.Collapsed;
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

				// Set default values
				if (Roles.Count > 0)
				{
					Account.Role = Roles[0];
				}
				if (Statuses.Count > 0)
				{
					Account.Status = Statuses[0];
				}
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
			ConfirmPasswordError = string.Empty;
			StaffError = string.Empty;
			RoleError = string.Empty;
			UsernameErrorVisibility = Visibility.Collapsed;
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

			if (string.IsNullOrWhiteSpace(Password))
			{
				ConfirmPasswordError = "Vui lòng nhập mật khẩu";
				ConfirmPasswordErrorVisibility = Visibility.Visible;
				hasError = true;
			}

			if (string.IsNullOrWhiteSpace(ConfirmPassword))
			{
				ConfirmPasswordError = "Vui lòng xác nhận mật khẩu";
				ConfirmPasswordErrorVisibility = Visibility.Visible;
				hasError = true;
			}

			if (Password != ConfirmPassword)
			{
				ConfirmPasswordError = "Mật khẩu xác nhận không khớp";
				ConfirmPasswordErrorVisibility = Visibility.Visible;
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

			if (hasError)
			{
				return;
			}

			// Check if username exists
			bool usernameExists = await _dataService.IsUsernameExistsAsync(Account.Username);
			if (usernameExists)
			{
				UsernameError = $"Tên đăng nhập '{Account.Username}' đã tồn tại";
				UsernameErrorVisibility = Visibility.Visible;
				return;
			}

			try
			{
				Account.CreatedDate = DateTime.Now;
				Account.Password = Password; // Gán Password vào Account

				var success = await _dataService.AddAccountAsync(Account);

				if (success)
				{
					AccountSaved?.Invoke(this, Account);
					await _dialogService.ShowInfoAsync("Thông báo", "Thêm tài khoản mới thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Thêm tài khoản thất bại, vui lòng thử lại");
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