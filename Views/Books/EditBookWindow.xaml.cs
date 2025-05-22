using DoanPhamVietDuc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;

namespace DoanPhamVietDuc.Views.Books
{
	/// <summary>
	/// Interaction logic for EditBookWindow.xaml
	/// </summary>
	public partial class EditBookWindow : Window
	{
		private readonly EditBookViewModel _viewModel;
		public Book UpdatedBook { get; private set; }
		public EditBookWindow(Book book, IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();
			_viewModel = new EditBookViewModel(book, dataService,dialogService, this);

			_viewModel.BookUpdated += (sender, updatedBook) => {
				UpdatedBook = updatedBook;
			};

			DataContext = _viewModel;

			Loaded += async (_, __) =>
			{
				await _viewModel.LoadReferenceDataAsync();
			};

		}
	}
}
