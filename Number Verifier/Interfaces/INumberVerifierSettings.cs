using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Core.Settings;

namespace Sdl.Community.NumberVerifier.Interfaces
{
    public interface INumberVerifierSettings
    {
        void Reset(string propertyName);
        bool ExcludeTagText { get; set; }
        bool ReportAddedNumbers { get; set; }
        bool ReportRemovedNumbers { get; set; }
        bool ReportModifiedNumbers { get; set; }
        bool ReportModifiedAlphanumerics { get; set; }
		bool CustomsSeparatorsAlphanumerics { get; set; }
		bool HindiNumberVerification { get; set; }
		string AddedNumbersErrorType { get; set; }
        string RemovedNumbersErrorType { get; set; }
        string ModifiedNumbersErrorType { get; set; }
        string ModifiedAlphanumericsErrorType { get; set; }
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
        bool SourceThousandsCustomSeparator { get; set; }
        bool TargetThousandsCustomSeparator { get; set; }
        bool TargetDecimalCustomSeparator { get; set; }
        bool SourceDecimalCustomSeparator { get; set; }
        bool AllowLocalizations { get; set; }
        string GetSourceThousandsCustomSeparator { get; set; }
        string GetTargetThousandsCustomSeparator { get; set; }
        string GetSourceDecimalCustomSeparator { get; set; }
        string GetTargetDecimalCustomSeparator { get; set; }
		string GetAlphanumericsCustomSeparator { get; set; }
		string GetHindi { get; set; }
		List<TargetFileSetting> TargetFileSettings { get; set; }

		IEnumerable<string> GetSourceDecimalSeparators();
        IEnumerable<string> GetSourceThousandSeparators();

        IEnumerable<string> GetTargetDecimalSeparators();
        IEnumerable<string> GetTargetThousandSeparators();
		string GetAlphanumericCustomSeparator();
		string GetHindiNumber();
	}
}