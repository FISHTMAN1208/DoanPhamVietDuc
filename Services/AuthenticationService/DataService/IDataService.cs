using DoanPhamVietDuc.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Services.AuthenticationService.DataService
{
	public interface IDataService
	{
		// ===== BOOKS =====
		Task<List<Book>> GetAllBooksAsync();
		Task<Book> GetBookByIdAsync(int id);
		Task<bool> AddBookAsync(Book book);
		Task<bool> UpdateBookAsync(Book book);
		Task<List<Book>> SearchBooksAsync(string searchText);
		Task<bool> DeleteBookAsync(int id);
		Task<bool> IsISBNCodeExistsAsync(string isbnCode, int? excludeId = null);

		// ===== CATEGORIES =====
		Task<List<Category>> GetAllCategoriesAsync();
		Task<Category> GetCategoryByIdAsync(int id);
		Task<bool> AddCategoryAsync(Category category);
		Task<bool> UpdateCategoryAsync(Category category);
		Task<bool> DeleteCategoryAsync(int id);
		Task<List<Category>> SearchCategoriesAsync(string searchText);
		Task<bool> IsCategoryNameExistsAsync(string categoryName, int? excludeId = null);

		// ===== SUPPLIERS =====
		Task<List<Supplier>> GetAllSuppliersAsync();
		Task<Supplier> GetSupplierByIdAsync(int id);
		Task<bool> AddSupplierAsync(Supplier supplier);
		Task<bool> UpdateSupplierAsync(Supplier supplier);
		Task<bool> DeleteSupplierAsync(int id);
		Task<List<Supplier>> SearchSuppliersAsync(string searchText);
		Task<bool> IsSupplierNameExistsAsync(string supplierName, int? excludeId = null);

		// ===== LANGUAGES =====
		Task<List<Language>> GetAllLanguagesAsync();
		Task<Language> GetLanguageByIdAsync(int id);
		Task<bool> AddLanguageAsync(Language language);
		Task<bool> UpdateLanguageAsync(Language language);
		Task<bool> DeleteLanguageAsync(int id);
		Task<List<Language>> SearchLanguagesAsync(string searchText);
		Task<bool> IsLanguageNameExistsAsync(string languageName, int? excludeId = null);

		// ===== BOOK COVER TYPES =====
		Task<List<BookCoverType>> GetAllBookCoverTypesAsync();
		Task<BookCoverType> GetBookCoverTypeByIdAsync(int id);
		Task<bool> AddBookCoverTypeAsync(BookCoverType bookCoverType);
		Task<bool> UpdateBookCoverTypeAsync(BookCoverType bookCoverType);
		Task<List<BookCoverType>> SearchBookCoverTypesAsync(string searchText);
		Task<bool> DeleteBookCoverTypeAsync(int id);
		Task<bool> IsBookCoverTypeNameExistsAsync(string bookCoverTypeName, int? excludeId = null);

		// ===== IMPORTS =====
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

		// ===== INVOICES =====
		Task<List<Invoice>> GetAllInvoicesAsync();
		Task<Invoice> GetInvoiceByIdAsync(int id);
		Task<bool> AddInvoiceAsync(Invoice invoice, List<InvoiceDetail> details);
		Task<bool> UpdateInvoiceAsync(Invoice invoice, List<InvoiceDetail> newDetails);
		Task<bool> DeleteInvoiceAsync(int id);
		Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string newStatus);
		Task<decimal> GetTotalInvoiceAmountInPeriodAsync(DateTime startDate, DateTime endDate);
		Task<Dictionary<string, int>> GetInvoiceCountByStatusAsync();
		Task<List<Invoice>> SearchInvoicesAsync(string searchText);
		Task<bool> UpdateInvoiceInfoAsync(Invoice invoice);
		Task<bool> ProcessPaymentAsync(int invoiceId, string paymentMethod);
		Task<bool> UpdateInvoiceDetailsAsync(int invoiceId, List<InvoiceDetail> details);
		Task<bool> UpdateInvoiceTotalAmount(int invoiceId, decimal totalAmount);

		// ===== ACCOUNTS =====
		Task<Account> GetAccountByUsernameAsync(string username);
		Task<Account> GetAccountByStaffIdAsync(int staffId);
		Task<bool> AddAccountAsync(Account account);
		Task<bool> UpdateAccountAsync(Account account);
		Task<bool> DeleteAccountAsync(int accountId);
		Task<List<Account>> GetAllAccountsAsync();
		Task<List<Account>> SearchAccountsAsync(string searchText);
		Task<bool> UpdateLastLoginAsync(int accountID);
		Task<bool> IsUsernameExistsAsync(string username, int? excludeId = null);
		Task<bool> VerifyPasswordAsync(string username, string currentPassword);
		Task<bool> UpdatePasswordAsync(string username, string newPassword);

		// ===== STAFF =====
		Task<List<Staff>> GetAllStaffsAsync();
		Task<Staff> GetStaffByIdAsync(int staffId);
		Task<bool> AddStaffAsync(Staff staff);
		Task<bool> EditStaffAsync(Staff staff);
		Task<bool> DeleteStaffAsync(int staffId);
		Task<bool> IsStaffEmailExistsAsync(string email, int? excludeId = null);
		Task<bool> IsStaffPhoneExistsAsync(string phone, int? excludeId = null);
		Task<List<Staff>> SearchStaffsAsync(string searchText);
		Task<int> GetStaffCountByStatusAsync(string status);
		Task<List<Staff>> GetStaffsWithoutAccountAsync();

		//====STATICS====
		Task<List<CategoryRevenue>> GetRevenueByCategoryAsync(DateTime? startDate, DateTime? endDate, string status = null);
	}
}
