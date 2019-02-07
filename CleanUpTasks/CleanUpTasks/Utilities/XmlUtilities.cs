using System.IO;
using System.Xml.Serialization;
using Sdl.Community.CleanUpTasks.Models;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public static class XmlUtilities
    {
        public static ConversionItemList Deserialize(string openPath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(ConversionItemList));

            using (TextReader reader = new StreamReader(openPath))
            {
                object obj = deserializer.Deserialize(reader);
                return (ConversionItemList)obj;
            }
        }

        public static void Serialize(ConversionItemList list, string savePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConversionItemList));

            using (TextWriter writer = new StreamWriter(savePath))
            {
                serializer.Serialize(writer, list);
            }
        }
    }
}