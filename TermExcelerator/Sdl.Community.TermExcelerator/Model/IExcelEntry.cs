using System;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Model
{
	public interface IExcelEntry : IEntry, IEquatable<ExcelEntry>
	{
		string SearchText { get; set; }
	}
}