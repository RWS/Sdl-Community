using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Model
{
	public class MicrosoftTag
	{
		public MicrosoftTag(Tag sdlTag)
		{
			SdlTag = sdlTag;
		}

		public string PadLeft { get; set; } = string.Empty;

		public string PadRight { get; set; } = string.Empty;

		public Tag SdlTag { get; }
	}
}