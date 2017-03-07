using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using Sdl.Community.PostEdit.Compare.Core;

namespace PostEdit.Compare
{
    public class Common
    {

        public static Regex RegDecimalNumbers = new Regex(@"[\.\,]+(?<x1>.*)", RegexOptions.IgnoreCase);
        public static DateTime DateNull = new DateTime(1819, 1, 1, 0, 0, 0);

        public static decimal RoundUp(decimal d1, decimal d2, int ilen)
        {
            decimal rd = 0;

            //get the value up to ilen decimal points
            var s1 = Math.Round(d1 * d2, (ilen + 1)).ToString(CultureInfo.InvariantCulture);


            var roundUpLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < ilen + 1)
                    roundUpLastDigit = false;
            }
            else
                roundUpLastDigit = false;


            if (roundUpLastDigit)
            {
                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                if (s1LastDecimalPoint != "0")
                    s1ReturnValue += "9";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";
            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, ilen, MidpointRounding.AwayFromZero);



            return rd;
        }
        public static decimal RoundUp(decimal d1, int ilen)
        {
            decimal rd = 0;

            //get the value up to ilen decimal points
            var s1 = Math.Round(d1, (ilen + 1)).ToString(CultureInfo.InvariantCulture);


            var roundUpLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < ilen + 1)
                    roundUpLastDigit = false;
            }
            else
                roundUpLastDigit = false;

            if (roundUpLastDigit)
            {
                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                if (s1LastDecimalPoint != "0")
                    s1ReturnValue += "9";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";
            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, ilen, MidpointRounding.AwayFromZero);



            return rd;
        }
        /// <summary>
        /// Round up (AwayFromZero)
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static decimal RoundUp(decimal d1)
        {
            decimal rd = 0;

            d1 = Math.Round(d1, 1, MidpointRounding.AwayFromZero);

            //get the value up to 4 decimal points
            var s1 = Math.Round(d1, 1).ToString(CultureInfo.InvariantCulture);


            var roundUpLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < 1)
                    roundUpLastDigit = false;
            }
            else
                roundUpLastDigit = false;



            if (roundUpLastDigit)
            {

                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                if (s1LastDecimalPoint != "0")
                    s1ReturnValue += "9";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";

            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, 0, MidpointRounding.AwayFromZero);

            return rd;
        }

        public static decimal RoundDown(decimal d1, int ilen)
        {
            decimal rd = 0;

            //get the value up to ilen decimal points
            var s1 = Math.Round(d1, (ilen + 1)).ToString(CultureInfo.InvariantCulture);


            var roundDownLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < ilen + 1)
                    roundDownLastDigit = false;
            }
            else
                roundDownLastDigit = false;


            if (roundDownLastDigit)
            {
                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                if (s1LastDecimalPoint != "0")
                    s1ReturnValue += "0";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";
            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, ilen, MidpointRounding.ToEven);



            return rd;
        }

        public static decimal Round(decimal d1, int ilen)
        {
            decimal rd = 0;

            //get the value up to ilen decimal points
            var s1 = Math.Round(d1, (ilen + 1)).ToString(CultureInfo.InvariantCulture);


            var roundUpLastDigit = true;
            var s1ReturnValue = "";


            //check if there are enough decimal places
            var mRegDecimalNumbers = RegDecimalNumbers.Match(s1);
            if (mRegDecimalNumbers.Success)
            {
                var decimalDigits = mRegDecimalNumbers.Groups["x1"].Value.Trim();

                if (decimalDigits.Length < ilen + 1)
                    roundUpLastDigit = false;
            }
            else
                roundUpLastDigit = false;


            if (roundUpLastDigit)
            {
                //remove the last decimal point
                s1ReturnValue = s1.Substring(0, s1.Length - 1);

                //get the last decimal point value
                var s1LastDecimalPoint = s1.Substring(s1.Length - 1);

                //test if the last decimal point ==0;
                //if (Convert.ToInt32(s1_lastDecimalPoint) == 5)
                //    s1_returnValue += "5";
                //else
                s1ReturnValue += s1LastDecimalPoint.ToString();
                //if (Convert.ToInt32(s1_lastDecimalPoint) >=5)
                //    s1_returnValue += "9";

                if (s1ReturnValue.Trim() == "")
                    s1ReturnValue = "0";
            }
            else
            {
                s1ReturnValue = s1;
            }

            rd = Convert.ToDecimal(s1ReturnValue);

            rd = Math.Round(rd, ilen, MidpointRounding.AwayFromZero);



            return rd;
        }

        public static Settings.Language GetLanguageFromCultureInfo(string cultureInfo)
        {
            if (cultureInfo.Trim() == string.Empty)
                return null;

            return (from ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures) 
                    where string.Compare(ci.Name, cultureInfo, StringComparison.OrdinalIgnoreCase) == 0 
                    select new Settings.Language(ci.Name, "", ci.TwoLetterISOLanguageName, ci.EnglishName)).FirstOrDefault();
        }
    }
}
