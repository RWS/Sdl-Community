using System.IO;
using System.Xml.Serialization;

namespace PostEdit.Compare.Automation
{
    public class SettingsSerializer
    {
        
        public static void SaveSettings(AutomationComunicationSettings settings)
        {
            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(AutomationComunicationSettings));
                stream = new FileStream(settings.ApplicationSettingsFullPath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, settings);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        public static AutomationComunicationSettings ReadSettings()
        {
            var settings = new AutomationComunicationSettings();
            if (!File.Exists(settings.ApplicationSettingsFullPath))
            {
                SaveSettings(settings);
            }

            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(AutomationComunicationSettings));
                stream = new FileStream(settings.ApplicationSettingsFullPath, FileMode.Open);
                settings = (AutomationComunicationSettings)serializer.Deserialize(stream) ??
                           new AutomationComunicationSettings();

                settings.ApplicationSettingsFullPath = Path.Combine(settings.ApplicationSettingsPath, "PostEdit.Versions.Automation.settings.xml");

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
