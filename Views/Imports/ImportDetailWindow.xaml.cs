using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.PdfExportService;
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

namespace DoanPhamVietDuc.Views.Imports
{

	/// <summary>
	/// Interaction logic for ImportDetailWindow.xaml
	/// </summary>
	public partial class ImportDetailWindow : Window
	{
		private ImportDetailViewModel _viewModel;

		public ImportDetailWindow(Import import)
		{
			InitializeComponent();

			// Khởi tạo ViewModel với dependency injection
			var pdfExportService = new PdfExportService();
			_viewModel = new ImportDetailViewModel(import, this, pdfExportService);

			// Set DataContext
			DataContext = _viewModel;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
