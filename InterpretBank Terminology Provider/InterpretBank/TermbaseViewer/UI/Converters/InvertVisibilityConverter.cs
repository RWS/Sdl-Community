using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InterpretBank.TermbaseViewer.UI.Converters;

public class InvertVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is Visibility visibilityValue)
		{
			return (visibilityValue == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
		}

		return value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}