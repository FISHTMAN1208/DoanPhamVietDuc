using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Languages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
    public class LanguageListViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private ObservableCollection<Language> _languages;
		public ObservableCollection<Language> Languages
		{
			get => _languages;
			set => SetProperty(ref _languages, value);
		}

		private Language _selectedLanguage;
		public Language SelectedLanguage
		{
			get => _selectedLanguage;
			set => SetProperty(ref _selectedLanguage, value);
		}

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				if(SetProperty(ref _searchText, value))
				{
					SearchLanguageCommand.Execute(SearchText);
				}	
			}
		}

		public ICommand LoadLanguagesCommand { get; }
		public ICommand AddLanguageCommand { get; }
		public ICommand EditLanguageCommand { get; }
		public ICommand DeleteLanguageCommand { get; }
		public ICommand SearchLanguageCommand { get; }
		public ICommand RefreshCommand { get; }
		public ICommand LanguagesCommand { get; }

		public LanguageListViewModel(IDataService dataService, IDialogService dialogService)
		{
			_dataService = dataService;
			_dialogService = dialogService;

			Title = $"Quản lý thể loại";

			Languages = new ObservableCollection<Language>();

			LoadLanguagesCommand = new AsyncRelayCommand(async _ => await LoadLanguagesAsync());

			SearchLanguageCommand = new AsyncRelayCommand(async _ => await SearchLanguagesAsync());

			RefreshCommand = new AsyncRelayCommand(async _ => await LoadLanguagesAsync());

			AddLanguageCommand = new RelayCommand(_ => AddLanguageAsync());

			EditLanguageCommand = new AsyncRelayCommand(
				async param => await EditLanguageAsync(param as Language ?? SelectedLanguage),
				_ => SelectedLanguage != null);

			DeleteLanguageCommand = new AsyncRelayCommand(
				async param => await DeleteLanguageAsync(param as Language ?? SelectedLanguage),
				_ => SelectedLanguage != null);

			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			try
			{
				await LoadLanguagesAsync().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
		}

		public async Task LoadLanguagesAsync()
		{
			try
			{
				IsBusy = true;

				var selectedLanguageID = SelectedLanguage?.LanguageID;

				var languages = await _dataService.GetAllLanguagesAsync();
				Languages.Clear();

				foreach (var language in languages)
				{
					Languages.Add(language);
				}

				if (selectedLanguageID.HasValue)
				{
					SelectedLanguage = Languages.FirstOrDefault(c => c.LanguageID == selectedLanguageID);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải danh sách ngon ngu: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}

		}

		public async Task AddLanguageAsync()
		{

			var addLanguageWindow = new AddLanguageWindow(_dataService, _dialogService);
			var dialogResult = addLanguageWindow.ShowDialog();

			if (dialogResult == true && addLanguageWindow.NewLanguage != null)
			{
				Languages.Add(addLanguageWindow.NewLanguage);
				SelectedLanguage = addLanguageWindow.NewLanguage;
			}
			if (dialogResult == true)
			{
				await LoadLanguagesAsync();
			}
		}

		private async Task EditLanguageAsync(Language languageToEdit)
		{
			if (languageToEdit == null)
			{
				return;
			}

			try
			{
				var editLanguageWindow = new EditLanguageWindow(languageToEdit, _dialogService, _dataService);
				var dialogResult = editLanguageWindow.ShowDialog();

				if (dialogResult == true && editLanguageWindow.UpdatedLanguage != null)
				{
					await LoadLanguagesAsync();
					SelectedLanguage = Languages.FirstOrDefault(c => c.LanguageID == editLanguageWindow.UpdatedLanguage.LanguageID);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi cập nhật ngon ngu: {ex.Message}");
			}
		}

		private async Task DeleteLanguageAsync(Language languageToDelete)
		{
			if (languageToDelete == null)
			{
				return;
			}

			if (languageToDelete.Books != null && languageToDelete.Books.Count > 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo",
					$"Không thể xóa ngôn ngữ '{languageToDelete.LanguageName}' vì có {languageToDelete.Books.Count} sách thuộc danh mục này");
				return;
			}

			var confirm = await _dialogService.ShowConfirmAsync(
				"Xác nhận xóa",
				$"Bạn có chắc muốn xóa ngôn ngữ '{languageToDelete.LanguageName}' không?");

			if (confirm)
			{
				try
				{
					var success = await _dataService.DeleteLanguageAsync(SelectedLanguage.LanguageID);
					if (success)
					{
						Languages.Remove(languageToDelete);
						SelectedLanguage = null;
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

		private async Task SearchLanguagesAsync()
		{
			if (string.IsNullOrWhiteSpace(SearchText))
			{
				await LoadLanguagesAsync();
				return;
			}

			try
			{
				IsBusy = true;
				var languages = await _dataService.SearchLanguagesAsync(SearchText);
				Languages.Clear();

				foreach (var language in languages)
				{
					Languages.Add(language);
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
