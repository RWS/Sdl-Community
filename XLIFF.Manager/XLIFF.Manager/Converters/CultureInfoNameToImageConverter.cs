using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using Sdl.Community.XLIFF.Manager.Service;

namespace Sdl.Community.XLIFF.Manager.Converters
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
			if (string.IsNullOrEmpty((string)value))
			{
				return null;
			}

			var bitmap = _imageService.GetImage((string)value);
			return bitmap;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
