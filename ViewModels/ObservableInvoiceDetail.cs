using DoanPhamVietDuc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoanPhamVietDuc.ViewModels
{
	public class ObservableInvoiceDetail : BaseViewModel
	{
		private int _bookID;
		private int _quantity;
		private decimal _unitPrice;
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

		public decimal UnitPrice
		{
			get => _unitPrice;
			set
			{
				if (SetProperty(ref _unitPrice, value))
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
			Subtotal = Quantity * UnitPrice;
		}

		// Chuyển đổi từ ObservableInvoiceDetail sang InvoiceDetail
		public InvoiceDetail ToInvoiceDetail()
		{
			return new InvoiceDetail
			{
				BookID = BookID,
				Quantity = Quantity,
				UnitPrice = UnitPrice,
				Subtotal = Subtotal
			};
		}

		// Khởi tạo từ InvoiceDetail
		public static ObservableInvoiceDetail FromInvoiceDetail(InvoiceDetail detail)
		{
			return new ObservableInvoiceDetail
			{
				BookID = detail.BookID,
				Quantity = detail.Quantity,
				UnitPrice = detail.UnitPrice,
				Subtotal = detail.Subtotal,
				Book = detail.Book
			};
		}
	}
}