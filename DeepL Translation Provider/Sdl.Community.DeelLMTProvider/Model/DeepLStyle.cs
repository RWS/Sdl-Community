using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class StyleRulesResponse
    {
        [JsonProperty("style_rules")]
        public List<DeepLStyle> StyleRules { get; set; }
    }
    public class DeepLStyle
    {
        [JsonProperty("style_id")]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
    }
}