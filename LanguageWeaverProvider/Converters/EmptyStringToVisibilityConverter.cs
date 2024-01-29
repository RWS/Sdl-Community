using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LanguageWeaverProvider.Converters
{
	[ValueConversion(typeof(string), typeof(Visibility))]
	public class EmptyStringToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter is null)
			{
				return string.IsNullOrWhiteSpace((string)value) ? Visibility.Collapsed : Visibility.Visible;
			}

			return string.IsNullOrWhiteSpace((string)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}