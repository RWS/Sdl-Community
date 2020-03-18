using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sdl.Community.MTCloud.Provider.Converters
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class BeGlobalVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var valResult = value?.ToString().Replace(" ", string.Empty);
				string inputParameter = parameter?.ToString() ?? "";

				return inputParameter.Equals(valResult) ? Visibility.Visible : Visibility.Collapsed;
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