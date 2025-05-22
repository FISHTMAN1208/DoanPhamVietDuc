using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Imports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class ImportListViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private ObservableCollection<Import> _imports;
		public ObservableCollection<Import> Imports
		{
			get => _imports;
			set => SetProperty(ref _imports, value);
		}
		private Import _selectedImport;
		public Import SelectedImport
		{
			get => _selectedImport;
			set => SetProperty(ref _selectedImport, value);
		}

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				if (SetProperty(ref _searchText, value))
				{
					SearchImportsCommand.Execute(null);
				}
			}
		}

		public ICommand LoadImportsCommand { get; }
		public ICommand SearchImportsCommand { get; }
		public ICommand AddImportCommand { get; }
		public ICommand EditImportCommand { get; }
		public ICommand DeleteImportCommand { get; }
		public ICommand ViewDetailCommand { get; }
		public ICommand RefreshCommand { get; }

		public ImportListViewModel(IDataService dataService, IDialogService dialogService)
		{ 
			_dataService = dataService;
			_dialogService = dialogService;

			Title = "Quản lý nhập hàng";

			Imports = new ObservableCollection<Import>();

			LoadImportsCommand = new AsyncRelayCommand(async _ => await LoadImportsAsync());
			SearchImportsCommand = new AsyncRelayCommand(async _ => await SearchImportsAsync());
			AddImportCommand = new RelayCommand(_ => AddImportAsync());

			EditImportCommand = new AsyncRelayCommand(
				async param => await EditImportAsync(param as Import ?? SelectedImport),
				_ => SelectedImport != null && SelectedImport.ImportStatus != "Đã nhập");
			DeleteImportCommand = new AsyncRelayCommand(
				async param => await DeleteImportAsync(param as Import ?? SelectedImport),
				_ => SelectedImport != null && SelectedImport.ImportStatus != "Đã nhập");
			ViewDetailCommand = new RelayCommand(
				_ => ViewImportDetail(),
				_ => SelectedImport != null);
			RefreshCommand = new AsyncRelayCommand(async _ => await LoadImportsAsync());

			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			try
			{
				await LoadImportsAsync().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
		}

		private async Task LoadImportsAsync()
		{
			try
			{
				IsBusy = true;

				var imports = await _dataService.GetAllImportsAsync();

				Imports.Clear();
				foreach (var import in imports)
				{
					Imports.Add(import);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải dữ liệu nhập hàng: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task SearchImportsAsync()
		{
			if(string.IsNullOrWhiteSpace(SearchText))
			{
				await LoadImportsAsync();
				return;
			}

			try
			{
				IsBusy = true;

				var imports = await _dataService.SearchImportsAsync(SearchText);

				Imports.Clear();
				foreach (var import in imports)
				{
					Imports.Add(import);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tìm kiếm dữ liệu: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task AddImportAsync()
		{
			var addImportWindow = new AddImportWindow(_dialogService, _dataService);
			var result = addImportWindow.ShowDialog();

			if(result == true)
			{
				LoadImportsAsync();	
			}	
		}

		private async Task EditImportAsync(Import importToEdit)
		{
			if(importToEdit == null)
			{
				return;
			}	

			if(importToEdit.ImportStatus == "Đã nhập")
			{
				await _dialogService.ShowInfoAsync("Thông báo","Không thể sửa phiếu đã nhập");
				return;
			}	

			var editImportWindow = new EditImportWindow(importToEdit, _dataService, _dialogService);
			var result = editImportWindow.ShowDialog();

			if (result == true)
			{
				LoadImportsAsync();
			}
		}

		private async Task DeleteImportAsync(Import importToDelete)
		{
			if(importToDelete == null)
			{
				return;
			}
			if (importToDelete.ImportStatus == "Đã nhập")
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Không thể xóa phiếu đã nhập");
				return;
			}

			var confirm = await _dialogService.ShowConfirmAsync("Xác nhận xóa", $"Bạn có chắc muốn xóa phiếu nhập #{importToDelete.ImportID} không");

			if (confirm == true)
			{
				try
				{
					var success = await _dataService.DeleteImportAsync(importToDelete.ImportID);

					if (success == true)
					{
						Imports.Remove(importToDelete);
						return;
					}
					else
					{
						await _dialogService.ShowInfoAsync("Lỗi", "Lỗi khi xóa phiếu nhập");
					}
				}
				catch (Exception ex)
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Lỗi khi xóa phiếu nhập");
				}
			}
		}

		private async Task ViewImportDetail()
		{
			if(SelectedImport == null)
			{
				return;
			}	
			var detailImportWindow = new ImportDetailWindow(SelectedImport);
			detailImportWindow.ShowDialog();
		}

	}
}
