﻿using DoanPhamVietDuc.Models;
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

namespace DoanPhamVietDuc.Views.Suppliers
{
	/// <summary>
	/// Interaction logic for SupplierDetailWindow.xaml
	/// </summary>
	public partial class SupplierDetailWindow : Window
	{
		public SupplierDetailWindow(Supplier supplier)
		{
			InitializeComponent();
			DataContext = supplier;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
        }
    }
}
