using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace LanguageWeaverProvider.Converters
{
	[ValueConversion(typeof(IEnumerable<object>), typeof(Visibility))]
	public class EmptyCollectionToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var hasContent = value is not null && value is IEnumerable<object> colection && colection.Any();
			return parameter.ToVisibility(hasContent);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}