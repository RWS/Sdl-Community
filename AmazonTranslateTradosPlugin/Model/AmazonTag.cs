using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Model
{
    /// <summary>
    /// Used to add info to associate with an SDL tag object
    /// </summary>
    internal class AmazonTag
    {
        internal AmazonTag(Tag tag)
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