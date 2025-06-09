using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Helpers.Commands;
using DoanPhamVietDuc.Models;

namespace DoanPhamVietDuc.ViewModels
{
	public class ImportDetailViewModel : BaseViewModel
	{
		private readonly Window _window;

		private Import _import;
		public Import Import
		{
			get => _import;
			set => SetProperty(ref _import, value);
		}

		private ObservableCollection<ImportDetail> _importDetails;
		public ObservableCollection<ImportDetail> ImportDetails
		{
			get => _importDetails;
			set => SetProperty(ref _importDetails, value);
		}

		public ImportDetailViewModel(Import import, Window window)
		{
			_window = window;

			Title = $"Chi tiết phiếu nhập #{import.ImportID}";

			// Gán dữ liệu
			Import = import;
			ImportDetails = new ObservableCollection<ImportDetail>(import.ImportDetails ?? new List<ImportDetail>());

		}
	}
}
