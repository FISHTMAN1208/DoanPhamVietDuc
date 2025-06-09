using DoanPhamVietDuc.Enums;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Services.NavigationService;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class RegisterViewModel : BaseViewModel
	{
		private readonly IAuthenticationService _authService;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;
		private readonly IDataService _dataService;

		private string _username;
		private string _password;
		private string _confirmPassword;
		private string _selectedRole = UserRoles.Staff;
		private Staff _selectedStaff;
		private bool _isLoading;

		public string Username
		{
			get => _username;
			set => SetProperty(ref _username, value);
		}

		public string Password
		{
			get => _password;
			set => SetProperty(ref _password, value);
		}

		public string ConfirmPassword
		{
			get => _confirmPassword;
			set => SetProperty(ref _confirmPassword, value);
		}

		public string SelectedRole
		{
			get => _selectedRole;
			set => SetProperty(ref _selectedRole, value);
		}

		public Staff SelectedStaff
		{
			get => _selectedStaff;
			set => SetProperty(ref _selectedStaff, value);
		}

		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}

		public ObservableCollection<string> Roles { get; }
		public ObservableCollection<Staff> AvailableStaffs { get; }

		public ICommand RegisterCommand { get; }
		public ICommand BackToLoginCommand { get; }

		public RegisterViewModel(IAuthenticationService authService, IDialogService dialogService,
			INavigationService navigationService, IDataService dataService)
		{
			_authService = authService;
			_dialogService = dialogService;
			_navigationService = navigationService;
			_dataService = dataService;

			Title = "Đăng ký tài khoản";

			Roles = new ObservableCollection<string>(UserRoles.AllRoles);
			AvailableStaffs = new ObservableCollection<Staff>();

			RegisterCommand = new AsyncRelayCommand(async _ => await RegisterAsync(), _ => CanRegister());
			BackToLoginCommand = new RelayCommand(_ => BackToLogin());

			_ = LoadStaffsAsync();

			System.Diagnostics.Debug.WriteLine("RegisterViewModel: Khởi tạo thành công");
		}

		private bool CanRegister()
		{
			return !IsLoading &&
				   !string.IsNullOrWhiteSpace(Username) &&
				   !string.IsNullOrWhiteSpace(Password) &&
				   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
				   SelectedStaff != null;
		}

		private async Task RegisterAsync()
		{
			System.Diagnostics.Debug.WriteLine("RegisterAsync: Bắt đầu đăng ký");
			IsLoading = true;

			try
			{
				var request = new RegisterRequest
				{
					Username = Username,
					Password = Password, // Sử dụng plaintext
					ConfirmPassword = ConfirmPassword,
					StaffId = SelectedStaff.StaffID,
					Role = SelectedRole
				};

				var validationContext = new ValidationContext(request);
				var validationResults = new List<ValidationResult>();
				bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

				if (!isValid)
				{
					var errorMessages = validationResults.Select(r => r.ErrorMessage).ToList();
					var errorMessage = string.Join("\n", errorMessages);
					await _dialogService.ShowInfoAsync("Lỗi", errorMessage);
					System.Diagnostics.Debug.WriteLine($"RegisterAsync: Validation failed - {errorMessage}");
					return;
				}

				var result = await _authService.RegisterAsync(request);

				if (result.IsSuccess)
				{
					await _dialogService.ShowInfoAsync("Thành công", "Tạo tài khoản thành công!");
					System.Diagnostics.Debug.WriteLine("RegisterAsync: Đăng ký thành công, quay về login");
					BackToLogin();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", result.ErrorMessage);
					System.Diagnostics.Debug.WriteLine($"RegisterAsync: Lỗi - {result.ErrorMessage}");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"RegisterAsync: Exception - {ex.Message}");
				await _dialogService.ShowInfoAsync("Lỗi", $"Có lỗi xảy ra: {ex.Message}");
			}
			finally
			{
				IsLoading = false;
			}
		}

		private void BackToLogin()
		{
			System.Diagnostics.Debug.WriteLine("BackToLogin: Gọi BackToLoginRequested event");
			BackToLoginRequested?.Invoke();
		}

		private async Task LoadStaffsAsync()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine("LoadStaffsAsync: Bắt đầu load danh sách nhân viên");

				var staffs = await _dataService.GetAllStaffsAsync();

				AvailableStaffs.Clear();
				foreach (var staff in staffs)
				{
					AvailableStaffs.Add(staff);
					System.Diagnostics.Debug.WriteLine($"LoadStaffsAsync: Thêm staff - {staff.FullName}");
				}

				System.Diagnostics.Debug.WriteLine($"LoadStaffsAsync: Đã load {AvailableStaffs.Count} nhân viên");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"LoadStaffsAsync: Lỗi - {ex.Message}");
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi tải danh sách nhân viên: {ex.Message}");
			}
		}

		public event Action BackToLoginRequested;
	}
}