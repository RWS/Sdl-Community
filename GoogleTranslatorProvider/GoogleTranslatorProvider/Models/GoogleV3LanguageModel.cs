using System.Globalization;

namespace GoogleTranslatorProvider.Models
{
	public class GoogleV3LanguageModel
	{
		public string GoogleLanguageCode { get; set; }
		public bool SupportTarget { get; set; }
		public bool SupportSource { get; set; }
		public CultureInfo CultureInfo { get; set; }
	}
}