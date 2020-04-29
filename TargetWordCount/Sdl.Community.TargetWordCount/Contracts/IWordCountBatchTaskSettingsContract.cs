using System.ComponentModel;
using Sdl.Community.TargetWordCount.Models;

namespace Sdl.Community.TargetWordCount.Contracts
{
	internal abstract class IWordCountBatchTaskSettingsContract : IWordCountBatchTaskSettings
	{
		public string Culture { get; set; }
		public bool IncludeSpaces { get; set; }
		public BindingList<InvoiceItem> InvoiceRates { get; set; }		
		public bool ReportLockedSeperately { get; set; }
		public bool UseLineCount { get; set; }
		public bool UseSource { get; set; }
		public string CharactersPerLine { get; set; }
	}
}