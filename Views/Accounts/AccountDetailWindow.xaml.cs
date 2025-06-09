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
using DoanPhamVietDuc.Views.Accounts;
using DoanPhamVietDuc.ViewModels;

namespace DoanPhamVietDuc.Views.Accounts
{
	public partial class AccountDetailWindow : Window
	{
		public AccountDetailWindow(Account account, IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();
			DataContext = new AccountDetailViewModel(account, dialogService, dataService);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
        }
    }
}
