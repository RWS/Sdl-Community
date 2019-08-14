using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class JobPosting
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "web_url")]
        public string WebUrl { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "time_posted")]
        public DateTime TimePosted { get; set; }

        [JsonProperty(PropertyName = "language_pairs")]
        public List<string> LanguagePairs { get; set; }

        [JsonProperty(PropertyName = "volume_amount")]
        public long VolumeAmount { get; set; }

        [JsonProperty(PropertyName = "volume_unit")]
        public string VolumeUnit { get; set; }

        [JsonProperty(PropertyName = "disc_spec_id")]
        public int DiscSpecId { get; set; }

        [JsonProperty(PropertyName = "disc_other")]
        public string OtherDiscipline { get; set; }

        [JsonProperty(PropertyName = "language_service_ids")]
        public List<int> LanguageServiceIds { get; set; }

    }
}
