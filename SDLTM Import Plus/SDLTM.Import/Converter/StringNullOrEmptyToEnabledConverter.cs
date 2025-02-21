using System;
using System.Globalization;
using System.Windows.Data;

namespace SDLTM.Import.Converter
{
   public  class StringNullOrEmptyToEnabledConverter: IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
			return string.IsNullOrEmpty(value as string);
		}

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    return null;
	    }
    }
}
