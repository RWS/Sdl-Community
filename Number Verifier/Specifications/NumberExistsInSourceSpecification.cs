using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Specifications
{
    public class NumberExistsInSourceSpecification : IVerifySpecification
    {
        public bool IsSatisfiedBy(INumberResults numberResults)
        {
            return numberResults.SourceNumbers.Count > 0;
        }
    }
}
