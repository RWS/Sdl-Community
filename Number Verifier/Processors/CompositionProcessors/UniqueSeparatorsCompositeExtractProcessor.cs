using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors.CompositionProcessors
{
    public class UniqueSeparatorsCompositeExtractProcessor: IExtractProcessor
    {
        public IExtractProcessor[] Nodes;

        public IEnumerable<string> Extract(IExtractData extractData)
        {
            return (from n in Nodes
                    from r in n.Extract(extractData)
                    select r).ToList().Distinct();
        }
    }
}
