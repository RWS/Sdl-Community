using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows.Media.Imaging;
using Sdl.Community.XLIFF.Manager.Common;


namespace Sdl.Community.XLIFF.Manager.Service
{
	public class ImageService
	{
		private readonly PathInfo _pathInfo;

		public ImageService(PathInfo pathInfo)
		{
			_pathInfo = pathInfo;
		}

		public BitmapImage GetImage(string name, Size imageSize)
		{
			try
			{				
				var filePath = Path.Combine(_pathInfo.FlagsFolderPath, name);
				if (!File.Exists(filePath))
				{
					return null;
				}

				Icon icon;
				using (var stream = new FileStream(filePath, FileMode.Open))
				{
					icon = new Icon(stream, imageSize);
					stream.Flush();
					stream.Close();
				}

				var bitmap = icon.ToBitmap();
				bitmap.MakeTransparent();

				return Convert(bitmap, null);
			}
			catch
			{
				return null;
			}
		}

		public void ExtractFlags()
		{
			var asm = Assembly.GetExecutingAssembly();
			var resourceName = asm.GetName().Name + ".Resources.Flags.Flags.zip";
			var fileStream = asm.GetManifestResourceStream(resourceName);

			if (fileStream == null)
			{
				return;
			}

			if (File.Exists(_pathInfo.FlagsFilePath))
			{
				File.Delete(_pathInfo.FlagsFilePath);
			}

			using (var reader = new BinaryReader(fileStream))
			{
				using (var writer = new BinaryWriter(new FileStream(_pathInfo.FlagsFilePath, FileMode.Create)))
				{
					var buffer = new byte[64 * 1024];
					var numread = reader.Read(buffer, 0, buffer.Length);

					while (numread > 0)
					{
						writer.Write(buffer, 0, numread);
						numread = reader.Read(buffer, 0, buffer.Length);
					}

					writer.Flush();
				}
			}

			using (var zipFile = ZipFile.Open(_pathInfo.FlagsFilePath, ZipArchiveMode.Read))
			{
				foreach (var fileEntry in zipFile.Entries)
				{
					var outputFile = Path.Combine(_pathInfo.FlagsFolderPath, fileEntry.FullName);
					if (!File.Exists(outputFile))
					{
						fileEntry.ExtractToFile(outputFile);
					}
				}
			}
		}

		public BitmapImage Convert(object value, string imageName)
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
