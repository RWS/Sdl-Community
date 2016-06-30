using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Interfaces
{
    public interface INumberVerifierMethods
    {
        string OmitZero(string number);
        string NormalizeNumberNoSeparator(string decimalSeparators, string thousandSeparators, string normalizedNumber);
    }
}
