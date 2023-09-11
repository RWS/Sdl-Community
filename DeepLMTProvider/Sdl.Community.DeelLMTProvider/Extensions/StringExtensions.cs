using System.Text.RegularExpressions;

namespace Sdl.Community.DeepLMTProvider.Extensions
{
    public static class StringExtensions
    {
        public static void RemoveTags(this string taggedString, out string untaggedString) =>
            untaggedString = Regex.Replace(taggedString, @"<[^>]*>([^<]*)</[^>]*>", "");
    }
}