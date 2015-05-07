using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class Links
    {
        [JsonProperty(PropertyName = "self")]
        public Link Self { get; set; }

        [JsonProperty(PropertyName = "next")]
        public Link Next { get; set; }

        [JsonProperty(PropertyName = "prev")]
        public Link Previous { get; set; }
    }
}
