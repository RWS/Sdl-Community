using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors.LocalizationProcessors
{
    public class DecimalPreventLocalizationExtractProcessor : IExtractProcessor
    {

        public IEnumerable<string> Extract(IExtractData extractData)
        {
            return extractData.Settings.GetSourceDecimalSeparators();
        }
    }
}
