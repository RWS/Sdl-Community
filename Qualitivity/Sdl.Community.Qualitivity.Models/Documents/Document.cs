using System;

namespace Sdl.Community.Structures.Documents
{
  
    [Serializable]
    public class Document : ICloneable
    {
        public int Id { get; set; }

        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }

        public string StudioProjectId { get; set; }
        

        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }

        public Document()
        {
            Id = -1;
            DocumentId = string.Empty;
            DocumentName = string.Empty;
            DocumentPath = string.Empty;

            StudioProjectId = string.Empty;

            SourceLanguage = string.Empty;
            TargetLanguage = string.Empty;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
