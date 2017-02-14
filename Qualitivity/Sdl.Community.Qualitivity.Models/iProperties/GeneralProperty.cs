using System;

namespace Sdl.Community.Structures.iProperties
{
    [Serializable]
    public class GeneralProperty : ICloneable
    {
        public int Id { get; set; }       
        public string Name { get; set; }
        public string ValueType { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        

        public GeneralProperty()
        {
            Id = -1;
            ValueType = string.Empty;
            Name = string.Empty;
            Value = string.Empty;
            Text = string.Empty;
            
        }
        public GeneralProperty(string name, string valueType, string value, string text = "")
        {
            Id = -1;            
            Name = name;
            ValueType = valueType;
            Value = value;
            Text = text;

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
