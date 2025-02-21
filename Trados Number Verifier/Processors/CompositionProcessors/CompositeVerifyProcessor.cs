using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Processors.CompositionProcessors
{
    public class CompositeVerifyProcessor : IVerifyProcessor
    {
        public IVerifyProcessor[] Nodes;
        public IEnumerable<ErrorReporting> Verify(INumberResults numberResults)
        {
            return from n in this.Nodes
                   from r in n.Verify(numberResults)
                   select r;
        }
    }
}
