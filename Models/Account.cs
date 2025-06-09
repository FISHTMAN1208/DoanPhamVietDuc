using DoanPhamVietDuc.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class Account
	{
		[Key]
		[Required]
		public int AccountID { get; set; }

		[Required]
		[MaxLength(50)]
		[UsernameValidation]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public int StaffID { get; set; }

		[Required]
		[MaxLength(20)]
		[RoleValidation]
		public string Role { get; set; }

		[Required]
		[MaxLength(20)]
		public string Status { get; set; }

		public DateTime? LastLogin { get; set; }

		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;


		public string? CreatedBy { get; set; }

		// Navigation property
		[ForeignKey("StaffID")]
		public virtual Staff Staff { get; set; }

		// Helper methods
		public bool IsActive => Status == "Active";
		public bool IsAdmin => Role == "Admin";
		public bool IsStaff => Role == "Staff";
	}
}