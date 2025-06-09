using DoanPhamVietDuc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DoanPhamVietDuc.Views.Staffs
{
	public partial class AddStaffWindow : Window
	{
		private readonly AddStaffViewModel _viewModel;
		public Staff Newstaff { get; private set; }
		
		public AddStaffWindow(IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();

			_viewModel = new AddStaffViewModel(dataService, dialogService, this);

			// Đăng ký event handler để lưu reference đến Book được tạo
			_viewModel.StaffSaved += (sender, staff) => {
				Newstaff = staff;
			};
			DataContext = _viewModel;

			Loaded += async (_, __) =>
			{
				await _viewModel.LoadReferenceDataAsync();
			};
		}
	}
}
