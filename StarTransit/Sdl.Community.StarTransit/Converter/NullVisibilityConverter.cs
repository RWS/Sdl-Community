using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sdl.Community.StarTransit.Converter
{
	[ValueConversion(typeof(string), typeof(Visibility))]
	public class NullVisibilityConverter : IValueConverter
	{
		 enum Parameters
		{
			Inverted
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isEmptyString = string.IsNullOrEmpty(value as string);
			// display element binded to a string which is not empty
			if (parameter != null)
			{
				var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);

				if (direction == Parameters.Inverted)
					return !isEmptyString ? Visibility.Visible : Visibility.Collapsed;
			}
			//hide element if the strings is null or empty
			return isEmptyString? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
