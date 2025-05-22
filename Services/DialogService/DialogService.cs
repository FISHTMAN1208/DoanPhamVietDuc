using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DoanPhamVietDuc.Services.DialogService
{
	public class DialogService : IDialogService
	{
		public Task<bool> ShowConfirmAsync(string title, string message)
		{
			var result = MessageBox.Show(
				message,
				title,
				MessageBoxButton.YesNo,
				MessageBoxImage.Question
				);
			return Task.FromResult( result == MessageBoxResult.Yes);
		}

		public Task ShowInfoAsync (string title, string message)
		{
			MessageBox.Show(
				message,
				title,
				MessageBoxButton.OK,
				MessageBoxImage.Information);

			return Task.CompletedTask;
		}
		public Task<string> ShowInputAsync(string title, string defaultInput = "")
		{
			return Task.FromResult(defaultInput);
		}
	}
}
