using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DoanPhamVietDuc.Views.Books
{
    public partial class AddBookWindow : Window
	{
		private readonly AddBookViewModel _viewModel;
		public Book Newbook { get; private set; }

		public AddBookWindow(IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();

			_viewModel = new AddBookViewModel(dialogService, dataService, this);

			// Đăng ký event handler để lưu reference đến Book được tạo
			_viewModel.BookSaved += (sender, book) => {
				Newbook = book;
			};
			DataContext = _viewModel;

			// Gọi sau khi UI đã load để đảm bảo binding an toàn
			Loaded += async (_, __) =>
			{
				await _viewModel.LoadReferenceDataAsync();
			};
		}
	}

}