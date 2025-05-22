using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class Import
	{
		[Key]
		[Required]
		public int ImportID { get; set; }

		public int SupplierID { get; set; }


		public int? StaffID { get; set; }

		[Required]
		public DateTime ImportDate { get; set; }

		[Required]
		public decimal TotalAmount { get; set; }

		[Required]
		[MaxLength(20)]
		public string ImportStatus { get; set; }

		[MaxLength(500)]  
		public string Notes { get; set; }

		[Required]
		public DateTime CreateTime { get; set; } = DateTime.Now;

		[Required]
		[MaxLength(100)]
		public string CreateBy { get; set; }

		//========== Navigation Properties ==========
		[ForeignKey("SupplierID")]
		public virtual Supplier Supplier { get; set; }

		[ForeignKey("StaffID")]
		public virtual Staff Staff { get; set; }

		public virtual ICollection<ImportDetail> ImportDetails { get; set; }
	}
}