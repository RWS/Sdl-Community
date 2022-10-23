using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MicrosoftTranslatorProvider.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class InvertableBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolValue = value != null && (bool)value;
			if (parameter == null)
			{
				return boolValue ? Visibility.Visible : Visibility.Collapsed;
			}

			var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
			switch (direction)
			{
				case Parameters.Inverted:
					return boolValue ? Visibility.Collapsed : Visibility.Visible;
				default:
					return boolValue ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> null;
	}
}