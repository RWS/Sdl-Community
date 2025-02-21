using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SDLTM.Import.Converter
{
	public class StringNullOrEmptyToVisibilityConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.IsNullOrEmpty(value as string)
				? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
