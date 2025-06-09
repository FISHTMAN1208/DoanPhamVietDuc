using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Books;
using DoanPhamVietDuc.Views.Staffs;
using DoanPhamVietDuc.Views.Suppliers;
using static System.Reflection.Metadata.BlobBuilder;

namespace DoanPhamVietDuc.ViewModels
{
    public class SupplierListViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private ObservableCollection<Supplier> _suppliers;
		public ObservableCollection<Supplier> Suppliers
		{
			get => _suppliers;
			set => SetProperty(ref _suppliers, value);
		}

		private Supplier _selectedSupplier;
		public Supplier SelectedSupplier
		{
			get => _selectedSupplier;
			set => SetProperty(ref _selectedSupplier, value);
		}

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				if(SetProperty(ref _searchText, value))
				{
					SearchSupplierCommand.Execute(value);
				}	
			}
		}

		public ICommand LoadSuppliersCommand { get; }
		public ICommand AddSupplierCommand { get; }
		public ICommand ViewDetailCommand { get; }
		public ICommand EditSupplierCommand { get; }
		public ICommand DeleteSupplierCommand { get; }
		public ICommand SearchSupplierCommand { get; }
		public ICommand RefreshCommand { get; }

		public SupplierListViewModel(IDataService dataService, IDialogService dialogService)
		{
			_dataService = dataService;
			_dialogService = dialogService;

			Suppliers = new ObservableCollection<Supplier>();

			LoadSuppliersCommand = new AsyncRelayCommand(async _ => await LoadSuppliersAsync());

			SearchSupplierCommand = new AsyncRelayCommand(async _ => await SearchSuppliersAsync());

			RefreshCommand = new AsyncRelayCommand(async _ => await LoadSuppliersAsync());

			ViewDetailCommand = new RelayCommand(_ => ViewSupplierDetail(), _ => SelectedSupplier != null);

			AddSupplierCommand = new RelayCommand(_ => AddSupplierAsync());

			EditSupplierCommand = new AsyncRelayCommand(
				async param => await EditSupplierAsync(param as Supplier ?? SelectedSupplier), _ => SelectedSupplier != null);

			DeleteSupplierCommand = new AsyncRelayCommand(
				async param => await DeleteSupplierAsync(param as Supplier ?? SelectedSupplier), _ => SelectedSupplier != null);

			_ = InitializeAsync();
		}

		private async Task InitializeAsync()
		{
			try
			{
				await LoadSuppliersAsync().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
		}

		public async Task LoadSuppliersAsync()
		{
			try
			{
				IsBusy = true;

				var selectedSupplierID = SelectedSupplier?.SupplierID;

				var suppliers = await _dataService.GetAllSuppliersAsync();
				Suppliers.Clear();

				foreach (var supplier in suppliers)
				{
					Suppliers.Add(supplier);
				}

				if (selectedSupplierID.HasValue)
				{
					SelectedSupplier = Suppliers.FirstOrDefault(c => c.SupplierID == selectedSupplierID);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải danh sách nhà cung cấp: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void ViewSupplierDetail()
		{
			if (SelectedSupplier != null)
			{
				SupplierDetailWindow detailWindow = new SupplierDetailWindow(SelectedSupplier);
				detailWindow.ShowDialog();
			}
		}

		public async Task AddSupplierAsync()
		{

			var addSupplierWindow = new AddSupplierWindow(_dataService, _dialogService);
			var dialogResult = addSupplierWindow.ShowDialog();

			if (dialogResult == true && addSupplierWindow.NewSupplier != null)
			{
				Suppliers.Add(addSupplierWindow.NewSupplier);
				SelectedSupplier = addSupplierWindow.NewSupplier;
			}
			if (dialogResult == true)
			{
				await LoadSuppliersAsync();
			}
		}

		private async Task EditSupplierAsync(Supplier supplierToEdit)
		{
			if (supplierToEdit == null)
			{
				return;
			}

			try
			{
				var editSupplierWindow = new EditSupplierWindow(supplierToEdit, _dialogService, _dataService);
				var dialogResult = editSupplierWindow.ShowDialog();

				if (dialogResult == true && editSupplierWindow.UpdatedSupplier != null)
				{
					await LoadSuppliersAsync();
					SelectedSupplier = Suppliers.FirstOrDefault(c => c.SupplierID == editSupplierWindow.UpdatedSupplier.SupplierID);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi cập nhật danh mục: {ex.Message}");
			}
		}

		private async Task DeleteSupplierAsync(Supplier supplierToDelete)
		{
			if (supplierToDelete == null)
			{
				return;
			}

			if(supplierToDelete.Books != null && supplierToDelete.Books.Count > 0) 
			{
				await _dialogService.ShowInfoAsync("Thông báo", $"Không thể xóa nhà cung cấp '{supplierToDelete.SupplierName}' vì có {supplierToDelete.Books.Count} sách thuộc nhà cung cấp này");
				return;
			}
			var confirm = await _dialogService.ShowConfirmAsync("Xác nhận xóa", $"Bạn có chắc muốn xóa nhà cung cấp '{supplierToDelete.SupplierName}' không ?");

			if (confirm)
			{
				try
				{
					var success = await _dataService.DeleteSupplierAsync(_selectedSupplier.SupplierID);
					if (success)
					{
						Suppliers.Remove(supplierToDelete);
						SelectedSupplier = null;
					}
					else
					{
						await _dialogService.ShowInfoAsync("Lỗi", "Không thể xóa thể loại!");
					}
				}
				catch (Exception ex)
				{
					await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi xóa thể loại: {ex.Message}");
				}
			}
		}

		private async Task SearchSuppliersAsync()
		{
			if (string.IsNullOrWhiteSpace(SearchText))
			{
				await LoadSuppliersAsync();
				return;
			}

			try
			{
				IsBusy = true;
				var suppliers = await _dataService.SearchSuppliersAsync(SearchText);
				Suppliers.Clear();

				foreach (var supplier in suppliers)
				{
					Suppliers.Add(supplier);
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
	}
}
