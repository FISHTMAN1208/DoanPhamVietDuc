using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DoanPhamVietDuc.Views.BookCoverTypes
{
    /// <summary>
    /// Interaction logic for EditBookCoverType.xaml
    /// </summary>
    public partial class EditBookCoverTypeWindow : Window
    {
		public readonly EditBookCoverTypeViewModel _viewmodel;
		public BookCoverType UpdatedBookCoverType { get; private set; }
		public EditBookCoverTypeWindow(BookCoverType bookcovertype,IDataService dataService, IDialogService dialogService)
		{
			_viewmodel = new EditBookCoverTypeViewModel(bookcovertype, dataService, dialogService, this);
			_viewmodel.BookCoverTypeUpdated += (sender, bookCoverType) =>
			{
				UpdatedBookCoverType = bookCoverType;
			};
			DataContext = _viewmodel;
			InitializeComponent();
		}
	}
}
