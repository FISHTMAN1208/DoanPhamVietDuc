using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DoanPhamVietDuc.ViewModels
{
	public class EditCategoryViewModel : BaseViewModel
	{
		private readonly IDialogService _dialogService;
		private readonly IDataService _dataService;
		private readonly Window _window;
		public event EventHandler<Category> CategoryUpdated;

		private Category _category;
		public Category Category
		{
			get => _category;
			set => SetProperty(ref _category, value);
		}

		private Category _originalCategory;

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }

		public EditCategoryViewModel(Category category, IDataService dataService, IDialogService dialogService, Window window)
		{
			_dialogService = dialogService;
			_dataService = dataService;
			_window = window;

			_originalCategory = category;

			// Tạo bản sao của Category để chỉnh sửa
			Category = new Category
			{
				CategoryID = category.CategoryID,
				CategoryName = category.CategoryName,
				ModifyBy = category.ModifyBy,
				ModifyTime = category.ModifyTime
			};

			Title = $"Sửa danh mục: {category.CategoryName}";

			SaveCommand = new AsyncRelayCommand(async _ => await SaveCategoryAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
		}

		private async Task SaveCategoryAsync()
		{
			if (string.IsNullOrWhiteSpace(Category.CategoryName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên danh mục");
				return;
			}

			bool nameExists = await _dataService.IsCategoryNameExistsAsync(Category.CategoryName);
			if (nameExists)
			{
				await _dialogService.ShowInfoAsync("Thông báo", $"Thể loại '{Category.CategoryName}' đã tồn tại, vui lòng chọn tên khác");
				return;
			}

			if (string.IsNullOrWhiteSpace(Category.ModifyBy))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên người thêm");
				return;
			}

			if (string.IsNullOrWhiteSpace(Category.ModifyBy))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên người cập nhật");
				return;
			}

			try
			{
				Category.ModifyTime = DateTime.Now;

				var success = await _dataService.UpdateCategoryAsync(Category);

				if (success)
				{
					CategoryUpdated?.Invoke(this, Category);
					await _dialogService.ShowInfoAsync("Thông báo", "Cập nhật danh mục thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Cập nhật danh mục thất bại, vui lòng thử lại");
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