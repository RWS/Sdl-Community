using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Service;

namespace Sdl.Community.XLIFF.Manager.Converters
{
	public class CultureInfoNameToImageConverter : IValueConverter
	{
		private readonly ImageService _imageService;		

		public CultureInfoNameToImageConverter()
		{			
			_imageService = new ImageService(new PathInfo());
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.IsNullOrEmpty((string)value))
			{
				return null;
			}
		
			var imageBitmap = _imageService.GetImage(value + ".ico", new Size(24, 24));
			return imageBitmap;
		
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
