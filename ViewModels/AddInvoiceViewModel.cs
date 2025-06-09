using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;

namespace DoanPhamVietDuc.ViewModels
{
	public class AddInvoiceViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;

		private Invoice _invoice;
		public Invoice Invoice
		{
			get => _invoice;
			set => SetProperty(ref _invoice, value);
		}

		private ObservableInvoiceDetail _currentDetail;
		public ObservableInvoiceDetail CurrentDetail
		{
			get => _currentDetail;
			set => SetProperty(ref _currentDetail, value);
		}

		private ObservableCollection<ObservableInvoiceDetail> _invoiceDetails;
		public ObservableCollection<ObservableInvoiceDetail> InvoiceDetails
		{
			get => _invoiceDetails;
			set => SetProperty(ref _invoiceDetails, value);
		}

		private ObservableCollection<Book> _books;
		public ObservableCollection<Book> Books
		{
			get => _books;
			set => SetProperty(ref _books, value);
		}

		private decimal _totalAmount;
		public decimal TotalAmount
		{
			get => _totalAmount;
			set => SetProperty(ref _totalAmount, value);
		}

		private List<string> _statusOptions = new List<string> { "Chưa thanh toán", "Đã thanh toán", "Hủy" };
		public List<string> StatusOptions => _statusOptions;

		private List<string> _paymentMethods = new List<string> { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng" };
		public List<string> PaymentMethods => _paymentMethods;

		public ICommand AddDetailCommand { get; }
		public ICommand DeleteDetailCommand { get; }
		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public AddInvoiceViewModel(IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			Title = "Thêm hóa đơn";

			Invoice = new Invoice
			{
				CreateTime = DateTime.Now,
				StaffID = 1,
				Status = "Chưa thanh toán",
				PaymentMethod = "Tiền mặt"
			};

			InvoiceDetails = new ObservableCollection<ObservableInvoiceDetail>();
			ResetCurrentDetail();

			AddDetailCommand = new RelayCommand(_ => AddDetail(), _ => CanAddDetail());
			DeleteDetailCommand = new RelayCommand(detail => RemoveDetail(detail as ObservableInvoiceDetail));
			SaveCommand = new AsyncRelayCommand(async _ => await SaveInvoiceAsync(), _ => CanSaveInvoice());
			CancelCommand = new RelayCommand(_ => CancelAndClose());

			Task.Run(() => LoadReferenceDataAsync());
		}

		public async Task LoadReferenceDataAsync()
		{
			try
			{
				IsBusy = true;
				var books = await _dataService.GetAllBooksAsync();
				Books = new ObservableCollection<Book>(books);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"không thể tải dữ liệu: {ex.Message}");
				return;
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void ResetCurrentDetail()
		{
			CurrentDetail = new ObservableInvoiceDetail
			{
				Quantity = 1,
				UnitPrice = 1,
				Subtotal = 0
			};
		}

		private async void AddDetail()
		{
			if(CurrentDetail.BookID <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn sách");
				return;
			}	

			if(CurrentDetail.Quantity <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Số lượng phải lớn hơn 0");
				return;
			}	
			if(CurrentDetail.UnitPrice <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Không tìm thấy sách đã chọn");
			}	

			//Tìm sách
			var book = Books.FirstOrDefault(b => b.ID == CurrentDetail.BookID);
			if(book == null)
			{
				_dialogService.ShowInfoAsync("Lỗi", "Không tìm thấy sách đã chọn");
				return;
			}

			var detailToAdd = new ObservableInvoiceDetail
			{
				BookID = book.ID,
				Quantity = CurrentDetail.Quantity,
				UnitPrice = CurrentDetail.UnitPrice,
				Subtotal = CurrentDetail.Quantity * CurrentDetail.UnitPrice,

				Book = new Book
				{
					ID = book.ID,
					Title = book.Title,
					ISBNCode = book.ISBNCode,
					Author = book.Author,
				}
			};

			InvoiceDetails.Add(detailToAdd);
			CalculateTotalAmount();
			ResetCurrentDetail();
		}

		private void RemoveDetail(ObservableInvoiceDetail detail)
		{
            if (detail != null)
			{
				InvoiceDetails.Remove(detail);
				CalculateTotalAmount();
			}	
        }

		private void CalculateTotalAmount()
		{
			TotalAmount = InvoiceDetails.Sum(d => d.Subtotal);
		}

		private bool CanAddDetail()
		{
			return CurrentDetail?.BookID > 0 &&
				   CurrentDetail.Quantity > 0 &&
				   CurrentDetail.UnitPrice > 0;
		}

		private bool CanSaveInvoice()
		{
			return !string.IsNullOrWhiteSpace(Invoice.InvoiceCode) &&
				   !string.IsNullOrWhiteSpace(Invoice.CreateBy) &&
				   InvoiceDetails.Count > 0;
		}

		private async Task SaveInvoiceAsync()
		{
			if (string.IsNullOrWhiteSpace(Invoice.InvoiceCode))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập mã hóa đơn");
				return;
			}
			if (string.IsNullOrWhiteSpace(Invoice.CreateBy))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên người tạo");
				return;
			}
			if(InvoiceDetails.Count <= 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn ít nhất 1 sản phẩm");
				return;
			}

			try
			{
				Invoice.TotalAmount = TotalAmount;
				Invoice.CreateTime = DateTime.Now;
				Invoice.Staff = null;

				var invoiceDetailsList = InvoiceDetails.Select(d => new
				InvoiceDetail
				{
					BookID = d.BookID,
					Quantity = d.Quantity,
					UnitPrice = d.UnitPrice,
					Subtotal = d.Subtotal,
				}).ToList();

				var success = await _dataService.AddInvoiceAsync(Invoice, invoiceDetailsList);

				if (success)
				{
					await _dialogService.ShowInfoAsync("Thông báo", "Tạo hóa đơn thành công");
					_window.DialogResult = true;
					_window.Close();
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi dữ liệu {ex.Message}");
			}
		}

		private void CancelAndClose()
		{
			_window.DialogResult = false;
			_window.Close();
		}
	}
}
