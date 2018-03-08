using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
    public class CalendarDateTimePatterns
    {
        private System.Globalization.Calendar _Calendar;
        private System.Globalization.CultureInfo _Culture;

        private List<DateTimePattern> _Patterns;

        public CalendarDateTimePatterns(System.Globalization.CultureInfo culture, System.Globalization.Calendar cal)
        {
            _Culture = culture;
            _Calendar = cal;
            _Patterns = new List<DateTimePattern>();
        }

        // TODO not sure whether it makes sense to distinguish the 4 pattern types here as well as again through the PatternType - 
        //  maybe rather support an open list of DateTimePattern's each of which has a specific pattern type associated

        public List<DateTimePattern> Patterns
        {
            get { return _Patterns; }
            set { _Patterns = value; }
        }

        public string CultureName
        {
            get { return _Culture.Name; }
			set { _Culture = Core.CultureInfoExtensions.GetCultureInfo(value); }
        }

        [System.Xml.Serialization.XmlIgnore]
        public System.Globalization.CultureInfo Culture
        {
            get { return _Culture; }
        }

        [System.Xml.Serialization.XmlIgnore]
        public System.Globalization.Calendar Calendar
        {
            // TODO serialize calendar info as well (type name may be enough)
            get { return _Calendar; }
        }

    }

}
