using Sdl.Community.NumberVerifier.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.NumberVerifier.Extensions
{
    public static class NumberVerifierExtensions
    {
        public static NumberVerifierSettingsDTO ToSettingsDTO(this NumberVerifierSettings settings)
        {
            return new NumberVerifierSettingsDTO()
            {
                CheckInOrder = settings.CheckInOrder,
                RegexExclusionList = settings.RegexExclusionList,
                SourceExcludedRanges = settings.SourceExcludedRanges,
                TargetExcludedRanges = settings.TargetExcludedRanges,

                ExcludeTagText = settings.ExcludeTagText,
                ReportAddedNumbers = settings.ReportAddedNumbers,
                ReportRemovedNumbers = settings.ReportRemovedNumbers,
                ReportModifiedNumbers = settings.ReportModifiedNumbers,
                ReportModifiedAlphanumerics = settings.ReportModifiedAlphanumerics,
                ReportNumberFormatErrors = settings.ReportNumberFormatErrors,
                CustomsSeparatorsAlphanumerics = settings.CustomsSeparatorsAlphanumerics,
                HindiNumberVerification = settings.HindiNumberVerification,

                AddedNumbersErrorType = settings.AddedNumbersErrorType,
                RemovedNumbersErrorType = settings.RemovedNumbersErrorType,
                ModifiedNumbersErrorType = settings.ModifiedNumbersErrorType,
                ModifiedAlphanumericsErrorType = settings.ModifiedAlphanumericsErrorType,
                NumberFormatErrorType = settings.NumberFormatErrorType,

                ReportBriefMessages = settings.ReportBriefMessages,
                ReportExtendedMessages = settings.ReportExtendedMessages,
                AllowLocalizations = settings.AllowLocalizations,
                PreventLocalizations = settings.PreventLocalizations,
                RequireLocalizations = settings.RequireLocalizations,

                SourceThousandsSpace = settings.SourceThousandsSpace,
                SourceThousandsNobreakSpace = settings.SourceThousandsNobreakSpace,
                SourceThousandsThinSpace = settings.SourceThousandsThinSpace,
                SourceThousandsNobreakThinSpace = settings.SourceThousandsNobreakThinSpace,
                SourceThousandsComma = settings.SourceThousandsComma,
                SourceThousandsPeriod = settings.SourceThousandsPeriod,
                SourceNoSeparator = settings.SourceNoSeparator,

                TargetThousandsSpace = settings.TargetThousandsSpace,
                TargetThousandsNobreakSpace = settings.TargetThousandsNobreakSpace,
                TargetThousandsThinSpace = settings.TargetThousandsThinSpace,
                TargetThousandsNobreakThinSpace = settings.TargetThousandsNobreakThinSpace,
                TargetThousandsComma = settings.TargetThousandsComma,
                TargetThousandsPeriod = settings.TargetThousandsPeriod,
                TargetNoSeparator = settings.TargetNoSeparator,

                SourceDecimalComma = settings.SourceDecimalComma,
                SourceDecimalPeriod = settings.SourceDecimalPeriod,
                TargetDecimalComma = settings.TargetDecimalComma,
                TargetDecimalPeriod = settings.TargetDecimalPeriod,

                ExcludeLockedSegments = settings.ExcludeLockedSegments,
                Exclude100Percents = settings.Exclude100Percents,
                ExcludeUntranslatedSegments = settings.ExcludeUntranslatedSegments,
                ExcludeDraftSegments = settings.ExcludeDraftSegments,

                SourceOmitLeadingZero = settings.SourceOmitLeadingZero,
                TargetOmitLeadingZero = settings.TargetOmitLeadingZero,

                SourceThousandsCustom = settings.SourceThousandsCustom,
                TargetThousandsCustom = settings.TargetThousandsCustom,
                SourceDecimalCustom = settings.SourceDecimalCustom,
                TargetDecimalCustom = settings.TargetDecimalCustom,

                SourceThousandsCustomSeparator = settings.SourceThousandsCustomSeparator,
                TargetThousandsCustomSeparator = settings.TargetThousandsCustomSeparator,
                SourceDecimalCustomSeparator = settings.SourceDecimalCustomSeparator,
                TargetDecimalCustomSeparator = settings.TargetDecimalCustomSeparator,
                AlphanumericsCustomSeparator = settings.AlphanumericsCustomSeparator,
                HindiNumber = settings.HindiNumber
            };
        }

        public static void OverwriteNumberVerifierSettings(this NumberVerifierSettingsDTO settings, NumberVerifierSettings existingSettings)
        {
            if (existingSettings is null || settings is null) return;

            existingSettings.CheckInOrder = settings.CheckInOrder;
            existingSettings.RegexExclusionList = settings.RegexExclusionList;
            existingSettings.SourceExcludedRanges = settings.SourceExcludedRanges;
            existingSettings.TargetExcludedRanges = settings.TargetExcludedRanges;

            existingSettings.ExcludeTagText = settings.ExcludeTagText;
            existingSettings.ReportAddedNumbers = settings.ReportAddedNumbers;
            existingSettings.ReportRemovedNumbers = settings.ReportRemovedNumbers;
            existingSettings.ReportModifiedNumbers = settings.ReportModifiedNumbers;
            existingSettings.ReportModifiedAlphanumerics = settings.ReportModifiedAlphanumerics;
            existingSettings.ReportNumberFormatErrors = settings.ReportNumberFormatErrors;
            existingSettings.CustomsSeparatorsAlphanumerics = settings.CustomsSeparatorsAlphanumerics;
            existingSettings.HindiNumberVerification = settings.HindiNumberVerification;

            existingSettings.AddedNumbersErrorType = settings.AddedNumbersErrorType;
            existingSettings.RemovedNumbersErrorType = settings.RemovedNumbersErrorType;
            existingSettings.ModifiedNumbersErrorType = settings.ModifiedNumbersErrorType;
            existingSettings.ModifiedAlphanumericsErrorType = settings.ModifiedAlphanumericsErrorType;
            existingSettings.NumberFormatErrorType = settings.NumberFormatErrorType;

            existingSettings.ReportBriefMessages = settings.ReportBriefMessages;
            existingSettings.ReportExtendedMessages = settings.ReportExtendedMessages;
            existingSettings.AllowLocalizations = settings.AllowLocalizations;
            existingSettings.PreventLocalizations = settings.PreventLocalizations;
            existingSettings.RequireLocalizations = settings.RequireLocalizations;

            existingSettings.SourceThousandsSpace = settings.SourceThousandsSpace;
            existingSettings.SourceThousandsNobreakSpace = settings.SourceThousandsNobreakSpace;
            existingSettings.SourceThousandsThinSpace = settings.SourceThousandsThinSpace;
            existingSettings.SourceThousandsNobreakThinSpace = settings.SourceThousandsNobreakThinSpace;
            existingSettings.SourceThousandsComma = settings.SourceThousandsComma;
            existingSettings.SourceThousandsPeriod = settings.SourceThousandsPeriod;
            existingSettings.SourceNoSeparator = settings.SourceNoSeparator;

            existingSettings.TargetThousandsSpace = settings.TargetThousandsSpace;
            existingSettings.TargetThousandsNobreakSpace = settings.TargetThousandsNobreakSpace;
            existingSettings.TargetThousandsThinSpace = settings.TargetThousandsThinSpace;
            existingSettings.TargetThousandsNobreakThinSpace = settings.TargetThousandsNobreakThinSpace;
            existingSettings.TargetThousandsComma = settings.TargetThousandsComma;
            existingSettings.TargetThousandsPeriod = settings.TargetThousandsPeriod;
            existingSettings.TargetNoSeparator = settings.TargetNoSeparator;

            existingSettings.SourceDecimalComma = settings.SourceDecimalComma;
            existingSettings.SourceDecimalPeriod = settings.SourceDecimalPeriod;
            existingSettings.TargetDecimalComma = settings.TargetDecimalComma;
            existingSettings.TargetDecimalPeriod = settings.TargetDecimalPeriod;

            existingSettings.ExcludeLockedSegments = settings.ExcludeLockedSegments;
            existingSettings.Exclude100Percents = settings.Exclude100Percents;
            existingSettings.ExcludeUntranslatedSegments = settings.ExcludeUntranslatedSegments;
            existingSettings.ExcludeDraftSegments = settings.ExcludeDraftSegments;

            existingSettings.SourceOmitLeadingZero = settings.SourceOmitLeadingZero;
            existingSettings.TargetOmitLeadingZero = settings.TargetOmitLeadingZero;

            existingSettings.SourceThousandsCustom = settings.SourceThousandsCustom;
            existingSettings.TargetThousandsCustom = settings.TargetThousandsCustom;
            existingSettings.SourceDecimalCustom = settings.SourceDecimalCustom;
            existingSettings.TargetDecimalCustom = settings.TargetDecimalCustom;

            existingSettings.SourceThousandsCustomSeparator = settings.SourceThousandsCustomSeparator;
            existingSettings.TargetThousandsCustomSeparator = settings.TargetThousandsCustomSeparator;
            existingSettings.SourceDecimalCustomSeparator = settings.SourceDecimalCustomSeparator;
            existingSettings.TargetDecimalCustomSeparator = settings.TargetDecimalCustomSeparator;
            existingSettings.AlphanumericsCustomSeparator = settings.AlphanumericsCustomSeparator;
            existingSettings.HindiNumber = settings.HindiNumber;
        }
    }
}
