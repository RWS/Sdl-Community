using System.IO;
using System.Xml.Serialization;

namespace Sdl.Community.PostEdit.Versions.Structures
{
    public class SettingsSerializer
    {
        
        public static void SaveSettings(Settings settings)
        {
            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                stream = new FileStream(settings.ApplicationSettingsFullPath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, settings);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        public static Settings ReadSettings()
        {
            var settings = new Settings();
            if (!File.Exists(settings.ApplicationSettingsFullPath))
            {
                SaveSettings(settings);
            }

            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                stream = new FileStream(settings.ApplicationSettingsFullPath, FileMode.Open);
                settings = (Settings)serializer.Deserialize(stream) ?? new Settings();

                return settings;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
    }
}
