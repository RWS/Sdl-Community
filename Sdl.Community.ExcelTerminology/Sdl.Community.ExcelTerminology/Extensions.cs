using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Sdl.Community.ExcelTerminology
{
    public static class Extensions
    {
        public static string RemoveUriForbiddenCharacters(this string uriString)
        {
            var regex = new Regex(@"[$%+!*'(), ]");
            return regex.Replace(uriString, "");
        }
    }
}
