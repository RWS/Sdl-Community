using System;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents
{
 
    [Serializable]
    public class StateCountItem : ICloneable
    {
        public enum CountType
        {
            TransaltionMatch,
            ConfirmationStatus,
            None
        }
        //public string id { get; set; }
        [XmlIgnore]
        public int DocumentActivityId { get; set; }
        //public CountType count_type { get; set; }
        public string Name { get; set; }     
        public int Value { get; set; }
      

        public StateCountItem()
        {
            //id = Guid.NewGuid().ToString();
            DocumentActivityId = -1;
            //count_type = CountType.None;
            Name = string.Empty;
            Value = 0;
      
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
