using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Model
{
	public interface IExcelEntry : IEntry
    {
        string SearchText { get; set; }
    }
}