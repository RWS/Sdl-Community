using System;
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
                XElement elem = XElement.Parse(pattern);

                return true;
            }
            catch (XmlException)
            {
                // TODO: Log
            }

            return false;
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