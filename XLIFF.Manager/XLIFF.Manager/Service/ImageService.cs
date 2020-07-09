using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Sdl.Core.Globalization;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class ImageService
	{
		public BitmapImage GetImage(string name)
		{
			try
			{				
				var bitmap = new Language(name).GetFlagImage();
				return Convert(bitmap);
			}
			catch
			{
				return null;
			}
		}

		public BitmapImage Convert(object value)
		{
			if (value != null && value is Image image)
			{
				var memoryStream = new MemoryStream();
				var bitmap = new BitmapImage();
				bitmap.BeginInit();
				image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
				memoryStream.Seek(0, SeekOrigin.Begin);
				bitmap.StreamSource = memoryStream;
				bitmap.EndInit();

				bitmap.Freeze();

				return bitmap;
			}

			return null;
		}
	}
}
