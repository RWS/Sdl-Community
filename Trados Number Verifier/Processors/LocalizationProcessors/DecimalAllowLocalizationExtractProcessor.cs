using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors.LocalizationProcessors
{
    public class DecimalAllowLocalizationExtractProcessor : IExtractProcessor
    {

        public IEnumerable<string> Extract(IExtractData extractData)
        {
            var sourceDecimalSeparators = extractData.Settings.GetSourceDecimalSeparators();
            var targetDecimalSeparators = extractData.Settings.GetTargetDecimalSeparators();

            return sourceDecimalSeparators.Concat(targetDecimalSeparators);
        }
    }
}
