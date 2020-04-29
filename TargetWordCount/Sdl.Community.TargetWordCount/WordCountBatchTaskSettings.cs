using System.ComponentModel;
using Sdl.Community.TargetWordCount.Models;
using Sdl.Core.Settings;

namespace Sdl.Community.TargetWordCount
{
	public class WordCountBatchTaskSettings : SettingsGroup, IWordCountBatchTaskSettings
	{
		public string Culture
		{
			get { return GetSetting<string>(nameof(Culture)); }
			set { GetSetting<string>(nameof(Culture)).Value = value; }
		}

		public bool IncludeSpaces
		{
			get { return GetSetting<bool>(nameof(IncludeSpaces)); }
			set { GetSetting<bool>(nameof(IncludeSpaces)).Value = value; }
		}

		public BindingList<InvoiceItem> InvoiceRates
		{
			get { return GetSetting<BindingList<InvoiceItem>>(nameof(InvoiceRates)); }
			set { GetSetting<BindingList<InvoiceItem>>(nameof(InvoiceRates)).Value = value; }
		}

		public bool ReportLockedSeperately
		{
			get { return GetSetting<bool>(nameof(ReportLockedSeperately)); }
			set { GetSetting<bool>(nameof(ReportLockedSeperately)).Value = value; }
		}

		public bool UseLineCount
		{
			get { return GetSetting<bool>(nameof(UseLineCount)); }
			set { GetSetting<bool>(nameof(UseLineCount)).Value = value; }
		}

		public bool UseSource
		{
			get { return GetSetting<bool>(nameof(UseSource)); }
			set { GetSetting<bool>(nameof(UseSource)).Value = value; }
		}

		public string CharactersPerLine
		{
			get { return GetSetting<string>(nameof(CharactersPerLine)); }
			set { GetSetting<string>(nameof(CharactersPerLine)).Value = value; }
		}

		protected override object GetDefaultValue(string settingId)
		{
			switch (settingId)
			{
				case nameof(Culture):
					return "English";

				case nameof(ReportLockedSeperately):
					return true;

				case nameof(UseSource):
					return false;

				case nameof(InvoiceRates):
					return new BindingList<InvoiceItem>()
					{
						new InvoiceItem(RateType.Locked, string.Empty),
						new InvoiceItem(RateType.PerfectMatch, string.Empty),
						new InvoiceItem(RateType.ContextMatch, string.Empty),
						new InvoiceItem(RateType.Repetitions, string.Empty),
						new InvoiceItem(RateType.CrossFileRepetitions, string.Empty),
						new InvoiceItem(RateType.OneHundred, string.Empty),
						new InvoiceItem(RateType.NinetyFive, string.Empty),
						new InvoiceItem(RateType.EightyFive, string.Empty),
						new InvoiceItem(RateType.SeventyFive, string.Empty),
						new InvoiceItem(RateType.Fifty, string.Empty),
						new InvoiceItem(RateType.New, string.Empty),
						new InvoiceItem(RateType.Total, string.Empty)
					};

				case nameof(IncludeSpaces):
					return false;

				case nameof(UseLineCount):
					return false;

				case nameof(CharactersPerLine):
					return string.Empty;
			}

			return base.GetDefaultValue(settingId);
		}
	}
}