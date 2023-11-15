using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Model
{
	public class ExcelEntry : Entry
	{
		public bool IsDirty { get; set; }
		public string SearchText { get; set; }
	}
}