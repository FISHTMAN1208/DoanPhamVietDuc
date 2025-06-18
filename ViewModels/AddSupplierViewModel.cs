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
using System.Text.RegularExpressions;

namespace DoanPhamVietDuc.ViewModels
{
    public class AddSupplierViewModel : BaseViewModel
	{
		private List<string> _statusOptions = new List<string> { "Active", "Inactive" };
		public List<string> StatusOptions => _statusOptions;
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		public event EventHandler<Supplier> SupplierCreated;
		private HashSet<string> _existingSupplierNames;

		private Supplier _supplier;
		public Supplier Supplier
		{
			get => _supplier;
			set => SetProperty(ref _supplier, value);
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public AddSupplierViewModel(IDataService dataService, IDialogService dialogService, Window window)
		{
			Supplier = new Supplier();
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			SaveCommand = new AsyncRelayCommand(async _ => await SaveSupplierAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
		}

		public async Task SaveSupplierAsync()
		{

			if (string.IsNullOrWhiteSpace(Supplier.SupplierName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên nhà cung cấp");
				return;
			}

			bool nameExists = await _dataService.IsSupplierNameExistsAsync(Supplier.SupplierName);
			if (nameExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo",$"Nhà cung cấp '{Supplier.SupplierName}' đã tồn tại. Vui lòng chọn tên khác.");
				return;
			}

			if (string.IsNullOrWhiteSpace(Supplier.SupplierAddress))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập địa chỉ");
				return;
			}

			if (string.IsNullOrWhiteSpace(Supplier.SupplierPhone))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập số điện thoại");
				return;
			}
			string phonePattern = @"^(?:0)(3[2-9]|5[689]|7[0|6-9]|8[1-9]|9[0-9])[0-9]{7}$";
			if (!Regex.IsMatch(Supplier.SupplierPhone, phonePattern))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại hợp lệ");
				return;
			}
			if (string.IsNullOrWhiteSpace(Supplier.SupplierEmail))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập email");
				return;
			}
			string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
			if (!Regex.IsMatch(Supplier.SupplierEmail, emailPattern))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Email không hợp lệ. Vui lòng nhập email đúng định dạng (ví dụ: user@domain.com).");
				return;
			}
			if (string.IsNullOrWhiteSpace(Supplier.TaxCode))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mã thuế");
				return;
			}

			try
			{
				if (Supplier == null)
				{
					Supplier = new Supplier();
				}
				if (string.IsNullOrWhiteSpace(Supplier.Status))
				{
					Supplier.Status = "Active";
				}

				var success = await _dataService.AddSupplierAsync(Supplier);

				if (success)
				{
					SupplierCreated?.Invoke(this, Supplier);
					await _dialogService.ShowInfoAsync("Thông báo", "Thêm nhà cung cấp thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Thêm nhà cung cấp thất bại");
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
