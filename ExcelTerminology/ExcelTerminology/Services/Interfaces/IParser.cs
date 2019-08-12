using System.Collections.Generic;

namespace ExcelTerminology.Services.Interfaces
{
	public interface IParser
    {
         IList<string> Parse(string term);
    }
}