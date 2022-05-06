using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.MTConnectors.Google.GoogleService
{
    internal class TagInfo
    {
        private readonly Tag _tag;


        public TagInfo(Tag tag)
        {
            _tag = tag;
            IsClosed = false;
            PadLeft = string.Empty;
            PadRight = string.Empty;
        }

        public string TagKey => _tag.TagID + "-" + _tag.Anchor;

        public TagType TagType => _tag.Type;

        public bool IsClosed { get; set; }

        public string PadLeft { get; set; }

        public string PadRight { get; set; }

        public string OpeningTag => "<tg" + TagKey + ">";

        public string ClosingTag => "</tg" + TagKey + ">";

        public string StandaloneTag => "<tg" + TagKey + "/>";

        public Tag SdlTag => _tag;

    }
}
