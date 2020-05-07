using System;
using System.Globalization;
using System.Windows.Data;

namespace Sdl.Community.XLIFF.Manager.Converters
{
	public class CultureInfoNameToDisplayNameConverter : IValueConverter
	{	
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.IsNullOrEmpty((string)value))
			{
				return null;
			}
			
			var cultureInfo = new CultureInfo(value.ToString());
			
			return cultureInfo.DisplayName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
