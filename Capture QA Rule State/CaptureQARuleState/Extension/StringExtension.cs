using System.Text.RegularExpressions;

namespace CaptureQARuleState.Extension
{
    public static class StringExtension
    {
        public static bool EndsWithDigits(this string input) =>
            !string.IsNullOrEmpty(input) && Regex.IsMatch(input, @"\d+$");


        public static string TrimEndingDigits(this string s) => s.TrimEnd("0123456789".ToCharArray());
        
        public static string GetEndingDigits(this string s)
        {
            var m = Regex.Match(s, @"\d+$");
            return m.Success ? m.Value : string.Empty;
        }
    }
}