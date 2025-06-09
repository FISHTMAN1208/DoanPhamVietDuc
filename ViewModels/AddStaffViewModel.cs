using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;

namespace DoanPhamVietDuc.ViewModels
{
	public class AddStaffViewModel: BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		public event EventHandler<Staff> StaffSaved;


		private Staff _staff;
		public Staff Staff
		{
			get => _staff;
			set => SetProperty(ref _staff, value);
		}

		private ObservableCollection<string> _statusOptions;
		public ObservableCollection<string> StatusOptions
		{
			get => _statusOptions;
			set => SetProperty( ref _statusOptions, value);
		}

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

		public AddStaffViewModel( IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			Title = "Thêm nhân viên";
			Staff = new Staff();

			StatusOptions = new ObservableCollection<string> { "Đang làm", "Nghỉ việc" };
			SaveCommand = new AsyncRelayCommand(async _ => await SaveStaffAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());

			_ = LoadReferenceDataAsync();
		}

		public async Task LoadReferenceDataAsync()
		{
			try
			{
				IsBusy = true;

				var imports = await _dataService.GetAllImportsAsync();
				var invoices = await _dataService.GetAllInvoicesAsync();

				Imports = new ObservableCollection<Import>(imports);
				Invoices = new ObservableCollection<Invoice>(invoices);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể lấy dữ liệu{ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		public async Task SaveStaffAsync()
		{
			try
			{
				IsBusy = true;

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

				var success = await _dataService.AddStaffAsync(Staff);

				if (success)
				{
					StaffSaved?.Invoke(this, Staff);
					await _dialogService.ShowInfoAsync("Thông báo", "Thêm nhân viên thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Thêm nhân viên thất bại");
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Đã xảy ra lỗi {ex.Message}");
			}
			finally
			{
				IsBusy = false;	
			}
		}

		private void CancelAndClose()
		{
			_window.DialogResult = false;
			_window.Close();
		}
	}
}
