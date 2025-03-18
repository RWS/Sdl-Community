using Sdl.Core.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.PeerResolvers;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class DeepLCachedResult
    {
        public string SourceText { get; set; }

        public string TargetText { get; set; }

        public CultureCode SourceLanguage { get; set; }

        public CultureCode TargetLanguage { get; set; }
    }
}
