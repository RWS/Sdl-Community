using System;

namespace Sdl.Community.PostEdit.Versions.Structures
{
    [Serializable]
    public class LanguageProperty: ICloneable
    {
       
        public string id { get; set; }
        public string name { get; set; }
     

        public LanguageProperty()
        {
            id = string.Empty;
            name = string.Empty;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
