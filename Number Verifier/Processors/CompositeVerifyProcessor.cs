using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Processors
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
