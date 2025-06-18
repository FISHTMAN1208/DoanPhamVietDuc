using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class Language
	{
		[Key]
		public int LanguageID { get; set; }

		[Required]
		[MaxLength(50)]
		public string LanguageName { get; set; }

		[Required]
		[MaxLength(50)]
		public string Code { get; set; }

		public ICollection<Book> Books { get; set; }
	}
}
