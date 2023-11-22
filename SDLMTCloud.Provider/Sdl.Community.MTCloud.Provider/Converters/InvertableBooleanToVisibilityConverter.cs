using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sdl.Community.MTCloud.Provider.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class InvertableBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolValue = value is not null && (bool)value;
			if (parameter is null)
			{
				return boolValue ? Visibility.Visible : Visibility.Collapsed;
			}

			var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
			return direction switch
			{
				Parameters.Inverted => boolValue ? Visibility.Collapsed : Visibility.Visible,
				_ => boolValue ? Visibility.Visible : Visibility.Collapsed,
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> null;
	}

	public enum Parameters
	{
		Inverted
	}
}
