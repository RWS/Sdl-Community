using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.PostEdit.Versions.Structures
{
    [Serializable]
    public class Settings: ICloneable
    {


        public string versions_folder_path { get; set; }
        public bool create_shallow_copy { get; set; }
        public bool create_subfolder_projects { get; set; }

        public List<Project> projects { get; set; }

        public string ApplicationSettingsPath { get; set; }
        public string ApplicationSettingsFullPath { get; set; }

        public Settings()
        {
            ApplicationSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostEdit.Compare");
            if (!Directory.Exists(ApplicationSettingsPath))
                Directory.CreateDirectory(ApplicationSettingsPath);
            ApplicationSettingsFullPath = Path.Combine(ApplicationSettingsPath, "PostEdit.Versions.settings.xml");


            var versionsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PostEdit.Compare");
            if (!Directory.Exists(versionsFolderPath))
                Directory.CreateDirectory(versionsFolderPath);
            versionsFolderPath = Path.Combine(versionsFolderPath, "Versions");
          
            if (!Directory.Exists(versionsFolderPath))
                Directory.CreateDirectory(versionsFolderPath);


            versions_folder_path = versionsFolderPath;
            create_shallow_copy = true;
            create_subfolder_projects = true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
