using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DoanPhamVietDuc.Data;
using DoanPhamVietDuc.Services;
using DoanPhamVietDuc.ViewModels;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Services.NavigationService;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.AuthenticationService;
using DoanPhamVietDuc.Views.Authentication;
using DoanPhamVietDuc.Helpers;

namespace DoanPhamVietDuc
{
	public partial class App : Application
	{
		private readonly ServiceProvider _serviceProvider;
		private LoginWindow _currentLoginWindow;
		private MainWindow _currentMainWindow;
		private bool _isLoggingOut = false; // Thêm flag để track trạng thái logout

		public static T GetService<T>() where T : class
		{
			return ((App)Application.Current)._serviceProvider.GetService<T>();
		}

		public App()
		{
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

			ServiceCollection services = new ServiceCollection();
			ConfigureServices(services);
			_serviceProvider = services.BuildServiceProvider();
		}

		private void ConfigureServices(ServiceCollection services)
		{
			// Đăng ký DbContext
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer("Server=LAPTOPCUADUC\\SQLEXPRESS;Database=BookStoreDB;Trusted_Connection=True;TrustServerCertificate=True");
			});

			services.AddSingleton<Func<ApplicationDbContext>>(provider => () =>
			{
				var options = new DbContextOptionsBuilder<ApplicationDbContext>()
					.UseSqlServer("Server=LAPTOPCUADUC\\SQLEXPRESS;Database=BookStoreDB;Trusted_Connection=True;TrustServerCertificate=True")
					.Options;
				return new ApplicationDbContext(options);
			});

			// Đăng ký các Services
			services.AddTransient<IDataService, DataService>();
			services.AddSingleton<IDialogService, DialogService>();
			services.AddSingleton<INavigationService, NavigationService>();
			services.AddSingleton<IAuthenticationService, AuthenticationService>();

			// Đăng ký các ViewModel
			services.AddTransient<MainViewModel>();
			services.AddTransient<BookListViewModel>();
			services.AddTransient<CategoryListViewModel>();
			services.AddTransient<SupplierListViewModel>();
			services.AddTransient<LanguageListViewModel>();
			services.AddTransient<BookCoverTypeListViewModel>();
			services.AddTransient<ImportListViewModel>();
			services.AddTransient<InvoiceListViewModel>();
			services.AddTransient<StaffListViewModel>();
			services.AddTransient<AccountListViewModel>();
			services.AddTransient<LoginViewModel>();
			services.AddTransient<RegisterViewModel>();
			services.AddTransient<RevenueByCategoryViewModel>();

			// Đăng ký Windows
			services.AddTransient<MainWindow>();
			services.AddTransient<LoginWindow>();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Tạo database và migrate passwords
			using (var scope = _serviceProvider.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				dbContext.Database.EnsureCreated();

				// Password migration
				ShowLoginWindow();
			}
		}

		private void ShowLoginWindow()
		{
			try
			{
				// Đóng MainWindow nếu đang hiển thị (khi logout)
				if (_currentMainWindow != null)
				{
					// Unsubscribe khỏi event trước khi đóng để tránh trigger shutdown
					_currentMainWindow.Closing -= MainWindow_Closing;
					_currentMainWindow.Close();
					_currentMainWindow = null;
				}

				_currentLoginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
				var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();

				// Set DataContext trực tiếp bằng LoginViewModel
				_currentLoginWindow.DataContext = loginViewModel;

				// Subscribe to login success event
				loginViewModel.OnLoginSuccess += OnLoginSuccess;

				// Subscribe to show register event
				loginViewModel.ShowRegisterRequested += ShowRegisterInLoginWindow;

				// Subscribe to window closing event
				_currentLoginWindow.Closing += LoginWindow_Closing;

				_currentLoginWindow.Show();
				System.Diagnostics.Debug.WriteLine("ShowLoginWindow: Hiển thị LoginWindow thành công");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Lỗi ShowLoginWindow: {ex.Message}");
				MessageBox.Show($"Lỗi khi hiển thị LoginWindow: {ex.Message}");
			}
		}

		private void LoginWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("LoginWindow đang đóng");
			// Chỉ thoát ứng dụng nếu không có MainWindow và không đang trong quá trình logout
			if (_currentMainWindow == null && !_isLoggingOut)
			{
				System.Diagnostics.Debug.WriteLine("Thoát ứng dụng do đóng LoginWindow");
				this.Shutdown();
			}
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("MainWindow đang đóng");
			// Chỉ thoát ứng dụng nếu không có LoginWindow và không đang trong quá trình logout
			if (_currentLoginWindow == null && !_isLoggingOut)
			{
				System.Diagnostics.Debug.WriteLine("Thoát ứng dụng do đóng MainWindow");
				this.Shutdown();
			}
		}

		private void ShowRegisterInLoginWindow()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine("ShowRegisterInLoginWindow: Chuyển sang RegisterView");

				var registerViewModel = _serviceProvider.GetRequiredService<RegisterViewModel>();

				// Subscribe to back to login event
				registerViewModel.BackToLoginRequested += ShowLoginInLoginWindow;

				// Chuyển DataContext sang RegisterViewModel
				_currentLoginWindow.DataContext = registerViewModel;

				System.Diagnostics.Debug.WriteLine("ShowRegisterInLoginWindow: Đã chuyển sang RegisterViewModel");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Lỗi ShowRegisterInLoginWindow: {ex.Message}");
				MessageBox.Show($"Lỗi khi chuyển sang Register: {ex.Message}");
			}
		}

		private void ShowLoginInLoginWindow()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine("ShowLoginInLoginWindow: Quay lại LoginView");

				var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();

				// Subscribe to events
				loginViewModel.OnLoginSuccess += OnLoginSuccess;
				loginViewModel.ShowRegisterRequested += ShowRegisterInLoginWindow;

				// Chuyển DataContext về LoginViewModel
				_currentLoginWindow.DataContext = loginViewModel;

				System.Diagnostics.Debug.WriteLine("ShowLoginInLoginWindow: Đã quay lại LoginViewModel");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Lỗi ShowLoginInLoginWindow: {ex.Message}");
				MessageBox.Show($"Lỗi khi quay lại Login: {ex.Message}");
			}
		}

		private void OnLoginSuccess()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine("OnLoginSuccess: Bắt đầu");

				// Tạo MainWindow TRƯỚC KHI đóng LoginWindow
				_currentMainWindow = _serviceProvider.GetRequiredService<MainWindow>();
				var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();

				System.Diagnostics.Debug.WriteLine("OnLoginSuccess: Tạo MainWindow và ViewModel thành công");

				_currentMainWindow.DataContext = mainViewModel;

				// Subscribe to logout event
				mainViewModel.OnLogoutRequested += OnLogoutRequested;

				// Subscribe to MainWindow closing event
				_currentMainWindow.Closing += MainWindow_Closing;

				// Hiển thị MainWindow TRƯỚC KHI đóng LoginWindow
				_currentMainWindow.Show();
				System.Diagnostics.Debug.WriteLine("OnLoginSuccess: Hiển thị MainWindow thành công");

				// Đóng LoginWindow SAU KHI MainWindow đã hiển thị
				if (_currentLoginWindow != null)
				{
					// Unsubscribe khỏi event trước khi đóng
					_currentLoginWindow.Closing -= LoginWindow_Closing;
					_currentLoginWindow.Close();
					_currentLoginWindow = null;
					System.Diagnostics.Debug.WriteLine("OnLoginSuccess: Đóng LoginWindow thành công");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Lỗi OnLoginSuccess: {ex.Message}");
				MessageBox.Show($"Lỗi khi chuyển sang MainWindow: {ex.Message}");
			}
		}

		private void OnLogoutRequested()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine("OnLogoutRequested: Bắt đầu logout");

				// Set flag để báo hiệu đang trong quá trình logout
				_isLoggingOut = true;

				// Reset login form
				var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
				loginViewModel.Username = "";
				loginViewModel.Password = "";

				// Hiển thị LoginWindow (sẽ tự động đóng MainWindow)
				ShowLoginWindow();

				// Reset flag sau khi hoàn thành
				_isLoggingOut = false;

				System.Diagnostics.Debug.WriteLine("OnLogoutRequested: Hoàn thành logout");
			}
			catch (Exception ex)
			{
				_isLoggingOut = false; // Reset flag trong trường hợp có lỗi
				System.Diagnostics.Debug.WriteLine($"Lỗi OnLogoutRequested: {ex.Message}");
				MessageBox.Show($"Lỗi khi logout: {ex.Message}");
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("App đang thoát");
			_serviceProvider?.Dispose();
			base.OnExit(e);
		}
	}
}