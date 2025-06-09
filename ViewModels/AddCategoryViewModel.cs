using System;
using System.Collections.Generic;
using System.Data;
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
    public class AddCategoryViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly Window _window;
		public event EventHandler<Category> CategoryCreated;

		private Category _language;
		public Category Category
		{
			get => _language;
			set => SetProperty(ref _language, value);	
		}

		public ICommand SaveCommand { get;}
		public ICommand CancelCommand { get;}

		public AddCategoryViewModel(IDataService dataService, IDialogService dialogService, Window window)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_window = window;

			Title = $"Thêm thể loại";

			Category = new Category
			{
				ModifyTime = DateTime.Now,
			};

			SaveCommand = new AsyncRelayCommand(async _ => await SaveCategoryAsync());
			CancelCommand = new RelayCommand(_ => CancelAndClose());
		}

		public async Task SaveCategoryAsync()
		{
			if (string.IsNullOrWhiteSpace(Category.CategoryName))
			{
				await _dialogService.ShowInfoAsync("Thông báo", "Vui lòng nhập tên thể loại");
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

			try
			{
				Category.ModifyTime = DateTime.Now;
				var success = await _dataService.AddCategoryAsync(Category);

				if (success)
				{
					CategoryCreated?.Invoke(this, Category);
					await _dialogService.ShowInfoAsync("Thông báo", "Thêm thể loại thành công");
					_window.DialogResult = true;
					_window.Close();
				}
				else
				{
					await _dialogService.ShowInfoAsync("Lỗi", "Thêm thể loại thất bại");
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
