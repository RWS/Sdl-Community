using Sdl.Community.NumberVerifier.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Services
{
    public class NumberVerifierProfileManager
    {
        private readonly NumberVerifierSettingsImporter _importer;
        private readonly NumberVerifierSettingsExporter _exporter;

        public NumberVerifierProfileManager(NumberVerifierSettingsImporter importer, NumberVerifierSettingsExporter exporter)
        {
            _importer = importer ?? throw new ArgumentNullException(nameof(importer));
            _exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
        }

        public NumberVerifierSettingsImporter Importer => _importer;

        public NumberVerifierSettingsExporter Exporter => _exporter;

        public bool ProfileHasChanged(NumberVerifierSettings settings)
        {
            if (string.IsNullOrEmpty(settings.ProfilePath)) return false;

            try
            {
                var settingsDto = _importer.ImportSettings(settings.ProfilePath);
                return settingsDto == null || !SettingsAreEqual(settingsDto, settings);
            }
            catch
            {
                return true;
            }
        }

        private bool SettingsAreEqual(NumberVerifierSettingsDTO a, NumberVerifierSettings b)
        {
            if (a == null || b == null) return false;

            return a.CheckInOrder == b.CheckInOrder &&
                   a.ExcludeTagText == b.ExcludeTagText &&
                   a.ReportAddedNumbers == b.ReportAddedNumbers &&
                   a.ReportRemovedNumbers == b.ReportRemovedNumbers &&
                   a.ReportModifiedNumbers == b.ReportModifiedNumbers &&
                   a.ReportModifiedAlphanumerics == b.ReportModifiedAlphanumerics &&
                   a.ReportNumberFormatErrors == b.ReportNumberFormatErrors &&
                   a.CustomsSeparatorsAlphanumerics == b.CustomsSeparatorsAlphanumerics &&
                   a.HindiNumberVerification == b.HindiNumberVerification &&
                   a.AddedNumbersErrorType == b.AddedNumbersErrorType &&
                   a.RemovedNumbersErrorType == b.RemovedNumbersErrorType &&
                   a.ModifiedNumbersErrorType == b.ModifiedNumbersErrorType &&
                   a.ModifiedAlphanumericsErrorType == b.ModifiedAlphanumericsErrorType &&
                   a.NumberFormatErrorType == b.NumberFormatErrorType &&
                   a.ReportBriefMessages == b.ReportBriefMessages &&
                   a.ReportExtendedMessages == b.ReportExtendedMessages &&
                   a.AllowLocalizations == b.AllowLocalizations &&
                   a.PreventLocalizations == b.PreventLocalizations &&
                   a.RequireLocalizations == b.RequireLocalizations &&
                   a.SourceThousandsSpace == b.SourceThousandsSpace &&
                   a.SourceThousandsNobreakSpace == b.SourceThousandsNobreakSpace &&
                   a.SourceThousandsThinSpace == b.SourceThousandsThinSpace &&
                   a.SourceThousandsNobreakThinSpace == b.SourceThousandsNobreakThinSpace &&
                   a.SourceThousandsComma == b.SourceThousandsComma &&
                   a.SourceThousandsPeriod == b.SourceThousandsPeriod &&
                   a.SourceNoSeparator == b.SourceNoSeparator &&
                   a.TargetThousandsSpace == b.TargetThousandsSpace &&
                   a.TargetThousandsNobreakSpace == b.TargetThousandsNobreakSpace &&
                   a.TargetThousandsThinSpace == b.TargetThousandsThinSpace &&
                   a.TargetThousandsNobreakThinSpace == b.TargetThousandsNobreakThinSpace &&
                   a.TargetThousandsComma == b.TargetThousandsComma &&
                   a.TargetThousandsPeriod == b.TargetThousandsPeriod &&
                   a.TargetNoSeparator == b.TargetNoSeparator &&
                   a.SourceDecimalComma == b.SourceDecimalComma &&
                   a.SourceDecimalPeriod == b.SourceDecimalPeriod &&
                   a.TargetDecimalComma == b.TargetDecimalComma &&
                   a.TargetDecimalPeriod == b.TargetDecimalPeriod &&
                   a.ExcludeLockedSegments == b.ExcludeLockedSegments &&
                   a.Exclude100Percents == b.Exclude100Percents &&
                   a.ExcludeUntranslatedSegments == b.ExcludeUntranslatedSegments &&
                   a.ExcludeDraftSegments == b.ExcludeDraftSegments &&
                   a.SourceOmitLeadingZero == b.SourceOmitLeadingZero &&
                   a.TargetOmitLeadingZero == b.TargetOmitLeadingZero &&
                   a.SourceThousandsCustom == b.SourceThousandsCustom &&
                   a.TargetThousandsCustom == b.TargetThousandsCustom &&
                   a.SourceDecimalCustom == b.SourceDecimalCustom &&
                   a.TargetDecimalCustom == b.TargetDecimalCustom &&
                   a.SourceThousandsCustomSeparator == b.SourceThousandsCustomSeparator &&
                   a.TargetThousandsCustomSeparator == b.TargetThousandsCustomSeparator &&
                   a.SourceDecimalCustomSeparator == b.SourceDecimalCustomSeparator &&
                   a.TargetDecimalCustomSeparator == b.TargetDecimalCustomSeparator &&
                   a.AlphanumericsCustomSeparator == b.AlphanumericsCustomSeparator &&
                   a.HindiNumber == b.HindiNumber &&
                   ListsAreEqual(a.RegexExclusionList, b.RegexExclusionList) &&
                   ListsAreEqual(a.SourceExcludedRanges, b.SourceExcludedRanges) &&
                   ListsAreEqual(a.TargetExcludedRanges, b.TargetExcludedRanges);
        }

        private bool ListsAreEqual<T>(List<T> list1, List<T> list2)
        {
            return list1?.SequenceEqual(list2 ?? new List<T>()) ?? list2 == null;
        }
    }

}
