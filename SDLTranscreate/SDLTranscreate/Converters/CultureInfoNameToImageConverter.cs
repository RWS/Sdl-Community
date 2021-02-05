using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Trados.Transcreate.Service;

namespace Trados.Transcreate.Converters
{
	public class CultureInfoNameToImageConverter : IValueConverter
	{
		private readonly ImageService _imageService;

		public CultureInfoNameToImageConverter()
		{
			_imageService = new ImageService();
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var itemValue = value?.ToString();
			if (string.IsNullOrEmpty(itemValue))
			{
				return null;
			}

			if (!itemValue.Contains(","))
			{
				return _imageService.GetImage(itemValue); 
			}

			var items = itemValue.Split(',').ToList();
			if (items.Count > 1)
			{			
				var sourceCulture = items[0];
				var targetCulture = items[1];				
				return _imageService.GetImage(parameter?.ToString() == "Source" ? sourceCulture : targetCulture); ;
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
