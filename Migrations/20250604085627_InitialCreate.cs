using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoanPhamVietDuc.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookCoverTypes",
                columns: table => new
                {
                    BookCoverTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookCoverTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCoverTypes", x => x.BookCoverTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageID);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.StaffID);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierID);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_Accounts_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomerPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK_Invoices_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    BookCoverTypeID = table.Column<int>(type: "int", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PublishTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PageCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ISBNCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PublisherName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Books_BookCoverTypes_BookCoverTypeID",
                        column: x => x.BookCoverTypeID,
                        principalTable: "BookCoverTypes",
                        principalColumn: "BookCoverTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Books_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Books_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Books_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Imports",
                columns: table => new
                {
                    ImportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imports", x => x.ImportID);
                    table.ForeignKey(
                        name: "FK_Imports_Staffs_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID");
                    table.ForeignKey(
                        name: "FK_Imports_Suppliers_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierID");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                columns: table => new
                {
                    InvoiceDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceID = table.Column<int>(type: "int", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.InvoiceDetailID);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices_InvoiceID",
                        column: x => x.InvoiceID,
                        principalTable: "Invoices",
                        principalColumn: "InvoiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportDetails",
                columns: table => new
                {
                    ImportDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportID = table.Column<int>(type: "int", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitImportPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportDetails", x => x.ImportDetailID);
                    table.ForeignKey(
                        name: "FK_ImportDetails_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ImportDetails_Imports_ImportID",
                        column: x => x.ImportID,
                        principalTable: "Imports",
                        principalColumn: "ImportID");
                });

            migrationBuilder.InsertData(
                table: "BookCoverTypes",
                columns: new[] { "BookCoverTypeID", "BookCoverTypeName" },
                values: new object[,]
                {
                    { 1, "Bìa cứng" },
                    { 2, "Bìa mềm" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "CategoryName", "ModifyBy", "ModifyTime" },
                values: new object[,]
                {
                    { 1, "Văn học", "Phạm Việt Đức", new DateTime(2025, 6, 4, 15, 56, 25, 980, DateTimeKind.Local).AddTicks(5092) },
                    { 2, "Kinh tế", "Hoàng Thu Hiền", new DateTime(2025, 6, 4, 15, 56, 25, 980, DateTimeKind.Local).AddTicks(5095) },
                    { 3, "Kỹ năng sống", "Nguyễn Việt Anh", new DateTime(2025, 6, 4, 15, 56, 25, 980, DateTimeKind.Local).AddTicks(5096) }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "LanguageID", "Code", "LanguageName" },
                values: new object[,]
                {
                    { 1, "vi", "Tiếng Việt" },
                    { 2, "en", "Tiếng Anh" }
                });

            migrationBuilder.InsertData(
                table: "Staffs",
                columns: new[] { "StaffID", "Address", "BirthDate", "Email", "FullName", "HireDate", "Phone", "Position", "Status" },
                values: new object[,]
                {
                    { 1, "Hà Nội", new DateTime(2003, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "lethithuan@gmail.com", "Lê Thị Thu An", new DateTime(2024, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "0968475213", "Nhân viên chính thức", "Đang làm" },
                    { 2, "Hà Nội", new DateTime(2005, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "nvh@gmail.com", "Nguyễn Việt Hùng", new DateTime(2023, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "0356789145", "Nhân viên parttime", "Đang làm" }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "SupplierID", "Status", "SupplierAddress", "SupplierEmail", "SupplierName", "SupplierPhone", "TaxCode" },
                values: new object[] { 1, "Active", "Hà Nội", "kimdong@example.com", "NXB Kim Đồng", "0123456789", "123456789" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountID", "CreatedBy", "CreatedDate", "LastLogin", "Password", "Role", "StaffID", "Status", "Username" },
                values: new object[,]
                {
                    { 1, "Phạm Việt Đức", new DateTime(2025, 6, 4, 15, 56, 25, 980, DateTimeKind.Local).AddTicks(5357), null, "123456", "Admin", 1, "Active", "phamvietduc123" },
                    { 2, "Hoàng Thu Hiền", new DateTime(2025, 6, 4, 15, 56, 25, 980, DateTimeKind.Local).AddTicks(5361), null, "19062003", "Staff", 2, "Inactive", "chemchem123" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "ID", "Author", "BookCoverTypeID", "CategoryID", "Description", "ISBNCode", "ImageUrl", "LanguageID", "ModifyBy", "ModifyTime", "PageCount", "Price", "PublishTime", "PublisherName", "Quantity", "SupplierID", "Title" },
                values: new object[] { 1, "Nguyễn Nhật Ánh", 1, 1, "Mắt biếc là một tác phẩm của nhà văn Nguyễn Nhật Ánh...", "978-604-1-12345-6", "/Images/mat-biec.jpg", 1, "Admin", new DateTime(2025, 6, 4, 15, 56, 25, 980, DateTimeKind.Local).AddTicks(5328), 300, 85000m, new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "NXB Kim Đồng", 100, 1, "Mắt biếc" });

            migrationBuilder.InsertData(
                table: "Imports",
                columns: new[] { "ImportID", "CreateBy", "CreateTime", "ImportDate", "ImportStatus", "Notes", "StaffID", "SupplierID", "TotalAmount" },
                values: new object[] { 1, "Phạm Việt Đức", new DateTime(2025, 6, 4, 15, 56, 25, 980, DateTimeKind.Local).AddTicks(5503), new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã nhập", "Hàng loại 3, dùng để test", 1, 1, 300000m });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "InvoiceID", "CreateBy", "CreateTime", "CustomerName", "CustomerPhone", "InvoiceCode", "InvoiceDate", "Notes", "PaymentMethod", "StaffID", "Status", "TotalAmount" },
                values: new object[] { 1, "Lê Thị Thu An", new DateTime(2025, 6, 4, 15, 56, 25, 980, DateTimeKind.Local).AddTicks(5452), "Trương Mã Hóa", "0969288214", "2025-M1901", new DateTime(2025, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Không có", "Tiền mặt", 1, "Đã thanh toán", 720000m });

            migrationBuilder.InsertData(
                table: "ImportDetails",
                columns: new[] { "ImportDetailID", "BookID", "ImportID", "Quantity", "Subtotal", "UnitImportPrice" },
                values: new object[] { 1, 1, 1, 10, 300000m, 30000m });

            migrationBuilder.InsertData(
                table: "InvoiceDetails",
                columns: new[] { "InvoiceDetailID", "BookID", "InvoiceID", "Quantity", "Subtotal", "UnitPrice" },
                values: new object[] { 1, 1, 1, 3, 720000m, 240000m });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_StaffID",
                table: "Accounts",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookCoverTypeID",
                table: "Books",
                column: "BookCoverTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryID",
                table: "Books",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Books_LanguageID",
                table: "Books",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_Books_SupplierID",
                table: "Books",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetails_BookID",
                table: "ImportDetails",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetails_ImportID",
                table: "ImportDetails",
                column: "ImportID");

            migrationBuilder.CreateIndex(
                name: "IX_Imports_StaffID",
                table: "Imports",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Imports_SupplierID",
                table: "Imports",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_BookID",
                table: "InvoiceDetails",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceID",
                table: "InvoiceDetails",
                column: "InvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_StaffID",
                table: "Invoices",
                column: "StaffID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "ImportDetails");

            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "Imports");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "BookCoverTypes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Staffs");
        }
    }
}
