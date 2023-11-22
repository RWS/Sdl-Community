namespace Multilingual.XML.FileType.Processors
{
    internal class RegexMatch
    {
        public MatchRule Rule
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public TagType Type
        {
            get;
            set;
        }

        public enum TagType
        {
            Placeholder, TagPairOpening, TagPairClosing
        }
    }
}
