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
using Microsoft.EntityFrameworkCore.Query;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;

namespace DoanPhamVietDuc.ViewModels
{
    public class AddLanguageViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		public event EventHandler<Language> LanguageCreated;

		private Language _language;
		public Language Language
		{
			get => _language;
			set => SetProperty(ref _language, value);
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public AddLanguageViewModel(IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			Language = new Language();

			SaveCommand = new AsyncRelayCommand(async _ => await SaveLanguageAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
		}

		public async Task SaveLanguageAsync()
		{
			if (string.IsNullOrWhiteSpace(Language.LanguageName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập ngôn ngữ");
				return;
			}

			bool nameExists = await _dataService.IsLanguageNameExistsAsync(Language.LanguageName);
			if (nameExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo", $"Thể loại '{Language.LanguageName}' đã tồn tại, vui lòng chọn tên khác");
				return;
			}
			if (string.IsNullOrWhiteSpace(Language.Code))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mã ngôn ngữ");
				return;
			}

			try
			{
				if (Language == null)
				{
					Language = new Language();
				}
				var success = await _dataService.AddLanguageAsync(Language);

				if (success)
				{
					LanguageCreated?.Invoke(this, Language);
					await _dialogService.ShowInfoAsync("Thông báo", "Thêm ngôn ngữ thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Thêm ngôn ngữ thất bại");
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
