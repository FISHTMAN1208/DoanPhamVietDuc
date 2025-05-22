using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class ImportDetail
	{
		[Key]
		[Required]
		public int ImportDetailID { get; set; }

		[Required]
		public int ImportID { get; set; }

		[Required]
		public int BookID { get; set; }

		[Required]
		public int Quantity { get; set; }

		[Required]
		public decimal UnitImportPrice { get; set; }

		[Required]
		public decimal Subtotal { get; set; }

		//========== Navigation Properties ==========

		[ForeignKey("BookID")]
		public virtual Book Book { get; set; }

		[ForeignKey("ImportID")]
		public virtual Import Import { get; set; }
	}
}