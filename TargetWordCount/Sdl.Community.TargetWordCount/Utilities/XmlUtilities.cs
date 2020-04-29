using System.IO;
using System.Xml.Serialization;
using Sdl.Community.TargetWordCount.Models;

namespace Sdl.Community.TargetWordCount.Utilities
{
	public static class XmlUtilities
    {
        public static SerializableSettings Deserialize(string openPath)
        {
            var deserializer = new XmlSerializer(typeof(SerializableSettings));

            using (TextReader reader = new StreamReader(openPath))
            {
                object obj = deserializer.Deserialize(reader);
                return (SerializableSettings)obj;
            }
        }

        public static void Serialize(SerializableSettings settings, string savePath)
        {
            var serializer = new XmlSerializer(typeof(SerializableSettings));

            using (TextWriter writer = new StreamWriter(savePath))
            {
                serializer.Serialize(writer, settings);
            }
        }
    }
}