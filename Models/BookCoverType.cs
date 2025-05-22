using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class BookCoverType
	{
		[Key]
		[Required]
		public int BookCoverTypeID { get; set; }
		[Required]
		public string BookCoverTypeName { get; set; }

		public ICollection<Book> Books { get; set; }

	}
}
