using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoanPhamVietDuc.Views.Staffs
{
	/// <summary>
	/// Interaction logic for EditStaffWindow.xaml
	/// </summary>
	public partial class EditStaffWindow : Window
	{
		private readonly EditStaffViewModel _viewModel;
		public Staff UpdatedStaff { get; private set; }
		public EditStaffWindow(Staff staff, IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();
			_viewModel = new EditStaffViewModel(staff, dataService, dialogService, this);

			_viewModel.StaffUpdated += (sender, updatedStaff) => {
				UpdatedStaff = updatedStaff;
			};

			DataContext = _viewModel;

			Loaded += async (_, __) =>
			{
				await _viewModel.LoadReferenceDataAsync();
			};

		}
	}
}
