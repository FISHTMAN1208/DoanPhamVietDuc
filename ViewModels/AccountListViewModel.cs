using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Accounts;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class AccountListViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private ObservableCollection<Account> _accounts;
		public ObservableCollection<Account> Accounts
		{
			get => _accounts;
			set => SetProperty(ref _accounts, value);
		}

		private Account _selectedAccount;
		public Account SelectedAccount
		{
			get => _selectedAccount;
			set => SetProperty(ref _selectedAccount, value);
		}

		private ObservableCollection<Account> _filteredAccounts;
		public ObservableCollection<Account> FilteredAccounts
		{
			get => _filteredAccounts;
			set => SetProperty(ref _filteredAccounts, value);
		}

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				if (SetProperty(ref _searchText, value))
				{
					SearchCommand.Execute(SearchText);
				}
			}
		}

		public ICommand LoadAccountsCommand { get; }
		public ICommand AddAccountCommand { get; }
		public ICommand EditAccountCommand { get; }
		public ICommand DeleteAccountCommand { get; }
		public ICommand ViewDetailAccountCommand { get; }
		public ICommand RefreshCommand { get; }
		public ICommand SearchCommand { get; }

		public AccountListViewModel(IDataService dataService, IDialogService dialogService)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			Title = "Danh sách tài khoản";

			LoadAccountsCommand = new AsyncRelayCommand(async _ => await LoadAccountsAsync());
			RefreshCommand = new AsyncRelayCommand(async _ => await RefreshAsync());

			AddAccountCommand = new AsyncRelayCommand(async _ => await AddAccountAsync());

			EditAccountCommand = new AsyncRelayCommand(
				async param => await EditAccountAsync(param as Account ?? SelectedAccount),
				_ => SelectedAccount != null);

			DeleteAccountCommand = new AsyncRelayCommand(
				async param => await DeleteAccountAsync(param as Account ?? SelectedAccount),
				param => (param as Account ?? SelectedAccount) != null);

			ViewDetailAccountCommand = new RelayCommand(
				_ => ViewAccountDetail(),
				_ => SelectedAccount != null);

			SearchCommand = new AsyncRelayCommand(async _ => await SearchAccountsAsync());

			// Khởi tạo
			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			try
			{
				await LoadAccountsAsync().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
		}

		private async Task LoadAccountsAsync()
		{
			IsBusy = true;
			try
			{
				var accounts = await _dataService.GetAllAccountsAsync();
				Accounts = new ObservableCollection<Account>(accounts);
				FilteredAccounts = new ObservableCollection<Account>(accounts);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải danh sách tài khoản: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task RefreshAsync()
		{
			SearchText = string.Empty;
			await LoadAccountsAsync();
		}

		private void FilterAccounts()
		{
			if (Accounts == null)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(SearchText))
			{
				FilteredAccounts = new ObservableCollection<Account>(Accounts);
			}
			else
			{
				var filtered = Accounts.Where(a =>
					a.Username.ToLower().Contains(SearchText.ToLower()) ||
					a.Role.ToLower().Contains(SearchText.ToLower()) ||
					a.Status.ToLower().Contains(SearchText.ToLower()) ||
					(a.Staff?.FullName?.ToLower().Contains(SearchText.ToLower()) ?? false)
				).ToList();

				FilteredAccounts = new ObservableCollection<Account>(filtered);
			}
		}

		private void ViewAccountDetail()
		{
			if (SelectedAccount != null)
			{
				var detailWindow = new AccountDetailWindow(SelectedAccount, _dialogService, _dataService);
				detailWindow.ShowDialog();
			}
		}

		private async Task AddAccountAsync()
		{
			var addAccountWindow = new AddAccountWindow(_dialogService, _dataService);
			var dialogResult = addAccountWindow.ShowDialog();

			if (dialogResult == true && addAccountWindow.NewAccount != null)
			{
				Accounts.Add(addAccountWindow.NewAccount);
				FilterAccounts();
				SelectedAccount = addAccountWindow.NewAccount;
			}
			else if (dialogResult == true)
			{
				await LoadAccountsAsync();
			}
		}

		private async Task EditAccountAsync(Account accountToEdit)
		{
			if (accountToEdit == null)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var editAccountWindow = new EditAccountWindow(accountToEdit, _dialogService, _dataService);
				var dialogResult = editAccountWindow.ShowDialog();

				if (dialogResult == true && editAccountWindow.UpdatedAccount != null)
				{
					var index = Accounts.IndexOf(accountToEdit);
					if (index >= 0)
					{
						Accounts[index] = editAccountWindow.UpdatedAccount;
						SelectedAccount = editAccountWindow.UpdatedAccount;
					}
					FilterAccounts();
				}
				else if (dialogResult == true)
				{
					await LoadAccountsAsync();
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi cập nhật tài khoản: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task DeleteAccountAsync(Account accountToDelete)
		{
			if (accountToDelete == null)
			{
				return;
			}

			var confirm = await _dialogService.ShowConfirmAsync(
				"Xác nhận xóa",
				$"Bạn có chắc muốn xóa tài khoản '{accountToDelete.Username}' không?");

			if (confirm)
			{
				var success = await _dataService.DeleteAccountAsync(accountToDelete.AccountID);
				if (success)
				{
					Accounts.Remove(accountToDelete);
					FilterAccounts();
					await _dialogService.ShowInfoAsync("Thành công", "Đã xóa tài khoản thành công!");
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Không thể xóa tài khoản!");
				}
			}
		}

		private async Task SearchAccountsAsync()
		{
			if (string.IsNullOrWhiteSpace(SearchText))
			{
				await LoadAccountsAsync();
				return;
			}

			try
			{
				IsBusy = true;
				var accounts = await _dataService.SearchAccountsAsync(SearchText);
				Accounts.Clear();

				foreach (var account in accounts)
				{
					Accounts.Add(account);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tìm kiếm dữ liệu: {ex.Message}");
				return;
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}