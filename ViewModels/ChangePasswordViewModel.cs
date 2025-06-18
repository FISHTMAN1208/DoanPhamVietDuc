using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;

namespace DoanPhamVietDuc.ViewModels
{
	public class ChangePasswordViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;
		private readonly Account _currentAccount;

		private string _currentPassword;
		private string _newPassword;
		private string _confirmPassword;

		public string CurrentPassword
		{
			get => _currentPassword;
			set => SetProperty(ref _currentPassword, value);
		}

		public string NewPassword
		{
			get => _newPassword;
			set => SetProperty(ref _newPassword, value);
		}

		public string ConfirmPassword
		{
			get => _confirmPassword;
			set => SetProperty(ref _confirmPassword, value);
		}

		public ICommand ChangePasswordCommand { get; }
		public ICommand CancelCommand { get; }

		public event EventHandler<bool> CloseRequested;

		public ChangePasswordViewModel(Account account, IDialogService dialogService, IDataService dataService)
		{
			_currentAccount = account;
			_dialogService = dialogService;
			_dataService = dataService;

			ChangePasswordCommand = new AsyncRelayCommand(async _ => await ExecuteChangePasswordAsync());
			CancelCommand = new RelayCommand(_ => ExecuteCancel());
		}

		private async Task ExecuteChangePasswordAsync()
		{
			try
			{
				if (!ValidateInput())
					return;

				IsBusy = true;

				// Verify current password
				bool isCurrentPasswordValid = await _dataService.VerifyPasswordAsync(_currentAccount.Username, CurrentPassword);
				if (!isCurrentPasswordValid)
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Mật khẩu hiện tại không đúng!");
					return;
				}

				// Update password
				bool result = await _dataService.UpdatePasswordAsync(_currentAccount.Username, NewPassword);
				if (result)
				{
					await _dialogService.ShowInfoAsync("Thành công", "Đổi mật khẩu thành công!");
					CloseRequested?.Invoke(this, true);
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Không thể cập nhật mật khẩu. Vui lòng thử lại!");
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Đã xảy ra lỗi: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void ExecuteCancel()
		{
			CloseRequested?.Invoke(this, false);
		}

		private bool ValidateInput()
		{
			// Check if current password is empty
			if (string.IsNullOrWhiteSpace(CurrentPassword))
			{
				_dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mật khẩu hiện tại!");
				return false;
			}

			// Check if new password is empty
			if (string.IsNullOrWhiteSpace(NewPassword))
			{
				_dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mật khẩu mới!");
				return false;
			}

			// Check if confirm password is empty
			if (string.IsNullOrWhiteSpace(ConfirmPassword))
			{
				_dialogService.ShowInfoAsync("Thông báo", "Vui lòng xác nhận mật khẩu mới!");
				return false;
			}

			// Check password strength
			if (!IsValidPassword(NewPassword))
			{
				_dialogService.ShowInfoAsync("Mật khẩu không hợp lệ",
					"Mật khẩu mới không đáp ứng yêu cầu bảo mật!\n\n" +
					"Mật khẩu phải:\n" +
					"• Có ít nhất 8 ký tự\n" +
					"• Chứa ít nhất 1 chữ hoa và 1 chữ thường\n" +
					"• Chứa ít nhất 1 số hoặc ký tự đặc biệt");
				return false;
			}

			// Check if new password matches confirm password
			if (NewPassword != ConfirmPassword)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Mật khẩu mới và xác nhận mật khẩu không khớp!");
				return false;
			}

			// Check if new password is different from current password
			if (CurrentPassword == NewPassword)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Mật khẩu mới phải khác với mật khẩu hiện tại!");
				return false;
			}

			return true;
		}

		private bool IsValidPassword(string password)
		{
			if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
				return false;

			// Check for at least one uppercase letter
			if (!Regex.IsMatch(password, @"[A-Z]"))
				return false;

			// Check for at least one lowercase letter
			if (!Regex.IsMatch(password, @"[a-z]"))
				return false;

			// Check for at least one digit or special character
			if (!Regex.IsMatch(password, @"[\d\W]"))
				return false;

			return true;
		}
	}
}