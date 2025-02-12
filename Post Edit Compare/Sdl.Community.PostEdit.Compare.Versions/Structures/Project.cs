using System;
using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Versions.Structures
{
    [Serializable]
    public class Project: ICloneable
    {
        public string projectFileName { get; set; } 
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string createdAt { get; set; }
        public string createdBy { get; set; }
        public LanguageProperty sourceLanguage { get; set; }
        public List<LanguageProperty> targetLanguages { get; set; }
        public int translatableCount { get; set; }
        public int referenceCount { get; set; }
        public int localizableCount { get; set; }
        public int unKnownCount { get; set; }
        public List<FileProperty> files { get; set; }   
        public string projectType { get; set; }
        public string location { get; set; }
        public List<ProjectVersion> projectVersions { get; set; }


        public Project()
        {
            projectFileName = string.Empty;
            id = string.Empty;
            name = string.Empty;
            description = string.Empty;
            createdAt = string.Empty;
            createdBy = string.Empty;
            sourceLanguage = new LanguageProperty();
            targetLanguages = new List<LanguageProperty>();
            translatableCount = 0;
            referenceCount = 0;
            localizableCount = 0;
            unKnownCount = 0;
            files = new List<FileProperty>();           
            projectType = "Standard Studio Project";
            location = string.Empty;
            projectVersions = new List<ProjectVersion>();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
