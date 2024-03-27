using System;
using System.Globalization;
using System.Windows.Data;

namespace MicrosoftTranslatorProvider.Converters
{
	public class EvenIndexToBooleanConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 2 && values[0] is int index && values[1] is int count)
			{
				return index % 2 == 0;
			}
			return false;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}