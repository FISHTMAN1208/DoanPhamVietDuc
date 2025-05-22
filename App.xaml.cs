using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DoanPhamVietDuc.Data;
using DoanPhamVietDuc.Services;
using DoanPhamVietDuc.ViewModels;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Services.NavigationService;

namespace DoanPhamVietDuc
{
	public partial class App : Application
	{
		private readonly ServiceProvider _serviceProvider;

		public static T GetService<T>() where T : class
		{
			return ((App)Application.Current)._serviceProvider.GetService<T>();
		}
		public App()
		{
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

			// Đăng ký các ViewModel
			services.AddTransient<MainViewModel>();
			services.AddTransient<BookListViewModel>();
			services.AddTransient<CategoryListViewModel>();
			services.AddTransient<SupplierListViewModel>();
			services.AddTransient<LanguageListViewModel>();
			services.AddTransient<BookCoverTypeListViewModel>();
			services.AddTransient<ImportListViewModel>();

			// Đăng ký MainWindow
			services.AddSingleton<MainWindow>(provider => new MainWindow
			{
				DataContext = provider.GetRequiredService<MainViewModel>()
			});
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();

			// Tạo database nếu chưa tồn tại
			using (var scope = _serviceProvider.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				dbContext.Database.EnsureCreated();
			}
		}
	}
}