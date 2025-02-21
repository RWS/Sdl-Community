using System;
using System.Globalization;
using System.Windows.Data;

namespace InterpretBank.TermbaseViewer.UI.Converters
{
	public class WidthToThirdConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double width)
			{
				return width / 3;
			}

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}