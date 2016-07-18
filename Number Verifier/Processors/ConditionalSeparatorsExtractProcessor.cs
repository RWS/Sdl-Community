using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors
{
    public class ConditionalSeparatorsExtractProcessor: IExtractProcessor
    {
        public IExtractSpecification Specification;
        public IExtractProcessor TruthProcessor;

        public IEnumerable<string> Extract(IExtractData extractData)
        {
            if (Specification.IsSatisfiedBy(extractData))
            {
                return TruthProcessor.Extract(extractData);
            }
            return Enumerable.Empty<string>();
        }
    }
}
