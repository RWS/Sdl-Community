using System;
using System.Collections.Generic;
using Sdl.Community.Structures.Documents.Records;

namespace Sdl.Community.Structures.Documents
{
    
    [Serializable]
    public class DocumentActivity : ICloneable
    {
       
        public int Id { get; set; }
        public string DocumentActivityType { get; set; } 

        public int ProjectActivityId { get; set; } //activity id                               
        public int ProjectId { get; set; } //project id 

        public Document TranslatableDocument { get; set; }
        public string DocumentId { get; set; } //document id                         
        
           
        public DateTime? Started { get; set; }            
        public DateTime? Stopped { get; set; }
             
        public long TicksActivity { get; set; }//elapsed time ticks - active        
        public long TicksRecords { get; set; }//elapsed time ticks - records

        public long WordCount { get; set; }  //count 
          
        public List<Record> Records { get; set; } //track changes records
        public DocumentStateCounters DocumentStateCounters { get; set; } //DocumentStateCounters
   

        public DocumentActivity()
        {
            Id = -1;
            DocumentActivityType = string.Empty;

            ProjectActivityId = -1;

            TranslatableDocument = new Document();
            DocumentId = string.Empty;
            ProjectId = -1;

            Started = null;
            Stopped = null;

            TicksActivity = 0;
            TicksRecords = 0;

            WordCount = 0;
            
            Records = new List<Record>();
            DocumentStateCounters = new DocumentStateCounters();                        
        }

        public object Clone()
        {
            var tca = new DocumentActivity
            {
                Id = Id,
                DocumentActivityType = DocumentActivityType,
                ProjectActivityId = ProjectActivityId,
                DocumentId = DocumentId,
                TranslatableDocument = (Document) TranslatableDocument.Clone(),
                ProjectId = ProjectId,
                Started = Started,
                Stopped = Stopped,
                TicksActivity = TicksActivity,
                TicksRecords = TicksRecords,
                WordCount = WordCount,
                Records = new List<Record>()
            };




            foreach (var tcr in Records)
                tca.Records.Add((Record)tcr.Clone());

            tca.DocumentStateCounters = (DocumentStateCounters)DocumentStateCounters.Clone();
         


            return tca;
        }
    }
}
