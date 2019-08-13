using System.Collections.Generic;

namespace Sdl.Community.ExcelTerminology.Services.Interfaces
{
	public interface IParser
    {
         IList<string> Parse(string term);
    }
}