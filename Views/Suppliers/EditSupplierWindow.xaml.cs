using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
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

namespace DoanPhamVietDuc.Views.Suppliers
{
	/// <summary>
	/// Interaction logic for EditSupplierWindow.xaml
	/// </summary>
	public partial class EditSupplierWindow : Window
	{
		private readonly EditSupplierViewModel _viewModel;
		public Supplier UpdatedSupplier { get; private set; }

		public EditSupplierWindow(Supplier supplier, IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();

			_viewModel = new EditSupplierViewModel(supplier, dataService, dialogService, this);

			_viewModel.SupplierUpdated += (sender, updatedSupplier) => {
				UpdatedSupplier = updatedSupplier;
			};

			DataContext = _viewModel;
		}
	}
}
