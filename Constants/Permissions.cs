using DoanPhamVietDuc.Enums;

namespace DoanPhamVietDuc.Constants
{
	public static class Permissions
	{
		// Book permissions
		public const string CanViewBooks = "CanViewBooks";
		public const string CanCreateBooks = "CanCreateBooks";
		public const string CanEditBooks = "CanEditBooks";
		public const string CanDeleteBooks = "CanDeleteBooks";

		// Category permissions
		public const string CanViewCategories = "CanViewCategories";
		public const string CanManageCategories = "CanManageCategories";

		// Supplier permissions
		public const string CanViewSuppliers = "CanViewSuppliers";
		public const string CanManageSuppliers = "CanManageSuppliers";

		// Language permissions - THÊM MỚI
		public const string CanViewLanguages = "CanViewLanguages";
		public const string CanManageLanguages = "CanManageLanguages";

		// BookCoverType permissions - THÊM MỚI
		public const string CanViewBookCoverType = "CanViewBookCoverType";
		public const string CanManageBookCoverType = "CanManageBookCoverType";

		// Import permissions
		public const string CanViewImports = "CanViewImports";
		public const string CanCreateImports = "CanCreateImports";
		public const string CanEditImports = "CanEditImports";
		public const string CanDeleteImports = "CanDeleteImports";

		// Invoice permissions
		public const string CanViewInvoices = "CanViewInvoices";
		public const string CanCreateInvoices = "CanCreateInvoices";
		public const string CanEditInvoices = "CanEditInvoices";
		public const string CanDeleteInvoices = "CanDeleteInvoices";

		// Staff permissions
		public const string CanViewStaff = "CanViewStaff";
		public const string CanCreateStaff = "CanCreateStaff";
		public const string CanEditStaff = "CanEditStaff";
		public const string CanDeleteStaff = "CanDeleteStaff";

		// Report permissions
		public const string CanViewReports = "CanViewReports";
		public const string CanExportReports = "CanExportReports";

		// System permissions
		public const string CanManageSystem = "CanManageSystem";

		// Personal permissions
		public const string CanEditProfile = "CanEditProfile";
		public const string CanChangePassword = "CanChangePassword";
	}

	public static class RolePermissions
	{
		public static readonly Dictionary<string, string[]> Permissions = new()
		{
			[UserRoles.Admin] = new[]
            {
                // Books
                Constants.Permissions.CanViewBooks, Constants.Permissions.CanCreateBooks,
				Constants.Permissions.CanEditBooks, Constants.Permissions.CanDeleteBooks,

                // Categories & Suppliers
                Constants.Permissions.CanViewCategories, Constants.Permissions.CanManageCategories,
				Constants.Permissions.CanViewSuppliers, Constants.Permissions.CanManageSuppliers,

                // Languages & BookCoverTypes
                Constants.Permissions.CanViewLanguages, Constants.Permissions.CanManageLanguages,
				Constants.Permissions.CanViewBookCoverType, Constants.Permissions.CanManageBookCoverType,
                
                // Imports
                Constants.Permissions.CanViewImports, Constants.Permissions.CanCreateImports,
				Constants.Permissions.CanEditImports, Constants.Permissions.CanDeleteImports,
                
                // Invoices
                Constants.Permissions.CanViewInvoices, Constants.Permissions.CanCreateInvoices,
				Constants.Permissions.CanEditInvoices, Constants.Permissions.CanDeleteInvoices,
                
                // Staff
                Constants.Permissions.CanViewStaff, Constants.Permissions.CanCreateStaff,
				Constants.Permissions.CanEditStaff, Constants.Permissions.CanDeleteStaff,
                
                // Reports
                Constants.Permissions.CanViewReports, Constants.Permissions.CanExportReports,
                
                // System
                Constants.Permissions.CanManageSystem,
                
                // Personal
                Constants.Permissions.CanEditProfile, Constants.Permissions.CanChangePassword
			},

			[UserRoles.Staff] = new[]
            {
                // Books
                Constants.Permissions.CanViewBooks, Constants.Permissions.CanCreateBooks,
				Constants.Permissions.CanEditBooks, Constants.Permissions.CanDeleteBooks,

                // Categories & Suppliers
                Constants.Permissions.CanViewCategories, Constants.Permissions.CanManageCategories,
				Constants.Permissions.CanViewSuppliers, Constants.Permissions.CanManageSuppliers,

                // Languages & BookCoverTypes
                Constants.Permissions.CanViewLanguages, Constants.Permissions.CanManageLanguages,
				Constants.Permissions.CanViewBookCoverType, Constants.Permissions.CanManageBookCoverType,
                
                // Imports
                Constants.Permissions.CanViewImports, Constants.Permissions.CanCreateImports,
				Constants.Permissions.CanEditImports, Constants.Permissions.CanDeleteImports,
                
                // Invoices 
                Constants.Permissions.CanViewInvoices, Constants.Permissions.CanCreateInvoices,
				Constants.Permissions.CanEditInvoices, Constants.Permissions.CanDeleteInvoices,
                
                // Personal
                Constants.Permissions.CanEditProfile, Constants.Permissions.CanChangePassword
			}
		};

		public static bool HasPermission(string role, string permission)
		{
			return Permissions.ContainsKey(role) && Permissions[role].Contains(permission);
		}

		public static string[] GetPermissions(string role)
		{
			return Permissions.ContainsKey(role) ? Permissions[role] : new string[0];
		}
	}
}