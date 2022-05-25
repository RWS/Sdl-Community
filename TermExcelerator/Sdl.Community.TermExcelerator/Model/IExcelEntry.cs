using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Model
{
	public interface IExcelEntry : IEntry
    {
        string SearchText { get; set; }
    }
}