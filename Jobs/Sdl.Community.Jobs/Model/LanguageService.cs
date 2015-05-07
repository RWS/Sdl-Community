using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class LanguageService
    {
        [JsonProperty(PropertyName = "lang_service_id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "lang_service_name")]
        public string Name { get; set; }
    }
}
