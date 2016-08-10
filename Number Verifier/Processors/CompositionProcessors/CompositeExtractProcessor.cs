using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors.CompositionProcessors
{
    public class CompositeExtractProcessor:IExtractProcessor
    {
        public IExtractProcessor[] Nodes;

        public IEnumerable<string>Extract(IExtractData extractData)
        {
            return from n in this.Nodes
                   from r in n.Extract(extractData)
                   select r;
        }
    
    }
}
