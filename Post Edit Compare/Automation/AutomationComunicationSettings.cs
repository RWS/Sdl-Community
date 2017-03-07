using System;
using System.IO;
using System.Xml.Serialization;

namespace Sdl.Community.PostEdit.Versions.Automation
{
    [Serializable]
    public class AutomationComunicationSettings
    {
        [XmlIgnore]
        public string ApplicationSettingsPath { get; set; }
        [XmlIgnore]
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
