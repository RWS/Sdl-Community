using System;

namespace Sdl.Community.Structures.iProperties
{
    [Serializable]
    public class ViewProperty: ICloneable
    {
        public int Id { get; set; }
        public string ViewName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public string Text { get; set; }
        

        public ViewProperty()
        {
            Id = -1;
            ViewName = string.Empty;
            Name = string.Empty;
            ValueType = string.Empty;
            Value = string.Empty;
            Text = string.Empty;
          
        }
        public ViewProperty(string viewName, string name, string valueType, string value, string text = "")
        {
            Id = -1;
            ViewName = viewName;
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
