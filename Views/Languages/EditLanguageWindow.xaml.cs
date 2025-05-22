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

namespace DoanPhamVietDuc.Views.Languages
{
	/// <summary>
	/// Interaction logic for EditLanguageWindow.xaml
	/// </summary>
	public partial class EditLanguageWindow : Window
	{
		private readonly EditLanguageViewModel _viewModel;
		public Language UpdatedLanguage { get; private set; }

		public EditLanguageWindow(Language language, IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();

			_viewModel = new EditLanguageViewModel(language, dataService, dialogService, this);

			_viewModel.LanguageUpdated += (sender, updatedLanguage) => {
				UpdatedLanguage = updatedLanguage;
			};

			DataContext = _viewModel;
		}
	}
}
