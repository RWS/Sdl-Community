using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Interfaces
{
    public interface IExtractData
    {
        INumberVerifierSettings Settings { get; set; }
        IEnumerable<string> ExtractList { get; set; } 
        //List<string> NumberCollection { get; set; }
        //List<string> NormalizedNumberCollection { get; set; }

        //string Text { get; set; }
        //string Separators { get; set; }
    }
}
