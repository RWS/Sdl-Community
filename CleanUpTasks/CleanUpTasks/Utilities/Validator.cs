using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public static class Validator
    {
        public static bool IsValidXml(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            try
            {
				// Pattern could contain $
				// We replace $ for verifying well-formed xml
				   pattern = pattern.Replace("$", "A");		

				var elem = XElement.Parse(pattern);

                return true;
            }
            catch (XmlException ex)
            {
	            return false;
	            //        if (pattern.Contains("=\"<") && pattern.Contains(">\""))
	            //        {
	            //         var tagFirstQuotesPosition = pattern.IndexOf("=\"<", StringComparison.Ordinal);

	            //         var tagCharacterFirstPosition = tagFirstQuotesPosition + 2;
	            //         var builder = new StringBuilder(pattern);
	            //         builder.Remove(tagCharacterFirstPosition, 1);
	            //         builder.Insert(tagCharacterFirstPosition, "&lt;");

	            //         var tagLastQuotesPosition = builder.ToString().IndexOf(">\"", StringComparison.Ordinal);

	            //         builder.Remove(tagLastQuotesPosition, 1);
	            //         builder.Insert(tagLastQuotesPosition, "&gt;");		

	            //var encodedPattern = builder.ToString();
	            //         try
	            //         {
	            //          XElement.Parse(encodedPattern);
	            //          return true;
	            //         }
	            //         catch (Exception e)
	            //         {
	            //          return false;
	            //         }	 
	            //        }	  
            }
        }

        public static bool IsValidRegexPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            try
            {
                new Regex(pattern);
                return true;
            }
            catch (ArgumentException)
            {
            }

            return false;
        }
    }
}