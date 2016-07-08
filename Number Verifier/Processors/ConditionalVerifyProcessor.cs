using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Processors
{
    public class ConditionalVerifyProcessor : IVerifyProcessor
    {
        public IVerifySpecification Specification;
        public IVerifyProcessor TruthProcessor;
        public IEnumerable<ErrorReporting> Verify(INumberResults numberResults)
        {
            if(this.Specification.IsSatisfiedBy(numberResults))
            {
                return TruthProcessor.Verify(numberResults);
            }
            return Enumerable.Empty<ErrorReporting>();
        }
    }
}
