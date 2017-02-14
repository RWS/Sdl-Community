using System;

namespace Sdl.Community.Structures.DQF
{
    [Serializable]
    public class DqfProjectTask: ICloneable
    {
        public int Id { get; set; }
        public int TableTausdqfprojectsId { get; set; }
        public int ProjectActivityId { get; set; }
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DqfTranslatorKey { get; set; }
        public string DqfProjectKey { get; set; }
        public int DqfProjectId { get; set; }
        public int DqfTaskId { get; set; }

        public DateTime? Uploaded {get;set;}       
        public string TargetLanguage { get; set; }        
        public int CatTool { get; set; }

        public int TotalSegments { get; set; }

        public DqfProjectTask()
        {
            Id = -1;
            TableTausdqfprojectsId = -1;
            ProjectActivityId = -1;
            DocumentId = string.Empty;
            DocumentName = string.Empty;
            DqfTranslatorKey = string.Empty;
            DqfProjectKey = string.Empty;
            DqfProjectId = -1;
            DqfTaskId = -1;

            Uploaded = null;            
            TargetLanguage = string.Empty;

            CatTool = 23;

            TotalSegments = 0;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
