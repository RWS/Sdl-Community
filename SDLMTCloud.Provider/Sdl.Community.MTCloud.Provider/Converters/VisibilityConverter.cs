using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Sdl.Community.MTCloud.Provider.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class VisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var valueString = value?.ToString();
				var parameterString = parameter?.ToString() ?? string.Empty;
				if (parameterString.Contains("#"))
				{
					var parameters = parameterString.Split('#');
					return parameters.Any(param => param.Equals(valueString, StringComparison.InvariantCultureIgnoreCase)) ? Visibility.Visible
						: Visibility.Collapsed;
				}
				return string.Compare(parameterString, valueString, StringComparison.InvariantCultureIgnoreCase) == 0 
					? Visibility.Visible 
					: Visibility.Collapsed;
			}
			catch
			{
				return Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value?.ToString() != Visibility.Collapsed.ToString();
		}
	}
}