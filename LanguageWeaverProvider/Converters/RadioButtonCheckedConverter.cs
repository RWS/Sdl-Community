using System;
using System.Globalization;
using System.Windows.Data;

namespace LanguageWeaverProvider.Converters
{
	public class RadioButtonCheckedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Equals(value, parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? parameter : null;
		}
	}
}