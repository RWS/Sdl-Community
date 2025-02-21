using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Specifications
{
    public class OrVerifySettingsSpecification: IExtractSpecification
    {
        public IExtractSpecification[] Specifications;

        public bool IsSatisfiedBy(IExtractData numberExtractResults)
        {
            return Specifications.Any(s => s.IsSatisfiedBy(numberExtractResults));
        }
    }
}
