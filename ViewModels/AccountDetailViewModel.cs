using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class AccountDetailViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;
		private Account _account;
		private Staff _staff;
		private string _createdBy;
		private DateTime? _lastLogin;
		private bool _isActive;

		public Account Account
		{
			get => _account;
			set => SetProperty(ref _account, value);
		}

		public Staff Staff
		{
			get => _staff;
			set => SetProperty(ref _staff, value);
		}

		public string CreatedBy
		{
			get => _createdBy;
			set => SetProperty(ref _createdBy, value);
		}

		public DateTime? LastLogin
		{
			get => _lastLogin;
			set => SetProperty(ref _lastLogin, value);
		}

		public bool IsActive
		{
			get => _isActive;
			set => SetProperty(ref _isActive, value);
		}

		public ICommand EditCommand { get; }

		public AccountDetailViewModel(Account account, IDialogService dialogService, IDataService dataService)
		{
			_dialogService = dialogService;
			_dataService = dataService;
			Account = account;

			EditCommand = new AsyncRelayCommand(async _ => await EditAccountAsync());

			// Load additional data
			LoadAccountDetails();
		}

		private async void LoadAccountDetails()
		{
			try
			{
				// Load staff information using StaffID
				if (Account.StaffID > 0)
				{
					Staff = await _dataService.GetStaffByIdAsync(Account.StaffID);
				}

				// Load created by information using CreatedBy username
				if (!string.IsNullOrEmpty(Account.CreatedBy))
				{
					var createdByAccount = await _dataService.GetAccountByUsernameAsync(Account.CreatedBy);
					CreatedBy = createdByAccount?.Staff?.FullName ?? Account.CreatedBy;
				}

				// Set other properties
				LastLogin = Account.LastLogin;
				IsActive = Account.Status == "Active";
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải thông tin chi tiết: {ex.Message}");
			}
		}

		private async Task EditAccountAsync()
		{
			var editAccountWindow = new EditAccountWindow(Account, _dialogService, _dataService);
			var dialogResult = editAccountWindow.ShowDialog();

			if (dialogResult == true && editAccountWindow.UpdatedAccount != null)
			{
				Account = editAccountWindow.UpdatedAccount;

				// Reload details after update
				LoadAccountDetails();

				await _dialogService.ShowInfoAsync("Thành công", "Cập nhật tài khoản thành công!");
			}
		}
	}
}