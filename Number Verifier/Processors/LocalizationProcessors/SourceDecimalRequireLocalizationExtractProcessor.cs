using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors.LocalizationProcessors
{
    public class SourceDecimalRequireLocalizationExtractProcessor : IExtractProcessor
    {
        
        public IEnumerable<string> Extract(IExtractData extractData)
        {
            return extractData.Settings.GetSourceDecimalSeparators();
        }
    }
}
