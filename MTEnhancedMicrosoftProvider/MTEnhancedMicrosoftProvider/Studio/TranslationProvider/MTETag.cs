using Sdl.LanguagePlatform.Core;

namespace MTEnhancedMicrosoftProvider.Studio.TranslationProvider
{
	public class MTETag
	{
		public MTETag(Tag sdlTag)
		{
			SdlTag = sdlTag;
		}

		public string PadLeft { get; set; } = string.Empty;

		public string PadRight { get; set; } = string.Empty;

		public Tag SdlTag { get; }
	}
}