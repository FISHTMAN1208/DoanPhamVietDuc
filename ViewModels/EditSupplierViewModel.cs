using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;

namespace DoanPhamVietDuc.ViewModels
{
    public class EditSupplierViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;
		private readonly Window _window;
		public event EventHandler<Supplier> SupplierUpdated;

		private Supplier _supplier;
		public Supplier Supplier
		{
			get => _supplier;
			set => SetProperty(ref _supplier, value);
		}

		private List<string> _statusOptions = new List<string> { "Active", "Inactive" };
		public List<string> StatusOptions => _statusOptions;

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public EditSupplierViewModel(Supplier supplier, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dialogService = dialogService;
			_dataService = dataService;
			_window = window;

			Supplier = new Supplier
			{
				SupplierID = supplier.SupplierID,
				SupplierName = supplier.SupplierName,
				SupplierAddress = supplier.SupplierAddress,
				SupplierPhone = supplier.SupplierPhone,
				SupplierEmail = supplier.SupplierEmail,
				TaxCode = supplier.TaxCode,
				Status = supplier.Status
			};

			Title = $"Sửa nhà cung cấp: {supplier.SupplierName}";

			SaveCommand = new AsyncRelayCommand(async _ => await SaveSupplierAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
		}

		private async Task SaveSupplierAsync()
		{
			if (string.IsNullOrWhiteSpace(Supplier.SupplierName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên nhà cung cấp");
				return;
			}
			bool nameExists = await _dataService.IsSupplierNameExistsAsync(
					Supplier.SupplierName,
					Supplier.SupplierID); 

			if (nameExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo",
					$"Nhà cung cấp '{Supplier.SupplierName}' đã tồn tại. Vui lòng chọn tên khác.");
				return;
			}
			if (string.IsNullOrWhiteSpace(Supplier.SupplierPhone))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập số điện thoại");
				return;
			}
			if (string.IsNullOrWhiteSpace(Supplier.SupplierAddress))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập địa chỉ");
				return;
			}
			if (string.IsNullOrWhiteSpace(Supplier.SupplierEmail))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập email");
				return;
			}
			if (string.IsNullOrWhiteSpace(Supplier.TaxCode))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mã thuế");
				return;
			}

			try
			{
				var success = await _dataService.UpdateSupplierAsync(Supplier);

				if (success)
				{
					SupplierUpdated?.Invoke(this, Supplier);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật nhà cung cấp thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Cập nhật nhà cung cấp thất bại, vui lòng thử lại");
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