using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ExcelTerminology.Services
{
    public interface IParser
    {
         IList<string> Parse(string term);
    }
}
