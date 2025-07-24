using System.Linq;
using System.Text.RegularExpressions;

namespace QATracker.Extension
{
    public static class StringExtension
    {
        public static bool ContainsDigit(this string s) =>
            s.Any(char.IsDigit);

        public static string TrimEndingDigits(this string s) => s.TrimEnd("0123456789".ToCharArray());
        
        public static string GetEndingDigits(this string s)
        {
            var m = Regex.Match(s, @"\d+$");
            return m.Success ? m.Value : string.Empty;
        }
    }
}