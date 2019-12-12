using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SDLCommunityCleanUpTasks.Models;

namespace SDLCommunityCleanUpTasks.Utilities
{
	public static class TextUtilities
    {
        public static bool NormalStringCompare(this string original, string searchText, bool isCaseSensitive = false, CultureInfo cultureInfo = null)
        {
            bool isMatch = false;

            if (cultureInfo != null)
            {
                if (isCaseSensitive)
                {
                    isMatch = cultureInfo.CompareInfo.IndexOf(original, searchText) >= 0;
                }
                else
                {
                    isMatch = cultureInfo.CompareInfo.IndexOf(original, searchText, CompareOptions.IgnoreCase) >= 0;
                }
            }
            else
            {
                if (isCaseSensitive)
                {
                    isMatch = original.IndexOf(searchText, StringComparison.Ordinal) >= 0;
                }
                else
                {
                    isMatch = original.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }

            return isMatch;
        }

        public static string NormalStringReplace(this string original, string search, string replacement, bool isCaseSensitive)
        {
            string result = string.Empty;

            if (isCaseSensitive)
            {
                var replacedString = original.Replace(search, replacement);

                // If oldValue is not found in the current instance, the method returns the current
                // instance unchanged.
                if (!object.ReferenceEquals(original, replacedString))
                {
                    result = replacedString;
                }
            }
            else
            {
                result = CaseInsensitiveReplace(original, search, replacement);
            }

            return result;
        }

        public static bool RegexCompare(this string original, string searchText, bool isCaseSensitive)
        {
            bool isMatch = false;

            // Regex always uses current culture
            try
            {
                if (isCaseSensitive)
                {
                    isMatch = Regex.IsMatch(original, searchText);
                }
                else
                {
                    isMatch = Regex.IsMatch(original, searchText, RegexOptions.IgnoreCase);
                }
            }
            catch (ArgumentException)
            {
                //Ignore invalid expressions
                // Maybe add a logger in the future
            }

            return isMatch;
        }

        public static string RegexReplace(this string original, string search, ReplacementText replacement, bool isCaseSensitive,
                                          MatchEvaluator strConv = null)
        {

            var result = string.Empty;

            // Regex uses current culture by default

            if (isCaseSensitive)
            {
                if (replacement.ToLower)
                {
                    result = Regex.Replace(original, search, m => m.Value.ToLower());
                }
                else if (replacement.ToUpper)
                {
                    result = Regex.Replace(original, search, m => m.Value.ToUpper());
                }
                else if (strConv != null)
                {
                    result = Regex.Replace(original, search, strConv);
                }
                else
                {
                    result = Regex.Replace(original, search, replacement.Text);
                }
            }
            else
            {
                if (replacement.ToLower)
                {
                    result = Regex.Replace(original, search, m => m.Value.ToLower(), RegexOptions.IgnoreCase);
                }
                else if (replacement.ToUpper)
                {
                    result = Regex.Replace(original, search, m => m.Value.ToUpper(), RegexOptions.IgnoreCase);
                }
                else if (strConv != null)
                {
                    result = Regex.Replace(original, search, strConv, RegexOptions.IgnoreCase);
                }
                else
                {
                    result = Regex.Replace(original, search, replacement.Text, RegexOptions.IgnoreCase);
                }
            }

            return result;
        }

        /// <summary>
        /// Code adapted from
        /// <see cref="http://www.blackbeltcoder.com/Articles/strings/a-case-insensitive-version-of-string-replace"/>
        /// </summary>
        /// <param name="original"></param>
        /// <param name="search"></param>
        /// <param name="replacement"></param>
        /// <returns>New string</returns>
        private static string CaseInsensitiveReplace(this string original, string search, string replacement)
        {
            StringBuilder result = new StringBuilder(original.Length);
            int pos = 0;

            while (true)
            {
                if (pos > original.Length)
                    break;

                int i = original.IndexOf(search, pos, StringComparison.InvariantCultureIgnoreCase);
                if (i < 0)
                    break;

                result.Append(original, pos, i - pos);
                result.Append(replacement);

                pos = i + search.Length;
            }

            var count = original.Length - pos;
            if (count >= 0)
            {
                result.Append(original, pos, original.Length - pos);
            }

            return result.ToString();
        }
    }
}