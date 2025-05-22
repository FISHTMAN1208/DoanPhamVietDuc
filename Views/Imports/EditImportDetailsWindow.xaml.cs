using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DoanPhamVietDuc.Views.Imports
{
	/// <summary>
	/// Interaction logic for EditImportDetailsWindow.xaml
	/// </summary>
	public partial class EditImportDetailsWindow : Window
	{
		private readonly EditImportDetailsViewModel _viewModel;
		public Import UpdatedImport { get; private set; }

		public EditImportDetailsWindow(Import import, IDataService dataService, IDialogService dialogService)
		{
			InitializeComponent();

			_viewModel = new EditImportDetailsViewModel(import, dataService, dialogService, this);

			_viewModel.ImportUpdated += (sender, updatedImport) => {
				UpdatedImport = updatedImport;
			};

			DataContext = _viewModel;

			Loaded += async (_, __) => {
				await _viewModel.LoadDataAsync();
			};
		}

		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (sender is DataGrid dataGrid && dataGrid.SelectedItem is ObservableImportDetail selectedDetail)
			{
				_viewModel.EditDetailCommand.Execute(selectedDetail);
			}
		}
	}
}