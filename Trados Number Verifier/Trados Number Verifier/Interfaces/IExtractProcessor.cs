using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Interfaces
{
    public interface IExtractProcessor
    {
        IEnumerable<string> Extract(IExtractData extractData);
    }
}
