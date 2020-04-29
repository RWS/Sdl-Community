using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Sdl.Community.TuToTm.UiHelpers
{
	[ValueConversion(typeof(System.Drawing.Image), typeof(System.Windows.Media.ImageSource))]

	public class ImageToBitmapImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}

			var image = (System.Drawing.Image)value;
			var memoryStream = new MemoryStream();
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			image.Save(memoryStream, ImageFormat.Png);
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
