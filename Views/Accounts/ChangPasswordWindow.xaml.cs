using System;
using System.Windows;
using System.Windows.Input;
using DoanPhamVietDuc.Models;
using DoanPhamVietDuc.Services.AuthenticationService.DataService;
using DoanPhamVietDuc.Services.DialogService;
using DoanPhamVietDuc.ViewModels;

namespace DoanPhamVietDuc.Views.Accounts
{
	/// <summary>
	/// Interaction logic for ChangPasswordWindow.xaml
	/// </summary>
	public partial class ChangPasswordWindow : Window
	{
		private ChangePasswordViewModel _viewModel;

		public ChangPasswordWindow(Account account, IDialogService dialogService, IDataService dataService)
		{
			InitializeComponent();

			_viewModel = new ChangePasswordViewModel(account, dialogService, dataService);
			DataContext = _viewModel;

			// Subscribe to close event
			_viewModel.CloseRequested += OnCloseRequested;

			// Set focus to first password box when window loads
			this.Loaded += (s, e) => txtCurrentPassword.Focus();

			// Add Enter key handling
			this.KeyDown += ChangPasswordWindow_KeyDown;
		}

		private void OnCloseRequested(object sender, bool result)
		{
			DialogResult = result;
			Close();
		}

		private void ChangPasswordWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (_viewModel.ChangePasswordCommand.CanExecute(null))
					_viewModel.ChangePasswordCommand.Execute(null);
			}
			else if (e.Key == Key.Escape)
			{
				if (_viewModel.CancelCommand.CanExecute(null))
					_viewModel.CancelCommand.Execute(null);
			}
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			var passwordBox = sender as System.Windows.Controls.PasswordBox;
			if (passwordBox == null) return;

			// Update ViewModel properties when password changes
			if (passwordBox.Name == "txtCurrentPassword")
			{
				_viewModel.CurrentPassword = passwordBox.Password;
			}
			else if (passwordBox.Name == "txtNewPassword")
			{
				_viewModel.NewPassword = passwordBox.Password;
			}
			else if (passwordBox.Name == "txtConfirmPassword")
			{
				_viewModel.ConfirmPassword = passwordBox.Password;
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			// Unsubscribe from events to prevent memory leaks
			if (_viewModel != null)
			{
				_viewModel.CloseRequested -= OnCloseRequested;
			}
			base.OnClosed(e);
		}
	}
}