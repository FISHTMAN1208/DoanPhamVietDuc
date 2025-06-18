using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
    public class AddImportViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		private event EventHandler<Import> ImportSaved;

		private Import _import;
		public Import Import
		{
			get => _import;
			set => SetProperty(ref _import, value);
		}


		private ObservableImportDetail _currentDetail;
		public ObservableImportDetail CurrentDetail
		{
			get => _currentDetail;
			set => SetProperty(ref _currentDetail, value);
		}

		private ObservableCollection<ObservableImportDetail> _importDetails;
		public ObservableCollection<ObservableImportDetail> ImportDetails
		{
			get => _importDetails;
			set => SetProperty(ref _importDetails, value);
		}

		private ObservableCollection<Book> _books;
		public ObservableCollection<Book> Books
		{
			get => _books;
			set => SetProperty(ref _books, value);
		}

		private ObservableCollection<Supplier> _suppliers;
		public ObservableCollection<Supplier> Suppliers
		{
			get => _suppliers;
			set => SetProperty(ref _suppliers, value);
		}

		private decimal _totalAmount;
		public decimal TotalAmount
		{
			get => _totalAmount;
			set => SetProperty(ref _totalAmount, value);
		}

		private List<string> _statusOptions = new List<string> { "Đang xử lý", "Đã nhập", "Hủy bỏ" };
		public List<string> StatusOptions => _statusOptions;

		public ICommand AddDetailCommand { get; }
		public ICommand RemoveDetailCommand { get; }
		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public AddImportViewModel(IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			Title = "Thêm phiếu nhập";

			Import = new Import
			{
				ImportDate = DateTime.Now,
				CreateTime = DateTime.Now,
				StaffID = 1,
				SupplierID = 1,
				ImportStatus = "Đang xử lý"
			};
			ImportDetails = new ObservableCollection<ObservableImportDetail>();

			// Khởi tạo 1 detail rỗng
			ResetCurrentDetail();

			AddDetailCommand = new RelayCommand(_ => AddDetail(), _ => CanAddDetail());
			RemoveDetailCommand = new RelayCommand(detail => RemoveDetail(detail as ObservableImportDetail));
			SaveCommand = new AsyncRelayCommand(async _ => await SaveImportAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());

			Task.Run(() => LoadReferenceDataAsync());
		}

		private async Task LoadReferenceDataAsync()
		{
			try
			{
				IsBusy = true;

				var suppliers = await _dataService.GetAllSuppliersAsync();
				var books = await _dataService.GetAllBooksAsync();

				Suppliers = new ObservableCollection<Supplier>(suppliers);
				Books = new ObservableCollection<Book>(books);

				
				if (Suppliers.Count > 0)
				{
					Import.SupplierID = Suppliers[0].SupplierID;
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể lấy dữ liệu {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void ResetCurrentDetail()
		{
			CurrentDetail = new ObservableImportDetail
			{
				Quantity = 1,
				UnitImportPrice = 0,
				Subtotal = 0
			};
		}

		private void AddDetail()
		{
			Console.WriteLine($"BookID: {CurrentDetail.BookID}, Quantity: {CurrentDetail.Quantity}, UnitImportPrice: {CurrentDetail.UnitImportPrice}");

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

			if (CurrentDetail.UnitImportPrice <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Đơn giá nhập phải lớn hơn 0");
				return;
			}

			// Tìm sách
			var book = Books.FirstOrDefault(b => b.ID == CurrentDetail.BookID);
			if (book == null)
			{
				_dialogService.ShowInfoAsync("Lỗi", "Không tìm thấy sách đã chọn");
				return;
			}

			var detailToAdd = new ObservableImportDetail
			{
				BookID = book.ID,
				Quantity = CurrentDetail.Quantity,
				UnitImportPrice = CurrentDetail.UnitImportPrice,
				Subtotal = CurrentDetail.Quantity * CurrentDetail.UnitImportPrice
			};

			// Lưu trữ thông tin hiển thị của Book 
			detailToAdd.Book = new Book
			{
				ID = book.ID,
				Title = book.Title,
				ISBNCode = book.ISBNCode,
				Author = book.Author
			};

			// Thêm vào danh sách
			ImportDetails.Add(detailToAdd);

			CalculateTotalAmount();

			ResetCurrentDetail();
		}

		private void RemoveDetail(ObservableImportDetail detail)
		{
			if (detail != null)
			{
				ImportDetails.Remove(detail);
				CalculateTotalAmount();
			}
		}

		private void CalculateTotalAmount()
		{
			TotalAmount = ImportDetails.Sum(d => d.Subtotal);
		}

		private bool CanAddDetail()
		{
			return CurrentDetail?.BookID > 0 &&
				   CurrentDetail.Quantity > 0 &&
				   CurrentDetail.UnitImportPrice > 0;
		}

		private async Task SaveImportAsync()
		{
			var errors = new List<string>();

			if (Import.SupplierID <= 0) errors.Add("Vui lòng chọn nhà cung cấp");
			if (string.IsNullOrWhiteSpace(Import.CreateBy)) errors.Add("Vui lòng nhập tên người tạo");
			if (string.IsNullOrWhiteSpace(Import.Notes)) errors.Add("Vui lòng nhập ghi chúb");
			if (ImportDetails.Count == 0) errors.Add("Vui lòng thêm ít nhất một sản phẩm");
			if (string.IsNullOrWhiteSpace(Import.ImportStatus) || !StatusOptions.Contains(Import.ImportStatus));

			if (errors.Any())
			{
				await _dialogService.ShowInfoAsync("Thông báo", string.Join("\n", errors));
				return;
			}

			try
			{
				Import.TotalAmount = TotalAmount;
				Import.CreateTime = DateTime.Now;

				Import.Supplier = null;
				Import.Staff = null;

				var importDetailsList = ImportDetails.Select(d => new ImportDetail
				{
					BookID = d.BookID,
					Quantity = d.Quantity,
					UnitImportPrice = d.UnitImportPrice,
					Subtotal = d.Subtotal
				}).ToList();

				var success = await _dataService.AddImportAsync(Import, importDetailsList);

				if (success)
				{
					await _dialogService.ShowInfoAsync("Thông báo", "Tạo phiếu nhập thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Tạo phiếu nhập thất bại, vui lòng thử lại");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception in SaveImportAsync: {ex.Message}");
				if (ex.InnerException != null)
				{
					Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
				}
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
