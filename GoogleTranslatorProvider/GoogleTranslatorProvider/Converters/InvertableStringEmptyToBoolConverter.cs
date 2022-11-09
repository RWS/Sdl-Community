using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows.Data;

namespace GoogleTranslatorProvider.Converters
{
	public class InvertableStringEmptyToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isEmpty = string.IsNullOrEmpty(value as string);
			if (parameter is null)
			{
				return isEmpty;
			}

			var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
			return direction == Parameters.Inverted ? !isEmpty
													: isEmpty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}