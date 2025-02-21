using Sdl.Community.NumberVerifier.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Interfaces
{
    public interface IVerifyProcessor
    {
        IEnumerable<ErrorReporting> Verify(INumberResults numberResults);
    }
}
