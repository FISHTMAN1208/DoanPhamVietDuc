using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.Views.Books;
using DoanPhamVietDuc.Views.Staffs;
using HarfBuzzSharp;
using Microsoft.Extensions.DependencyInjection;
using static System.Reflection.Metadata.BlobBuilder;

namespace DoanPhamVietDuc.ViewModels
{
	public class StaffListViewModel : BaseViewModel
	{
		private readonly IDataService _dataService;
		private readonly IDialogService _dialogService;
		private readonly IServiceProvider _serviceProvider;

		public ObservableCollection<Staff> Staffs { get; }
		public ICollectionView FilteredStaffs { get; }

		private string _searchText;
		public string SearchText
		{
			get => _searchText;
			set
			{
				if(SetProperty(ref _searchText, value))
				{
					SearchCommand.Execute(SearchText);
				}	
			}
		}

		private Staff _selectedStaff;
		public Staff SelectedStaff
		{
			get => _selectedStaff;
			set => SetProperty(ref _selectedStaff, value);
		}

		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}

		public ICommand AddStaffCommand { get;}
		public ICommand EditStaffCommand { get;}
		public ICommand DeleteStaffCommand { get;}
		public ICommand ViewDetailCommand { get;}
		public ICommand RefreshCommand { get;}
		public ICommand SearchCommand { get;}
		
		public StaffListViewModel(IDataService dataService, IDialogService dialogService, IServiceProvider serviceProvider)
		{
			_dataService = dataService;
			_dialogService = dialogService;
			_serviceProvider = serviceProvider;

			Title = "Quản lý nhân viên";

			Staffs = new ObservableCollection<Staff>();
			FilteredStaffs = CollectionViewSource.GetDefaultView(Staffs);

			ViewDetailCommand = new RelayCommand(_ => ViewStaffDetail(), _ => SelectedStaff != null);

			AddStaffCommand = new AsyncRelayCommand(async _ => await AddStaffAsync());

			SearchCommand = new AsyncRelayCommand(async _ => await SearchStaffsAsync());

			EditStaffCommand = new AsyncRelayCommand(
				async param => await EditStaffAsync(param as Staff ?? SelectedStaff),
			_ => SelectedStaff != null);

			DeleteStaffCommand = new AsyncRelayCommand(
				async param => await DeleteStaffAsync(param as Staff ?? SelectedStaff),
				_ => SelectedStaff != null);

			_ = InitializeAsync();

		}

		private async Task InitializeAsync()
		{
			try
			{
				await LoadStaffAsync().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Không thể khởi tạo: {ex.Message}");
			}
		}

		private async Task LoadStaffAsync()
		{
			try
			{
				IsBusy = true;
				var staffs = await _dataService.GetAllStaffsAsync();
				Staffs.Clear();

				foreach (var staff in staffs)
				{
					Staffs.Add(staff);
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi tải danh sách nhân viên {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task AddStaffAsync()
		{
			try
			{
				var addStaffWindow = new AddStaffWindow(_dialogService, _dataService);
				var dialogResult = addStaffWindow.ShowDialog();

				if (dialogResult == true && addStaffWindow.Newstaff != null)
				{
					Staffs.Add(addStaffWindow.Newstaff);
					SelectedStaff = addStaffWindow.Newstaff;
				}
				else if (dialogResult == true)
				{
					await LoadStaffAsync();
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi mở cửa sổ thêm nhân viên: {ex.Message}");
			}
		}

		private async Task EditStaffAsync(Staff staffToEdit)
		{
			if (staffToEdit == null)
				return;

			try
			{
				IsBusy = true;

				var editStaffWindow = new EditStaffWindow(staffToEdit, _dialogService, _dataService);
				var dialogResult = editStaffWindow.ShowDialog();

				if (dialogResult == true)
				{
					if (editStaffWindow.UpdatedStaff != null)
					{
						var index = Staffs.IndexOf(staffToEdit);
						if (index >= 0)
						{
							Staffs[index] = editStaffWindow.UpdatedStaff;
							SelectedStaff = editStaffWindow.UpdatedStaff;
						}
					}
					await LoadStaffAsync();
				}
			}
			catch (Exception ex)
			{
				await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi cập nhật nhân viên: {ex.Message}");
			}
			finally
			{
				IsBusy = false;
			}
		}


		private void ViewStaffDetail()
		{
			if (SelectedStaff != null)
			{
				StaffDetailWindow detailWindow = new StaffDetailWindow(SelectedStaff);
				detailWindow.ShowDialog();
			}
		}

		private async Task DeleteStaffAsync(Staff staffToDelete)
		{
			if (staffToDelete == null)
			{
				return;
			}

			var confirm = await _dialogService.ShowConfirmAsync(
				"Xác nhận xóa",
				$"Bạn có chắc muốn nhân viên '{staffToDelete.FullName}' không?");

			if (confirm)
			{
				try
				{
					var success = await _dataService.DeleteStaffAsync(SelectedStaff.StaffID);
					if (success)
					{
						Staffs.Remove(staffToDelete);
						SelectedStaff = null;
					}
					else
					{
						await _dialogService.ShowInfoAsync("Lỗi", "Không thể xóa nhân viên!");
					}
				}
				catch (Exception ex)
				{
					await _dialogService.ShowInfoAsync("Lỗi", $"Lỗi khi xóa nhân viên: {ex.Message}");
				}
			}
		}

		private async Task SearchStaffsAsync()
		{
			if (string.IsNullOrWhiteSpace(SearchText))
			{
				await LoadStaffAsync();
				return;
			}

			try
			{
				IsBusy = true;
				var staffs = await _dataService.SearchStaffsAsync(SearchText);
				Staffs.Clear();

				foreach (var staff in staffs)
				{
					Staffs.Add(staff);
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



		private async Task RefreshAsync()
		{
			System.Diagnostics.Debug.WriteLine("RefreshAsync: Làm mới danh sách");
			SearchText = string.Empty;
			SelectedStaff = null;
			await LoadStaffAsync();
		}
	}
}
