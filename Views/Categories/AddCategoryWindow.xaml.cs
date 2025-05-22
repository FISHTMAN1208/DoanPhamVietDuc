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
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace DoanPhamVietDuc.Views.Categories
{
	public partial class AddCategoryWindow : Window
	{
		public readonly AddCategoryViewModel _viewmodel;
		public Category NewCategory { get; private set; }
		public AddCategoryWindow(IDataService dataService, IDialogService dialogService)
		{
			InitializeComponent();
			_viewmodel = new AddCategoryViewModel(dataService, dialogService, this);
			_viewmodel.CategoryCreated += (sender, category) =>
			{
				NewCategory = category;
			};
			DataContext = _viewmodel;	
		}
	}
}
