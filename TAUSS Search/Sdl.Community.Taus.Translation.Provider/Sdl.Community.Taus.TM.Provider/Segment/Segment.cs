using System.Collections.Generic;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Segment
{
    public class Segment
    {
        public string Id { get; set; }

    

        public Language SourceLanguage { get; set; }       
        public Language TargetLanguage { get; set; }
        
        public string SourceText { get; set; }
        public List<SegmentSection> SourceSections { get; set; }

        public string TargetText { get; set; }
        public List<SegmentSection> TargetSections { get; set; }

        public Industry Industry { get; set; }        
        public ContentType ContentType { get; set; }        
        public Owner Owner { get; set; }        
        public Product Product { get; set; }        
        public Provider Provider { get; set; }

        

        public double MatchPercentage { get; set; }

        public Segment()
        {
            Id = string.Empty;

           
            SourceLanguage = new Language();
            SourceSections = new List<SegmentSection>();

            TargetLanguage = new Language();
            TargetSections = new List<SegmentSection>();

            SourceText = string.Empty;
            TargetText = string.Empty;

            Industry = new Industry();
            ContentType = new ContentType();
            Owner = new Owner();
            Product = new Product();
            Provider = new Provider();

            MatchPercentage = 0;
        }
    }
}
