using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Sdl.Core.Globalization;

namespace Multilingual.Excel.FileType.Converters
{
	public class CultureInfoNameToDisplayNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var itemValue = value?.ToString();
			if (string.IsNullOrEmpty(itemValue))
			{
				return "All Languages";
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
				var language = new Language(parameter?.ToString() == "Source" ? sourceCulture : targetCulture);

				return language.DisplayName;
			}

			return "All Languages";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
