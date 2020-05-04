using System.Collections.Generic;

namespace Sdl.Community.TermExcelerator.Services.Interfaces
{
	public interface IParser
    {
         IList<string> Parse(string term);
    }
}