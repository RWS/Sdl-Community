using Sdl.Terminology.TerminologyProvider.Core;

namespace ExcelTerminology.Model
{
	public interface IExcelEntry : IEntry
    {
        string SearchText { get; set; }
    }
}