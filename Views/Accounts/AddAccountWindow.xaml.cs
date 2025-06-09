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

namespace DoanPhamVietDuc.Views.Accounts
{
	public partial class AddAccountWindow : Window
	{
		private AddAccountViewModel _viewModel;
		public Account NewAccount { get; private set; }

		public AddAccountWindow(IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();
			_viewModel = new AddAccountViewModel(dialogService, dataService, this);
			_viewModel.AccountSaved += (s, account) => NewAccount = account;
			DataContext = _viewModel;
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (_viewModel != null)
			{
				_viewModel.Password = ((PasswordBox)sender).Password;
			}
		}

		private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (_viewModel != null)
			{
				_viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
		}
	}
}
