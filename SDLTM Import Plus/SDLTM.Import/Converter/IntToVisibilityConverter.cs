using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SDLTM.Import.Converter
{
	public class IntToVisibilityConverter : IValueConverter
	{
		private enum Parameters
		{
			Inverted
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				var collectionCount = (int) value;
				if (parameter != null )
				{
					var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
					if (direction == Parameters.Inverted)
					{
						return collectionCount != 0 ? Visibility.Collapsed : Visibility.Visible;
					}
				}
				return collectionCount != 0 ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
