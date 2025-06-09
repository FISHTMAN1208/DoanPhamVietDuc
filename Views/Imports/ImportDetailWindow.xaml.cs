using DoanPhamVietDuc.Models;
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
	/// Interaction logic for ImportDetailWindow.xaml
	/// </summary>
	public partial class ImportDetailWindow : Window
	{
		
		public ImportDetailWindow(Import import)
		{
			InitializeComponent();
			DataContext = import;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
        }
    }
}
