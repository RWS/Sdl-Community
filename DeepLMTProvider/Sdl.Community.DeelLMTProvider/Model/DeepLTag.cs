using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Model
{
	internal class DeepLTag
	{
		internal DeepLTag(Tag tag)
		{
			SdlTag = tag;
			PadLeft = string.Empty;
			PadRight = string.Empty;
		}

		internal string PadLeft { get; set; }

		internal string PadRight { get; set; }

		internal Tag SdlTag { get; }
	}
}
