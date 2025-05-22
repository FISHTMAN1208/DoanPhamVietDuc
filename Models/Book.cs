using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class Book
	{
		[Key]
		public int ID { get; set; }

		[Required]
		public int CategoryID { get; set; }

		[Required]
		public int SupplierID { get; set; }

		[Required]
		public int LanguageID { get; set; }

		[Required]
		public int BookCoverTypeID { get; set; }

		[Required]
		public string Author { get; set; }

		[Required]
		public DateTime ModifyTime { get; set; } = DateTime.Now;

		[Required]
		public string ModifyBy { get; set; }

		[MaxLength(50)]
		[Required]
		public string Title { get; set; }

		[Required]
		public DateTime PublishTime { get; set; }

		[Required]
		public decimal Price { get; set; }

		[Required]
		public int PageCount { get; set; }

		[MaxLength(1000)]
		[Required]
		public string Description { get; set; }

		[MaxLength(20)]
		[Required]
		public string ISBNCode { get; set; }

		[Required]
		public int Quantity { get; set; }

		[MaxLength(50)]
		[Required]
		public string PublisherName { get; set; }

		[Required]
		public string ImageUrl { get; set; }

		//========== Navigation Properties ==========
		[ForeignKey("CategoryID")]
		public virtual Category Category { get; set; }

		[ForeignKey("SupplierID")]
		public virtual Supplier Supplier { get; set; }

		[ForeignKey("LanguageID")]
		public virtual Language Language { get; set; }

		[ForeignKey("BookCoverTypeID")]
		public virtual BookCoverType BookCoverType { get; set; }

	}
}
