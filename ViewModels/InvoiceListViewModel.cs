using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Invoices;

namespace DoanPhamVietDuc.ViewModels
{
	public class InvoiceListViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private ObservableCollection<Invoice> _invoices;
		public ObservableCollection<Invoice> Invoices
		{
			get => _invoices;
			set => SetProperty(ref _invoices, value);
		}

		private Invoice _selectedInvoice;
		public Invoice SelectedInvoice
		{
			get => _selectedInvoice;
			set => SetProperty(ref _selectedInvoice, value);
		}

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				if (SetProperty(ref _searchText, value))
				{
					SearchInvoicesCommand.Execute(null);
				}
			}
		}

		public ICommand LoadInvoicesCommand { get; }
		public ICommand SearchInvoicesCommand { get; }
		public ICommand AddInvoiceCommand { get; }
		public ICommand EditInvoiceCommand { get; }
		public ICommand DeleteInvoiceCommand { get; }
		public ICommand ViewDetailCommand { get; }
		public ICommand RefreshCommand { get; }

		public InvoiceListViewModel(IDataService dataService, IDialogService dialogService)
		{
			_dataService = dataService;
			_dialogService = dialogService;

			Title = "Quản lý hóa đơn";

			Invoices = new ObservableCollection<Invoice>();

			LoadInvoicesCommand = new AsyncRelayCommand(async _ => await LoadInvoicesAsync());

			SearchInvoicesCommand = new AsyncRelayCommand(async _ => await SearchInvoiceAsync());

			AddInvoiceCommand = new RelayCommand(_ => AddInvoiceAsync());

			EditInvoiceCommand = new AsyncRelayCommand(
				async param => await EditInvoiceAsync(param as Invoice ?? SelectedInvoice),
				_ => SelectedInvoice != null && SelectedInvoice.Status != "Đã thanh toán");

			DeleteInvoiceCommand = new AsyncRelayCommand(
				async param => await DeleteInvoiceAsync(param as Invoice ?? SelectedInvoice),
				_ => SelectedInvoice != null && SelectedInvoice.Status != "Đã thanh toán");

			ViewDetailCommand = new RelayCommand(
				_ => ViewInvoiceDetail(),
				_ => SelectedInvoice != null);

			RefreshCommand = new AsyncRelayCommand(async _ => await LoadInvoicesAsync());

			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			try
			{
				await LoadInvoicesAsync().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
		}

		private async Task LoadInvoicesAsync()
		{
			try
			{
				IsBusy = true;
				var invoices = await _dataService.GetAllInvoicesAsync();

				Invoices.Clear();
				foreach (var invoice in invoices)
				{
					Invoices.Add(invoice);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task SearchInvoiceAsync()
		{
			if(string.IsNullOrWhiteSpace(SearchText))
			{
				await LoadInvoicesAsync();
				return;
			}

			try
			{
				IsBusy = true;
				var invoices = await _dataService.SearchInvoicesAsync(SearchText);
				Invoices.Clear();

				foreach (var invoice in invoices)
				{
					Invoices.Add(invoice);
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

		private async Task AddInvoiceAsync()
		{
			var addInvoiceWindow = new AddInvoiceWindow(_dialogService, _dataService);
			var result = addInvoiceWindow.ShowDialog();

			if(result == true)
			{
				await LoadInvoicesAsync();
			}	
		}

		private async Task EditInvoiceAsync(Invoice invoiceToEdit)
		{
			if (invoiceToEdit == null)
			{
				return;
			}

			if(invoiceToEdit.Status == "Đã thanh toán")
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Không thể sửa hóa đơn");
				return;
			}	

			var editInvoiceWindow = new EditInvoiceWindow(invoiceToEdit, _dataService, _dialogService);
			var result = editInvoiceWindow.ShowDialog();

			if (result == true)
			{
				await LoadInvoicesAsync();
			}
		}

		private async Task DeleteInvoiceAsync(Invoice invoiceToDelete)
		{
			if (invoiceToDelete == null)
			{
				return;

			}
			if (invoiceToDelete.Status == "Đã thanh toán")
			{
				await _dialogService.ShowInfoAsync("Lỗi", "Không thể hủy hóa đơn đã thanh toán");
				return;
			}

			var confirm = await _dialogService.ShowConfirmAsync("Xác nhận hủy", $"Bạn có chắc chắn muốn hủy hóa đơn #{invoiceToDelete.InvoiceCode} không?");

			if (confirm == true)
			{
				try
				{
					var success = await _dataService.DeleteInvoiceAsync(invoiceToDelete.InvoiceID);

					if (success == true)
					{
						await LoadInvoicesAsync();
					}
					else
					{
						await _dialogService.ShowInfoAsync("Lỗi", "Lỗi khi xóa hóa đơn");
					}
				}
				catch (Exception ex)
				{
					await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi xóa hóa đơn: {ex.Message}");
				}
			}
		}

		private void ViewInvoiceDetail()
		{
			if (SelectedInvoice == null)
			{
				return;
			}

			var detailInvoiceWindow = new InvoiceDetailWindow(SelectedInvoice); 
			detailInvoiceWindow.ShowDialog();
		}

	}
}
