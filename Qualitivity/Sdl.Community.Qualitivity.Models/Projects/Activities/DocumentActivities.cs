using System;
using System.Collections.Generic;
using Sdl.Community.Structures.Documents;

namespace Sdl.Community.Structures.Projects.Activities
{
   
    //shallow copy of the document
    [Serializable]
    public class DocumentActivities : ICloneable
    {

        public int Id { get; set; }
        public int ProjectActivityId { get; set; }

        public string DocumentId { get; set; }
        public Document TranslatableDocument { get; set; }

        public long DocumentActivityTicks { get; set; }
        public long DocumentRecordsTicks { get; set; }

        public List<int> DocumentActivityIds { get; set; }

        public DocumentActivities()
        {
            Id = -1;
            ProjectActivityId = -1;

            DocumentId = string.Empty;
            TranslatableDocument = new Document();

            DocumentActivityTicks = 0;
            DocumentRecordsTicks = 0;

            DocumentActivityIds = new List<int>();
        }

        public object Clone()
        {
            var documentActivities = new DocumentActivities
            {
                Id = Id,
                ProjectActivityId = ProjectActivityId,
                DocumentId = DocumentId,
                TranslatableDocument = (Document) TranslatableDocument.Clone(),
                DocumentActivityTicks = DocumentActivityTicks,
                DocumentRecordsTicks = DocumentRecordsTicks,
                DocumentActivityIds = new List<int>()
            };



            foreach (var value in DocumentActivityIds)
                documentActivities.DocumentActivityIds.Add(value);


            return documentActivities;
        }
    }
}
