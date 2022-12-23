using Sdl.LanguagePlatform.Core;

namespace GoogleCloudTranslationProvider.Models
{
	public class GCTPTag
	{
		public GCTPTag(Tag tag)
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