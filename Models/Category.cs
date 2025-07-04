﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.Models
{
	public class Category
	{
		[Key]
		[Required]
		public int CategoryID { get; set; }

		[Required]
		[MaxLength(50)]
		public string CategoryName { get; set; }

		[Required]
		public DateTime ModifyTime { get; set; } = DateTime.Now;

		[Required]
		[MaxLength(50)]
		public string ModifyBy { get; set; }

		public virtual ICollection<Book> Books { get; set; }
	}
}
