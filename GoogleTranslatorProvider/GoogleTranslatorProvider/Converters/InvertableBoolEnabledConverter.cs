using System;
using System.Globalization;
using System.Windows.Data;

namespace GoogleCloudTranslationProvider.Converters
{
	public class InvertableBoolEnabledConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolValue = value is not null && (bool)value;
			if (parameter is null)
			{
				return boolValue;
			}

			var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
			return direction == Parameters.Inverted ? !boolValue
													: (object)boolValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> null;
	}
}