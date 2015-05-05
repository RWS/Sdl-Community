using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Sdl.Community.Jobs.Model
{
    public class Language
    {
        [JsonProperty(PropertyName = "language_code")]
        public string LanguageCode { get; set; }

        [JsonProperty(PropertyName = "language_name")]
        public string LanguageName { get; set; }
    }
}
