using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Interfaces
{
    public interface INumberVerifierSeparators
    {
        IEnumerable<string> ReturnSeparators(IExtractProcessor separators);
    }
}
