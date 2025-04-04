using Sdl.Community.PostEdit.Compare.Core.SDLXLIFF;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sdl.Community.PostEdit.Compare.Core.Extension
{
    public static class SegmentSectionExtension
    {
        public static string ToPlain(this List<SegmentSection> segment)
        {
            var plainText = "";
            foreach (var segmentSection in segment)
            {
                if (segmentSection.Type != SegmentSection.ContentType.Text)
                {
                    var regex = new Regex(@"<\s*(\w+)[^>]*\/?>");
                    var match = regex.Match(segmentSection.Content);
                    plainText += match.Success ? $"<{match.Groups[1].Value}{match.Groups[2].Value}>" : string.Empty;
                }
                else
                    plainText += segmentSection.Content;
            }

            return plainText;
        }
    }
}