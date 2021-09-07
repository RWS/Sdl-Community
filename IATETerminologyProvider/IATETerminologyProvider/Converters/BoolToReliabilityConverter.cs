using System;
using System.Globalization;
using System.Windows.Data;
using Sdl.Community.IATETerminologyProvider.Model;

namespace Sdl.Community.IATETerminologyProvider.Converters
{
	public class BoolToReliabilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var checkboxFlag = (Reliability.Reliabilities)Enum.Parse(typeof(Reliability.Reliabilities), (string)parameter);
			var result = System.Convert.ToBoolean(checkboxFlag & (Reliability.Reliabilities)value);
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var checkboxFlag = (Reliability.Reliabilities)Enum.Parse(typeof(Reliability.Reliabilities), (string)parameter);
			return (bool)value
				? checkboxFlag
				: checkboxFlag | Reliability.Reliabilities.Not;
		}
	}
}