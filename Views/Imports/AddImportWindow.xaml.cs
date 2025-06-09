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

namespace DoanPhamVietDuc.Views.Imports
{
    /// <summary>
    /// Interaction logic for AddImportWindow.xaml
    /// </summary>
    public partial class AddImportWindow : Window
	{
		private readonly AddImportViewModel _viewModel;

		public AddImportWindow(IDialogService dialogService, IDataService dataService) 
		{
			InitializeComponent();
			_viewModel = new AddImportViewModel(dataService, dialogService, this);
			DataContext = _viewModel;
		}
	}
}
