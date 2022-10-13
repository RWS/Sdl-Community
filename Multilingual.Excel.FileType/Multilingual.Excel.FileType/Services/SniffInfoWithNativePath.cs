using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Services
{
	public class SniffInfoWithNativePath : SniffInfo
	{
		public string NativeFilePath { get; set; }

		public SniffInfoWithNativePath(string nativeFilePath)
		{
			NativeFilePath = nativeFilePath;
		}
	}
}
