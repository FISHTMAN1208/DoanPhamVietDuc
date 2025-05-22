using Microsoft.EntityFrameworkCore;
using DoanPhamVietDuc.Models;

namespace DoanPhamVietDuc.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Book> Books { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Supplier> Suppliers { get; set; }
		public DbSet<Language> Languages { get; set; }
		public DbSet<BookCoverType> BookCoverTypes { get; set; }
		public DbSet<Staff> Staffs { get; set; }
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Import> Imports { get; set; }
		public DbSet<ImportDetail> ImportDetails { get; set; }
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Import>()
				.HasMany(i => i.ImportDetails)
				.WithOne(id => id.Import)
				.HasForeignKey(id => id.ImportID)
			.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<ImportDetail>()
				.HasOne(id => id.Book)
				.WithMany()
				.HasForeignKey(id => id.BookID)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Import>()
				.HasOne(i => i.Supplier)
				.WithMany()
				.HasForeignKey(i => i.SupplierID)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Import>()
				.HasOne(i => i.Staff)
				.WithMany(s => s.Imports)
				.HasForeignKey(i => i.StaffID)
				.OnDelete(DeleteBehavior.NoAction);


			SeedData(modelBuilder);
		}

		private void SeedData(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasData
				(
					new Category { CategoryID = 1, CategoryName = "Văn học", ModifyTime = DateTime.Now, ModifyBy = "Phạm Việt Đức"},
					new Category { CategoryID = 2, CategoryName = "Kinh tế", ModifyTime = DateTime.Now, ModifyBy = "Hoàng Thu Hiền" },
					new Category { CategoryID = 3, CategoryName = "Kỹ năng sống", ModifyTime = DateTime.Now, ModifyBy = "Nguyễn Việt Anh" }
				);

			modelBuilder.Entity<BookCoverType>().HasData
				(
					new BookCoverType { BookCoverTypeID = 1, BookCoverTypeName = "Bìa cứng" },
					new BookCoverType { BookCoverTypeID = 2, BookCoverTypeName = "Bìa mềm" }
				);
			modelBuilder.Entity<Language>().HasData
				(
					new Language { LanguageID = 1, LanguageName = "Tiếng Việt", Code = "vi" },
					new Language { LanguageID = 2, LanguageName = "Tiếng Anh", Code = "en" }
				);
			modelBuilder.Entity<Supplier>().HasData
				(
					new Supplier
					{
						SupplierID = 1,
						SupplierName = "NXB Kim Đồng",
						SupplierPhone = "0123456789",
						SupplierEmail = "kimdong@example.com",
						SupplierAddress = "Hà Nội",
						TaxCode = "123456789",
						Status = "Active"}
					);

			modelBuilder.Entity<Book>().HasData
				(
					new Book
					{
						ID = 1,
						CategoryID = 1,
						SupplierID = 1,
						LanguageID = 1,
						BookCoverTypeID = 1,
						Author = "Nguyễn Nhật Ánh",
						Title = "Mắt biếc",
						PublishTime = new DateTime(2019, 1, 1),
						Price = 85000,
						PageCount = 300,
						Description = "Mắt biếc là một tác phẩm của nhà văn Nguyễn Nhật Ánh...",
						ISBNCode = "978-604-1-12345-6",
						Quantity = 100,
						PublisherName = "NXB Kim Đồng",
						ImageUrl = "/Images/mat-biec.jpg",
						ModifyTime = DateTime.Now,
						ModifyBy = "Admin"
					}
				);

			modelBuilder.Entity<Account>().HasData
				(
					new Account
					{
						AccountID = 1,
						Username = "phamvietduc123",
						PasswordHash = "123456",
						StaffID = 1,
						Role = "Admin",
						Status = "Active",
						CreatedDate = DateTime.Now,
						CreatedBy = "Phạm Việt Đức"
					},

					new Account
					{
						AccountID = 2,
						Username = "chemchem123",
						PasswordHash = "19062003",
						StaffID = 1,
						Role = "Staff",
						Status = "Inactive",
						CreatedDate = DateTime.Now,
						CreatedBy = "Hoàng Thu Hiền"
					}
				);
			modelBuilder.Entity<Staff>().HasData
				(
					new Staff
					{
						StaffID = 1,
						FullName = "Lê Thị Thu An",
						Phone = "0968475213",
						Email = "lethithuan@gmail.com",
						Address = "Hà Nội",
						BirthDate = new DateTime(2003, 02, 15),
						Position = "Nhân viên chính thức",
						HireDate = new DateTime(2024, 06, 14),
						Status = "Đang làm",
					},
					new Staff
					{
						StaffID = 2,
						FullName = "Nguyễn Việt Hùng",
						Phone = "0356789145",
						Email = "nvh@gmail.com",
						Address = "Hà Nội",
						BirthDate = new DateTime(2005, 10, 05),
						Position = "Nhân viên parttime",
						HireDate = new DateTime(2023, 02, 23),
						Status = "Đang làm",
					}
				);
			modelBuilder.Entity<InvoiceDetail>().HasData
				(
					new InvoiceDetail
					{
						InvoiceDetailID = 1,
						InvoiceID = 1,
						BookID = 1,
						Quantity = 3,
						UnitPrice = 240000,
						Subtotal = 720000
					}
				);
			modelBuilder.Entity<Invoice>().HasData
				(
					new Invoice
					{
						InvoiceID = 1,
						InvoiceCode = "2025-M1901",
						InvoiceDate = new DateTime(2025, 05, 18),
						CustomerName = "Trương Mã Hóa",
						CustomerPhone = "0969288214",
						TotalAmount = 720000,
						PaymentMethod = "Tiền mặt",
						Status = "Đã thanh toán",
						Notes = "Không có",
						StaffID = 1,
						CreateTime = DateTime.Now,
						CreateBy = "Lê Thị Thu An",
					}
				);
			modelBuilder.Entity<ImportDetail>().HasData
				(
					new ImportDetail
					{
						ImportDetailID = 1,
						ImportID = 1,
						BookID = 1,
						Quantity = 10,
						UnitImportPrice = 30000,
						Subtotal = 300000

					}
				);
			modelBuilder.Entity<Import>().HasData
				(
					new Import
					{
						ImportID = 1 ,
						SupplierID = 1,
						StaffID= 1,
						ImportDate = new DateTime(2025, 5 , 15),
						TotalAmount = 300000,
						ImportStatus = "Đã nhập",
						Notes = "Hàng loại 3, dùng để test",
						CreateTime = DateTime.Now,
						CreateBy = "Phạm Việt Đức"
					}
				);
		}

	}
}