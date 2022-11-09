using Sdl.LanguagePlatform.Core;

namespace GoogleTranslatorProvider.Models
{
	public class GTPTag
	{
		public GTPTag(Tag tag)
		{
			SdlTag = tag;
			PadLeft = string.Empty;
			PadRight = string.Empty;
		}

		public string PadLeft { get; set; }

		public string PadRight { get; set; }

		public Tag SdlTag { get; }
	}
}