using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Sdl.Community.DeepLMTProvider.Extensions
{
    public static class StringExtensions
    {
        public static void DecodeText(this string encoded, out string decoded)
        {
            if (encoded.Contains("%")) encoded = Uri.UnescapeDataString(encoded);
            encoded = HttpUtility.HtmlDecode(encoded);
            decoded = encoded;
        }

        public static string RemoveTags(this string input)
        {
            var pattern = "<.*?>"; // This regular expression matches any HTML or XML tag
            return Regex.Replace(input, pattern, string.Empty);
        }
    }
}