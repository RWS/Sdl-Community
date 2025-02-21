using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class Discipline
    {
        [JsonProperty(PropertyName = "disc_spec_id")]
        public int DiscSpecId { get; set; }

        [JsonProperty(PropertyName = "disc_spec_name")]
        public string DiscSpecName { get; set; }

    }
}
