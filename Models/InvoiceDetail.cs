using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class InvoiceDetail
	{
		[Key]
		[Required]
		public int InvoiceDetailID { get; set; }

		[Required]
		public int InvoiceID { get; set; }

		[Required]
		public int BookID { get; set; }

		[Required]
		public int Quantity { get; set; }

		[Required]
		public decimal UnitPrice { get; set; }

		[Required]
		public decimal Subtotal { get; set; }

		// Navigation properties
		[ForeignKey("InvoiceID")]
		public virtual Invoice Invoice { get; set; }

		[ForeignKey("BookID")]
		public virtual Book Book { get; set; }
	}
}