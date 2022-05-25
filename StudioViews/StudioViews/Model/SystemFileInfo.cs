using System;
using System.IO;
using Sdl.Community.StudioViews.ViewModel;

namespace Sdl.Community.StudioViews.Model
{
	public class SystemFileInfo : BaseModel
	{
		public SystemFileInfo(string fullPath)
		{
			if (fullPath == null || !File.Exists(fullPath))
			{
				throw new FileNotFoundException(fullPath);
			}

			var file = new System.IO.FileInfo(fullPath);
			
			FullPath = fullPath;
			Name = System.IO.Path.GetFileName(fullPath);
			Path = System.IO.Path.GetDirectoryName(fullPath);
			Modified = file.LastWriteTime;
			ModifiedString = Modified.ToShortDateString() + " " + Modified.ToShortTimeString();
			Size = file.Length;
			SizeString = GetFileSizeInBytes(Size);
		}
		
		public string FullPath { get; set; }
		
		public string Name { get; set; }

		public string Path { get; set; }

		public DateTime Modified { get; set; }

		public long Size { get; set; }

		public string ModifiedString { get; set; }

		public string SizeString { get; set; }

		private string GetFileSizeInBytes(long totalBytes)
		{
			if (totalBytes >= 1073741824) //Giga Bytes
			{
				var fileSize = Math.Truncate(decimal.Divide(totalBytes, 1073741824));
				return string.Format("{0:##.##} GB", fileSize);
			}

			if (totalBytes >= 1048576) //Mega Bytes
			{
				var fileSize = Math.Truncate(decimal.Divide(totalBytes, 1048576));
				return string.Format("{0:##.##} MB", fileSize);
			}

			if (totalBytes >= 1024) //Kilo Bytes
			{
				var fileSize = Math.Truncate(decimal.Divide(totalBytes, 1024));
				return string.Format("{0:##.##} KB", fileSize);
			}

			if (totalBytes > 0)
			{
				var fileSize = totalBytes;
				return string.Format("{0:##.##} Bytes", fileSize);
			}

			return "0 Bytes";
		}

	}
}
