using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Specifications
{
    public class ReportExtendedMessageSpecification:IVerifySpecification
    {
        public bool IsSatisfiedBy(INumberResults numberResults)
        {
            return numberResults.Settings.ReportExtendedMessages;
        }
    }
}
