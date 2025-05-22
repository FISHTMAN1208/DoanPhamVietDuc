using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Books;
using DoanPhamVietDuc.Views.Categories;


namespace DoanPhamVietDuc.ViewModels
{
	public class CategoryListViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;

		private ObservableCollection<Category> _categories;
		public ObservableCollection<Category> Categories
		{
			get => _categories;
			set => SetProperty(ref _categories, value);
		}

		private Category _selectedCategory;
		public Category SelectedCategory
		{
			get => _selectedCategory;
			set => SetProperty( ref _selectedCategory, value);
		}

		public ICommand LoadCategoriesCommand { get; }
		public ICommand AddCategoryCommand { get; }
		public ICommand EditCategoryCommand { get; }
		public ICommand DeleteCategoryCommand { get; }
		public ICommand RefreshCommand { get; }

		public CategoryListViewModel(IDataService dataService, IDialogService dialogService)
		{
			_dataService = dataService;
			_dialogService = dialogService;

			Title = $"Quản lý thể loại";

			Categories = new ObservableCollection<Category>();

			LoadCategoriesCommand = new AsyncRelayCommand(async _ => await LoadCategoriesAsync());

			RefreshCommand = new AsyncRelayCommand(async _ => await LoadCategoriesAsync());

			AddCategoryCommand = new RelayCommand(_ => AddCategoryAsync());

			EditCategoryCommand = new AsyncRelayCommand(
				async param => await EditCategoryAsync(param as Category ?? SelectedCategory),
				_ => SelectedCategory != null);

			DeleteCategoryCommand = new AsyncRelayCommand(
				async param => await DeleteCategoryAsync(param as Category ?? SelectedCategory),
				_ => SelectedCategory != null);

			Task.Run(() => LoadCategoriesAsync());
		}

		public async Task LoadCategoriesAsync()
		{
			try
			{
				IsBusy = true;

				var selectedCategoryID = SelectedCategory?.CategoryID;

				var categories = await _dataService.GetAllCategoriesAsync();
				Categories.Clear();

                foreach (var category in categories)
                {
					Categories.Add(category);
                }

				if(selectedCategoryID.HasValue)
				{
					SelectedCategory = Categories.FirstOrDefault(c => c.CategoryID == selectedCategoryID);
				}	
            }
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể tải danh sách danh mục: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}

		}

		public async Task AddCategoryAsync()
		{
			
			var addCategoryWindow = new AddCategoryWindow(_dataService, _dialogService);
			var dialogResult = addCategoryWindow.ShowDialog();

			if (dialogResult == true && addCategoryWindow.NewCategory != null)
			{
				Categories.Add(addCategoryWindow.NewCategory);
				SelectedCategory = addCategoryWindow.NewCategory;
			}
			if (dialogResult == true)
			{
				await LoadCategoriesAsync();
			}	
		}

		private async Task EditCategoryAsync(Category categoryToEdit)
		{
			if (categoryToEdit == null)
			{
				return;
			}

			try
			{
				var editCategoryWindow = new EditCategoryWindow(categoryToEdit, _dialogService, _dataService);
				var dialogResult = editCategoryWindow.ShowDialog();

				if (dialogResult == true && editCategoryWindow.UpdatedCategory != null)
				{
					await LoadCategoriesAsync();
					SelectedCategory = Categories.FirstOrDefault(c => c.CategoryID == editCategoryWindow.UpdatedCategory.CategoryID);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi cập nhật danh mục: {ex.Message}");
			}
		}

		private async Task DeleteCategoryAsync(Category categoryToDelete)
		{
			if (categoryToDelete == null)
			{
				return;
			}

			if (categoryToDelete.Books != null && categoryToDelete.Books.Count > 0)
			{
				await _dialogService.ShowInfoAsync("Thông báo",
					$"Không thể xóa danh mục '{categoryToDelete.CategoryName}' vì có {categoryToDelete.Books.Count} sách thuộc danh mục này");
				return;
			}

			var confirm = await _dialogService.ShowConfirmAsync(
				"Xác nhận xóa",
				$"Bạn có chắc muốn xóa thể loại '{categoryToDelete.CategoryName}' không?");

			if (confirm)
			{
				try
				{
					var success = await _dataService.DeleteCategoryAsync(SelectedCategory.CategoryID);
					if (success)
					{
						Categories.Remove(categoryToDelete);
						SelectedCategory = null;
					}
					else
					{
						await _dialogService.ShowInfoAsync("Lỗi", "Không thể xóa thể loại!");
					}
				}
				catch (Exception ex)
				{
					await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi xóa danh mục: {ex.Message}");
				}
			}
		}

	}
}
