using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors.CompositionProcessors
{
    public class ApostrophSeparatorsCompositeExtractProcessor : IExtractProcessor
    {
        public IExtractProcessor[] Nodes;
      
        public IEnumerable<string> Extract(IExtractData extractData)
        {
            var defaultSelectedSeparators = (from n in Nodes
                                             from r in n.Extract(extractData)
                                             select r).ToList();

            return defaultSelectedSeparators.Contains("'") ?
                defaultSelectedSeparators.Concat(new[] { @"\u2019", @"\u2027" }) :
                defaultSelectedSeparators;
        }
    }
}
