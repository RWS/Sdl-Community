using System.Net;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
    // Plain text placed inside, and read back from, the XML/HTML fragment DeepL parses when tag handling is on.
    // Without tag handling DeepL reads and writes plain text, so text is left as is both ways.
    public static class MarkupText
    {
        // Only &, < and > change meaning in text; HtmlEncode would also encode quotes and umlauts
        public static string Escape(string text, TagFormat tagHandling) =>
            tagHandling == TagFormat.None
                ? text
                : text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

        // HtmlDecode also covers XML: an XML reply only holds the five XML entities and numeric
        // references (v1 escapes quotes, v2 does not)
        public static string Decode(string text, TagFormat tagHandling) =>
            tagHandling == TagFormat.None ? text : WebUtility.HtmlDecode(text);
    }
}
