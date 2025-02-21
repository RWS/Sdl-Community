using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.SDLBatchAnonymize.Converters
{
    public class InvertableBoolEnabledConverter : IValueConverter
	{
		enum Parameters
		{
			Inverted
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolValue = value != null && (bool)value;
			if (parameter == null) return boolValue;
			var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
			if (direction == Parameters.Inverted)
				return !boolValue;
			return boolValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
