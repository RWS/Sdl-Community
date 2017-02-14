using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.Rates
{
  
    [Serializable]
    public class LanguageRateGroup : ICloneable
    {
        
        public int Id { get; set; }       
        public string Name { get; set; }  
        public string Description { get; set; }     
        public string Currency { get; set; }     
        public int DocumentActivityId { get; set; } //document activity id

        public LanguageRateGroupAnalysisBand DefaultAnalysisBand { get; set; }     
        public List<LanguageRate> LanguageRates { get; set; }
        public List<LanguageRateGroupLanguage> GroupLanguages { get; set; }       
      

        public LanguageRateGroup()
        {
            Id = -1;
            Name = string.Empty;
            Description = string.Empty;
            Currency = string.Empty;
            DocumentActivityId = -1;

            DefaultAnalysisBand = new LanguageRateGroupAnalysisBand();
            LanguageRates = new List<LanguageRate>();
            GroupLanguages = new List<LanguageRateGroupLanguage>();
            
        }
        public object Clone()
        {
            var lrg = new LanguageRateGroup
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Currency = Currency,
                DocumentActivityId = DocumentActivityId,
                DefaultAnalysisBand = (LanguageRateGroupAnalysisBand) DefaultAnalysisBand.Clone(),
                LanguageRates = new List<LanguageRate>()
            };



            foreach (var price in LanguageRates)
                lrg.LanguageRates.Add((LanguageRate)price.Clone());

            lrg.GroupLanguages = new List<LanguageRateGroupLanguage>();
            foreach (var language in GroupLanguages)
                lrg.GroupLanguages.Add((LanguageRateGroupLanguage)language.Clone());

      


            return lrg;
        }
    }
}
