using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace DoanPhamVietDuc.ViewModels
{
	public class InvoiceDetailViewModel : BaseViewModel
	{
		private readonly Window _window;

		private Invoice _invoice;
		public Invoice Invoice
		{
			get => _invoice;
			set => SetProperty(ref _invoice, value);
		}

		private ObservableCollection<InvoiceDetail> _invoiceDetails;
		public ObservableCollection<InvoiceDetail> InvoiceDetails
		{
			get => _invoiceDetails;
			set => SetProperty(ref _invoiceDetails, value);
		}

		public ICommand CloseCommand { get; }

		public InvoiceDetailViewModel(Invoice invoice, Window window)
		{
			_window = window;

			Title = $"Chi tiết hóa đơn #{invoice.InvoiceID}";

			Invoice = invoice;
			InvoiceDetails = new ObservableCollection<InvoiceDetail>(invoice.InvoiceDetails ?? new List<InvoiceDetail>());

			CloseCommand = new RelayCommand(_ => _window.Close());
		}
	}
}
