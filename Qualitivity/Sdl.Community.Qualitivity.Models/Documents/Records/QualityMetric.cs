using System;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents.Records
{
   
    [Serializable]
    public class QualityMetric : ICloneable
    {

        public enum ItemStatus
        {
            Open,
            Resolved,
            Ignore
        }
        
        //used in case there are multiple edits of the same quality metric
        //accross multiple sessions; this will allow you to interlink these.
        public string Guid { get; set; }

        public string Id { get; set; }

        [XmlIgnore]
        public int RecordId { get; set; }
        [XmlIgnore]
        public int DocumentActivityId { get; set; }

        public ItemStatus Status { get; set; }

        public string Name { get; set; }
   
        //it needs to keep an independant copy of these 2 values
        //no real need to create a direct instance
        public string SeverityName { get; set; }      
        public int SeverityValue { get; set; }              
                            
        public string Content { get; set; }     
        public string Comment { get; set; }

            
        
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

        [XmlIgnore]
        public DateTime? Modified { get; set; }
        [XmlAttribute(AttributeName = "modified")]
        public string XmlModified
        {
            get
            {             
                if (Modified == null)                
                    return string.Empty;
                
                //2015-01-01T12:08:01.232  
                var dt = Helper.DateTimeToSqLite(Modified.Value); 

                return dt;
             
            }
            set
            {             
                Modified = value.Length == 0 ? null : Helper.DateTimeFromSqLite(value);
            
            }
        }




        public string UserName { get; set; }
        [XmlIgnore]
        public bool Updated { get; set; }
        [XmlIgnore]
        public bool Removed { get; set; }

        public string DocumentId { get; set; }
        public string SegmentId { get; set; }
        public string ParagraphId { get; set; }

        public QualityMetric()
        {
            Guid = System.Guid.NewGuid().ToString();
            Id = System.Guid.NewGuid().ToString();
            
            RecordId = -1;
            DocumentActivityId = -1;

            Status = ItemStatus.Open;

            Name = string.Empty;

            //it needs to keep an independant copay of these 2 values
            //no real need to create a direct instance
            SeverityName = string.Empty;
            SeverityValue = 0;
            
            
            Content = string.Empty;
            Comment = string.Empty;
            Created = DateTime.Now;
            Modified = DateTime.Now;

            UserName = string.Empty;

            Updated = false;
            Removed = false;

            DocumentId = string.Empty;
            SegmentId = string.Empty;
            ParagraphId = string.Empty;
            
        }


        public object Clone()
        {
           
            var qualityMetric = new QualityMetric
            {
                Guid = Guid,
                Id = Id,
                RecordId = RecordId,
                DocumentActivityId = DocumentActivityId,
                Status = Status,
                Name = Name,
                SeverityName = SeverityName,
                SeverityValue = SeverityValue,
                Content = Content,
                Comment = Comment,
                Created = Created,
                Modified = Modified,
                Updated = Updated,
                Removed = Removed,
                UserName = UserName,
                DocumentId = DocumentId,
                SegmentId = SegmentId,
                ParagraphId = ParagraphId
            };

           



            return qualityMetric;
        }
    }
}
