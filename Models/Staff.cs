using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class Staff
	{
		[Key]
		[Required]
		public int StaffID { get; set; }

		[Required]
		[MaxLength(100)]
		public string FullName { get; set; }

		[Required]
		[MaxLength(15)]
		public string Phone { get; set; }

		[Required]
		[MaxLength(100)]
		public string Email { get; set; }

		[Required]
		[MaxLength(255)]
		public string Address { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }

		[Required]
		[MaxLength(50)]
		public string Position { get; set; }  

		[Required]
		public DateTime HireDate { get; set; }

		[Required]
		[MaxLength(20)]
		public string Status { get; set; } 

		// Navigation properties
		public virtual ICollection<Import> Imports { get; set; }
		public virtual ICollection<Invoice> Invoices { get; set; }
	}
}