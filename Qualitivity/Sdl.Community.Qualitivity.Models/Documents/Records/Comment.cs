using System;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents.Records
{
  
    [Serializable]
    public class Comment : ICloneable
    {
        public string Id { get; set; }
        [XmlIgnore]
        public int RecordId { get; set; }
        [XmlIgnore]
        public int DocumentActivityId { get; set; }

        public string Author { get; set; }

        [XmlIgnore]
        public DateTime? Created { get; set; }
        [XmlAttribute(AttributeName = "created")]
        public string XmlCreated
        {
            get
            {            
                return Created == null ? string.Empty : Helper.DateTimeToSqLite(Created.Value);           
            }
            set
            {           
                Created = value.Length == 0 ? null : Helper.DateTimeFromSqLite(value);             
            }
        }

        public string Severity { get; set; } //Low, Medium, High       
        public string Content { get; set; }        
        public string Version { get; set; }        

        public Comment()
        {
            Id = Guid.NewGuid().ToString();
            RecordId = -1;
            DocumentActivityId = -1;

            Author = string.Empty;
            Created = null;
            Severity = "Low";
            Content = string.Empty;
            Version = string.Empty;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
