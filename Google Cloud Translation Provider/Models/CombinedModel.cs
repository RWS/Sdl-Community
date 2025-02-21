using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloudTranslationProvider.Models
{
    public class CombinedModel
    {
        public string Dataset { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string SourceLanguageCode { get; set; }
        public string TargetLanguageCode { get; set; }
    }
}
