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

namespace DoanPhamVietDuc.Views.Invoices
{
    /// <summary>
    /// Interaction logic for EditInvoiceDetailsWindow.xaml
    /// </summary>
    public partial class EditInvoiceDetailsWindow : Window
    {
        private readonly EditInvoiceDetailsViewModel _viewModel;
		public Invoice UpdatedInvoice { get; private set; }
		public EditInvoiceDetailsWindow(Invoice invoice, IDataService dataService, IDialogService dialogService)
        {
            InitializeComponent();
			_viewModel = new EditInvoiceDetailsViewModel(invoice, dataService, dialogService, this);

			_viewModel.InvoiceUpdated += (sender, updatedInvoice) => {
				UpdatedInvoice = updatedInvoice;
			};

			DataContext = _viewModel;

			Loaded += async (_, __) => {
				await _viewModel.LoadDataAsync();
			};
		}
    }
}
