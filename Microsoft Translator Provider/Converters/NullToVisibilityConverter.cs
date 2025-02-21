using System.Globalization;
using System;
using System.Windows.Data;
using System.Windows;

namespace MicrosoftTranslatorProvider.Converters
{
	class NullToVisibilityConverter : IValueConverter
    {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			=> value is null ? Visibility.Collapsed : Visibility.Visible;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
