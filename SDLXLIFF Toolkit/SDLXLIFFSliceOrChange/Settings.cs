using System;
using System.IO;
using System.Xml;

namespace SDLXLIFFSliceOrChange
{
    public static class Settings
    {
        public static String SettingsFile
        {
            get
            {
                String settingsFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"SDL\OpenExchange\SDLXLIFF Toolkit\SDLXLIFF toolkit.xml");
                if (!Directory.Exists(Path.GetDirectoryName(settingsFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));

                if (!File.Exists(settingsFilePath))
                {
                    using (StreamWriter sw = new StreamWriter(settingsFilePath, true))
                    {
                        sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                        sw.WriteLine("<Settings></Settings>");
                    }
                }
                return settingsFilePath;
            }
        }

        public static String LogFile
        {
            get
            {
                String settingsFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"SDL\OpenExchange\SDLXLIFF Toolkit\SDLXLIFF toolkit log.txt");
                if (!Directory.Exists(Path.GetDirectoryName(settingsFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));

                if (!File.Exists(settingsFilePath))
                {
                    using (StreamWriter sw = new StreamWriter(settingsFilePath, true))
                    {
                        sw.WriteLine("");
                    }
                }
                return settingsFilePath;
            }
        }

        private const string SettingsPath = "Settings";
        private const string LanguageCulturePath = "LanguageCuture";

        public static String GetXMLPath(String element)
        {
            return String.Format(@"//{0}", element);
        }

        public static void SaveSelectedCulture(String culture)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            XmlNode xmlNodeLanguageCuture = xmlNodeSettings.SelectSingleNode(GetXMLPath(LanguageCulturePath));
            if (xmlNodeLanguageCuture != null)
                xmlNodeLanguageCuture.InnerText = culture;
            else
            {
                xmlNodeLanguageCuture = xmlDoc.CreateElement(LanguageCulturePath);
                xmlNodeLanguageCuture.InnerText = culture;
                xmlNodeSettings.AppendChild(xmlNodeLanguageCuture);
            }

            xmlDoc.Save(SettingsFile);
        }

        public static String GetSavedCulture()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(SettingsFile);
            var xmlNodeSettings = xmlDoc.SelectSingleNode(GetXMLPath(SettingsPath));

            XmlNode xmlNodeLanguageCuture = xmlNodeSettings.SelectSingleNode(GetXMLPath(LanguageCulturePath));
            return xmlNodeLanguageCuture == null ? "en-US" : xmlNodeLanguageCuture.InnerText;
        }
    }
}
