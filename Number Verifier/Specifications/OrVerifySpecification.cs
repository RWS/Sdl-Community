using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Specifications
{
    public class OrVerifySpecification : IVerifySpecification
    {
        public IVerifySpecification[] Specifications;
        public bool IsSatisfiedBy(INumberResults numberResults)
        {
            return this.Specifications.Any(s => s.IsSatisfiedBy(numberResults));
        }
    }
}
