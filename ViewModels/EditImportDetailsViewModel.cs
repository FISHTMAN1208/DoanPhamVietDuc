using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Helpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditImportDetailsViewModel : BaseViewModel
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

		private ObservableCollection<Book> _books;
		public ObservableCollection<Book> Books
		{
			get => _books;
			set => SetProperty(ref _books, value);
		}

		private ObservableCollection<ObservableImportDetail> _importDetails;
		public ObservableCollection<ObservableImportDetail> ImportDetails
		{
			get => _importDetails;
			set => SetProperty(ref _importDetails, value);
		}

		private ObservableImportDetail _currentDetail;
		public ObservableImportDetail CurrentDetail
		{
			get => _currentDetail;
			set => SetProperty(ref _currentDetail, value);
		}

		private ObservableImportDetail _selectedDetail;
		public ObservableImportDetail SelectedDetail
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

		// Kiểm soát hiển thị panel chỉnh sửa
		private bool _isEditingDetail;
		public bool IsEditingDetail
		{
			get => _isEditingDetail;
			set => SetProperty(ref _isEditingDetail, value);
		}

		// Lưu trữ chi tiết gốc
		private ObservableImportDetail _originalDetail;

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }
		public ICommand AddDetailCommand { get; }
		public ICommand EditDetailCommand { get; }
		public ICommand UpdateDetailCommand { get; }
		public ICommand CancelEditDetailCommand { get; }
		public ICommand RemoveDetailCommand { get; }
		public ICommand ActionCommand { get; }



		public EditImportDetailsViewModel(Import import, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

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

			Title = $"Sửa chi tiết phiếu nhập #{import.ImportID}";

			// Khởi tạo collections
			Books = new ObservableCollection<Book>();
			ImportDetails = new ObservableCollection<ObservableImportDetail>();

			// Khởi tạo chi tiết trống
			ResetCurrentDetail();

			// Đăng ký commands
			SaveCommand = new AsyncRelayCommand(async _ => await SaveImportDetailsAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
			AddDetailCommand = new RelayCommand(_ => AddDetail(), _ => CanAddDetail() && !IsEditingDetail);
			EditDetailCommand = new RelayCommand(param => StartEditDetail(param as ObservableImportDetail));
			UpdateDetailCommand = new RelayCommand(_ => UpdateDetail(), _ => CanAddDetail() && IsEditingDetail);
			CancelEditDetailCommand = new RelayCommand(_ => CancelEditDetail());
			RemoveDetailCommand = new RelayCommand(param => RemoveDetail(param as ObservableImportDetail));

			ActionCommand = new RelayCommand(_ => {
				if (IsEditingDetail)
					UpdateDetail();
				else
					AddDetail();
			}, _ => CanAddDetail());
		}

		public async Task LoadDataAsync()
		{
			try
			{
				IsBusy = true;

				// Load books từ service
				var books = await _dataService.GetAllBooksAsync();
				Books = new ObservableCollection<Book>(books);

				// Load chi tiết phiếu nhập
				await LoadImportDetailsAsync();
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải dữ liệu: {ex.Message}");
				Console.WriteLine($"Error in LoadDataAsync: {ex}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task LoadImportDetailsAsync()
		{
			try
			{
				// Lấy import đầy đủ từ database
				var fullImport = await _dataService.GetImportByIdAsync(Import.ImportID);
				if (fullImport == null || fullImport.ImportDetails == null)
					return;

				// Chuyển đổi từ ImportDetail sang ObservableImportDetail
				ImportDetails.Clear();
				foreach (var detail in fullImport.ImportDetails)
				{
					var observableDetail = new ObservableImportDetail
					{
						BookID = detail.BookID,
						Quantity = detail.Quantity,
						UnitImportPrice = detail.UnitImportPrice,
						Subtotal = detail.Subtotal,
						Book = new Book
						{
							ID = detail.Book?.ID ?? 0,
							Title = detail.Book?.Title,
							ISBNCode = detail.Book?.ISBNCode,
							Author = detail.Book?.Author
						}
					};
					ImportDetails.Add(observableDetail);
				}

				// Tính tổng tiền
				CalculateTotalAmount();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error loading import details: {ex.Message}");
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải chi tiết phiếu nhập: {ex.Message}");
			}
		}

		private void StartEditDetail(ObservableImportDetail detail)
		{
			if (detail == null) return;

			// Lưu bản sao của chi tiết gốc
			_originalDetail = new ObservableImportDetail
			{
				BookID = detail.BookID,
				Quantity = detail.Quantity,
				UnitImportPrice = detail.UnitImportPrice,
				Subtotal = detail.Subtotal,
				Book = detail.Book
			};

			// Gán giá trị cho CurrentDetail
			CurrentDetail = new ObservableImportDetail
			{
				BookID = detail.BookID,
				Quantity = detail.Quantity,
				UnitImportPrice = detail.UnitImportPrice,
				Subtotal = detail.Subtotal,
				Book = detail.Book
			};

			// Chuyển sang chế độ sửa
			IsEditingDetail = true;
			SelectedDetail = detail;
		}

		private void ResetCurrentDetail()
		{
			CurrentDetail = new ObservableImportDetail
			{
				Quantity = 1,
				UnitImportPrice = 0,
				Subtotal = 0
			};
			IsEditingDetail = false;
		}

		private void AddDetail()
		{
			// Kiểm tra dữ liệu đầu vào
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

			// Tạo một chi tiết mới để thêm
			var detailToAdd = new ObservableImportDetail
			{
				BookID = CurrentDetail.BookID,
				Quantity = CurrentDetail.Quantity,
				UnitImportPrice = CurrentDetail.UnitImportPrice,
				Subtotal = CurrentDetail.Quantity * CurrentDetail.UnitImportPrice,
				Book = new Book
				{
					ID = book.ID,
					Title = book.Title,
					ISBNCode = book.ISBNCode,
					Author = book.Author
				}
			};

			// Thêm vào danh sách
			ImportDetails.Add(detailToAdd);

			// Tính lại tổng tiền
			CalculateTotalAmount();

			// Reset để nhập chi tiết tiếp
			ResetCurrentDetail();
		}

		private void UpdateDetail()
		{
			// Kiểm tra dữ liệu đầu vào
			if (CurrentDetail.BookID <= 0 || CurrentDetail.Quantity <= 0 || CurrentDetail.UnitImportPrice <= 0)
			{
				_dialogService.ShowInfoAsync("Thông báo", "Vui lòng kiểm tra lại dữ liệu");
				return;
			}

			// Tìm sách
			var book = Books.FirstOrDefault(b => b.ID == CurrentDetail.BookID);
			if (book == null)
			{
				_dialogService.ShowInfoAsync("Lỗi", "Không tìm thấy sách đã chọn");
				return;
			}

			// Tính lại subtotal
			CurrentDetail.Subtotal = CurrentDetail.Quantity * CurrentDetail.UnitImportPrice;

			// Cập nhật Book
			CurrentDetail.Book = new Book
			{
				ID = book.ID,
				Title = book.Title,
				ISBNCode = book.ISBNCode,
				Author = book.Author
			};

			// Tìm và thay thế chi tiết trong danh sách
			var index = ImportDetails.IndexOf(SelectedDetail);
			if (index >= 0)
			{
				ImportDetails[index] = CurrentDetail;
			}

			// Tính lại tổng tiền
			CalculateTotalAmount();

			// Reset trạng thái
			ResetCurrentDetail();
			SelectedDetail = null;
		}

		private void CancelEditDetail()
		{
			// Khôi phục về trạng thái ban đầu
			ResetCurrentDetail();
			SelectedDetail = null;
		}

		private void RemoveDetail(ObservableImportDetail detail)
		{
			if (detail != null)
			{
				ImportDetails.Remove(detail);
				CalculateTotalAmount();

				// Nếu đang sửa chi tiết này, hủy sửa
				if (IsEditingDetail && SelectedDetail == detail)
				{
					CancelEditDetail();
				}
			}
		}

		private void CalculateTotalAmount()
		{
			TotalAmount = ImportDetails.Sum(d => d.Subtotal);
			Import.TotalAmount = TotalAmount;
		}

		private bool CanAddDetail()
		{
			return CurrentDetail?.BookID > 0 &&
				   CurrentDetail.Quantity > 0 &&
				   CurrentDetail.UnitImportPrice > 0;
		}

		private async Task SaveImportDetailsAsync()
		{
			if (ImportDetails.Count == 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng thêm ít nhất một sản phẩm");
				return;
			}

			try
			{
				// Chuyển đổi từ ObservableImportDetail sang ImportDetail
				var importDetails = ImportDetails.Select(d => new ImportDetail
				{
					ImportID = Import.ImportID,
					BookID = d.BookID,
					Quantity = d.Quantity,
					UnitImportPrice = d.UnitImportPrice,
					Subtotal = d.Subtotal
					// Không gán Book để tránh lỗi
				}).ToList();

				// Gọi service để cập nhật
				var success = await _dataService.UpdateImportDetailsAsync(Import.ImportID, importDetails);

				if (success)
				{
					// Cập nhật tổng tiền
					Import.TotalAmount = TotalAmount;
					await _dataService.UpdateImportTotalAmount(Import.ImportID, TotalAmount);

					ImportUpdated?.Invoke(this, Import);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật chi tiết phiếu nhập thành công");
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