using Newtonsoft.Json;
using System.Collections.Generic;

namespace InterpretBank.SettingsService
{
    public class Settings
    {
        public string DatabaseFilepath { get; set; }

        public List<string> Glossaries { get; set; }

        [JsonIgnore]
        public string SettingsId { get; set; }

        public List<string> Tags { get; set; }

        public bool UseTags { get; set; }
    }
}