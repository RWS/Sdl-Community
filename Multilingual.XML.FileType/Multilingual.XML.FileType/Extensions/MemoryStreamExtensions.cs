using System.IO;
using System.Text;

namespace Multilingual.XML.FileType.Extensions
{
	internal static class MemoryStreamExtensions
	{
		internal static void WriteString(this MemoryStream memoryStream, string text)
		{
			var textBytes = Encoding.UTF8.GetBytes(text);

			memoryStream.Write(textBytes, 0, textBytes.Length);
		}
	}
}
