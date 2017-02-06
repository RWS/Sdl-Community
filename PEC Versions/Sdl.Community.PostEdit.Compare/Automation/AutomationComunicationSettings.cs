using System;
using System.IO;
using System.Xml.Serialization;

namespace PostEdit.Compare.Automation
{
    [Serializable]
    public class AutomationComunicationSettings
    {
        [XmlIgnoreAttribute]
        public string ApplicationSettingsPath { get; set; }
        [XmlIgnoreAttribute]
        public string ApplicationSettingsFullPath { get; set; }


        public string folderPathLeft { get; set; }
        public string folderPathRight { get; set; }

        public AutomationComunicationSettings()
        {
            ApplicationSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostEdit.Compare");
            if (!Directory.Exists(ApplicationSettingsPath))
                Directory.CreateDirectory(ApplicationSettingsPath);
            ApplicationSettingsFullPath = Path.Combine(ApplicationSettingsPath, "PostEdit.Versions.Automation.settings.xml");

            folderPathLeft = string.Empty;
            folderPathRight = string.Empty;

        }
    }
}