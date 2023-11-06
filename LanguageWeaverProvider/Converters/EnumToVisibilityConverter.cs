using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LanguageWeaverProvider.Converters
{
	[ValueConversion(typeof(Enum), typeof(Visibility))]
	public class EnumToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isVisible = value is not null
						 && parameter is not null
						 && (AuthenticationType)value == (AuthenticationType)parameter;
			return isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
