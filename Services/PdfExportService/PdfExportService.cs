using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.PdfExportService;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace DoanPhamVietDuc.Services.PdfExportService
{
	public class PdfExportService : IPdfExportService
	{
		public async Task<bool> ExportImportToPdfAsync(Import import, string filePath)
		{
			try
			{
				Document document = new Document(PageSize.A4, 25, 25, 30, 30);
				PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

				document.Open();

				// Tạo font Unicode cho tiếng Việt
				string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
				BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
				Font titleFont = new Font(baseFont, 18, Font.BOLD);
				Font headerFont = new Font(baseFont, 14, Font.BOLD);
				Font normalFont = new Font(baseFont, 12, Font.NORMAL);
				Font smallFont = new Font(baseFont, 10, Font.NORMAL);

				// Header - Tiêu đề
				Paragraph title = new Paragraph("PHIẾU NHẬP HÀNG", titleFont);
				title.Alignment = Element.ALIGN_CENTER;
				title.SpacingAfter = 20f;
				document.Add(title);

				// Thông tin phiếu nhập
				PdfPTable infoTable = new PdfPTable(2);
				infoTable.WidthPercentage = 100;
				infoTable.SetWidths(new float[] { 1f, 1f });
				infoTable.SpacingAfter = 20f;

				// Cột trái
				PdfPCell leftCell = new PdfPCell();
				leftCell.Border = Rectangle.NO_BORDER;
				leftCell.AddElement(new Paragraph($"Mã phiếu: {import.ImportID}", normalFont));
				leftCell.AddElement(new Paragraph($"Nhà cung cấp: {import.Supplier?.SupplierName ?? "N/A"}", normalFont));
				leftCell.AddElement(new Paragraph($"Người tạo: {import.CreateBy}", normalFont));

				// Cột phải
				PdfPCell rightCell = new PdfPCell();
				rightCell.Border = Rectangle.NO_BORDER;
				rightCell.AddElement(new Paragraph($"Ngày nhập: {import.ImportDate:dd/MM/yyyy}", normalFont));
				rightCell.AddElement(new Paragraph($"Trạng thái: {import.ImportStatus}", normalFont));
				rightCell.AddElement(new Paragraph($"Thời gian tạo: {import.CreateTime:dd/MM/yyyy HH:mm}", normalFont));

				infoTable.AddCell(leftCell);
				infoTable.AddCell(rightCell);
				document.Add(infoTable);

				// Ghi chú (nếu có)
				if (!string.IsNullOrEmpty(import.Notes))
				{
					Paragraph notesHeader = new Paragraph("Ghi chú:", headerFont);
					notesHeader.SpacingAfter = 5f;
					document.Add(notesHeader);

					Paragraph notes = new Paragraph(import.Notes, normalFont);
					notes.SpacingAfter = 20f;
					document.Add(notes);
				}

				// Bảng chi tiết hàng nhập
				Paragraph detailHeader = new Paragraph("DANH SÁCH HÀNG NHẬP", headerFont);
				detailHeader.Alignment = Element.ALIGN_CENTER;
				detailHeader.SpacingAfter = 10f;
				document.Add(detailHeader);

				// Tạo bảng chi tiết
				PdfPTable detailTable = new PdfPTable(7);
				detailTable.WidthPercentage = 100;
				detailTable.SetWidths(new float[] { 0.8f, 2f, 1.5f, 1.5f, 1f, 1.2f, 1.5f });
				detailTable.SpacingAfter = 20f;

				// Header của bảng
				string[] headers = { "STT", "Tiêu đề", "Mã ISBN", "Tác giả", "Số lượng", "Đơn giá", "Thành tiền" };
				foreach (string header in headers)
				{
					PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
					headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
					headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
					headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
					headerCell.Padding = 8f;
					detailTable.AddCell(headerCell);
				}

				// Dữ liệu chi tiết
				int stt = 1;
				decimal totalAmount = 0;

				if (import.ImportDetails != null && import.ImportDetails.Any())
				{
					foreach (var detail in import.ImportDetails)
					{
						// STT
						PdfPCell sttCell = new PdfPCell(new Phrase(stt.ToString(), normalFont));
						sttCell.HorizontalAlignment = Element.ALIGN_CENTER;
						sttCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						sttCell.Padding = 5f;
						detailTable.AddCell(sttCell);

						// Tiêu đề
						PdfPCell titleCell = new PdfPCell(new Phrase(detail.Book?.Title ?? "N/A", normalFont));
						titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						titleCell.Padding = 5f;
						detailTable.AddCell(titleCell);

						// Mã ISBN
						PdfPCell isbnCell = new PdfPCell(new Phrase(detail.Book?.ISBNCode ?? "N/A", normalFont));
						isbnCell.HorizontalAlignment = Element.ALIGN_CENTER;
						isbnCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						isbnCell.Padding = 5f;
						detailTable.AddCell(isbnCell);

						// Tác giả
						PdfPCell authorCell = new PdfPCell(new Phrase(detail.Book?.Author ?? "N/A", normalFont));
						authorCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						authorCell.Padding = 5f;
						detailTable.AddCell(authorCell);

						// Số lượng
						PdfPCell quantityCell = new PdfPCell(new Phrase(detail.Quantity.ToString(), normalFont));
						quantityCell.HorizontalAlignment = Element.ALIGN_CENTER;
						quantityCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						quantityCell.Padding = 5f;
						detailTable.AddCell(quantityCell);

						// Đơn giá
						PdfPCell priceCell = new PdfPCell(new Phrase($"{detail.UnitImportPrice:N0} VNĐ", normalFont));
						priceCell.HorizontalAlignment = Element.ALIGN_RIGHT;
						priceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						priceCell.Padding = 5f;
						detailTable.AddCell(priceCell);

						// Thành tiền
						decimal subtotal = detail.Subtotal;
						PdfPCell subtotalCell = new PdfPCell(new Phrase($"{subtotal:N0} VNĐ", normalFont));
						subtotalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
						subtotalCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						subtotalCell.Padding = 5f;
						detailTable.AddCell(subtotalCell);

						totalAmount += subtotal;
						stt++;
					}
				}

				document.Add(detailTable);

				// Tổng tiền
				PdfPTable totalTable = new PdfPTable(2);
				totalTable.WidthPercentage = 100;
				totalTable.SetWidths(new float[] { 4f, 1f });

				PdfPCell totalLabelCell = new PdfPCell(new Phrase("TỔNG TIỀN:", headerFont));
				totalLabelCell.Border = Rectangle.NO_BORDER;
				totalLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
				totalLabelCell.VerticalAlignment = Element.ALIGN_MIDDLE;
				totalLabelCell.Padding = 10f;

				PdfPCell totalValueCell = new PdfPCell(new Phrase($"{totalAmount:N0} VNĐ", headerFont));
				totalValueCell.BackgroundColor = BaseColor.LIGHT_GRAY;
				totalValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
				totalValueCell.VerticalAlignment = Element.ALIGN_MIDDLE;
				totalValueCell.Padding = 10f;

				totalTable.AddCell(totalLabelCell);
				totalTable.AddCell(totalValueCell);
				document.Add(totalTable);

				// Footer
				Paragraph footer = new Paragraph($"\nBáo cáo được tạo lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", smallFont);
				footer.Alignment = Element.ALIGN_CENTER;
				footer.SpacingBefore = 30f;
				document.Add(footer);

				document.Close();
				writer.Close();

				return true;
			}
			catch (Exception ex)
			{
				// Log error nếu cần
				System.Diagnostics.Debug.WriteLine($"Error exporting PDF: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> ExportInvoiceToPdfAsync(Invoice invoice, string filePath)
		{
			try
			{
				Document document = new Document(PageSize.A4, 25, 25, 30, 30);
				PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

				document.Open();

				// Tạo font Unicode cho tiếng Việt
				string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
				BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
				Font titleFont = new Font(baseFont, 18, Font.BOLD);
				Font headerFont = new Font(baseFont, 14, Font.BOLD);
				Font normalFont = new Font(baseFont, 12, Font.NORMAL);
				Font smallFont = new Font(baseFont, 10, Font.NORMAL);

				// Header - Tiêu đề
				Paragraph title = new Paragraph("HÓA ĐƠN BÁN HÀNG", titleFont);
				title.Alignment = Element.ALIGN_CENTER;
				title.SpacingAfter = 20f;
				document.Add(title);

				// Thông tin hóa đơn
				PdfPTable infoTable = new PdfPTable(2);
				infoTable.WidthPercentage = 100;
				infoTable.SetWidths(new float[] { 1f, 1f });
				infoTable.SpacingAfter = 20f;

				// Cột trái
				PdfPCell leftCell = new PdfPCell();
				leftCell.Border = Rectangle.NO_BORDER;
				leftCell.AddElement(new Paragraph($"Mã hóa đơn: {invoice.InvoiceID}", normalFont));
				leftCell.AddElement(new Paragraph($"Mã hiển thị: {invoice.InvoiceCode}", normalFont));
				leftCell.AddElement(new Paragraph($"Nhân viên tạo: {invoice.CreateBy}", normalFont));
				leftCell.AddElement(new Paragraph($"Thời gian tạo: {invoice.CreateTime:dd/MM/yy HH:mm}", normalFont));

				// Cột phải
				PdfPCell rightCell = new PdfPCell();
				rightCell.Border = Rectangle.NO_BORDER;
				rightCell.AddElement(new Paragraph($"Tên khách hàng: {invoice.CustomerName}", normalFont));
				rightCell.AddElement(new Paragraph($"Số điện thoại: {invoice.CustomerPhone}", normalFont));
				rightCell.AddElement(new Paragraph($"Trạng thái: {invoice.Status}", normalFont));
				rightCell.AddElement(new Paragraph($"Phương thức thanh toán: {invoice.PaymentMethod}", normalFont));

				infoTable.AddCell(leftCell);
				infoTable.AddCell(rightCell);
				document.Add(infoTable);

				// Ghi chú (nếu có)
				if (!string.IsNullOrEmpty(invoice.Notes))
				{
					Paragraph notesHeader = new Paragraph("Ghi chú:", headerFont);
					notesHeader.SpacingAfter = 5f;
					document.Add(notesHeader);

					Paragraph notes = new Paragraph(invoice.Notes, normalFont);
					notes.SpacingAfter = 20f;
					document.Add(notes);
				}

				// Bảng chi tiết hóa đơn
				Paragraph detailHeader = new Paragraph("CHI TIẾT HÓA ĐƠN", headerFont);
				detailHeader.Alignment = Element.ALIGN_CENTER;
				detailHeader.SpacingAfter = 10f;
				document.Add(detailHeader);

				// Tạo bảng chi tiết
				PdfPTable detailTable = new PdfPTable(6);
				detailTable.WidthPercentage = 100;
				detailTable.SetWidths(new float[] { 0.8f, 2f, 1.5f, 1.5f, 1f, 1.2f, 1.5f });
				detailTable.SpacingAfter = 20f;

				// Header của bảng
				string[] headers = { "STT", "Tiêu đề", "Mã ISBN", "Tác giả", "Số lượng", "Đơn giá", "Thành tiền" };
				foreach (string header in headers)
				{
					PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
					headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
					headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
					headerCell.VerticalAlignment = Element.ALIGN_MIDDLE;
					headerCell.Padding = 8f;
					detailTable.AddCell(headerCell);
				}

				// Dữ liệu chi tiết
				int stt = 1;
				decimal totalAmount = 0;

				if (invoice.InvoiceDetails != null && invoice.InvoiceDetails.Any())
				{
					foreach (var detail in invoice.InvoiceDetails)
					{
						// STT
						PdfPCell sttCell = new PdfPCell(new Phrase(stt.ToString(), normalFont));
						sttCell.HorizontalAlignment = Element.ALIGN_CENTER;
						sttCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						sttCell.Padding = 5f;
						detailTable.AddCell(sttCell);

						// Tiêu đề
						PdfPCell titleCell = new PdfPCell(new Phrase(detail.Book?.Title ?? "N/A", normalFont));
						titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						titleCell.Padding = 5f;
						detailTable.AddCell(titleCell);

						// Mã ISBN
						PdfPCell isbnCell = new PdfPCell(new Phrase(detail.Book?.ISBNCode ?? "N/A", normalFont));
						isbnCell.HorizontalAlignment = Element.ALIGN_CENTER;
						isbnCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						isbnCell.Padding = 5f;
						detailTable.AddCell(isbnCell);

						// Tác giả
						PdfPCell authorCell = new PdfPCell(new Phrase(detail.Book?.Author ?? "N/A", normalFont));
						authorCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						authorCell.Padding = 5f;
						detailTable.AddCell(authorCell);

						// Số lượng
						PdfPCell quantityCell = new PdfPCell(new Phrase(detail.Quantity.ToString(), normalFont));
						quantityCell.HorizontalAlignment = Element.ALIGN_CENTER;
						quantityCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						quantityCell.Padding = 5f;
						detailTable.AddCell(quantityCell);

						// Đơn giá
						PdfPCell priceCell = new PdfPCell(new Phrase($"{detail.UnitPrice:N0} VNĐ", normalFont));
						priceCell.HorizontalAlignment = Element.ALIGN_RIGHT;
						priceCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						priceCell.Padding = 5f;
						detailTable.AddCell(priceCell);

						// Thành tiền
						decimal subtotal = detail.Subtotal;
						PdfPCell subtotalCell = new PdfPCell(new Phrase($"{subtotal:N0} VNĐ", normalFont));
						subtotalCell.HorizontalAlignment = Element.ALIGN_RIGHT;
						subtotalCell.VerticalAlignment = Element.ALIGN_MIDDLE;
						subtotalCell.Padding = 5f;
						detailTable.AddCell(subtotalCell);

						totalAmount += subtotal;
						stt++;
					}
				}

				document.Add(detailTable);

				// Tổng tiền
				PdfPTable totalTable = new PdfPTable(2);
				totalTable.WidthPercentage = 100;
				totalTable.SetWidths(new float[] { 4f, 1f });

				PdfPCell totalLabelCell = new PdfPCell(new Phrase("TỔNG TIỀN:", headerFont));
				totalLabelCell.Border = Rectangle.NO_BORDER;
				totalLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
				totalLabelCell.VerticalAlignment = Element.ALIGN_MIDDLE;
				totalLabelCell.Padding = 10f;

				PdfPCell totalValueCell = new PdfPCell(new Phrase($"{totalAmount:N0} VNĐ", headerFont));
				totalValueCell.BackgroundColor = BaseColor.LIGHT_GRAY;
				totalValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
				totalValueCell.VerticalAlignment = Element.ALIGN_MIDDLE;
				totalValueCell.Padding = 10f;

				totalTable.AddCell(totalLabelCell);
				totalTable.AddCell(totalValueCell);
				document.Add(totalTable);

				// Footer
				Paragraph footer = new Paragraph($"\nBáo cáo được tạo lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", smallFont);
				footer.Alignment = Element.ALIGN_CENTER;
				footer.SpacingBefore = 30f;
				document.Add(footer);

				document.Close();
				writer.Close();

				return true;
			}
			catch (Exception ex)
			{
				// Log error nếu cần
				System.Diagnostics.Debug.WriteLine($"Error exporting PDF: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> ShowSaveFileDialogAsync(string fileName)
		{
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					Filter = "PDF files (*.pdf)|*.pdf",
					FileName = fileName,
					DefaultExt = "pdf",
					AddExtension = true
				};

				bool? result = saveFileDialog.ShowDialog();
				if (result == true)
				{
					// Trả về đường dẫn file đã chọn
					return !string.IsNullOrEmpty(saveFileDialog.FileName);
				}

				return false;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error showing save dialog: {ex.Message}");
				return false;
			}
		}

		public async Task<string> GetSaveFilePathAsync(string fileName)
		{
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					Filter = "PDF files (*.pdf)|*.pdf",
					FileName = fileName,
					DefaultExt = "pdf",
					AddExtension = true
				};

				bool? result = saveFileDialog.ShowDialog();
				if (result == true)
				{
					return saveFileDialog.FileName;
				}

				return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
	}
}