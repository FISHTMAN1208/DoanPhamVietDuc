using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DialogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;

namespace DoanPhamVietDuc.ViewModels
{
    public class EditBookCoverTypeViewModel : BaseViewModel
    {
		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;
		private readonly Window _window;
		public event EventHandler<BookCoverType> BookCoverTypeUpdated;

		private BookCoverType _bookCoverType;
		public BookCoverType BookCoverType
		{
			get => _bookCoverType;
			set => SetProperty(ref _bookCoverType, value);
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public EditBookCoverTypeViewModel(BookCoverType bookcovertype, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dialogService = dialogService;
			_dataService = dataService;
			_window = window;

			BookCoverType = new BookCoverType
			{
				BookCoverTypeID = bookcovertype.BookCoverTypeID,
				BookCoverTypeName = bookcovertype.BookCoverTypeName,
			};

			Title = $"Sửa nhà bìa sách: {bookcovertype.BookCoverTypeName}";

			SaveCommand = new AsyncRelayCommand(async _ => await SaveBookCoverTypeAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
		}

		private async Task SaveBookCoverTypeAsync()
		{
			if (string.IsNullOrWhiteSpace(BookCoverType.BookCoverTypeName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên bìa sách");
				return;
			}
			bool nameExists = await _dataService.IsBookCoverTypeNameExistsAsync(
					BookCoverType.BookCoverTypeName,
					BookCoverType.BookCoverTypeID);

			try
			{
				var success = await _dataService.UpdateBookCoverTypeAsync(BookCoverType);

				if (success)
				{
					BookCoverTypeUpdated?.Invoke(this, BookCoverType);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật bìa sách thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Cập nhật bìa sách thất bại, vui lòng thử lại");
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
