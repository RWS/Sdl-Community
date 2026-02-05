using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Model
{
    public class CohereSubscriptionOptions
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDoNotShowAgainVisible { get; set; }
        public string TrialUri { get; set; }
        public string BuyUri { get; set; }
    }

}
