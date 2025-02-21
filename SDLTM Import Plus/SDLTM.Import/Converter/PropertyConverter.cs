using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace SDLTM.Import.Converter
{
    public class PropertyConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{

			var tag = values[0] as string;
			var collection = values[1] as IList;

			return new Tuple<string, IList>(tag, collection);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
