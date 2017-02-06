using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sdl.Community.PostEdit.Versions
{
    public class Helper
    {

        public static string GetUniqueName(string baseName, List<string> existingNames)
        {
            var rs = string.Empty;


            for (var i = 0; i < 1000; i++)
            {
                var newName = baseName + "_" + i.ToString().PadLeft(4, '0');
                var foundName = existingNames.Any(name => string.Compare(name, newName, StringComparison.OrdinalIgnoreCase) == 0);
                if (foundName) continue;
                rs = newName;
                break;
            }

            return rs;
        }

        public static string GetStringFromDateTime(DateTime dateTime)
        {
            return dateTime.Year
                + "-" + dateTime.Month.ToString().PadLeft(2, '0')
                + "-" + dateTime.Day.ToString().PadLeft(2, '0')
                + "T" + dateTime.Hour.ToString().PadLeft(2, '0')
                + "." + dateTime.Minute.ToString().PadLeft(2, '0')
                + "." + dateTime.Second.ToString().PadLeft(2, '0');
        }

        public static DateTime GetDateTimeFromString(string strDateTime)
        {
            var dateTime = DateTime.Now;

            //2012-05-17
            var rDateTime = new Regex(@"(?<x1>\d{4})\-(?<x2>\d{2})\-(?<x3>\d{2})T(?<x4>\d{2})\.(?<x5>\d{2})\.(?<x6>\d{2})", RegexOptions.IgnoreCase);

            var mRDateTime = rDateTime.Match(strDateTime);
            if (!mRDateTime.Success) return dateTime;
            try
            {
                var yy = Convert.ToInt32(mRDateTime.Groups["x1"].Value);
                var mm = Convert.ToInt32(mRDateTime.Groups["x2"].Value);
                var dd = Convert.ToInt32(mRDateTime.Groups["x3"].Value);

                var hh = Convert.ToInt32(mRDateTime.Groups["x4"].Value);
                var MM = Convert.ToInt32(mRDateTime.Groups["x5"].Value);
                var ss = Convert.ToInt32(mRDateTime.Groups["x6"].Value);

                dateTime = new DateTime(yy, mm, dd, hh, MM, ss);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dateTime;
        }

    }
}
