using DoanPhamVietDuc.Data;
using DoanPhamVietDuc.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Services.DataService
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

					if(supplier.Books != null && supplier.Books.Count > 0)
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

					if(language.Books != null &&  language.Books.Count > 0)
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

						// Kiểm tra trạng thái
						if (import.ImportStatus == "Đã nhập")
						{
							return false; // Không cho phép xóa phiếu đã hoàn thành
						}

						// Hoàn lại số lượng sách từ các chi tiết
						foreach (var detail in import.ImportDetails)
						{
							var book = await context.Books.FindAsync(detail.BookID);
							if (book != null)
							{
								book.Quantity -= detail.Quantity; // Hoàn lại số lượng
								if (book.Quantity < 0)
								{
									book.Quantity = 0; // Đảm bảo số lượng không âm
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

	}
}