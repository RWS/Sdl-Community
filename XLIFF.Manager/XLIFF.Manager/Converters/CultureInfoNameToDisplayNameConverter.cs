using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Sdl.Core.Globalization;

namespace Sdl.Community.XLIFF.Manager.Converters
{
	public class CultureInfoNameToDisplayNameConverter : IValueConverter
	{	
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var itemValue = value?.ToString();
			if (string.IsNullOrEmpty(itemValue))
			{
				return null;
			}

			if (!itemValue.Contains(","))
			{
				return new Language(itemValue).DisplayName;
			}

			var items = itemValue.Split(',').ToList();
			if (items.Count > 1)
			{
				var sourceCulture = items[0];
				var targetCulture = items[1];
				return new Language(parameter?.ToString() == "Source" ? sourceCulture : targetCulture).DisplayName;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
