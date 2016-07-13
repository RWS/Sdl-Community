using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Interfaces
{
    public interface INumberResults
    {
        INumberVerifierSettings Settings { get; set; }
        List<string> SourceNumbers { get; set; }
        List<string> TargetNumbers { get; set; }

        string SourceText { get; set; }
        string  TargetText { get; set; }

    }
}
