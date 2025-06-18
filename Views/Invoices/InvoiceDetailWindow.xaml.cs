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
using DoanPhamVietDuc.Services.PdfExportService;
using DoanPhamVietDuc.ViewModels;

namespace DoanPhamVietDuc.Views.Invoices
{
	/// <summary>
	/// Interaction logic for InvoiceDetailWindow.xaml
	/// </summary>
	public partial class InvoiceDetailWindow : Window
	{
		private InvoiceDetailViewModel _viewModel;

		public InvoiceDetailWindow(Invoice invoice)
		{
			InitializeComponent();

			var pdfExportService = new PdfExportService();
			_viewModel = new InvoiceDetailViewModel(invoice, this, pdfExportService);

			// Set DataContext
			DataContext = _viewModel;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
