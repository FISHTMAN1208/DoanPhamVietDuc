using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
}