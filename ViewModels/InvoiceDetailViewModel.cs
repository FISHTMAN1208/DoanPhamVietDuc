using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using DoanPhamVietDuc.Services.PdfExportService;

namespace DoanPhamVietDuc.ViewModels
{
	public class InvoiceDetailViewModel : BaseViewModel
	{
		private readonly Window _window;
		private readonly IPdfExportService _pdfExportService;

		private Invoice _invoice;
		public Invoice Invoice
		{
			get => _invoice;
			set => SetProperty(ref _invoice, value);
		}

		private ObservableCollection<InvoiceDetail> _invoiceDetails;
		public ObservableCollection<InvoiceDetail> InvoiceDetails
		{
			get => _invoiceDetails;
			set => SetProperty(ref _invoiceDetails, value);
		}

		public ICommand CloseCommand { get; }
		public ICommand ExportPDFCommand { get; }

		public InvoiceDetailViewModel(Invoice invoice, Window window, IPdfExportService pdfExportService = null)
		{
			_window = window;
			_pdfExportService = pdfExportService ?? new PdfExportService();

			Title = $"Chi tiết hóa đơn #{invoice.InvoiceID}";


			Invoice = invoice;
			InvoiceDetails = new ObservableCollection<InvoiceDetail>(invoice.InvoiceDetails ?? new List<InvoiceDetail>());
			ExportPDFCommand = new AsyncRelayCommand(async _ => await ExportToPdfAsync());
			CloseCommand = new RelayCommand(_ => _window.Close());


		}

		private async Task ExportToPdfAsync()
		{
			if (Invoice == null) return;

			try
			{
				IsBusy = true;

				// Tạo tên file mặc định
				string defaultFileName = $"HoaDon_{Invoice.InvoiceID}_{DateTime.Now:yyyyMMdd}.pdf";

				// Hiển thị dialog để chọn nơi lưu file
				string filePath = await _pdfExportService.GetSaveFilePathAsync(defaultFileName);

				if (string.IsNullOrEmpty(filePath))
				{
					return;
				}

				// Xuất PDF
				bool success = await _pdfExportService.ExportInvoiceToPdfAsync(Invoice, filePath);

				if (success)
				{
					MessageBox.Show(
						$"Xuất file PDF thành công!\nĐường dẫn: {filePath}",
						"Thành công",
						MessageBoxButton.OK,
						MessageBoxImage.Information);

					var result = MessageBox.Show(
						"Bạn có muốn mở file PDF vừa tạo không?",
						"Mở file",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question);

					if (result == MessageBoxResult.Yes)
					{
						try
						{
							System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
							{
								FileName = filePath,
								UseShellExecute = true
							});
						}
						catch (Exception ex)
						{
							MessageBox.Show(
								$"Không thể mở file PDF: {ex.Message}",
								"Lỗi",
								MessageBoxButton.OK,
								MessageBoxImage.Warning);
						}
					}
				}
				else
				{
					MessageBox.Show(
						"Có lỗi xảy ra khi xuất file PDF. Vui lòng thử lại!",
						"Lỗi",
						MessageBoxButton.OK,
						MessageBoxImage.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Lỗi khi xuất PDF: {ex.Message}",
					"Lỗi",
					MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
