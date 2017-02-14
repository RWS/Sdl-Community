using System;

namespace Sdl.Community.Structures.QualityMetrics
{
   
    [Serializable]
    public class Severity : ICloneable
    {    
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }               
        public int Value { get; set; }

        public Severity()
        {
            Id = -1;
            GroupId = -1;
            Name = string.Empty;
            Value = 0;
        }
        public Severity(int id, int groupId)
        {
            Id = id;
            GroupId = groupId;
            Name = string.Empty;
            Value = 0;
        }
        public Severity(string name, int value, int groupId)
        {
            Id = -1;
            GroupId = groupId;
            Name = name;
            Value = value;
        }


        public object Clone()
        {
            return (Severity)MemberwiseClone();
        }
    }
}
