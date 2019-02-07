using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sdl.Community.TargetWordCount.Models;

namespace Sdl.Community.TargetWordCount.Contracts
{
	[ContractClassFor(typeof(IWordCountBatchTaskSettings))]
	internal abstract class IWordCountBatchTaskSettingsContract : IWordCountBatchTaskSettings
	{
		public string Culture
		{
			get
			{
				Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
				return default(string);
			}

			set
			{
				Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(value));
			}
		}

		public bool IncludeSpaces { get; set; }
		public List<InvoiceItem> InvoiceRates
		{
			get
			{
				Contract.Ensures(Contract.Result<List<InvoiceItem>>() != null);
				return default(List<InvoiceItem>);
			}

			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
			}
		}

		public bool ReportLockedSeperately { get; set; }

		public bool UseLineCount { get; set; }

		public bool UseSource { get; set; }

		public string CharactersPerLine
		{
			get
			{
				Contract.Ensures(Contract.Result<string>() != null);
				return default(string);
			}

			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
			}
		}
	}
}