using DoanPhamVietDuc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.ViewModels
{
	public class ObservableImportDetail : BaseViewModel
	{
		private int _bookID;
		private int _quantity;
		private decimal _unitImportPrice;
		private decimal _subtotal;
		private Book _book;

		public int BookID
		{
			get => _bookID;
			set
			{
				if (SetProperty(ref _bookID, value))
				{
					CalculateSubtotal();
				}
			}
		}

		public int Quantity
		{
			get => _quantity;
			set
			{
				if (SetProperty(ref _quantity, value))
				{
					CalculateSubtotal();
				}
			}
		}

		public decimal UnitImportPrice
		{
			get => _unitImportPrice;
			set
			{
				if (SetProperty(ref _unitImportPrice, value))
				{
					CalculateSubtotal();
				}
			}
		}

		public decimal Subtotal
		{
			get => _subtotal;
			set => SetProperty(ref _subtotal, value);
		}

		public Book Book
		{
			get => _book;
			set => SetProperty(ref _book, value);
		}

		private void CalculateSubtotal()
		{
			Subtotal = Quantity * UnitImportPrice;
		}

		// Chuyển đổi từ ObservableImportDetail sang ImportDetail
		public ImportDetail ToImportDetail()
		{
			return new ImportDetail
			{
				BookID = BookID,
				Quantity = Quantity,
				UnitImportPrice = UnitImportPrice,
				Subtotal = Subtotal,
			
			};
		}

		// Khởi tạo từ ImportDetail
		public static ObservableImportDetail FromImportDetail(ImportDetail detail)
		{
			return new ObservableImportDetail
			{
				BookID = detail.BookID,
				Quantity = detail.Quantity,
				UnitImportPrice = detail.UnitImportPrice,
				Subtotal = detail.Subtotal,
				Book = detail.Book
			};
		}
	}
}
