using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DoanPhamVietDuc.Converters
{
	public class BoolToVisConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null)
				return value;

			var options = parameter.ToString().Split('|');
			if (options.Length < 2)
				return value;

			if (value is bool boolValue)
			{
				return boolValue ? options[0] : options[1];
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BooleanToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null || !(value is bool))
				return value;

			string[] options = parameter.ToString().Split('|');
			if (options.Length < 2)
				return value;

			return (bool)value ? options[0] : options[1];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool boolValue)
			{
				return !boolValue;
			}
			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool boolValue)
			{
				return !boolValue;
			}
			return false;
		}
	}

	public class IndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is DataGridRow row)
			{
				return row.GetIndex() + 1;
			}
			return 0;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}

	public class RoleColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string role)
			{
				return role.ToLower() switch
				{
					"admin" => new SolidColorBrush(Color.FromRgb(239, 68, 68)), // Red
					"staff" => new SolidColorBrush(Color.FromRgb(59, 130, 246)), // Blue
					"manager" => new SolidColorBrush(Color.FromRgb(168, 85, 247)), // Purple
					_ => new SolidColorBrush(Color.FromRgb(100, 116, 139)) // Slate
				};
			}
			return new SolidColorBrush(Color.FromRgb(100, 116, 139));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class StatusColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string status)
			{
				return status.ToLower() switch
				{
					"active" => new SolidColorBrush(Color.FromRgb(16, 185, 129)),
					"inactive" => new SolidColorBrush(Color.FromRgb(239, 68, 68)),
					"suspended" => new SolidColorBrush(Color.FromRgb(245, 158, 11)), 
					"pending" => new SolidColorBrush(Color.FromRgb(59, 130, 246)), 
					_ => new SolidColorBrush(Color.FromRgb(100, 116, 139)) 
				};
			}
			return new SolidColorBrush(Color.FromRgb(100, 116, 139));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RoleIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string role)
			{
				return role.ToLower() switch
				{
					"admin" => PackIconKind.ShieldAccount,
					"staff" => PackIconKind.Account,
					_ => PackIconKind.AccountCircle
				};
			}
			return PackIconKind.AccountCircle;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BoolToActivityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool isActive)
			{
				return isActive ? "Active" : "Inactive";
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}