using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Views.Imports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditImportViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;

		public event EventHandler<Import> ImportUpdated;

		private Import _import;
		public Import Import
		{
			get => _import;
			set => SetProperty(ref _import, value);
		}

		private Import _originalImport;

		private ObservableCollection<Supplier> _suppliers;
		public ObservableCollection<Supplier> Suppliers
		{
			get => _suppliers;
			set => SetProperty(ref _suppliers, value);
		}

		private List<string> _statusOptions = new List<string> { "Đang xử lý", "Đã nhập", "Hủy bỏ" };
		public List<string> StatusOptions => _statusOptions;

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }
		public ICommand EditDetailsCommand { get; }

		public EditImportViewModel(Import import, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			_originalImport = import;

			// Tạo bản sao của import để tránh thay đổi đối tượng gốc
			Import = new Import
			{
				ImportID = import.ImportID,
				SupplierID = import.SupplierID,
				StaffID = import.StaffID,
				ImportDate = import.ImportDate,
				ImportStatus = import.ImportStatus ?? "Đang xử lý",
				TotalAmount = import.TotalAmount,
				Notes = import.Notes,
				CreateBy = import.CreateBy,
				CreateTime = import.CreateTime,
			};

			Title = "Sửa thông tin phiếu nhập";

			// Khởi tạo collections
			Suppliers = new ObservableCollection<Supplier>();

			// Đăng ký commands
			SaveCommand = new AsyncRelayCommand(async _ => await SaveImportAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
			EditDetailsCommand = new RelayCommand(_ => OpenEditDetailsWindow());
		}

		public async Task LoadReferenceDataAsync()
		{
			try
			{
				IsBusy = true;

				// Load suppliers từ service
				var suppliers = await _dataService.GetAllSuppliersAsync();

				// Gán vào collections
				Suppliers = new ObservableCollection<Supplier>(suppliers);
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

		private async Task SaveImportAsync()
		{
			// Kiểm tra dữ liệu đầu vào
			if (Import.SupplierID == 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn nhà cung cấp");
				return;
			}

			if (Import.ImportDate == default)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn ngày nhập");
				return;
			}

			if (string.IsNullOrWhiteSpace(Import.CreateBy))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên người cập nhật");
				return;
			}

			if (string.IsNullOrWhiteSpace(Import.ImportStatus))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn trạng thái");
				return;
			}

			try
			{
				// Cập nhật thời gian
				Import.CreateTime = DateTime.Now;

				// Gọi service để cập nhật
				var success = await _dataService.UpdateImportInfoAsync(Import);

				if (success)
				{
					ImportUpdated?.Invoke(this, Import);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật thông tin phiếu nhập thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Cập nhật thất bại, vui lòng thử lại");
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Đã xảy ra lỗi: {ex.Message}");
			}
		}

		private void OpenEditDetailsWindow()
		{
			var editDetailsWindow = new EditImportDetailsWindow(Import, _dataService, _dialogService);
			editDetailsWindow.ShowDialog();

			// Cập nhật lại tổng tiền sau khi sửa chi tiết
			Import = editDetailsWindow.UpdatedImport ?? Import;
		}

		private void CancelAndClose()
		{
			_window.DialogResult = false;
			_window.Close();
		}
	}
}