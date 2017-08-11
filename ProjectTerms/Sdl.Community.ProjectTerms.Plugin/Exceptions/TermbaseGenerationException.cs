using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ProjectTerms.Plugin.Exceptions
{
    public class TermbaseGenerationException : Exception
    {
        public TermbaseGenerationException(string message) : base(message)
        {
        }
    }
}
