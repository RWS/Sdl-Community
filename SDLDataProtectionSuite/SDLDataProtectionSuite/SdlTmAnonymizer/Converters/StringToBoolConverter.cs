using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Converters
{
	public class StringToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is not null && !string.IsNullOrWhiteSpace(value.ToString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}