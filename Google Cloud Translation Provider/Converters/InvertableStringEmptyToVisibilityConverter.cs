using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GoogleCloudTranslationProvider.Converters
{
	[ValueConversion(typeof(string), typeof(Visibility))]
	public class InvertableStringEmptyToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isEmpty = string.IsNullOrEmpty(value as string);

			if (parameter is null)
			{
				return isEmpty ? Visibility.Visible : Visibility.Collapsed;
			}

			if (Enum.TryParse(parameter.ToString(), out Parameters direction) && direction == Parameters.Inverted)
			{
				return isEmpty ? Visibility.Collapsed : Visibility.Visible;
			}

			return isEmpty ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}