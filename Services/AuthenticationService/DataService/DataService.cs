using DoanPhamVietDuc.Data;
using DoanPhamVietDuc.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Services.AuthenticationService.DataService
{
    public class DataService : IDataService
    {
        private readonly Func<ApplicationDbContext> _contextFactory;

        public DataService(Func<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Books
                        .AsNoTracking()
                        .Include(b => b.Category).AsNoTracking()
                        .Include(b => b.Supplier).AsNoTracking()
                        .Include(b => b.Language).AsNoTracking()
                        .Include(b => b.BookCoverType).AsNoTracking()
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Lỗi khi lấy dữ liệu sách: {ex.Message}");
                return new List<Book>();
            }
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Books
                        .AsNoTracking()
                        .Include(b => b.Category).AsNoTracking()
                        .Include(b => b.Supplier).AsNoTracking()
                        .Include(b => b.Language).AsNoTracking()
                        .Include(b => b.BookCoverType).AsNoTracking()
                        .FirstOrDefaultAsync(b => b.ID == id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy sách theo ID: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddBookAsync(Book book)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Books.Add(book);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm sách: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Books.Update(book);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật sách: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var book = await context.Books.FindAsync(id);
                    if (book == null)
                        return false;

                    context.Books.Remove(book);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa sách: {ex.Message}");
                return false;
            }
        }

		public async Task<List<Book>> SearchBooksAsync(string searchText)
		{
			using (var context = _contextFactory())
			{
				var query = context.Books
					.Include(i => i.Category)
					.Include(i => i.Language)
					.Include(i => i.BookCoverType)
					.Include(i => i.Supplier) 
					.AsQueryable();

				if (!string.IsNullOrWhiteSpace(searchText))
				{
					searchText = searchText.ToLower();
					query = query.Where(i =>
						i.ID.ToString().Contains(searchText) ||
						(i.Title != null && i.Title.ToLower().Contains(searchText)) ||
						(i.Author != null && i.Author.ToLower().Contains(searchText)) ||
						(i.Language != null && i.Language.LanguageName.ToLower().Contains(searchText)) ||
						(i.PublisherName != null && i.PublisherName.ToLower().Contains(searchText))
					);
				}

				return await query
					.OrderByDescending(i => i.PublishTime)
					.ToListAsync();
			}
		}

		public async Task<bool> IsISBNCodeExistsAsync(string isbnCode, int? excludeId = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = context.Books.AsQueryable();

                    if (excludeId.HasValue)
                    {
                        query = query.Where(s => s.ID != excludeId.Value);
                    }

                    return await query.AnyAsync(s =>
                        s.ISBNCode.ToLower() == isbnCode.ToLower());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra mã ISBN: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Categories
                        .Include(c => c.Books)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách thể loại: {ex.Message}");
                return new List<Category>();
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Categories
                        .FirstOrDefaultAsync(c => c.CategoryID == id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thể loại theo ID: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddCategoryAsync(Category category)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Categories.Add(category);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm thể loại: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Categories.Update(category);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật thể loại: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var category = await context.Categories
                        .Include(c => c.Books)
                        .FirstOrDefaultAsync(c => c.CategoryID == id);

                    if (category == null)
                    {
                        return false;
                    }
                    if (category.Books != null && category.Books.Count > 0)
                    {
                        return false;
                    }
                    context.Categories.Remove(category);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

		public async Task<List<Category>> SearchCategoriesAsync(string searchText)
		{
			using (var context = _contextFactory())
			{
				var query = context.Categories
					.AsQueryable();

				if (!string.IsNullOrWhiteSpace(searchText))
				{
					searchText = searchText.ToLower();
					query = query.Where(i =>
						i.CategoryID.ToString().Contains(searchText) ||
						(i.CategoryName != null && i.CategoryName.ToLower().Contains(searchText))
					);
				}

				return await query
					.OrderByDescending(i => i.ModifyTime)
					.ToListAsync();
			}
		}

		public async Task<bool> IsCategoryNameExistsAsync(string categoryName, int? excludeId = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = context.Categories.AsQueryable();

                    if (excludeId.HasValue)
                    {
                        query = query.Where(s => s.CategoryID != excludeId.Value);
                    }

                    return await query.AnyAsync(s =>
                        s.CategoryName.ToLower() == categoryName.ToLower());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra tên danh mục: {ex.Message}");
                return false;
            }
        }


        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Suppliers
                        .AsNoTracking()
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách nhà cung cấp: {ex.Message}");
                return new List<Supplier>();
            }
        }

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Suppliers
                        .FirstOrDefaultAsync(s => s.SupplierID == id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy nhà cung cấp theo ID: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddSupplierAsync(Supplier supplier)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Suppliers.Add(supplier);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm thể loại: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Suppliers.Update(supplier);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật thể loại: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var supplier = await context.Suppliers
                        .Include(s => s.Books)
                        .FirstOrDefaultAsync(s => s.SupplierID == id);

                    if (supplier == null)
                        return false;

                    if (supplier.Books != null && supplier.Books.Count > 0)
                    {
                        return false;
                    }

                    context.Suppliers.Remove(supplier);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa sách: {ex.Message}");
                return false;
            }
        }

		public async Task<List<Supplier>> SearchSuppliersAsync(string searchText)
		{
			using (var context = _contextFactory())
			{
				var query = context.Suppliers
					.AsQueryable();

				if (!string.IsNullOrWhiteSpace(searchText))
				{
					searchText = searchText.ToLower();
					query = query.Where(i =>
						i.SupplierID.ToString().Contains(searchText) ||
                        i.SupplierPhone.ToString().Contains(searchText)||
                        i.TaxCode.ToString().Contains(searchText)||
						(i.SupplierName != null && i.SupplierName.ToLower().Contains(searchText))
					);
				}

				return await query
					.OrderByDescending(i => i.SupplierID)
					.ToListAsync();
			}
		}

		public async Task<bool> IsSupplierNameExistsAsync(string supplierName, int? excludeId = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = context.Suppliers.AsQueryable();

                    if (excludeId.HasValue)
                    {
                        query = query.Where(s => s.SupplierID != excludeId.Value);
                    }

                    return await query.AnyAsync(s =>
                        s.SupplierName.ToLower() == supplierName.ToLower());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra tên nhà cung cấp: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Language>> GetAllLanguagesAsync()
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Languages.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách ngôn ngữ: {ex.Message}");
                return new List<Language>();
            }
        }

        public async Task<Language> GetLanguageByIdAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Languages
                        .FirstOrDefaultAsync(s => s.LanguageID == id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy ngôn ngữ theo ID: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddLanguageAsync(Language language)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Languages.Add(language);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm ngôn ngữ: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateLanguageAsync(Language language)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Languages.Update(language);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật ngôn ngữ: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteLanguageAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var language = await context.Languages
                        .Include(s => s.Books)
                        .FirstOrDefaultAsync(s => s.LanguageID == id);

                    if (language == null)
                        return false;

                    if (language.Books != null && language.Books.Count > 0)
                    {
                        return false;
                    }
                    context.Languages.Remove(language);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa ngôn ngữ {ex.Message}");
                return false;
            }
        }

		public async Task<List<Language>> SearchLanguagesAsync(string searchText)
		{
			using (var context = _contextFactory())
			{
				var query = context.Languages
					.AsQueryable();

				if (!string.IsNullOrWhiteSpace(searchText))
				{
					searchText = searchText.ToLower();
					query = query.Where(i =>
						i.LanguageID.ToString().Contains(searchText) ||
						(i.LanguageName != null && i.LanguageName.ToLower().Contains(searchText)||
						(i.Code != null && i.Code.ToLower().Contains(searchText)))
					);
				}

				return await query
					.OrderByDescending(i => i.LanguageName)
					.ToListAsync();
			}
		}

		public async Task<bool> IsLanguageNameExistsAsync(string languageName, int? excludeId = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = context.Languages.AsQueryable();

                    if (excludeId.HasValue)
                    {
                        query = query.Where(s => s.LanguageID != excludeId.Value);
                    }

                    return await query.AnyAsync(s =>
                        s.LanguageName.ToLower() == languageName.ToLower());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra tên ngôn ngữ: {ex.Message}");
                return false;
            }
        }

        public async Task<List<BookCoverType>> GetAllBookCoverTypesAsync()
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.BookCoverTypes.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách loại bìa: {ex.Message}");
                return new List<BookCoverType>();
            }
        }

        public async Task<BookCoverType> GetBookCoverTypeByIdAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.BookCoverTypes
                        .FirstOrDefaultAsync(s => s.BookCoverTypeID == id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy bìa sách theo ID: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddBookCoverTypeAsync(BookCoverType bookcovertype)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.BookCoverTypes.Add(bookcovertype);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm bìa sách: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateBookCoverTypeAsync(BookCoverType bookcovertype)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.BookCoverTypes.Update(bookcovertype);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật bìa sách: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteBookCoverTypeAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var bookcovertype = await context.BookCoverTypes
                        .Include(s => s.Books)
                        .FirstOrDefaultAsync(s => s.BookCoverTypeID == id);

                    if (bookcovertype == null)
                        return false;

                    if (bookcovertype.Books != null && bookcovertype.Books.Count > 0)
                    {
                        return false;
                    }
                    context.BookCoverTypes.Remove(bookcovertype);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa ngôn ngữ {ex.Message}");
                return false;
            }
        }

		public async Task<List<BookCoverType>> SearchBookCoverTypesAsync(string searchText)
		{
			using (var context = _contextFactory())
			{
				var query = context.BookCoverTypes
					.AsQueryable();

				if (!string.IsNullOrWhiteSpace(searchText))
				{
					searchText = searchText.ToLower();
					query = query.Where(i =>
						i.BookCoverTypeID.ToString().Contains(searchText) ||
						(i.BookCoverTypeName != null && i.BookCoverTypeName.ToLower().Contains(searchText))
					);
				}

				return await query
					.OrderByDescending(i => i.BookCoverTypeID)
					.ToListAsync();
			}
		}

		public async Task<bool> IsBookCoverTypeNameExistsAsync(string bookCoverTypeName, int? excludeId = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = context.BookCoverTypes.AsQueryable();

                    if (excludeId.HasValue)
                    {
                        query = query.Where(s => s.BookCoverTypeID != excludeId.Value);
                    }

                    return await query.AnyAsync(s =>
                        s.BookCoverTypeName.ToLower() == bookCoverTypeName.ToLower());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra bìa sách: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Import>> GetAllImportsAsync()
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Imports
                        .Include(i => i.Supplier)
                        .Include(i => i.Staff)
                        .Include(i => i.ImportDetails)
                            .ThenInclude(id => id.Book)
                        .OrderByDescending(i => i.ImportDate)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Lỗi khi truy xuất danh sách Import", ex);
            }
        }

        public async Task<Import> GetImportByIdAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Imports
                        .Include(i => i.Supplier)
                        .Include(i => i.ImportDetails)
                        .ThenInclude(id => id.Book)
                        .FirstOrDefaultAsync(i => i.ImportID == id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi phiếu nhập theo ID: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddImportAsync(Import import, List<ImportDetail> details)
        {
            using (var context = _contextFactory())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Thêm Import
                        await context.Imports.AddAsync(import);
                        await context.SaveChangesAsync();

                        foreach (var detail in details)
                        {
                            detail.ImportID = import.ImportID;

                            await context.ImportDetails.AddAsync(detail);

                            // Tìm và cập nhật Book
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book != null)
                            {
                                book.Quantity += detail.Quantity;
                                book.Price = detail.UnitImportPrice;
                            }
                        }

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Lỗi: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        }
                        return false;
                    }
                }
            }
        }
        public async Task<bool> UpdateImportAsync(Import import, List<ImportDetail> newDetails)
        {
            using (var context = _contextFactory())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Lấy phiếu nhập hiện tại
                        var existingImport = await context.Imports
                            .Include(i => i.ImportDetails)
                            .FirstOrDefaultAsync(i => i.ImportID == import.ImportID);

                        if (existingImport == null)
                        {
                            return false;
                        }

                        // Kiểm tra trạng thái phiếu nhập
                        if (existingImport.ImportStatus == "Đã nhập")
                        {
                            return false;
                        }

                        // Cập nhật thông tin phiếu nhập
                        existingImport.SupplierID = import.SupplierID;
                        existingImport.StaffID = import.StaffID;
                        existingImport.ImportDate = import.ImportDate;
                        existingImport.ImportStatus = import.ImportStatus;
                        existingImport.Notes = import.Notes;
                        existingImport.CreateBy = import.CreateBy;
                        existingImport.TotalAmount = import.TotalAmount;

                        // Lấy danh sách các chi tiết cũ để hoàn lại số lượng sách
                        var oldDetails = await context.ImportDetails
                            .Where(d => d.ImportID == import.ImportID)
                            .ToListAsync();

                        // Hoàn lại số lượng sách từ các chi tiết cũ
                        foreach (var oldDetail in oldDetails)
                        {
                            var book = await context.Books.FindAsync(oldDetail.BookID);
                            if (book != null)
                            {
                                book.Quantity -= oldDetail.Quantity; // Hoàn lại số lượng đã nhập
                                if (book.Quantity < 0)
                                {
                                    book.Quantity = 0; // Đảm bảo số lượng không âm
                                }
                            }
                        }

                        // Xóa các chi tiết cũ
                        context.ImportDetails.RemoveRange(oldDetails);

                        // Thêm các chi tiết mới
                        foreach (var detail in newDetails)
                        {
                            detail.ImportID = import.ImportID;
                            await context.ImportDetails.AddAsync(detail);

                            // Cập nhật số lượng sách
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book != null)
                            {
                                book.Quantity += detail.Quantity;
                                book.Price = detail.UnitImportPrice;
                            }
                        }

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Lỗi khi cập nhật phiếu nhập: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        }
                        return false;
                    }
                }
            }
        }
        public async Task<bool> DeleteImportAsync(int id)
        {
            using (var context = _contextFactory())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Lấy phiếu nhập và chi tiết
                        var import = await context.Imports
                            .Include(i => i.ImportDetails)
                            .FirstOrDefaultAsync(i => i.ImportID == id);

                        if (import == null)
                        {
                            return false;
                        }

                        if (import.ImportStatus == "Đã nhập")
                        {
                            return false; 
                        }

                        // Hoàn lại số lượng sách từ các chi tiết
                        foreach (var detail in import.ImportDetails)
                        {
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book != null)
                            {
                                book.Quantity -= detail.Quantity;
                                if (book.Quantity < 0)
                                {
                                    book.Quantity = 0; 
                                }
                            }
                        }

                        // Xóa các chi tiết
                        context.ImportDetails.RemoveRange(import.ImportDetails);

                        // Xóa phiếu nhập
                        context.Imports.Remove(import);

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Lỗi: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        // 6. Tìm kiếm phiếu nhập
        public async Task<List<Import>> SearchImportsAsync(string searchText)
        {
            using (var context = _contextFactory())
            {
                var query = context.Imports
                    .Include(i => i.Supplier)
                    .Include(i => i.Staff)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    searchText = searchText.ToLower();
                    query = query.Where(i =>
                        i.ImportID.ToString().Contains(searchText) ||
                        i.Supplier.SupplierName.ToLower().Contains(searchText) ||
                        i.Staff.FullName.ToLower().Contains(searchText) ||
                        i.ImportStatus.ToLower().Contains(searchText) ||
                        i.CreateBy.ToLower().Contains(searchText)
                    );
                }

                return await query
                    .OrderByDescending(i => i.ImportDate)
                    .ToListAsync();
            }
        }

        // 7. Lấy số lượng phiếu nhập theo trạng thái
        public async Task<Dictionary<string, int>> GetImportCountByStatusAsync()
        {
            using (var context = _contextFactory())
            {
                return await context.Imports
                    .GroupBy(i => i.ImportStatus)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);
            }
        }

        // 8. Lấy tổng giá trị nhập hàng trong khoảng thời gian
        public async Task<decimal> GetTotalImportAmountInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            using (var context = _contextFactory())
            {
                return await context.Imports
                    .Where(i => i.ImportDate >= startDate && i.ImportDate <= endDate && i.ImportStatus == "Đã nhập")
                    .SumAsync(i => i.TotalAmount);
            }
        }

        // 9. Cập nhật trạng thái phiếu nhập
        public async Task<bool> UpdateImportStatusAsync(int importId, string newStatus)
        {
            using (var context = _contextFactory())
            {
                var import = await context.Imports.FindAsync(importId);
                if (import == null)
                {
                    return false;
                }

                import.ImportStatus = newStatus;
                await context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> UpdateImportInfoAsync(Import import)
        {
            using (var context = _contextFactory())
            {
                try
                {
                    // Lấy phiếu nhập hiện tại
                    var existingImport = await context.Imports.FindAsync(import.ImportID);
                    if (existingImport == null)
                        return false;

                    // Kiểm tra trạng thái phiếu nhập
                    if (existingImport.ImportStatus == "Đã nhập")
                        return false;

                    // Cập nhật thông tin phiếu nhập
                    existingImport.SupplierID = import.SupplierID;
                    existingImport.StaffID = import.StaffID;
                    existingImport.ImportDate = import.ImportDate;
                    existingImport.ImportStatus = import.ImportStatus;
                    existingImport.Notes = import.Notes;
                    existingImport.CreateBy = import.CreateBy;
                    existingImport.CreateTime = import.CreateTime;

                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in UpdateImportInfoAsync: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> UpdateImportDetailsAsync(int importId, List<ImportDetail> newDetails)
        {
            using (var context = _contextFactory())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Lấy phiếu nhập hiện tại
                        var existingImport = await context.Imports
                            .Include(i => i.ImportDetails)
                            .FirstOrDefaultAsync(i => i.ImportID == importId);

                        if (existingImport == null)
                            return false;

                        // Kiểm tra trạng thái phiếu nhập
                        if (existingImport.ImportStatus == "Đã nhập")
                            return false;

                        // Hoàn lại số lượng sách từ các chi tiết cũ
                        foreach (var oldDetail in existingImport.ImportDetails)
                        {
                            var book = await context.Books.FindAsync(oldDetail.BookID);
                            if (book != null)
                            {
                                book.Quantity -= oldDetail.Quantity; // Hoàn lại số lượng đã nhập
                                if (book.Quantity < 0)
                                    book.Quantity = 0;
                            }
                        }

                        // Xóa các chi tiết cũ
                        context.ImportDetails.RemoveRange(existingImport.ImportDetails);

                        // Thêm các chi tiết mới
                        foreach (var detail in newDetails)
                        {
                            detail.ImportID = importId;
                            await context.ImportDetails.AddAsync(detail);

                            // Cập nhật số lượng sách
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book != null)
                            {
                                book.Quantity += detail.Quantity;
                                book.Price = detail.UnitImportPrice;
                            }
                        }

                        // Cập nhật tổng tiền
                        existingImport.TotalAmount = newDetails.Sum(d => d.Subtotal);

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error in UpdateImportDetailsAsync: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public async Task<bool> UpdateImportTotalAmount(int importId, decimal totalAmount)
        {
            using (var context = _contextFactory())
            {
                try
                {
                    var import = await context.Imports.FindAsync(importId);
                    if (import == null)
                        return false;

                    import.TotalAmount = totalAmount;
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in UpdateImportTotalAmount: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            using (var context = _contextFactory())
            {
                return await context.Accounts
                    .Include(a => a.Staff)
                    .FirstOrDefaultAsync(a => a.Username == username);
            }
        }

        public async Task<Staff> GetStaffByIdAsync(int staffID)
        {
            using (var context = _contextFactory())
            {
                return await context.Staffs
                    .FirstOrDefaultAsync(s => s.StaffID == staffID);
            }
        }

        public async Task<bool> UpdateLastLoginAsync(int accountID)
        {
            using (var context = _contextFactory())
            {
                try
                {
                    var account = await context.Accounts.FindAsync(accountID);
                    if (account != null)
                    {
                        account.LastLogin = DateTime.Now;
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<bool> AddAccountAsync(Account account)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Accounts.Add(account);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm tài khoản: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Accounts.Update(account);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi sửa tài khoản: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var account = await context.Accounts.FindAsync(accountId);
                    if (account == null)
                    {
                        return false;
                    }
                    context.Accounts.Remove(account);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa tài khoản: {ex.Message}");
                return false;
            }

        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Accounts
                        .Include(a => a.Staff)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách tài khoản: {ex.Message}");
                return new List<Account>();
            }
        }

        public async Task<bool> IsUsernameExistsAsync(string username, int? excludeId = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = context.Accounts.AsQueryable();

                    if (excludeId.HasValue)
                    {
                        query = query.Where(a => a.AccountID != excludeId.Value);
                    }

                    return await query.AnyAsync(a => a.Username.ToLower() == username.ToLower());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra username: {ex.Message}");
                return false;
            }
        }

		public async Task<List<Staff>> GetAllStaffsAsync()
		{
			try
			{
				using (var context = _contextFactory())
				{
					var staffs = await context.Staffs.ToListAsync();
					Console.WriteLine($"GetAllStaffsAsync: Retrieved {staffs.Count} staffs");
					return staffs;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy danh sách nhân viên: {ex.Message}");
				return new List<Staff>();
			}
		}

		public async Task<List<Staff>> GetStaffsWithoutAccountAsync()
        {
            try
            {
                using (var context = _contextFactory())
                {
                    // Lấy danh sách StaffID đã có account
                    var staffIdsWithAccount = await context.Accounts
                        .Select(a => a.StaffID)
                        .ToListAsync();

                    // Lấy staffs chưa có account
                    return await context.Staffs
                        .Where(s => s.Status == "Đang làm" && !staffIdsWithAccount.Contains(s.StaffID))
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy nhân viên chưa có tài khoản: {ex.Message}");
                return new List<Staff>();
            }
        }

        public async Task<Account> GetAccountByStaffIdAsync(int staffId)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Accounts
                        .Include(a => a.Staff)
                        .FirstOrDefaultAsync(a => a.StaffID == staffId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tìm account theo StaffID: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddStaffAsync(Staff staff)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.Staffs.Add(staff);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm nhân viên {ex.Message}");
                return false;
            }
        }

		public async Task<bool> EditStaffAsync(Staff staff)
		{
			try
			{
				using (var context = _contextFactory())
				{
					var existingStaff = await context.Staffs.FindAsync(staff.StaffID);
                    if (existingStaff == null)
                    {
                        return false;
                    }

                    existingStaff.FullName = staff.FullName;
                    existingStaff.Phone = staff.Phone;
                    existingStaff.Email = staff.Email;
                    existingStaff.Address = staff.Address;
                    existingStaff.BirthDate = staff.BirthDate;
                    existingStaff.Position = staff.Position;
                    existingStaff.HireDate = staff.HireDate;
                    existingStaff.Status = staff.Status;

                    return await context.SaveChangesAsync() > 0;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi cập nhật nhân viên {ex.Message}");
				return false;
			}
		}

        public async Task<bool> DeleteStaffAsync(int staffId)
		{
            try
            {
                using (var context = _contextFactory())
                {
                    var hasAccount = await context.Accounts.AnyAsync(a => a.StaffID == staffId);
                    if (hasAccount)
                    {
                        return false;
                    }

                    //Kiểm tra xem nhân viên có liên quan đến hóa đơn/nhập hàng hay không
                    var hasImport = await context.Imports.AnyAsync(a => a.StaffID == staffId);
                    var hasInvoice = await context.Invoices.AnyAsync(a => a.StaffID == staffId);

                    if (hasImport || hasInvoice)
                    {
                        return false;
                    }

                    var staff = await context.Staffs.FindAsync(staffId);
                    if (staff == null)
                    {
                        return false;
                    }

                    context.Staffs.Remove(staff);
                    return await context.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa nhân viên {ex.Message}");
                return true;
            }
		}

        public async Task<bool> IsStaffEmailExistsAsync(string email, int? excludeId = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = context.Staffs.AsQueryable();
                    if (excludeId.HasValue)
                    {
                        query = query.Where(s => s.StaffID != excludeId.Value);
                    }
                    return await query.AnyAsync(s => s.Email.ToLower() == email.ToLower());
                }
            }
            catch (Exception ex)
            {
				Console.WriteLine($"Lỗi khi kiểm tra email: {ex.Message}");
				return false;
			}
        }

		public async Task<List<Account>> SearchAccountsAsync(string searchText)
		{
			using (var context = _contextFactory())
			{
				var query = context.Accounts
					.Include(i => i.Staff)
					.AsQueryable();

				if (!string.IsNullOrWhiteSpace(searchText))
				{
					searchText = searchText.ToLower();
					query = query.Where(i =>
						i.AccountID.ToString().Contains(searchText) ||
						i.Staff != null && i.Staff.FullName.ToLower().Contains(searchText) ||
						i.Username != null && i.Username.ToLower().Contains(searchText)
					);
				}

				return await query
					.OrderByDescending(i => i.CreatedDate)
					.ToListAsync();
			}
		}

		public async Task<bool> IsStaffPhoneExistsAsync(string phone, int? excludeId = null)
		{
			try
			{
				using (var context = _contextFactory())
				{
					var query = context.Staffs.AsQueryable();
					if (excludeId.HasValue)
					{
						query = query.Where(s => s.StaffID != excludeId.Value);
					}
					return await query.AnyAsync(s => s.Phone == phone);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi kiểm tra số điện thoại: {ex.Message}");
				return false;
			}
		}

        public async Task<List<Staff>> SearchStaffsAsync(string searchText)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = context.Staffs.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        searchText = searchText.ToLower();

                        query = query.Where(s =>
                            s.FullName.ToLower().Contains(searchText) ||
                            s.Phone.ToLower().Contains(searchText) ||
                            s.Email.ToLower().Contains(searchText) ||
                            s.Position.ToLower().Contains(searchText) ||
                            s.StaffID.ToString().Contains(searchText)

                        );
                    }
                    return await query
                        .AsNoTracking()
                        .OrderBy(s => s.FullName)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
				Console.WriteLine($"Lỗi khi tìm kiếm nhân viên: {ex.Message}");
				return new List<Staff>();
			}
        }
		public async Task<int> GetStaffCountByStatusAsync(string status)
		{
			try
			{
				using (var context = _contextFactory())
				{
					return await context.Staffs
						.Where(s => s.Status == status)
						.CountAsync();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi: {ex.Message}");
				return 0;
			}
		}


		// Lấy danh sách hóa đơn
		public async Task<List<Invoice>> GetAllInvoicesAsync()
		{
			try
			{
				using (var context = _contextFactory())
				{
					return await context.Invoices
						.Include(i => i.Staff)
						.Include(i => i.InvoiceDetails)
							.ThenInclude(id => id.Book)
						.OrderByDescending(i => i.InvoiceDate)
						.ToListAsync();
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Lỗi khi truy xuất danh sách hóa đơn", ex);
			}
		}

		//Lấy hóa đơn theo ID
		public async Task<Invoice> GetInvoiceByIdAsync(int id)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    return await context.Invoices
                        .Include(i => i.Staff)
                        .Include(i => i.InvoiceDetails)
                        .ThenInclude(id => id.Book)
                        .FirstOrDefaultAsync(i => i.InvoiceID == id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi try xuất hóa đơn theo ID: {ex.Message}");
                return  null;
            }
        }

        //Thêm mới hóa đơn
        public async Task<bool> AddInvoiceAsync(Invoice invoice, List<InvoiceDetail> details)
        {
            using (var context = _contextFactory())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //Kiểm tra tồn kho trước khi tạo hóa đơn
                        foreach (var detail in details)
                        {
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book == null || book.Quantity < detail.Quantity)
                            {
                                Console.WriteLine($"Sách {detail.BookID} không đủ tồn kho");
                                return false;
                            }
                        }

                        invoice.InvoiceDate = DateTime.Now;
                        await context.Invoices.AddAsync(invoice);
                        await context.SaveChangesAsync();

                        //Thêm chi tiết hóa đơn và cập nhật tồn kho
                        foreach (var detail in details)
                        {
                            detail.InvoiceID = invoice.InvoiceID;
                            detail.Subtotal = detail.Quantity * detail.UnitPrice;
                            await context.InvoiceDetails.AddAsync(detail);

                            //Cập nhật số lượng sách
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book != null)
                            {
                                book.Quantity -= detail.Quantity;
                                if (book.Quantity < 0)
                                {
                                    book.Quantity = 0;
                                }
                            }
                        }

                        //Tính tổng tiền hóa đơn
                        invoice.TotalAmount = details.Sum(d => d.Subtotal);

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                        
                    }
                    catch (Exception ex)
                    {
						await transaction.RollbackAsync();
						Console.WriteLine($"Lỗi khi thêm hóa đơn: {ex.Message}");
						if (ex.InnerException != null)
						{
							Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
						}
                        return false;
					}

                }
            }
        }

        public async Task<bool> UpdateInvoiceAsync(Invoice invoice, List<InvoiceDetail> newDetails)
        {
            using(var context = _contextFactory())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var existingInvoice = await context.Invoices
                            .Include(i => i.InvoiceDetails)
                            .FirstOrDefaultAsync(i => i.InvoiceID == invoice.InvoiceID);

                        if (existingInvoice == null)
                        {
                            return false;
                        }

                        if (existingInvoice.Status == "Đã thanh toán")
                        {
                            return false;
                        }

                        //Hoàn lại số lượng sách từ chi tiết cũ
                        foreach (var oldDetail in existingInvoice.InvoiceDetails)
                        {
                            var book = await context.Books.FindAsync(oldDetail.BookID);
                            if (book != null)
                            {
                                book.Quantity += oldDetail.Quantity; // Hiển thị lại số lượng sách thay đổi
                            }
                        }
                        //Kiểm tra tồn kho cho chi tiết mới
                        foreach (var detail in newDetails)
                        {
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book == null || book.Quantity < detail.Quantity)
                            {
                                Console.WriteLine($"Sách {detail.BookID} không đủ tồn kho.");
                                return false;
                            }
                        }
                        //Cập nhật thông tin hóa đơn
                        existingInvoice.InvoiceCode = invoice.InvoiceCode;
                        existingInvoice.InvoiceDate = invoice.InvoiceDate;
                        existingInvoice.CustomerName = invoice.CustomerName;
                        existingInvoice.CustomerPhone = invoice.CustomerPhone;
                        existingInvoice.PaymentMethod = invoice.PaymentMethod;
                        existingInvoice.Status = invoice.Status;
                        existingInvoice.Notes = invoice.Notes;
                        existingInvoice.StaffID = invoice.StaffID;
                        existingInvoice.CreateBy = invoice.CreateBy;

                        //Xóa chi tiết cũ
                        context.InvoiceDetails.RemoveRange(existingInvoice.InvoiceDetails);

                        //Thêm chi tiết mới
                        foreach (var detail in newDetails)
                        {
                            detail.InvoiceID = invoice.InvoiceID;
                            detail.Subtotal = detail.Quantity * detail.UnitPrice;
                            await context.InvoiceDetails.AddAsync(detail);

                            //Cập nhật số lượng sách
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book != null)
                            {
                                book.Quantity -= detail.Quantity; //Giảm tồn kho

                                if (book.Quantity < 0)
                                {
                                    book.Quantity = 0;
                                }
                            }
                        }

                        //Cập nhật tổng tiền
                        existingInvoice.TotalAmount = newDetails.Sum(d => d.Subtotal);

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
						await transaction.RollbackAsync();
						Console.WriteLine($"Lỗi khi cập nhật hóa đơn: {ex.Message}");
						if (ex.InnerException != null)
						{
							Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
						}
						return false;
					}
                }
            }    
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            using (var context = _contextFactory())
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //Lấy hóa đơn và chi tiết
                        var invoice = await context.Invoices
                            .Include(i => i.InvoiceDetails)
                            .FirstOrDefaultAsync(i => i.InvoiceID == id);

                        if (invoice == null || invoice.Status == "Đã thanh toán")
                        {
                            return false;
                        }

                        //Hoàn lại số lượng sách
                        foreach (var detail in invoice.InvoiceDetails)
                        {
                            var book = await context.Books.FindAsync(detail.BookID);
                            if (book != null)
                            {
                                book.Quantity += detail.Quantity;
                            }
                        }

						context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);

						// Xóa hóa đơn
						context.Invoices.Remove(invoice);

						await context.SaveChangesAsync();
						await transaction.CommitAsync();
						return true;
					}
                    catch (Exception ex)
                    {
						await transaction.RollbackAsync();
						Console.WriteLine($"Lỗi khi hủy hóa đơn: {ex.Message}");
						return false;
					}
                }
            }
        }

		public async Task<List<Invoice>> SearchInvoicesAsync(string searchText)
		{
            using (var context = _contextFactory())
            {
                var query = context.Invoices
				.Include(i => i.Staff)
				.Include(i => i.InvoiceDetails)
                    .ThenInclude(id => id.Book)
                .AsQueryable();

				if (!string.IsNullOrWhiteSpace(searchText))
				{
					searchText = searchText.ToLower();
					query = query.Where(i =>
						i.InvoiceID.ToString().Contains(searchText) ||
						(i.CustomerName != null && i.CustomerName.ToLower().Contains(searchText)) ||
						(i.CustomerPhone != null && i.CustomerPhone.ToLower().Contains(searchText)) ||
						i.Staff.FullName.ToLower().Contains(searchText) ||
						i.Status.ToLower().Contains(searchText) ||
						i.CreateBy.ToLower().Contains(searchText)
					);
				}

				return await query
					.OrderByDescending(i => i.InvoiceDate)
					.ToListAsync();
			}
		}

        //Đếm số lượng hóa đơn theo trạng thái
        public async Task<Dictionary<string, int>> GetInvoiceCountByStatusAsync()
        {
            using(var context = _contextFactory())
            {
                return await context.Invoices
                    .GroupBy(i => i.Status)
                    .Select(g => new {Status = g.Key, Count = g.Count()})
					.ToDictionaryAsync(x => x.Status, x => x.Count);
			}    
        }

        //Tính tổng giá trị hóa đơn trong khoảng thời gian
		public async Task<decimal> GetTotalInvoiceAmountInPeriodAsync(DateTime startDate, DateTime endDate)
		{
			using (var context = _contextFactory())
			{
				return await context.Invoices
					.Where(i => i.CreateTime >= startDate && i.CreateTime <= endDate && i.Status == "Đã thanh toán")
					.SumAsync(i => i.TotalAmount);
			}
		}

		//Cập nhật trạng thái hóa đơn
		public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string newStatus)
		{
			using (var context = _contextFactory())
			{
				try
				{
					var invoice = await context.Invoices.FindAsync(invoiceId);
					if (invoice == null)
					{
						return false;
					}

					invoice.Status = newStatus;
					await context.SaveChangesAsync();
					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Lỗi khi cập nhật trạng thái hóa đơn: {ex.Message}");
					return false;
				}
			}
		}

		//Xử lý thanh toán
		public async Task<bool> ProcessPaymentAsync(int invoiceId, string paymentMethod)
		{
			using (var context = _contextFactory())
			{
				try
				{
					var invoice = await context.Invoices.FindAsync(invoiceId);
					if (invoice == null || invoice.Status == "Đã thanh toán")
					{
						return false;
					}

					invoice.PaymentMethod = paymentMethod;
					invoice.Status = "Đã thanh toán";
					await context.SaveChangesAsync();
					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Lỗi khi xử lý thanh toán: {ex.Message}");
					return false;
				}
			}
		}

		public async Task<bool> UpdateInvoiceInfoAsync(Invoice invoice)
		{
			using (var context = _contextFactory())
			{
				try
				{
					// Lấy hóa đơn hiện tại
					var existingInvoice = await context.Invoices.FindAsync(invoice.InvoiceID);
					if (existingInvoice == null)
						return false;

					// Kiểm tra trạng thái hóa đơn
					if (existingInvoice.Status == "Đã thanh toán")
						return false;

					// Cập nhật thông tin phiếu nhập
					existingInvoice.InvoiceCode = invoice.InvoiceCode;
					existingInvoice.CreateTime = DateTime.Now;
                    existingInvoice.CustomerName = invoice.CustomerName;
					existingInvoice.CustomerPhone = invoice.CustomerPhone;
					existingInvoice.PaymentMethod = invoice.PaymentMethod;
					existingInvoice.Status = invoice.Status;
					existingInvoice.Notes = invoice.Notes;
					existingInvoice.StaffID = invoice.StaffID;
					existingInvoice.CreateBy = invoice.CreateBy;
					await context.SaveChangesAsync();
					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error in UpdateInvoiceInfoAsync: {ex.Message}");
					return false;
				}
			}
		}

		public async Task<bool> UpdateInvoiceTotalAmount(int invoiceId, decimal totalAmount)
		{
			using (var context = _contextFactory())
			{
				try
				{
					var invoice = await context.Invoices.FindAsync(invoiceId);
					if (invoice == null) return false;
					invoice.TotalAmount = totalAmount;
					await context.SaveChangesAsync();
					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Lỗi khi cập nhật tổng tiền: {ex.Message}");
					return false;
				}
			}
		}

		public async Task<bool> UpdateInvoiceDetailsAsync(int invoiceId, List<InvoiceDetail> details)
		{
			using (var context = _contextFactory())
			{
				using (var transaction = await context.Database.BeginTransactionAsync())
				{
					try
					{
						var invoice = await context.Invoices
							.Include(i => i.InvoiceDetails)
							.FirstOrDefaultAsync(i => i.InvoiceID == invoiceId);
						if (invoice == null || invoice.Status == "Đã thanh toán")
							return false;

						foreach (var oldDetail in invoice.InvoiceDetails)
						{
							var book = await context.Books.FindAsync(oldDetail.BookID);
							if (book != null) book.Quantity += oldDetail.Quantity;
						}

						foreach (var detail in details)
						{
							var book = await context.Books.FindAsync(detail.BookID);
							if (book == null || book.Quantity < detail.Quantity) return false;
							book.Quantity -= detail.Quantity;
							if (book.Quantity < 0) book.Quantity = 0;
						}

						context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
						foreach (var detail in details)
						{
							detail.InvoiceID = invoiceId;
							detail.Subtotal = detail.Quantity * detail.UnitPrice;
							await context.InvoiceDetails.AddAsync(detail);
						}

						invoice.TotalAmount = details.Sum(d => d.Subtotal);
						await context.SaveChangesAsync();
						await transaction.CommitAsync();
						return true;
					}
					catch (Exception ex)
					{
						await transaction.RollbackAsync();
						Console.WriteLine($"Lỗi khi cập nhật chi tiết: {ex.Message}");
						return false;
					}
				}
			}
		}

        public async Task<List<CategoryRevenue>> GetRevenueByCategoryAsync(DateTime? startDate, DateTime? endDate, string status = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var query = from c in context.Categories
                                join b in context.Books on c.CategoryID equals b.CategoryID
                                join id in context.InvoiceDetails on b.ID equals id.BookID
                                join i in context.Invoices on id.InvoiceID equals i.InvoiceID
                                where (!startDate.HasValue || i.InvoiceDate >= startDate)
                                  && (!endDate.HasValue || i.InvoiceDate <= endDate)
                                  && (string.IsNullOrEmpty(status) || i.Status == status)

                                group id by c.CategoryName into g
                                select new CategoryRevenue
                                {
                                    CategoryName = g.Key,
                                    Revenue = g.Sum(x => x.Subtotal)
                                };

                    var result = await query.ToListAsync();
                    var totalRevenue = result.Sum(x => x.Revenue);
                    foreach (var item in result)
                    {
                        item.Percentage = totalRevenue > 0 ? (double)(item.Revenue / totalRevenue * 100) : 0;
                    }
                    return result.OrderByDescending(x => x.Revenue).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy doanh thu theo thể loại: {ex.Message}");
                throw new ApplicationException("Lỗi khi lấy doanh thu theo thể loại", ex);
            }
        }
    }
}