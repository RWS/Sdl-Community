using System;
using System.Globalization;
using System.Windows.Data;

namespace LanguageWeaverProvider.Converters
{
	[ValueConversion(typeof(string), typeof(bool))]
	public class EmptyStringToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return parameter switch
			{
				null => !string.IsNullOrWhiteSpace((string)value),
				_ => string.IsNullOrWhiteSpace((string)value)
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}