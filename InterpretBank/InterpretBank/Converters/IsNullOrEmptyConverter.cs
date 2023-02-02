using System;
using System.Globalization;
using System.Windows.Data;

namespace InterpretBank.Converters
{
	public class IsNullOrEmptyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var input = (string)value;
			var result = string.IsNullOrEmpty(input) ? (string)parameter : null;
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}