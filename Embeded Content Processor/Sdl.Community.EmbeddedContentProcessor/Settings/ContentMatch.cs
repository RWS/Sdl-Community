using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.EmbeddedContentProcessor.Settings
{
    public class ContentMatch
    {
        public MatchRule MatchRule { get; set; }

        public Int32 Index { get; set; }

        public string Value { get; set; }

        public TagType Type { get; set; }

        public enum TagType
        {
            Placeholder,
            TagPairOpening,
            TagPairClosing
        }
    }
}
