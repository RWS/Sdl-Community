using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class Link
    {
        [JsonProperty(PropertyName = "href")]
        public string Href { get; set; }
    }
}
