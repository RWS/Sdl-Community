using System;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents.Records
{

    [Serializable]
    public class RevisionMarker : ICloneable
    {
        public enum RevisionType
        {
            Insert,
            Delete,
            Unchanged
        }        
        public string Id { get; set; }
        [XmlIgnore]
        public string ContentSectionId { get; set; }
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
                //2015-01-01T12:08:01.232             
            }
            set
            {              
                Created = value.Length == 0 ? null : Helper.DateTimeFromSqLite(value);             
            }
        }    

        public RevisionType RevType { get; set; }

        public RevisionMarker()
        {
            Id = Guid.NewGuid().ToString();
            ContentSectionId = string.Empty;
            RecordId = -1;
            DocumentActivityId = -1;   
            Author = string.Empty;
            Created = null;
            RevType = RevisionType.Unchanged;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
