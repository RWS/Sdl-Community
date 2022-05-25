using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.MtEnhancedProvider.Converters
{
	public class InvertableStringEmptyToBoolConverter: IValueConverter
	{
		public enum Parameters
		{
			Inverted
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isEmpty = string.IsNullOrEmpty(value as string);

			if (parameter == null) return isEmpty;
			var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
			if (direction == Parameters.Inverted)
			{
				return !isEmpty;
			}
			return isEmpty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
