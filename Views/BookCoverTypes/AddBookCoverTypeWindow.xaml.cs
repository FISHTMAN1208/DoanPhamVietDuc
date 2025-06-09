using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
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
    /// Interaction logic for AddBookCoverTypeWindow.xaml
    /// </summary>
    public partial class AddBookCoverTypeWindow : Window
	{
		public readonly AddBookCoverTypeViewModel _viewmodel;
		public BookCoverType NewBookCoverType { get; private set; }
		public AddBookCoverTypeWindow(IDataService dataService, IDialogService dialogService)
		{
			_viewmodel = new AddBookCoverTypeViewModel(dataService, dialogService, this);
			_viewmodel.BookCoverTypeCreated += (sender, bookCoverType) =>
			{
				NewBookCoverType = bookCoverType;
			};
			DataContext = _viewmodel;
			InitializeComponent();
		}
	}
}
