using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Converters
{
    public class VisibilityToBooleanConverter: IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value != null)
		    {
			    return System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Hidden;
		    }

			return Visibility.Hidden;
		}

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    throw new NotImplementedException();
	    }
    }
}
