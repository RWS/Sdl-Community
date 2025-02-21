using System.Text;

namespace Multilingual.XML.FileType.Models
{
	public class MultilingualFileInfo
	{
		public string OriginalFilePath { get; set; }
		
		public string OutputFilePath { get; set; }

		public string MultilingualFilePath { get; set; }
		
		public LanguageMapping SourceLanguage { get; set; }
		
		public LanguageMapping TargetLanguage { get; set; }
		
		public Encoding FileEncoding { get; set; }
	}
}
