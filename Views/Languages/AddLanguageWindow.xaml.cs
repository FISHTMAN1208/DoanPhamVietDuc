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

namespace DoanPhamVietDuc.Views.Languages
{
    public partial class AddLanguageWindow : Window
	{
		public readonly AddLanguageViewModel _viewmodel;
		public Language NewLanguage { get; private set; }
		public AddLanguageWindow(IDataService dataService, IDialogService dialogService)
		{
			_viewmodel = new AddLanguageViewModel(dataService, dialogService, this);
			_viewmodel.LanguageCreated += (sender, language) =>
			{
				NewLanguage = language;
			};
			DataContext = _viewmodel;
			InitializeComponent();
		
		}
	}
}
