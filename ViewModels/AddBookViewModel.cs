using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using Microsoft.Win32;

namespace DoanPhamVietDuc.ViewModels
{
	public class AddBookViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;
		private readonly Window _window;
		public event EventHandler<Book> BookSaved;

		private Book _book;
		public Book Book
		{
			get => _book;
			set => SetProperty(ref _book, value);
		}
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

		private ObservableCollection<Language> _languages;
		public ObservableCollection<Language> Languages
		{
			get => _languages;
			set => SetProperty(ref _languages, value);
		}

		private ObservableCollection<Supplier> _supplier;
		public ObservableCollection<Supplier> Suppliers
		{
			get => _supplier;
			set => SetProperty(ref _supplier, value);
		}



		private ObservableCollection<BookCoverType> _bookcovertype;
		public ObservableCollection<BookCoverType> BookCoverTypes
		{
			get => _bookcovertype;
			set => SetProperty(ref _bookcovertype, value);
		}


		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }
		public ICommand BrowseImageCommand { get; }

		public AddBookViewModel(IDialogService dialogService, IDataService dataService, Window window)
		{
			_dialogService = dialogService;
			_dataService = dataService;
			_window = window;

			Title = "Thêm sách mới";
			// Initialize the Book object
			Book = new Book();

			SaveCommand = new AsyncRelayCommand(async _ => await SaveBookAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
			BrowseImageCommand = new RelayCommand(_ => BrowseImage());
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

				Categories = new ObservableCollection<Category>(categories);
				Suppliers = new ObservableCollection<Supplier>(suppliers);
				Languages = new ObservableCollection<Language>(languages);
				BookCoverTypes = new ObservableCollection<BookCoverType>(bookCoverTypes);

				// Thiết lập giá trị mặc định cho Book
				if (Categories.Count > 0)
				{
					Book.CategoryID = Categories[0].CategoryID;
				}
				if (Suppliers.Count > 0)
				{
					Book.SupplierID = Suppliers[0].SupplierID;
				}
				if (Languages.Count > 0)
				{
					Book.LanguageID = Languages[0].LanguageID;
				}
				if (BookCoverTypes.Count > 0)
				{
					Book.BookCoverTypeID = BookCoverTypes[0].BookCoverTypeID;
				}
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
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tiêu tên người thêm");
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
				var success = await _dataService.AddBookAsync(Book);

				if (success)
				{
					// Kích hoạt event
					BookSaved?.Invoke(this, Book);

					await _dialogService.ShowInfoAsync("Thông báo", "Thêm sách mới thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Thêm sách thất bại, vui lòng thử lại");
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
