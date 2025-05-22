using DoanPhamVietDuc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Services.DataService
{
	public interface IDataService
	{
		Task<List<Book>> GetAllBooksAsync();
		Task<Book> GetBookByIdAsync(int id);
		Task<bool> AddBookAsync(Book book);
		Task<bool> UpdateBookAsync(Book book);
		Task<bool> DeleteBookAsync(int id);
		Task<bool> IsISBNCodeExistsAsync(string isbnCode, int? excludeId = null);

		Task<List<Category>> GetAllCategoriesAsync();
		Task<Category> GetCategoryByIdAsync(int id);
		Task<bool> AddCategoryAsync(Category category);
		Task<bool> UpdateCategoryAsync(Category category);
		Task<bool> DeleteCategoryAsync(int id);
		Task<bool> IsCategoryNameExistsAsync(string categoryName, int? excludeId = null);

		Task<List<Supplier>> GetAllSuppliersAsync();
		Task<Supplier> GetSupplierByIdAsync(int id);
		Task<bool> AddSupplierAsync(Supplier supplier);
		Task<bool> UpdateSupplierAsync(Supplier supplier);
		Task<bool> DeleteSupplierAsync(int id);
		Task<bool> IsSupplierNameExistsAsync(string supplierName, int? excludeId = null);

		Task<List<Language>> GetAllLanguagesAsync();
		Task<Language> GetLanguageByIdAsync(int id);
		Task<bool> AddLanguageAsync(Language language);
		Task<bool> UpdateLanguageAsync(Language language);
		Task<bool> DeleteLanguageAsync(int id);
		Task<bool> IsLanguageNameExistsAsync(string languageName, int? excludeId = null);


		Task<List<BookCoverType>> GetAllBookCoverTypesAsync();
		Task<BookCoverType> GetBookCoverTypeByIdAsync(int id);
		Task<bool> AddBookCoverTypeAsync(BookCoverType bookcovertype);
		Task<bool> UpdateBookCoverTypeAsync(BookCoverType bookcovertype);
		Task<bool> DeleteBookCoverTypeAsync(int id);
		Task<bool> IsBookCoverTypeNameExistsAsync(string bookCoverTypeName, int? excludeId = null);

		Task<List<Import>> GetAllImportsAsync();
		Task<Import> GetImportByIdAsync(int id);
		Task<bool> AddImportAsync(Import import, List<ImportDetail> details);
		Task<bool> UpdateImportAsync(Import import, List<ImportDetail> details);
		Task<bool> DeleteImportAsync(int id);
		Task<bool> UpdateImportStatusAsync(int importId, string newStatus);
		Task<decimal> GetTotalImportAmountInPeriodAsync(DateTime startDate, DateTime endDate);
		Task<Dictionary<string, int>> GetImportCountByStatusAsync();
		Task<List<Import>> SearchImportsAsync(string searchText);
		Task<bool> UpdateImportDetailsAsync(int importId, List<ImportDetail> newDetails);
		Task<bool> UpdateImportInfoAsync(Import import);
		Task<bool> UpdateImportTotalAmount(int importId, decimal totalAmount);
	}
}
