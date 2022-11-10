using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System;

namespace GoogleTranslatorProvider.Converters
{
	public class EmptyToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var valueString = value?.ToString();
			return string.IsNullOrEmpty(valueString) ? Visibility.Collapsed
													 : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}