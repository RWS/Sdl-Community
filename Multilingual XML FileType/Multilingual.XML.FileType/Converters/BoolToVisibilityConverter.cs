using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Multilingual.XML.FileType.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
		public Visibility TrueValue { get; set; }
		public Visibility FalseValue { get; set; }

		public BoolToVisibilityConverter()
		{
			// set defaults
			TrueValue = Visibility.Visible;
			FalseValue = Visibility.Collapsed;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var valueBoolean = value != null && System.Convert.ToBoolean(value);
			var parameterBoolean = parameter != null && System.Convert.ToBoolean(parameter);

			if (valueBoolean && parameterBoolean)
			{
				return TrueValue;
			}

			if (!valueBoolean && !parameterBoolean)
			{
				return TrueValue;
			}

			return FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{

			if (Equals(value, TrueValue))
			{
				return true;
			}

			if (Equals(value, FalseValue))
			{
				return false;
			}

			return null;
		}
	}
}
