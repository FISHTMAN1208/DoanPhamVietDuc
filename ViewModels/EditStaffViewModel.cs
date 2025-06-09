using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using Microsoft.Win32;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditStaffViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		public event EventHandler<Staff> StaffUpdated;

		private Staff _staff;
		public Staff Staff
		{
			get => _staff;
			set => SetProperty(ref _staff, value);
		}

		private Staff _originalStaff;

		private ObservableCollection<Import> _imports;
		public ObservableCollection<Import> Imports
		{
			get => _imports;
			set => SetProperty(ref _imports, value);
		}

		private ObservableCollection<Invoice> _invoices;
		public ObservableCollection<Invoice> Invoices
		{
			get => _invoices;
			set => SetProperty(ref _invoices, value);
		}

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public EditStaffViewModel(Staff staff, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dialogService = dialogService;
			_staff = staff;
			_dataService = dataService;
			_window = window;

			_originalStaff = _staff;

			Staff = new Staff
			{
				StaffID = staff.StaffID,
				FullName = staff.FullName,
				Email = staff.Email,
				Phone = staff.Phone,
				Address = staff.Address,
				Position = staff.Position,
				HireDate = staff.HireDate,
				Status = staff.Status,
				BirthDate = staff.BirthDate,	

			};
			Title = "Sửa nhân viên";
			SaveCommand = new AsyncRelayCommand(async _ => await SaveStaffAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());

			//Colections

			Imports = new ObservableCollection<Import>();
			Invoices = new ObservableCollection<Invoice>();
		}

		public async Task LoadReferenceDataAsync()
		{
			try
			{
				IsBusy = true;

				var imports = await _dataService.GetAllImportsAsync();
				//var invoices = await _dataService.GetAllInvoicesAsync();

				Imports = new ObservableCollection<Import>(imports);
				Invoices = new ObservableCollection<Invoice>(Invoices);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải dữ liệu: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task SaveStaffAsync()
		{
			if (string.IsNullOrWhiteSpace(Staff.FullName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên nhân viên");
				return;
			}
			if (string.IsNullOrWhiteSpace(Staff.Phone))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập số điện thoại");
				return;
			}
			if (string.IsNullOrWhiteSpace(Staff.Email))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập email");
				return;
			}
			if (string.IsNullOrWhiteSpace(Staff.Address))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập địa chỉ");
				return;
			}
			if (string.IsNullOrWhiteSpace(Staff.Position))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập vị trí cho nhân viên");
				return;
			}
			// Kiểm tra email trùng lặp (nếu cần)
			bool emailExists = await _dataService.IsStaffEmailExistsAsync(Staff.Email, Staff.StaffID);
			if (emailExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Email đã tồn tại, vui lòng chọn email khác");
				return;
			}
			// Kiểm tra số điện thoại trùng lặp (nếu cần)
			bool phoneExists = await _dataService.IsStaffPhoneExistsAsync(Staff.Phone, Staff.StaffID);
			if (phoneExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Số điện thoại đã tồn tại, vui lòng chọn số khác");
				return;
			}

			try
			{
				var success = await _dataService.EditStaffAsync(Staff);
				if (success)
				{
					StaffUpdated?.Invoke(this, Staff);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật nhân viên thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Cập nhật nhân viên thất bại, vui lòng thử lại");
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
