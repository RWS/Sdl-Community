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
			var isEmpty = value is not null && string.IsNullOrEmpty(value as string);
			return parameter.ToVisibility(isEmpty);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}