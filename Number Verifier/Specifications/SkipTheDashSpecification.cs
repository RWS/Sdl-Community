using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Specifications
{
    public class SkipTheDashSpecification//:IExtractSpecification
    {
       
        public bool IsSatisfiedBy(IExtractData numberExtractResults)
        {
            //char[] dashSign = { '-', '\u2013', '\u2212' };
            //char[] space = { ' ', '\u00a0', '\u2009', '\u202F' };

            //if (numberExtractResults.Text.IndexOfAny(dashSign) == 0 &&
            //    numberExtractResults.Text.IndexOfAny(space) == 1)
            //{
            //    return true;
            //}

            return false;
        }
    }
}
