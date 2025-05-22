using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace DoanPhamVietDuc.ViewModels
{
    public class AddBookCoverTypeViewModel: BaseViewModel
    {
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		public event EventHandler<BookCoverType> BookCoverTypeCreated;

		private BookCoverType _bookCoverType;
		public BookCoverType BookCoverType
		{
			get => _bookCoverType;
			set => SetProperty(ref _bookCoverType, value);
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public AddBookCoverTypeViewModel(IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			BookCoverType = new BookCoverType();

			SaveCommand = new AsyncRelayCommand(async _ => await SaveBookCoverTypeAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
		}

		public async Task SaveBookCoverTypeAsync()
		{
			if (string.IsNullOrWhiteSpace(BookCoverType.BookCoverTypeName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên bìa");
				return;
			}

			bool nameExists = await _dataService.IsBookCoverTypeNameExistsAsync(BookCoverType.BookCoverTypeName);
			if (nameExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo", $"Tên bìa '{BookCoverType.BookCoverTypeName}' đã tồn tại, vui lòng chọn tên khác");
				return;
			}
	
			try
			{
				if (BookCoverType == null)
				{
					BookCoverType = new BookCoverType();
				}
				
				var success = await _dataService.AddBookCoverTypeAsync(BookCoverType);

				if (success)
				{
					BookCoverTypeCreated?.Invoke(this, BookCoverType);
					await _dialogService.ShowInfoAsync("Thông báo", "Thêm bìa sách thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Thêm bìa sách thất bại");
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
