using System;
using System.Globalization;
using System.Windows.Data;

namespace WebView2Test.Converters
{
	public class BooleanToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool and true ? "Log out" : "Log in";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}