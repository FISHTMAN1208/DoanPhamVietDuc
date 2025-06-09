using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.BookCoverTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
    public class BookCoverTypeListViewModel: BaseViewModel
    {
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private ObservableCollection<BookCoverType> _bookcovertypes;
		public ObservableCollection<BookCoverType> BookCoverTypes
		{
			get => _bookcovertypes;
			set => SetProperty(ref _bookcovertypes, value);
		}

		private BookCoverType _selectedBookCoverType;
		public BookCoverType SelectedBookCoverType
		{
			get => _selectedBookCoverType;
			set => SetProperty(ref _selectedBookCoverType, value);
		}

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				if(SetProperty(ref _searchText, value))
				{
					SearchBookCoverTypeCommand.Execute(SearchText);
				}	
			}
		}

		public ICommand LoadBookCoverTypesCommand { get; }
		public ICommand AddBookCoverTypeCommand { get; }
		public ICommand EditBookCoverTypeCommand { get; }
		public ICommand DeleteBookCoverTypeCommand { get; }
		public ICommand SearchBookCoverTypeCommand { get; }
		public ICommand RefreshCommand { get; }
		public ICommand LanguagesCommand { get; }

		public BookCoverTypeListViewModel(IDataService dataService, IDialogService dialogService)
		{
			_dataService = dataService;
			_dialogService = dialogService;

			Title = $"Quản lý bìa sách";

			BookCoverTypes = new ObservableCollection<BookCoverType>();

			LoadBookCoverTypesCommand = new AsyncRelayCommand(async _ => await LoadBookCoverTypesAsync());

			SearchBookCoverTypeCommand = new AsyncRelayCommand(async _ => await SearchBookCoverTypesAsync());

			RefreshCommand = new AsyncRelayCommand(async _ => await LoadBookCoverTypesAsync());

			AddBookCoverTypeCommand = new RelayCommand(_ => AddBookCoverTypeAsync());

			EditBookCoverTypeCommand = new AsyncRelayCommand(
				async param => await EditBookCoverTypeAsync(param as BookCoverType ?? SelectedBookCoverType),
				_ => SelectedBookCoverType != null);

			DeleteBookCoverTypeCommand = new AsyncRelayCommand(
				async param => await DeleteBookCoverTypeAsync(param as BookCoverType ?? SelectedBookCoverType),
				_ => SelectedBookCoverType != null);

			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			try
			{
				await LoadBookCoverTypesAsync().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
		}

		public async Task LoadBookCoverTypesAsync()
		{
			try
			{
				IsBusy = true;

				var selectedBookCoverTypeID = SelectedBookCoverType?.BookCoverTypeID;

				var bookcovertypes = await _dataService.GetAllBookCoverTypesAsync();
				BookCoverTypes.Clear();

				foreach (var bookcovertype in bookcovertypes)
				{
					BookCoverTypes.Add(bookcovertype);
				}

				if (selectedBookCoverTypeID.HasValue)
				{
					SelectedBookCoverType = BookCoverTypes.FirstOrDefault(c => c.BookCoverTypeID == selectedBookCoverTypeID);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải danh sách bìa: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}

		}

		public async Task AddBookCoverTypeAsync()
		{

			var addBookCoverTypeWindow = new AddBookCoverTypeWindow(_dataService, _dialogService);
			var dialogResult = addBookCoverTypeWindow.ShowDialog();

			if (dialogResult == true && addBookCoverTypeWindow.NewBookCoverType != null)
			{
				BookCoverTypes.Add(addBookCoverTypeWindow.NewBookCoverType);
				SelectedBookCoverType = addBookCoverTypeWindow.NewBookCoverType;
			}
			if (dialogResult == true)
			{
				await LoadBookCoverTypesAsync();
			}
		}

		private async Task EditBookCoverTypeAsync(BookCoverType bookCoverTypeToEdit)
		{
			if (bookCoverTypeToEdit == null)
			{
				return;
			}

			try
			{
				var editBookCoverTypeWindow = new EditBookCoverTypeWindow(bookCoverTypeToEdit,_dataService, _dialogService);
				var dialogResult = editBookCoverTypeWindow.ShowDialog();

				if (dialogResult == true && editBookCoverTypeWindow.UpdatedBookCoverType != null)
				{
					await LoadBookCoverTypesAsync();
					SelectedBookCoverType = BookCoverTypes.FirstOrDefault(c => c.BookCoverTypeID == editBookCoverTypeWindow.UpdatedBookCoverType.BookCoverTypeID);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi cập nhật bìa sách: {ex.Message}");
			}
		}

		private async Task DeleteBookCoverTypeAsync(BookCoverType bookCoverTypeToDelete)
		{
			if (bookCoverTypeToDelete == null)
			{
				return;
			}

			if (bookCoverTypeToDelete.Books != null && bookCoverTypeToDelete.Books.Count > 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo",
					$"Không thể xóa ngôn ngữ '{bookCoverTypeToDelete.BookCoverTypeName}' vì có {bookCoverTypeToDelete.Books.Count} sách thuộc danh mục này");
				return;
			}

			var confirm = await _dialogService.ShowConfirmAsync(
				"Xác nhận xóa",
				$"Bạn có chắc muốn xóa bìa sách '{bookCoverTypeToDelete.BookCoverTypeName}' không?");

			if (confirm)
			{
				try
				{
					var success = await _dataService.DeleteBookCoverTypeAsync(SelectedBookCoverType.BookCoverTypeID);
					if (success)
					{
						BookCoverTypes.Remove(bookCoverTypeToDelete);
						SelectedBookCoverType = null;
					}
					else
					{
						await _dialogService.ShowInfoAsync("Lỗi", "Không thể xóa ngôn ngữ!");
					}
				}
				catch (Exception ex)
				{
					await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi xóa ngôn ngữ: {ex.Message}");
				}
			}
		}

		private async Task SearchBookCoverTypesAsync()
		{
			if (string.IsNullOrWhiteSpace(SearchText))
			{
				await LoadBookCoverTypesAsync();
				return;
			}

			try
			{
				IsBusy = true;
				var bookCoverTypes = await _dataService.SearchBookCoverTypesAsync(SearchText);
				BookCoverTypes.Clear();

				foreach (var bookCoverType in bookCoverTypes)
				{
					BookCoverTypes.Add(bookCoverType);
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
