using System.Collections.Generic;
using Sdl.Community.Structures.Documents.Records;

namespace Sdl.Community.Qualitivity.Tracking
{
    /// <summary>
    /// keep track of the document (or documents) that are loaded into the editor
    /// </summary>
    public class TrackedDocuments
    {
        public string DocumentIdFirstOrDefault { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityType { get; set; }//Translation, Review, Sign-off
    

        public int ClientId { get; set; }
        public string ClientName { get; set; }


        public string ProjectIdStudio { get; set; }
        public string ProjectNameStudio { get; set; }
        public string ProjectPathStudio { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }

 
        public List<TrackedDocument> Documents { get; set; }
        public List<QualityMetric> QualityMetrics { get; set; }


        //manages the active object references from the documents
        //these will be checked and initialized each time the segment changes if neccessary
        public TrackedSegment ActiveSegment { get; set; }
        public TrackedDocument ActiveDocument { get; set; }



        public TrackedDocuments()
        {
            DocumentIdFirstOrDefault = string.Empty;

            ActivityDescription = string.Empty;
            ActivityType = string.Empty;

            ClientId = -1;
            ClientName = string.Empty;


            ProjectIdStudio = string.Empty;
            ProjectNameStudio = string.Empty;
            ProjectPathStudio = string.Empty;

            ProjectId = -1;
            ProjectName = string.Empty;
            ProjectPath = string.Empty;

           

            Documents = new List<TrackedDocument>();
            QualityMetrics = new List<QualityMetric>();


            ActiveSegment = new TrackedSegment();
            ActiveDocument = new TrackedDocument();
        }
       
    }
}
