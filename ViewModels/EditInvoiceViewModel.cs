using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Invoices;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditInvoiceViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;

		public event EventHandler<Invoice> InvoiceUpdated;

		private Invoice _invoice;
		private Invoice _originalInvoice;
		private List<string> _statusOptions = new List<string> { "Chưa thanh toán", "Đã thanh toán", "Hủy bỏ" };
		private List<string> _paymentMethods = new List<string> { "Tiền mặt", "Chuyển khoản" };
		private List<Staff> _staffList;
		private Staff _selectedStaff;

		public Invoice Invoice
		{
			get => _invoice;
			set => SetProperty(ref _invoice, value);
		}

		public List<string> StatusOptions => _statusOptions;
		public List<string> PaymentMethods => _paymentMethods;
		public List<Staff> StaffList
		{
			get => _staffList;
			set => SetProperty(ref _staffList, value);
		}

		public Staff SelectedStaff
		{
			get => _selectedStaff;
			set
			{
				if (SetProperty(ref _selectedStaff, value))
				{
					if (value != null)
					{
						Invoice.StaffID = value.StaffID;
					}
				}
			}
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }
		public ICommand EditDetailsCommand { get; }

		public EditInvoiceViewModel(Invoice invoice, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			Title = "Sửa thông tin hóa đơn";

			_originalInvoice = invoice;
			_invoice = new Invoice
			{
				InvoiceID = invoice.InvoiceID,
				InvoiceCode = invoice.InvoiceCode,
				InvoiceDate = invoice.InvoiceDate,
				CustomerName = invoice.CustomerName,
				CustomerPhone = invoice.CustomerPhone,
				TotalAmount = invoice.TotalAmount,
				PaymentMethod = invoice.PaymentMethod,
				Status = invoice.Status ?? "Chưa thanh toán",
				Notes = invoice.Notes,
				StaffID = invoice.StaffID,
				CreateBy = invoice.CreateBy,
				CreateTime = invoice.CreateTime
			};
			_staffList = new List<Staff>();

			SaveCommand = new AsyncRelayCommand(async _ => await SaveInvoiceAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
			EditDetailsCommand = new RelayCommand(_ => OpenEditDetailsWindow());

			// Tải dữ liệu tham chiếu
			LoadReferenceDataAsync();
		}

		public async Task LoadReferenceDataAsync()
		{
			try
			{
				// Tải danh sách nhân viên
				var staff = await _dataService.GetAllStaffsAsync();
				StaffList = staff.ToList();
				SelectedStaff = StaffList.FirstOrDefault(s => s.StaffID == Invoice.StaffID);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi tải dữ liệu tham chiếu: {ex.Message}");
			}
		}

		private async Task SaveInvoiceAsync()
		{
			if (string.IsNullOrWhiteSpace(Invoice.InvoiceCode))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mã hóa đơn");
				return;
			}

			if (Invoice.InvoiceDate == default)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn ngày hóa đơn");
				return;
			}

			if (string.IsNullOrWhiteSpace(Invoice.CreateBy))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên người cập nhật");
				return;
			}

			if (string.IsNullOrWhiteSpace(Invoice.Status))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn trạng thái");
				return;
			}

			if (SelectedStaff == null)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn nhân viên");
				return;
			}

			try
			{
				// Bỏ dòng này để giữ nguyên CreateTime
				// Invoice.CreateTime = DateTime.Now;
				var success = await _dataService.UpdateInvoiceInfoAsync(Invoice);

				if (success)
				{
					InvoiceUpdated?.Invoke(this, Invoice);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật thông tin thành công");
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
			var editDetailsWindow = new EditInvoiceDetailsWindow(Invoice, _dataService, _dialogService);
			editDetailsWindow.ShowDialog();

			Invoice = editDetailsWindow.UpdatedInvoice ?? Invoice;
		}

		private void CancelAndClose()
		{
			_window.DialogResult = false;
			_window.Close();
		}
	}
}