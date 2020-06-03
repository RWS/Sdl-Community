using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Sdl.Core.Globalization;

namespace Sdl.Community.XLIFF.Manager.Converters
{
	public class CultureInfoToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.IsNullOrEmpty((string)value))
			{
				return null;
			}
			var flagImage =new Language((string)value).GetFlagImage();

			var memoryStream = new MemoryStream();
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			flagImage.Save(memoryStream, ImageFormat.Png);
			memoryStream.Seek(0, SeekOrigin.Begin);
			bitmap.StreamSource = memoryStream;
			bitmap.EndInit();

			bitmap.Freeze();

			return bitmap;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
