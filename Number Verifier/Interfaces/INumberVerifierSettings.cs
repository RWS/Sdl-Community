using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Interfaces
{
	public interface INumberVerifierSettings
	{
		void Reset(string propertyName);

		bool CheckInOrder { get; set; }

		bool ExcludeTagText { get; set; }

		bool ReportAddedNumbers { get; set; }

		bool ReportRemovedNumbers { get; set; }

		bool ReportModifiedNumbers { get; set; }

		bool ReportModifiedAlphanumerics { get; set; }

		bool ReportNumberFormatErrors { get; set; }

		bool CustomsSeparatorsAlphanumerics { get; set; }

		bool HindiNumberVerification { get; set; }

		string AddedNumbersErrorType { get; set; }

		string RemovedNumbersErrorType { get; set; }

		string ModifiedNumbersErrorType { get; set; }

		string ModifiedAlphanumericsErrorType { get; set; }

		string NumberFormatErrorType { get; set; }

		bool ReportBriefMessages { get; set; }

		bool ReportExtendedMessages { get; set; }

		bool PreventLocalizations { get; set; }

		bool RequireLocalizations { get; set; }

		bool SourceThousandsSpace { get; set; }

		bool SourceThousandsThinSpace { get; set; }

		bool SourceThousandsNobreakSpace { get; set; }

		bool SourceThousandsNobreakThinSpace { get; set; }

		bool SourceThousandsComma { get; set; }

		bool SourceThousandsPeriod { get; set; }

		bool SourceNoSeparator { get; set; }

		bool TargetThousandsSpace { get; set; }

		bool TargetThousandsNobreakSpace { get; set; }

		bool TargetThousandsThinSpace { get; set; }

		bool TargetThousandsNobreakThinSpace { get; set; }

		bool TargetThousandsComma { get; set; }

		bool TargetNoSeparator { get; set; }

		bool SourceDecimalComma { get; set; }

		bool SourceDecimalPeriod { get; set; }

		bool TargetThousandsPeriod { get; set; }

		bool TargetDecimalComma { get; set; }

		bool TargetDecimalPeriod { get; set; }

		bool ExcludeLockedSegments { get; set; }

		bool Exclude100Percents { get; set; }

		bool ExcludeUntranslatedSegments { get; set; }

		bool ExcludeDraftSegments { get; set; }

		bool SourceOmitLeadingZero { get; set; }

		bool TargetOmitLeadingZero { get; set; }

		bool SourceThousandsCustom { get; set; }

		bool TargetThousandsCustom { get; set; }

		bool TargetDecimalCustom { get; set; }

		bool SourceDecimalCustom { get; set; }

		bool AllowLocalizations { get; set; }

		string SourceThousandsCustomSeparator { get; set; }

		string TargetThousandsCustomSeparator { get; set; }

		string SourceDecimalCustomSeparator { get; set; }

		string TargetDecimalCustomSeparator { get; set; }

		string AlphanumericsCustomSeparator { get; set; }

		string HindiNumber { get; set; }
		List<RegexPattern> RegexExclusionList { get; set; }

		IEnumerable<string> GetSourceDecimalSeparators();

		IEnumerable<string> GetSourceThousandSeparators();

		IEnumerable<string> GetTargetDecimalSeparators();

		IEnumerable<string> GetTargetThousandSeparators();

		string GetAlphanumericCustomSeparator();

		string GetHindiNumber();

		string GetSourceDecimalCommaSeparator();

		string GetSourceDecimalPeriodSeparator();

		string GetSourceDecimalCustomSeparator();

		string GetSourceThousandsSpaceSeparator();

		string GetSourceThousandsNobreakSpaceSeparator();

		string GetSourceThousandsThinSpaceSeparator();

		string GetSourceThousandsNobreakThinSpaceSeparator();

		string GetSourceThousandsCommaSeparator();

		string GetSourceThousandsPeriodSeparator();

		string GetSourceThousandsCustomSeparator();

		string GetTargetDecimalCommaSeparator();

		string GetTargetDecimalPeriodSeparator();

		string GetTargetDecimalCustomSeparator();

		string GetTargetThousandsSpaceSeparator();

		string GetTargetThousandsNobreakSpaceSeparator();

		string GetTargetThousandsThinSpaceSeparator();

		string GetTargetThousandsNobreakThinSpaceSeparator();

		string GetTargetThousandsCommaSeparator();

		string GetTargetThousandsPeriodSeparator();

		string GetTargetThousandsCustomSeparator();
	}
}