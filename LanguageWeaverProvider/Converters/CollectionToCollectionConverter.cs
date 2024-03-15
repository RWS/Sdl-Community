using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.Converters
{
	public class CollectionToCollectionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not IEnumerable<PairMapping> pairMappings || parameter is not string propertyName)
			{
				return null;
			}

			return pairMappings.SelectMany(pm => pm.GetType().GetProperty(propertyName)?.GetValue(pm) as List<PairMapping>);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}