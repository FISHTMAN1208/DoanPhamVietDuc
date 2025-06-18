using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class Supplier
	{
		[Required]
		[Key]
		public int SupplierID { get; set; }

		[Required]
		[MaxLength(50)]
		public string SupplierName { get; set; }

		[Required]
		[MaxLength(50)]
		public string SupplierPhone { get; set; }

		[Required]
		[MaxLength(50)]
		public string SupplierEmail { get; set; }

		[Required]
		[MaxLength(50)]
		public string SupplierAddress { get; set; }

		[Required]
		[MaxLength(50)]
		public string TaxCode { get; set; }

		[Required]
		[MaxLength(50)]
		public string Status { get; set; }

		public virtual ICollection<Book> Books { get; set; }
	}
}
