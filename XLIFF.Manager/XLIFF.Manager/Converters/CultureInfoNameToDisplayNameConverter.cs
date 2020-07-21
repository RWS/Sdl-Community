using System;
using System.Globalization;
using System.Windows.Data;
using Sdl.Core.Globalization;

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
			
			var language = new Language(value.ToString());
			
			return language.DisplayName;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
