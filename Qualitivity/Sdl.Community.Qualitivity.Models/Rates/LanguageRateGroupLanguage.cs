using System;

namespace Sdl.Community.Structures.Rates
{
    
    [Serializable]
    public class LanguageRateGroupLanguage : ICloneable
    {
        public enum LanguageType
        {
            Source,
            Target,
            None
        }

        public int Id { get; set; }
        public string LanguageIdCi { get; set; } //culture Info name 
        public int LanguageRateId { get; set; }
        public LanguageType Type { get; set; }

        public LanguageRateGroupLanguage()
        {
            Id = -1;
            LanguageIdCi = string.Empty;
            LanguageRateId = -1;
            Type = LanguageType.None;
        }
        public LanguageRateGroupLanguage(int id, string languageIdCi, int languageRateId, LanguageType type)
        {
            Id = id;
            LanguageIdCi = languageIdCi;
            LanguageRateId = languageRateId;
            Type = type;
        }
        public object Clone()
        {
            return (LanguageRateGroupLanguage)MemberwiseClone();
        }
    }
}
