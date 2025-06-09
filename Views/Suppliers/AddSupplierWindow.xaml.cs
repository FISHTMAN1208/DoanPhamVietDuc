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
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;

namespace DoanPhamVietDuc.Views.Suppliers
{
    /// <summary>
    /// Interaction logic for AddSupplierWindow.xaml
    /// </summary>
    public partial class AddSupplierWindow : Window
	{
		public readonly AddSupplierViewModel _viewmodel;
		public Supplier NewSupplier { get; private set; }
		public AddSupplierWindow(IDataService dataService, IDialogService dialogService)
		{
			InitializeComponent();
			_viewmodel = new AddSupplierViewModel(dataService, dialogService, this);
			_viewmodel.SupplierCreated += (sender, supplier) =>
			{
				NewSupplier = supplier;
			};
			DataContext = _viewmodel;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

        }
    }
}
