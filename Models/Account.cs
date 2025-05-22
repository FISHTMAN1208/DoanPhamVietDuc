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
		public string Username { get; set; }

		[Required]
		public string PasswordHash { get; set; }

		[Required]
		public int StaffID { get; set; }

		[Required]
		[MaxLength(20)]
		public string Role { get; set; } 

		[Required]
		[MaxLength(20)]
		public string Status { get; set; }

		public DateTime? LastLogin { get; set; }

		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;

		[Required]
		[MaxLength(100)]
		public string CreatedBy { get; set; }

		// Navigation property
		[ForeignKey("StaffID")]
		public virtual Staff Staff { get; set; }
	}
}