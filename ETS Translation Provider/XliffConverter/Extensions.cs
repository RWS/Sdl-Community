using Sdl.LanguagePlatform.Core;
using System.Text;

namespace XliffConverter
{
    public static class Extensions
    {
        public static string ToXliffString(this Segment segment)
        {
            // No matter what, always encode < to &lt; so xliff doesn't recognize this as part of a tag
            if (!segment.HasTags)
                return segment.ToPlain().Replace("<", "&lt;");

            StringBuilder result = new StringBuilder();
            foreach (SegmentElement element in segment.Elements)
            {
                if (element is Text)
                {
                    Text txt = (Text)element;
                    result.Append(txt.Value.Replace("<", "&lt;"));
                }
                else if (element is Tag)
                {
                    Tag tag = (Tag)element;
                    string tagString = tag.ToString().Replace("<", "&lt;");
                    switch (tag.Type)
                    {
                        case TagType.Start:
                            result.Append(string.Format("<bpt id=\"{0}\">{1}</bpt>", tag.TagID, tagString));
                            break;
                        case TagType.End:
                            result.Append(string.Format("<ept id=\"{0}\">{1}</ept>", tag.TagID, tagString));
                            break;
                        case TagType.UnmatchedStart:
                        case TagType.UnmatchedEnd:
                            result.Append(string.Format("<it id=\"{0}\">{1}</it>", tag.TagID, tagString));
                            break;
                        case TagType.Standalone:
                        case TagType.TextPlaceholder:
                        case TagType.LockedContent:
                            result.Append(string.Format("<x id=\"{0}\">{1}</x>", tag.TagID, tagString));
                            break;
                        case TagType.Undefined:
                        default:
                            System.Diagnostics.Debug.Assert(false, "Unexpected tag type");
                            break;
                    }
                }
            }
            return result.ToString();
        }

        // Encodes plain text string so that text like <PROJECT> will become safe for being within xliff. Our main
        // concern is just strings that contain non-trados tags.
    }
}
