using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Model
{
    public class TranslationData
    {
        public string QualityEstimation { get; set; }
        public string Translation { get; set; }
        public string ModelName { get; set; }
        public string Model { get; set; }
        public bool AutoSendFeedback { get; set; }
        public int Index { get; set; }
    }

}
