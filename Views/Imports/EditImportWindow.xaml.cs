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

namespace DoanPhamVietDuc.Views.Imports
{
	/// <summary>
	/// Interaction logic for EditImportWindow.xaml
	/// </summary>
	public partial class EditImportWindow : Window
	{
		private readonly EditImportViewModel _viewModel;
		public Import UpdatedImport { get; private set; }

		public EditImportWindow(Import import, IDataService dataService, IDialogService dialogService)
		{
			InitializeComponent();

			_viewModel = new EditImportViewModel(import, dataService, dialogService, this);

			_viewModel.ImportUpdated += (sender, updatedImport) => {
				UpdatedImport = updatedImport;
			};

			DataContext = _viewModel;

			Loaded += async (_, __) => {
				await _viewModel.LoadReferenceDataAsync();
			};
		}
	}
}