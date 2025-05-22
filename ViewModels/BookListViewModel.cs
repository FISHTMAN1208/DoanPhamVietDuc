using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Books;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class BookListViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		public ObservableCollection<Book> _books;
		public ObservableCollection<Book> Books
		{
			get => _books;
			set => SetProperty(ref _books, value);
		}
		private Book _selectedBook;
		public Book SelectedBook
		{
			get => _selectedBook;
			set => SetProperty(ref _selectedBook, value);
		}

		public ICommand LoadBooksCommand { get; }
		public ICommand ViewDetailCommand { get; }
		public ICommand AddBookCommand { get; }
		public ICommand EditBookCommand { get; }
		public ICommand DeleteBookCommand { get; }

		public BookListViewModel(IDataService dataService, IDialogService dialogService)
		{
			_dataService = dataService;
			_dialogService = dialogService;

			Title = "Danh sách sách";

			LoadBooksCommand = new AsyncRelayCommand(async _ => await LoadBooksAsync());

			DeleteBookCommand = new AsyncRelayCommand(
				async _ => await DeleteBookAsync(),
				_ => SelectedBook != null);

			//Command xem chi tiết sách
			ViewDetailCommand = new RelayCommand(_ => ViewBookDetail(), _=> SelectedBook != null);

			//Command thêm sách
			AddBookCommand = new RelayCommand(_ => AddBookAsync());

			//Command sửa sách
			EditBookCommand = new AsyncRelayCommand(
				async param => await EditBookAsync(param as Book ?? SelectedBook),
			_ => SelectedBook != null);

			// Load books khi khởi tạo
			Task.Run(() => LoadBooksAsync());
		}

		private async Task LoadBooksAsync()
		{
			IsBusy = true;
			try
			{
				var books = await _dataService.GetAllBooksAsync();
				Books = new ObservableCollection<Book>(books);
			}
			finally
			{
				IsBusy = false;
			}
		}
		private void ViewBookDetail()
		{
			if (SelectedBook != null)
			{
				BookDetailWindow detailWindow = new BookDetailWindow(SelectedBook);
				detailWindow.ShowDialog();
				
			}
		}

		private async Task AddBookAsync()
		{
			var addBookWindow = new AddBookWindow(_dialogService, _dataService);
			var dialogResult = addBookWindow.ShowDialog();

			if (dialogResult == true && addBookWindow.Newbook != null)
			{
				
				Books.Add(addBookWindow.Newbook);
				SelectedBook = addBookWindow.Newbook;
			}
			else if (dialogResult == true)
			{
				await LoadBooksAsync();
			}
		}

		private async Task EditBookAsync(Book bookToEdit)
		{
			if(bookToEdit == null)
			{
				return;
			}
			try
			{
				IsBusy = true;

				var editBookWindow = new EditBookWindow(bookToEdit, _dialogService, _dataService);
				var dialogResult = editBookWindow.ShowDialog();

				if (dialogResult == true && editBookWindow.UpdatedBook != null)
				{
					//Cập nhật lại UI
					var index = Books.IndexOf(bookToEdit);

					if (index >= 0)
					{
						Books[index] = editBookWindow.UpdatedBook;
						SelectedBook = editBookWindow.UpdatedBook;
					}
					await LoadBooksAsync();
				}
				else if (dialogResult == true)
				{
					await LoadBooksAsync();
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi cập nhật sách:{ex.Message}");
			}
			finally
			{
				IsBusy = false;	
			}
		}

		private async Task DeleteBookAsync()
		{
			if (SelectedBook == null)
			{
				return;
			}	

			var confirm = await _dialogService.ShowConfirmAsync(
				"Xác nhận xóa",
				$"Bạn có chắc muốn xóa sách '{SelectedBook.Title}' không?");

			if (confirm)
			{
				var success = await _dataService.DeleteBookAsync(SelectedBook.ID);
				if (success)
				{
					Books.Remove(SelectedBook);
					await _dialogService.ShowInfoAsync("Thành công", "Đã xóa sách thành công!");
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Không thể xóa sách!");
				}
			}
		}
	}
}
