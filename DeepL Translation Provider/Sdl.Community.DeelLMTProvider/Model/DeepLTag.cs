using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Model
{
	internal class DeepLTag
    {
        internal DeepLTag(Tag tag)
        {
            SdlTag = tag;
        }

        internal Tag SdlTag { get; }
    }
}