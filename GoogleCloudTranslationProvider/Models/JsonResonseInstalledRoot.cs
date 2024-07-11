using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloudTranslationProvider.Models
{
    public class JsonResonseInstalledRoot
    {
        [JsonProperty("installed")]
        public JsonResonseInstalled Installed { get; set; }
    }
}
