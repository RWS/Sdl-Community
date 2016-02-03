using System.Collections.Generic;
using Sdl.Community.ExcelTerminology.Model;

namespace Sdl.Community.ExcelTerminology.Services.Interfaces
{
    public interface IExcelTermLoaderService
    {
        Dictionary<int, ExcelTerm> LoadTerms();
    }
}