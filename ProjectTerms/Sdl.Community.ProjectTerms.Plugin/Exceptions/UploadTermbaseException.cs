using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ProjectTerms.Plugin.Exceptions
{
    public class UploadTermbaseException : Exception
    {
        public UploadTermbaseException(string message) : base(message)
        {
        }
    }
}
