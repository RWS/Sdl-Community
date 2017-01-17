using System;
using System.Collections.Generic;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Segment;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Settings
{
    [Serializable]
    public class SearchSettings
    {      
        public string UserName { get; set; }
        public string Password { get; set; }

        public string AppKey { get; set; }
        public string AuthKey { get; set; }

        public int Limit { get; set; }
        public int Timeout { get; set; }

        public string SourceLanguageId { get; set; }
        public string TargetLanguageId { get; set; }

        public string IgnoreTranslatedSegments { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public List<SegmentSection> SearchSections { get; set; }
      
        public string IndustryId { get; set; }
        public string ContentTypeId { get; set; }
        public string OwnerId { get; set; }
        public string ProductId { get; set; }
        public string ProviderId { get; set; }

        public string IndustryName { get; set; }
        public string ContentTypeName { get; set; }
        public string OwnerName { get; set; }
        public string ProductName { get; set; }
        public string ProviderName { get; set; }

        public PenaltySettings PenaltySettings { get; set; }

        public SearchSettings()
        {
            UserName = string.Empty;
            Password = string.Empty;

            AppKey = string.Empty;
            AuthKey = string.Empty;

            Limit = 20;//default
            Timeout = 10;//default;

            SourceLanguageId = string.Empty;
            TargetLanguageId = string.Empty;

            IgnoreTranslatedSegments = "False";
            
            SearchSections = new List<SegmentSection>();

            IndustryId = string.Empty;
            ContentTypeId = string.Empty;
            OwnerId = string.Empty;
            ProductId = string.Empty;
            ProviderId = string.Empty;

            IndustryName = string.Empty;
            ContentTypeName = string.Empty;
            OwnerName = string.Empty;
            ProductName = string.Empty;
            ProviderName = string.Empty;

            PenaltySettings = new PenaltySettings();
        }
    }
}
