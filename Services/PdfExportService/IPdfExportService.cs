using DoanPhamVietDuc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Services.PdfExportService
{
    public interface IPdfExportService
    {
  
		Task<bool> ExportImportToPdfAsync(Import import, string filePath);
		Task<bool>ExportInvoiceToPdfAsync(Invoice invoice, string filePath);
		Task<bool> ShowSaveFileDialogAsync(string fileName);
		Task<string> GetSaveFilePathAsync(string fileName);
	}
}
