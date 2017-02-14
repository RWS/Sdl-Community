using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sdl.Community.Structures
{
    public class Helper
    {
        public static string DateTimeToSqLite(DateTime? datetime)
        {
            if (datetime.HasValue)
            {
                var dateTimeFormat = "{0}-{1}-{2}T{3}:{4}:{5}.{6}";
                return string.Format(dateTimeFormat,
                    datetime.Value.Year
                    , datetime.Value.Month.ToString().PadLeft(2, '0')
                    , datetime.Value.Day.ToString().PadLeft(2, '0')
                    , datetime.Value.Hour.ToString().PadLeft(2, '0')
                    , datetime.Value.Minute.ToString().PadLeft(2, '0')
                    , datetime.Value.Second.ToString().PadLeft(2, '0')
                    , datetime.Value.Millisecond.ToString().PadLeft(3, '0').Substring(0, 3));
            }
            return string.Empty;
        }
        public static DateTime? DateTimeFromSqLite(string strDateTime)
        {
            //"2015-05-01T09:20:05.213"
            DateTime? dt = null;

            try
            {
                if (strDateTime.Trim() != string.Empty)
                    dt = DateTime.ParseExact(strDateTime, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
            }
            catch
            {
                // ignored
            }
            return dt;
        }

    }


    /// <summary>
    /// Serialize DateTime Class (<i>yyyy-MM-ddTHH:mm:ss.fff</i>)
    /// </summary>
    public class SerDateTime : IXmlSerializable
    {
        /// <summary>
        /// Default Constructor when time is not avalaible
        /// </summary>
        public SerDateTime() { }
        /// <summary>
        /// Default Constructor when time is avalaible
        /// </summary>
        /// <param name="pDateTime"></param>
        public SerDateTime(DateTime pDateTime)
        {
            DateTimeValue = pDateTime;
        }

        private DateTime? _dateTimeValue;
        /// <summary>
        /// Value
        /// </summary>
        public DateTime? DateTimeValue
        {
            get { return _dateTimeValue; }
            set { _dateTimeValue = value; }
        }

        // Xml Serialization Infrastructure
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (DateTimeValue == null)
            {
                writer.WriteString(string.Empty);
            }
            else
            {
                var dt = DateTimeValue.Value.Year
                    + "-" + DateTimeValue.Value.Month.ToString().PadLeft(2, '0')
                    + "-" + DateTimeValue.Value.Day.ToString().PadLeft(2, '0')
                    + "T" + DateTimeValue.Value.Hour.ToString().PadLeft(2, '0')
                    + ":" + DateTimeValue.Value.Minute.ToString().PadLeft(2, '0')
                    + ":" + DateTimeValue.Value.Second.ToString().PadLeft(2, '0')
                    + "." + DateTimeValue.Value.Millisecond.ToString().PadLeft(3, '0');

                writer.WriteString(dt);
                ///writer.WriteString(DateTimeValue.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                //writer.WriteString(SerializeObject.SerializeInternal(DateTimeValue.Value));
            }
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            var ltValue = reader.ReadString();
            reader.ReadEndElement();
            if (ltValue.Length == 0)
            {
                DateTimeValue = null;
            }
            else
            {
                //2015-01-01T12:08:01.232
                //Solo se admite yyyyMMdd
                //DateTimeValue = (DateTime)SerializeObject.Deserialize(typeof(DateTime), ltValue);
                DateTimeValue = new DateTime(
                                    int.Parse(ltValue.Substring(0, 4)), //year
                                    int.Parse(ltValue.Substring(5, 2)), //month
                                    int.Parse(ltValue.Substring(8, 2)), //day
                                    int.Parse(ltValue.Substring(11, 2)), //hour
                                    int.Parse(ltValue.Substring(14, 2)), //minute
                                    int.Parse(ltValue.Substring(17, 2)), //second
                                    int.Parse(ltValue.Substring(20)) //millisecond
                                    );
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
    }
    
}
