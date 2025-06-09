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
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;

namespace DoanPhamVietDuc.Views.Accounts
{
	public partial class EditAccountWindow : Window
	{
		private EditAccountViewModel _viewModel;
		public Account UpdatedAccount { get; private set; }

		public EditAccountWindow(Account account, IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();
			_viewModel = new EditAccountViewModel(account, dataService, dialogService, this);
			_viewModel.AccountUpdated += (s, updatedAccount) => UpdatedAccount = updatedAccount;
			DataContext = _viewModel;
		}

		private void OnWindowCloseRequested(object sender, bool result)
		{
			DialogResult = result;
			Close();
		}

		private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (_viewModel != null)
			{
				_viewModel.NewPassword = ((PasswordBox)sender).Password;
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