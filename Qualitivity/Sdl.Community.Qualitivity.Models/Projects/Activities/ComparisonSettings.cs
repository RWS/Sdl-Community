using System;

namespace Sdl.Community.Structures.Projects.Activities
{
   
    //shallow copy of the document
    [Serializable]
    public class ComparisonSettings : ICloneable
    {

        public int Id { get; set; }
        public int ProjectActivityId { get; set; }
        public int ComparisonType { get; set; }
        public bool ConsolidateChanges { get; set; }
        public bool IncludeTagsInComparison { get; set; }


        public ComparisonSettings()
        {
            Id = -1;
            ProjectActivityId = -1;
            ComparisonType = 0; //default 0=words, 1=characters
            ConsolidateChanges = true;
            IncludeTagsInComparison = true;          
        }

        public object Clone()
        {

            return MemberwiseClone();
        }
    }
}
