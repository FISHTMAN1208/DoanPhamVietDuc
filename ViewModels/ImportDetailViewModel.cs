using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.PdfExportService;

namespace DoanPhamVietDuc.ViewModels
{
	public class ImportDetailViewModel : BaseViewModel
	{
		private readonly Window _window;
		private readonly IPdfExportService _pdfExportService;

		private Import _import;
		public Import Import
		{
			get => _import;
			set => SetProperty(ref _import, value);
		}

		private ObservableCollection<ImportDetail> _importDetails;
		public ObservableCollection<ImportDetail> ImportDetails
		{
			get => _importDetails;
			set => SetProperty(ref _importDetails, value);
		}

		// Commands
		public ICommand ExportPDFCommand { get; private set; }

		public ImportDetailViewModel(Import import, Window window, IPdfExportService pdfExportService = null)
		{
			_window = window;
			_pdfExportService = pdfExportService ?? new PdfExportService();

			Title = $"Chi tiết phiếu nhập #{import.ImportID}";

			// Gán dữ liệu
			Import = import;
			ImportDetails = new ObservableCollection<ImportDetail>(import.ImportDetails ?? new List<ImportDetail>());
			ExportPDFCommand = new AsyncRelayCommand(async _ => await ExportToPdfAsync());


		}

		private async Task ExportToPdfAsync()
		{
			if (Import == null) return;

			try
			{
				IsBusy = true;

				// Tạo tên file mặc định
				string defaultFileName = $"PhieuNhap_{Import.ImportID}_{DateTime.Now:yyyyMMdd}.pdf";

				// Hiển thị dialog để chọn nơi lưu file
				string filePath = await _pdfExportService.GetSaveFilePathAsync(defaultFileName);

				if (string.IsNullOrEmpty(filePath))
				{
					return;
				}

				// Xuất PDF
				bool success = await _pdfExportService.ExportImportToPdfAsync(Import, filePath);

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