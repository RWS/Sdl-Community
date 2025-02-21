using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class Link
    {
        [JsonProperty(PropertyName = "href")]
        public string Href { get; set; }
    }
}
