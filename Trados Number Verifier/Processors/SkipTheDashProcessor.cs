using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Processors
{
    public class SkipTheDashProcessor:ISkipTheDashProcessor
    {
        public string Skip(string text)
        {
            return text.Substring(2);

        }
    }
}
