using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoanPhamVietDuc.Data;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Views.Books;
using DoanPhamVietDuc.Helpers.Commands;
using System.Windows;
using System.Collections.ObjectModel;
using DoanPhamVietDuc.Models;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditBookViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		public event EventHandler<Book> BookUpdated;

		private Book _book;
		public Book Book
		{
			get => _book;
			set => SetProperty(ref _book, value);
		}
		private Book _originalBook;

		private BitmapImage _imagePreview;
		public BitmapImage ImagePreview
		{
			get => _imagePreview;
			set => SetProperty(ref _imagePreview, value);
		}

		private bool _hasNoImage = true;
		public bool HasNoImage
		{
			get => _hasNoImage;
			set => SetProperty(ref _hasNoImage, value);
		}

		private ObservableCollection<Category> _categories;
		public ObservableCollection<Category> Categories
		{
			get => _categories;
			set => SetProperty(ref _categories, value);
		}

		private ObservableCollection<Supplier> _suppliers;
		public ObservableCollection<Supplier> Suppliers
		{
			get => _suppliers;
			set => SetProperty(ref _suppliers, value);
		}

		private ObservableCollection<Language> _languages;
		public ObservableCollection<Language> Languages
		{
			get => _languages;
			set => SetProperty(ref _languages, value);
		}

		private ObservableCollection<BookCoverType> _bookcovertypes;
		public ObservableCollection<BookCoverType> BookCoverTypes
		{
			get => _bookcovertypes;
			set => SetProperty(ref _bookcovertypes, value);
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }
		public ICommand BrowseImageCommand { get; }

		public EditBookViewModel(Book book, IDataService dataService, IDialogService dialogService, Window window )
		{
			_dialogService = dialogService;
			_book = book;
			_dataService = dataService;
			_window = window;

			_originalBook = book;

			Book = new Book
			{
				ID = book.ID,
				Title = book.Title,
				Author = book.Author,	
				CategoryID = book.CategoryID,
				SupplierID = book.SupplierID,
				LanguageID = book.LanguageID,
				BookCoverTypeID = book.BookCoverTypeID,
				Quantity = book.Quantity,
				PublisherName = book.PublisherName,
				PublishTime = book.PublishTime,
				PageCount = book.PageCount,
				Price = book.Price,
				ISBNCode = book.ISBNCode,
				Description = book.Description,
				ModifyBy = book.ModifyBy,	
				ModifyTime = book.ModifyTime,
				ImageUrl = book.ImageUrl,
				
			};

			Title = $"Sửa sách";

			if (!string.IsNullOrEmpty(book.ImageUrl))
			{
				try
				{
					ImagePreview = new BitmapImage(new Uri(Book.ImageUrl));
					HasNoImage = false;
				}
				catch
				{
					HasNoImage = true;
				}
			}
			SaveCommand = new AsyncRelayCommand(async _ => await SaveBookAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
			BrowseImageCommand = new RelayCommand(_ => BrowseImage());

			//Colections

			Categories = new ObservableCollection<Category>();
			Suppliers = new ObservableCollection<Supplier>();
			Languages = new ObservableCollection<Language>();
			BookCoverTypes = new ObservableCollection<BookCoverType>();
		}
		
		public async Task LoadReferenceDataAsync()
		{
			try
			{
				IsBusy = true;

				var categories = await _dataService.GetAllCategoriesAsync();
				var suppliers = await _dataService.GetAllSuppliersAsync();
				var languages = await _dataService.GetAllLanguagesAsync();
				var bookCoverTypes = await _dataService.GetAllBookCoverTypesAsync();

				// Debug - in ra số lượng mục trong mỗi danh sách
				//Console.WriteLine($"Categories: {categories.Count}");
				//Console.WriteLine($"Suppliers: {suppliers.Count}");
				//Console.WriteLine($"Languages: {languages.Count}");
				//Console.WriteLine($"BookCoverTypes: {bookCoverTypes.Count}");

				Categories = new ObservableCollection<Category>(categories);
				Suppliers = new ObservableCollection<Supplier>(suppliers);
				Languages = new ObservableCollection<Language>(languages);
				BookCoverTypes = new ObservableCollection<BookCoverType>(bookCoverTypes);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải dữ liệu: {ex.Message}");
				Console.WriteLine($"Error in LoadReferenceDataAsync: {ex}");
			}
			finally
			{
				IsBusy = false;
			}
		}
		private void BrowseImage()
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
				Title = "Chọn ảnh bìa sách"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				// Lưu đường dẫn ảnh
				Book.ImageUrl = openFileDialog.FileName;

				// Hiển thị preview ảnh
				try
				{
					ImagePreview = new BitmapImage(new Uri(Book.ImageUrl));
					HasNoImage = false;
				}
				catch
				{
					HasNoImage = true;
				}
			}
		}

		private async Task SaveBookAsync()
		{
			if (string.IsNullOrWhiteSpace(Book.Title))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tiêu đề sách");
				return;
			}
			if (string.IsNullOrWhiteSpace(Book.Author))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên tác giả");
				return;
			}
			if (string.IsNullOrWhiteSpace(Book.PublisherName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên nhà xuất bản");
				return;
			}
			if (string.IsNullOrWhiteSpace(Book.ModifyBy))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tiêu tên người cập nhật");
				return;
			}
			if (string.IsNullOrWhiteSpace(Book.Description))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mô tả");
				return;
			}
			if (string.IsNullOrWhiteSpace(Book.ISBNCode))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mã ISBN");
				return;
			}
			bool nameExists = await _dataService.IsCategoryNameExistsAsync(Book.ISBNCode);
			if (nameExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo", $"Mã '{Book.ISBNCode}' không hợp lệ, vui lòng chọn tên khác");
				return;
			}
			if (Book.Price <= 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Giá sách phải lớn hơn 0");
				return;
			}
			if (Book.PageCount <= 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Số trang sách phải lớn hơn 0");
				return;
			}

			try
			{
				Book.ModifyTime = DateTime.Now;

				if (string.IsNullOrWhiteSpace(Book.ImageUrl))
				{
					Book.ImageUrl = "/Images/defaultbook.jpg";
				}

				//Lưu sách vào database
				var success = await _dataService.UpdateBookAsync(Book);

				if (success)
				{
					// Kích hoạt event
					BookUpdated?.Invoke(this, Book);

					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật sách thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Cập nhật sách thất bại, vui lòng thử lại");
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Đã xảy ra lỗi: {ex.Message}");
			}
		}

		private void CancelAndClose()
		{
			_window.DialogResult = false;
			_window.Close();	
		}
	}
}
