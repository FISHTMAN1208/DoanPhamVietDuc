using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditLanguageViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;
		private readonly Window _window;
		public event EventHandler<Language> LanguageUpdated;

		private Language _language;
		public Language Language
		{
			get => _language;
			set => SetProperty(ref _language, value);
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public EditLanguageViewModel(Language language, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dialogService = dialogService;
			_dataService = dataService;
			_window = window;

			Language = new Language
			{
				LanguageID = language.LanguageID,
				LanguageName= language.LanguageName,
				Code = language.Code,
			};

			Title = $"Sửa nhà cung cấp: {language.LanguageName}";

			SaveCommand = new AsyncRelayCommand(async _ => await SaveLanguageAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
		}

		private async Task SaveLanguageAsync()
		{
			if (string.IsNullOrWhiteSpace(Language.LanguageName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên nhà cung cấp");
				return;
			}
			bool nameExists = await _dataService.IsLanguageNameExistsAsync(
					Language.LanguageName,
					Language.LanguageID); 

			if (nameExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo",
					$"Ngôn ngữ '{Language.LanguageName}' đã tồn tại. Vui lòng thử lại.");
				return;
			}
			
			try
			{
				var success = await _dataService.UpdateLanguageAsync(Language);

				if (success)
				{
					LanguageUpdated?.Invoke(this, Language);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật ngôn ngữ thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Cập nhật ngôn ngữ thất bại, vui lòng thử lại");
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
