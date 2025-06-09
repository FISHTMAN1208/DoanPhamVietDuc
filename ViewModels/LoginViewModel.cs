using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Services.AuthenticationService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Services.NavigationService;
using Microsoft.EntityFrameworkCore;
using DoanPhamVietDuc.Views.Authentication;

namespace DoanPhamVietDuc.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		private readonly IAuthenticationService _authService;
		private readonly IDialogService _dialogService;
		private readonly INavigationService _navigationService;

		private string _username = "phamvietduc123"; // Account main
		private string _password = "Phamvietduc2003*"; 
		private bool _isLoading;
		private bool _rememberMe;

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

		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}

		public bool RememberMe
		{
			get => _rememberMe;
			set => SetProperty(ref _rememberMe, value);
		}

		public ICommand LoginCommand { get; }
		public ICommand ShowRegisterCommand { get; }

		public LoginViewModel(IAuthenticationService authService, IDialogService dialogService, INavigationService navigationService)
		{
			_authService = authService;
			_dialogService = dialogService;
			_navigationService = navigationService;

			Title = "Đăng nhập";

			LoginCommand = new AsyncRelayCommand(async _ => await LoginAsync(), _ => CanLogin());
			ShowRegisterCommand = new RelayCommand(_ => ShowRegister());
		}

		private bool CanLogin()
		{
			bool canLogin = !IsLoading && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
			System.Diagnostics.Debug.WriteLine($"CanLogin: Result={canLogin}, Username={Username}, Password={(string.IsNullOrEmpty(Password) ? "empty" : "not empty")}");
			return canLogin;
		}

		private async Task LoginAsync()
		{
			System.Diagnostics.Debug.WriteLine("LoginAsync: Bắt đầu đăng nhập");
			IsLoading = true;

			try
			{
				System.Diagnostics.Debug.WriteLine($"LoginAsync: Username={Username}, Password={Password}");

				var result = await _authService.LoginAsync(Username, Password);

				System.Diagnostics.Debug.WriteLine($"LoginAsync: Kết quả đăng nhập - Success={result.IsSuccess}, Message={result.ErrorMessage}");

				if (result.IsSuccess)
				{
					await _dialogService.ShowInfoAsync("Thành công", "Đăng nhập thành công!");
					System.Diagnostics.Debug.WriteLine("LoginAsync: Sắp gọi OnLoginSuccess");
					OnLoginSuccess?.Invoke();
					System.Diagnostics.Debug.WriteLine("LoginAsync: Đã gọi OnLoginSuccess");
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", result.ErrorMessage ?? "Tên đăng nhập hoặc mật khẩu không đúng");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"LoginAsync: Exception - {ex.Message}");
				await _dialogService.ShowInfoAsync("Lỗi", $"Có lỗi xảy ra: {ex.Message}");
			}
			finally
			{
				IsLoading = false;
				System.Diagnostics.Debug.WriteLine("LoginAsync: Kết thúc");
			}
		}


		private void ShowRegister()
		{
			System.Diagnostics.Debug.WriteLine("ShowRegister: Gọi ShowRegisterRequested event");
			ShowRegisterRequested?.Invoke();
		}

		public event Action OnLoginSuccess;
		public event Action ShowRegisterRequested;
	}
}