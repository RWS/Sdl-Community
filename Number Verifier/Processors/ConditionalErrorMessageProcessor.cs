using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Processors
{
    public class ConditionalErrorMessageProcessor : IErrorMessageProcessor
    {
        public IVerifySpecification Specification;

        public IErrorMessageProcessor TruthProcessor;
        public string GenerateMessage(INumberResults numberResult)
        {
            if(this.Specification.IsSatisfiedBy(numberResult))
            {
                return TruthProcessor.GenerateMessage(numberResult);
            }
            return string.Empty;
        }
    }
}
