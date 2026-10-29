using System.Text.RegularExpressions;

namespace Sdl.Community.DeepLMTProvider.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveTags(this string input)
        {
            var pattern = "<.*?>"; // This regular expression matches any HTML or XML tag
            return Regex.Replace(input, pattern, string.Empty);
        }
    }
}