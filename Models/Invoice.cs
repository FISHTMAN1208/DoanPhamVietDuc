using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class Invoice
	{
		[Key]
		[Required]
		public int InvoiceID { get; set; }

		[Required]
		[MaxLength(20)]
		public string InvoiceCode { get; set; }

		public DateTime? InvoiceDate { get; set; }

		[MaxLength(100)]
		public string? CustomerName { get; set; }

		[MaxLength(15)]
		public string? CustomerPhone { get; set; }

		[Required]
		public decimal TotalAmount { get; set; }

		[Required]
		[MaxLength(20)]
		public string PaymentMethod { get; set; }

		[Required]
		[MaxLength(20)]
		public string Status { get; set; }

		[MaxLength(500)]  
		public string Notes { get; set; }

		[Required]
		public int StaffID { get; set; }

		[Required]
		public DateTime? CreateTime { get; set; } = DateTime.Now;

		[Required]
		[MaxLength(100)]
		public string CreateBy { get; set; }

		// Navigation properties
		[ForeignKey("StaffID")]
		public virtual Staff Staff { get; set; }

		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
	}
}