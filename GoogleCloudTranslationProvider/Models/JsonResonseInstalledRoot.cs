using Newtonsoft.Json;

namespace GoogleCloudTranslationProvider.Models
{
    public class JsonResonseInstalledRoot
    {
        [JsonProperty("installed")]
        public JsonResonseInstalled Installed { get; set; }
    }
}
