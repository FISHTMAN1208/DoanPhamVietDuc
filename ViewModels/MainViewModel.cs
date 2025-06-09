// MainViewModel.cs - ViewModels/MainViewModel.cs
using DoanPhamVietDuc.Constants;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService;
using DoanPhamVietDuc.Services.NavigationService;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		private readonly INavigationService _navigationService;
		private readonly IAuthenticationService _authService;

		public BaseViewModel CurrentViewModel => _navigationService.CurrentViewModel;

		// User info
		public CurrentUser CurrentUser => _authService.CurrentUser;
		public string UserDisplayName => CurrentUser?.DisplayName ?? "Chưa đăng nhập";
		public string UserRole => CurrentUser?.Account?.Role ?? "";

		// Navigation Commands
		public ICommand NavigateToBookListCommand { get; }
		public ICommand NavigateToCategoryManagerCommand { get; }
		public ICommand NavigateToSupplierManagerCommand { get; }
		public ICommand NavigateToLanguageManagerCommand { get; }
		public ICommand NavigateToBookCoverTypeManagerCommand { get; }
		public ICommand NavigateToImportManagerCommand { get; }
		public ICommand NavigateToInvoiceManagerCommand { get; }
		public ICommand NavigateToStatisticsCommand { get; }
		public ICommand NavigateToStaffManagerCommand { get; }
		public ICommand NavigateToAccountManagerCommand { get; }

		// Auth Commands
		public ICommand LogoutCommand { get; }
		public ICommand ShowProfileCommand { get; }

		// Permission Properties
		public bool CanViewBooks => HasPermission(Permissions.CanViewBooks);
		public bool CanManageCategories => HasPermission(Permissions.CanManageCategories);
		public bool CanManageSuppliers => HasPermission(Permissions.CanManageSuppliers);
		public bool CanManageLanguages => HasPermission(Permissions.CanViewLanguages);
		public bool CanManageBookCoverTypes => HasPermission(Permissions.CanViewBookCoverType);
		public bool CanViewImports => HasPermission(Permissions.CanViewImports);
		public bool CanViewInvoices => HasPermission(Permissions.CanViewInvoices);
		public bool CanViewReports => HasPermission(Permissions.CanViewReports);
		public bool CanManageStaff => HasPermission(Permissions.CanViewStaff);
		public bool CanManageAccounts => HasPermission(Permissions.CanManageSystem);

		public MainViewModel(INavigationService navigationService, IAuthenticationService authService)
		{
			_navigationService = navigationService;
			_authService = authService;

			Title = "Quản lý sách";

			// Initialize commands
			NavigateToBookListCommand = new RelayCommand(_ => _navigationService.NavigateTo<BookListViewModel>(),
														_ => CanViewBooks);

			NavigateToCategoryManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<CategoryListViewModel>(),
															   _ => CanManageCategories);

			NavigateToSupplierManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<SupplierListViewModel>(),
															   _ => CanManageSuppliers);

			NavigateToLanguageManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<LanguageListViewModel>(),
															   _ => CanManageLanguages);

			NavigateToBookCoverTypeManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<BookCoverTypeListViewModel>(),
																	_ => CanManageBookCoverTypes);

			NavigateToStaffManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<StaffListViewModel>(),
															_ => CanManageStaff);

			NavigateToImportManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<ImportListViewModel>(),
															 _ => CanViewImports);

			NavigateToInvoiceManagerCommand = new RelayCommand(_ => _navigationService.NavigateTo<InvoiceListViewModel>(),
															_ => CanViewInvoices);

			NavigateToStatisticsCommand = new RelayCommand(_ => {
				 _navigationService.NavigateTo<RevenueByCategoryViewModel>();
			}, _ => CanViewReports);

			NavigateToAccountManagerCommand = new RelayCommand(_ => {
				 _navigationService.NavigateTo<AccountListViewModel>();
			}, _ => CanManageAccounts);

			LogoutCommand = new AsyncRelayCommand(_ => ExecuteLogout());
			ShowProfileCommand = new RelayCommand(_ => ShowProfile());

			// Subscribe to user changes
			_authService.UserChanged += OnUserChanged;

			// Navigate to default view if authenticated
			if (_authService.IsAuthenticated)
			{
				_navigationService.NavigateTo<BookListViewModel>();
			}

			// Subscribe to navigation changes
			_navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
		}

		private bool HasPermission(string permission)
		{
			return CurrentUser?.HasPermission(permission) ?? false;
		}

		private async Task ExecuteLogout()
		{
			try
			{
				_authService.LogoutAsync();
				OnLogoutRequested?.Invoke();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");
			}
		}

		private void ShowProfile()
		{
			// Navigate to profile view
			// _navigationService.NavigateTo<ProfileViewModel>();
		}

		private void OnUserChanged(CurrentUser user)
		{
			// Refresh all properties
			RefreshUserProperties();
			RefreshPermissionProperties();
			RefreshCommands();
		}

		private void RefreshUserProperties()
		{
			OnPropertyChanged(nameof(CurrentUser));
			OnPropertyChanged(nameof(UserDisplayName));
			OnPropertyChanged(nameof(UserRole));
		}

		private void RefreshPermissionProperties()
		{
			OnPropertyChanged(nameof(CanViewBooks));
			OnPropertyChanged(nameof(CanManageCategories));
			OnPropertyChanged(nameof(CanManageSuppliers));
			OnPropertyChanged(nameof(CanManageLanguages));
			OnPropertyChanged(nameof(CanManageBookCoverTypes));
			OnPropertyChanged(nameof(CanViewImports));
			OnPropertyChanged(nameof(CanViewInvoices));
			OnPropertyChanged(nameof(CanViewReports));
			OnPropertyChanged(nameof(CanManageStaff));
			OnPropertyChanged(nameof(CanManageAccounts));
		}

		private void RefreshCommands()
		{
			(NavigateToBookListCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToCategoryManagerCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToSupplierManagerCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToLanguageManagerCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToBookCoverTypeManagerCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToImportManagerCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToInvoiceManagerCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToStatisticsCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToStaffManagerCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(NavigateToAccountManagerCommand as RelayCommand)?.RaiseCanExecuteChanged();
		}

		private void OnCurrentViewModelChanged()
		{
			OnPropertyChanged(nameof(CurrentViewModel));
		}

		// Event để thông báo cần logout
		public event Action OnLogoutRequested;
	}
}