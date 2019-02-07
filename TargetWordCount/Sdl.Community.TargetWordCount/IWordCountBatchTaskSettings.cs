using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sdl.Community.TargetWordCount.Contracts;
using Sdl.Community.TargetWordCount.Models;

namespace Sdl.Community.TargetWordCount
{
	[ContractClass(typeof(IWordCountBatchTaskSettingsContract))]
	public interface IWordCountBatchTaskSettings
	{
		string Culture { get; set; }
		bool IncludeSpaces { get; set; }
		List<InvoiceItem> InvoiceRates { get; set; }
		bool ReportLockedSeperately { get; set; }
		bool UseLineCount { get; set; }
		bool UseSource { get; set; }
		string CharactersPerLine { get; set; }
	}
}