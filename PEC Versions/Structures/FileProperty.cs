using System;

namespace Sdl.Community.PostEdit.Versions.Structures
{
    [Serializable]
    public class FileProperty : ICloneable
    {
        public enum FileType
        {
            Translatable,
            Localizable,
            Reference,
            Unknown            
        }
        
        public string name { get; set; }
        public string path { get; set; }
        public FileType fileType { get; set; }
        public string sourceId { get; set; }
        public string targetId { get; set; }
        public bool isSource { get; set; }


        public FileProperty()
        {
            name = string.Empty;
            path = string.Empty;
            fileType = FileType.Unknown;
            sourceId = string.Empty;
            targetId = string.Empty;
            isSource = true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
