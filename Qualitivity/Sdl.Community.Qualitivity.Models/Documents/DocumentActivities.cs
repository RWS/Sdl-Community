using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.Documents
{
   
    [Serializable]
    public class DocumentActivities : ICloneable
    {        
        public int Id { get; set; }
        public int ProjectActivityId { get; set; }
        public Document TranslatableDocument { get; set; }            
        public List<DocumentActivity> Activities { get; set; } 
       
        public DocumentActivities()
        {
            Id = -1;
            ProjectActivityId = -1;
            TranslatableDocument = new Document();
            Activities = new List<DocumentActivity>();
        }

        public object Clone()
        {
            var documentActivities = new DocumentActivities
            {
                Id = Id,
                TranslatableDocument = (Document) TranslatableDocument.Clone()
            };


            if (Activities != null)
            {
                documentActivities.Activities = new List<DocumentActivity>();
                foreach (var da in Activities)
                    documentActivities.Activities.Add((DocumentActivity)da.Clone());
            }
            else
                documentActivities.Activities = null;
  

            return documentActivities;
        }
    }
}
