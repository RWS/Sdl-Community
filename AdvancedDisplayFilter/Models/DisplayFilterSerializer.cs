using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sdl.Community.AdvancedDisplayFilter.Models
{
    public class DisplayFilterSerializer
    {
        public static string SerializeSettings<T>(T obj)
        {
            string result;

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(XmlWriter.Create(stream), obj);
                stream.Flush();

                stream.Position = 0;
                var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }

            return result;
        }

        public static T DeserializeSettings<T>(string xmlText) where T : new()
        {
            var reader = new StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));
            var obj = serializer.Deserialize(XmlReader.Create(reader));
            var result = (T)obj;

            return result;
        }
    }
}
