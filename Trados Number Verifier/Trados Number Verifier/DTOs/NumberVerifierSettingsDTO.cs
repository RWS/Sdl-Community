using Sdl.Community.NumberVerifier.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.DTOs
{
    public class NumberVerifierSettingsDTO
    {
        public bool CheckInOrder { get; set; }
        public List<RegexPattern> RegexExclusionList { get; set; }
        public List<ExcludedRange> SourceExcludedRanges { get; set; }
        public List<ExcludedRange> TargetExcludedRanges { get; set; }

        public bool ExcludeTagText { get; set; }
        public bool ReportAddedNumbers { get; set; }
        public bool ReportRemovedNumbers { get; set; }
        public bool ReportModifiedNumbers { get; set; }
        public bool ReportModifiedAlphanumerics { get; set; }
        public bool ReportNumberFormatErrors { get; set; }
        public bool CustomsSeparatorsAlphanumerics { get; set; }
        public bool HindiNumberVerification { get; set; }

        public string AddedNumbersErrorType { get; set; }
        public string RemovedNumbersErrorType { get; set; }
        public string ModifiedNumbersErrorType { get; set; }
        public string ModifiedAlphanumericsErrorType { get; set; }
        public string NumberFormatErrorType { get; set; }

        public bool ReportBriefMessages { get; set; }
        public bool ReportExtendedMessages { get; set; }
        public bool AllowLocalizations { get; set; }
        public bool PreventLocalizations { get; set; }
        public bool RequireLocalizations { get; set; }

        public bool SourceThousandsSpace { get; set; }
        public bool SourceThousandsNobreakSpace { get; set; }
        public bool SourceThousandsThinSpace { get; set; }
        public bool SourceThousandsNobreakThinSpace { get; set; }
        public bool SourceThousandsComma { get; set; }
        public bool SourceThousandsPeriod { get; set; }
        public bool SourceNoSeparator { get; set; }

        public bool TargetThousandsSpace { get; set; }
        public bool TargetThousandsNobreakSpace { get; set; }
        public bool TargetThousandsThinSpace { get; set; }
        public bool TargetThousandsNobreakThinSpace { get; set; }
        public bool TargetThousandsComma { get; set; }
        public bool TargetThousandsPeriod { get; set; }
        public bool TargetNoSeparator { get; set; }

        public bool SourceDecimalComma { get; set; }
        public bool SourceDecimalPeriod { get; set; }
        public bool TargetDecimalComma { get; set; }
        public bool TargetDecimalPeriod { get; set; }

        public bool ExcludeLockedSegments { get; set; }
        public bool Exclude100Percents { get; set; }
        public bool ExcludeUntranslatedSegments { get; set; }
        public bool ExcludeDraftSegments { get; set; }

        public bool SourceOmitLeadingZero { get; set; }
        public bool TargetOmitLeadingZero { get; set; }

        public bool SourceThousandsCustom { get; set; }
        public bool TargetThousandsCustom { get; set; }
        public bool SourceDecimalCustom { get; set; }
        public bool TargetDecimalCustom { get; set; }

        public string SourceThousandsCustomSeparator { get; set; }
        public string TargetThousandsCustomSeparator { get; set; }
        public string SourceDecimalCustomSeparator { get; set; }
        public string TargetDecimalCustomSeparator { get; set; }
        public string AlphanumericsCustomSeparator { get; set; }
        public string HindiNumber { get; set; }
    }
}
