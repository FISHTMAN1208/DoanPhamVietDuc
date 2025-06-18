﻿using System.Windows;
using System.Windows.Controls;
using DoanPhamVietDuc.ViewModels;

namespace DoanPhamVietDuc.Views.Authentication
{
	public partial class RegisterView : UserControl
	{
		public RegisterView()
		{
			InitializeComponent();
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (DataContext is RegisterViewModel viewModel)
			{
				viewModel.Password = PasswordBox.Password;
			}
		}

		private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (DataContext is RegisterViewModel viewModel)
			{
				viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
			}
		}
	}
}