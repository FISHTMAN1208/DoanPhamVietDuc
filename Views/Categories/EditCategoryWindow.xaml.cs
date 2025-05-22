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

namespace DoanPhamVietDuc.Views.Categories
{
	public partial class EditCategoryWindow : Window
	{
		private readonly EditCategoryViewModel _viewModel;
		public Category UpdatedCategory { get; private set; }

		public EditCategoryWindow(Category category, IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();

			_viewModel = new EditCategoryViewModel(category, dataService, dialogService, this);

			_viewModel.CategoryUpdated += (sender, updatedCategory) => {
				UpdatedCategory = updatedCategory;
			};

			DataContext = _viewModel;
		}
	}
}
