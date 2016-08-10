using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors.LocalizationProcessors
{
    public class ThousandAllowLocalizationExtractProcessor : IExtractProcessor
    {
         public IEnumerable<string> Extract(IExtractData extractData)
        {
            var sourceThousandsSeparators = extractData.Settings.GetSourceThousandSeparators();
            var targetThousandSeparators = extractData.Settings.GetTargetThousandSeparators();

            return sourceThousandsSeparators.Concat(targetThousandSeparators);
        }
    }
}
