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

        public static void RemoveTags(this string input, out string output)
        {
            var pattern = "<.*?>"; // This regular expression matches any HTML or XML tag
            output = Regex.Replace(input, pattern, string.Empty);
        }
    }
}