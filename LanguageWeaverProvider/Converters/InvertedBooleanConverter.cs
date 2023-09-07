using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LanguageWeaverProvider.Converters
{
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InvertedBoolean : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null || value is not bool boolValue)
			{
				return true;
			}

			return !boolValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}