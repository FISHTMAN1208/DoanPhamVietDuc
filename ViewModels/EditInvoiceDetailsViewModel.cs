using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Views.Invoices; // Giả định bạn có namespace này
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditInvoiceDetailsViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;

		public event EventHandler<Invoice> InvoiceUpdated;

		private Invoice _invoice;
		public Invoice Invoice
		{
			get => _invoice;
			set => SetProperty(ref _invoice, value);
		}

		private ObservableCollection<Book> _books;
		public ObservableCollection<Book> Books
		{
			get => _books;
			set => SetProperty(ref _books, value);
		}

		private ObservableCollection<ObservableInvoiceDetail> _invoiceDetails;
		public ObservableCollection<ObservableInvoiceDetail> InvoiceDetails
		{
			get => _invoiceDetails;
			set => SetProperty(ref _invoiceDetails, value);
		}

		private ObservableInvoiceDetail _currentDetail;
		public ObservableInvoiceDetail CurrentDetail
		{
			get => _currentDetail;
			set => SetProperty(ref _currentDetail, value);
		}

		private ObservableInvoiceDetail _selectedDetail;
		public ObservableInvoiceDetail SelectedDetail
		{
			get => _selectedDetail;
			set => SetProperty(ref _selectedDetail, value);
		}

		private decimal _totalAmount;
		public decimal TotalAmount
		{
			get => _totalAmount;
			set => SetProperty(ref _totalAmount, value);
		}

		private bool _isEditingDetail;
		public bool IsEditingDetail
		{
			get => _isEditingDetail;
			set => SetProperty(ref _isEditingDetail, value);
		}

		private ObservableInvoiceDetail _originalDetail;

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }
		public ICommand AddDetailCommand { get; }
		public ICommand EditDetailCommand { get; }
		public ICommand UpdateDetailCommand { get; }
		public ICommand CancelEditDetailCommand { get; }
		public ICommand RemoveDetailCommand { get; }
		public ICommand ActionCommand { get; }

		public Invoice UpdatedInvoice => Invoice;

		public EditInvoiceDetailsViewModel(Invoice invoice, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			Invoice = new Invoice
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

			Title = $"Sửa chi tiết hóa đơn #{invoice.InvoiceID}";

			Books = new ObservableCollection<Book>();
			InvoiceDetails = new ObservableCollection<ObservableInvoiceDetail>();
			ResetCurrentDetail();

			SaveCommand = new AsyncRelayCommand(async _ => await SaveInvoiceDetailsAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
			AddDetailCommand = new RelayCommand(_ => AddDetail(), _ => CanAddDetail() && !IsEditingDetail);
			EditDetailCommand = new RelayCommand(param => StartEditDetail(param as ObservableInvoiceDetail));
			UpdateDetailCommand = new RelayCommand(_ => UpdateDetail(), _ => CanAddDetail() && IsEditingDetail);
			CancelEditDetailCommand = new RelayCommand(_ => CancelEditDetail());
			RemoveDetailCommand = new RelayCommand(param => RemoveDetail(param as ObservableInvoiceDetail));
			ActionCommand = new RelayCommand(_ =>
			{
				if (IsEditingDetail)
					UpdateDetail();
				else
					AddDetail();
			}, _ => CanAddDetail());

			Task.Run(() => LoadDataAsync());
		}

		public async Task LoadDataAsync()
		{
			try
			{
				IsBusy = true;

				var books = await _dataService.GetAllBooksAsync();
				Books = new ObservableCollection<Book>(books);

				await LoadInvoiceDetailsAsync();
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

		private async Task LoadInvoiceDetailsAsync()
		{
			try
			{
				var fullInvoice = await _dataService.GetInvoiceByIdAsync(Invoice.InvoiceID);
				if (fullInvoice == null || fullInvoice.InvoiceDetails == null)
					return;

				InvoiceDetails.Clear();
				foreach (var detail in fullInvoice.InvoiceDetails)
				{
					var observableDetail = new ObservableInvoiceDetail
					{
						BookID = detail.BookID,
						Quantity = detail.Quantity,
						UnitPrice = detail.UnitPrice,
						Subtotal = detail.Subtotal,
						Book = new Book
						{
							ID = detail.Book?.ID ?? 0,
							Title = detail.Book?.Title,
							ISBNCode = detail.Book?.ISBNCode,
							Author = detail.Book?.Author
						}
					};
					InvoiceDetails.Add(observableDetail);
				}

				CalculateTotalAmount();
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải chi tiết hóa đơn: {ex.Message}");
			}
		}

		private void StartEditDetail(ObservableInvoiceDetail detail)
		{
			if (detail == null) return;

			_originalDetail = new ObservableInvoiceDetail
			{
				BookID = detail.BookID,
				Quantity = detail.Quantity,
				UnitPrice = detail.UnitPrice,
				Subtotal = detail.Subtotal,
				Book = detail.Book
			};

			CurrentDetail = new ObservableInvoiceDetail
			{
				BookID = detail.BookID,
				Quantity = detail.Quantity,
				UnitPrice = detail.UnitPrice,
				Subtotal = detail.Subtotal,
				Book = detail.Book
			};

			IsEditingDetail = true;
			SelectedDetail = detail;
		}

		private void ResetCurrentDetail()
		{
			CurrentDetail = new ObservableInvoiceDetail
			{
				Quantity = 1,
				UnitPrice = 0,
				Subtotal = 0
			};
			IsEditingDetail = false;
		}

		private void AddDetail()
		{
			if (CurrentDetail.BookID <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Vui lòng chọn sách");
				return;
			}

			if (CurrentDetail.Quantity <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Số lượng phải lớn hơn 0");
				return;
			}

			if (CurrentDetail.UnitPrice <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Đơn giá phải lớn hơn 0");
				return;
			}

			var book = Books.FirstOrDefault(b => b.ID == CurrentDetail.BookID);
			if (book == null)
			{
				_dialogService.ShowInfoAsync("Lỗi", "Không tìm thấy sách đã chọn");
				return;
			}

			var detailToAdd = new ObservableInvoiceDetail
			{
				BookID = CurrentDetail.BookID,
				Quantity = CurrentDetail.Quantity,
				UnitPrice = CurrentDetail.UnitPrice,
				Subtotal = CurrentDetail.Quantity * CurrentDetail.UnitPrice,
				Book = new Book
				{
					ID = book.ID,
					Title = book.Title,
					ISBNCode = book.ISBNCode,
					Author = book.Author
				}
			};

			InvoiceDetails.Add(detailToAdd);
			CalculateTotalAmount();
			ResetCurrentDetail();
		}

		private void UpdateDetail()
		{
			if (CurrentDetail.BookID <= 0 || CurrentDetail.Quantity <= 0 || CurrentDetail.UnitPrice <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Vui lòng kiểm tra lại dữ liệu");
				return;
			}

			var book = Books.FirstOrDefault(b => b.ID == CurrentDetail.BookID);
			if (book == null)
			{
				_dialogService.ShowInfoAsync("Lỗi", "Không tìm thấy sách đã chọn");
				return;
			}

			CurrentDetail.Subtotal = CurrentDetail.Quantity * CurrentDetail.UnitPrice;
			CurrentDetail.Book = new Book
			{
				ID = book.ID,
				Title = book.Title,
				ISBNCode = book.ISBNCode,
				Author = book.Author
			};

			var index = InvoiceDetails.IndexOf(SelectedDetail);
			if (index >= 0)
			{
				InvoiceDetails[index] = CurrentDetail;
			}

			CalculateTotalAmount();
			ResetCurrentDetail();
			SelectedDetail = null;
		}

		private void CancelEditDetail()
		{
			ResetCurrentDetail();
			SelectedDetail = null;
		}

		private void RemoveDetail(ObservableInvoiceDetail detail)
		{
			if (detail != null)
			{
				InvoiceDetails.Remove(detail);
				CalculateTotalAmount();

				if (IsEditingDetail && SelectedDetail == detail)
				{
					CancelEditDetail();
				}
			}
		}

		private void CalculateTotalAmount()
		{
			TotalAmount = InvoiceDetails.Sum(d => d.Subtotal);
			Invoice.TotalAmount = TotalAmount;
		}

		private bool CanAddDetail()
		{
			return CurrentDetail?.BookID > 0 &&
				   CurrentDetail.Quantity > 0 &&
				   CurrentDetail.UnitPrice > 0;
		}

		private async Task SaveInvoiceDetailsAsync()
		{
			if (InvoiceDetails.Count == 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng thêm ít nhất một sản phẩm");
				return;
			}

			try
			{
				var invoiceDetails = InvoiceDetails.Select(d => new InvoiceDetail
				{
					InvoiceID = Invoice.InvoiceID,
					BookID = d.BookID,
					Quantity = d.Quantity,
					UnitPrice = d.UnitPrice,
					Subtotal = d.Subtotal
				}).ToList();

				var success = await _dataService.UpdateInvoiceDetailsAsync(Invoice.InvoiceID, invoiceDetails);

				if (success)
				{
					Invoice.TotalAmount = TotalAmount;
					await _dataService.UpdateInvoiceTotalAmount(Invoice.InvoiceID, TotalAmount);

					InvoiceUpdated?.Invoke(this, Invoice);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật chi tiết hóa đơn thành công");
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

		private void CancelAndClose()
		{
			_window.DialogResult = false;
			_window.Close();
		}
	}
}