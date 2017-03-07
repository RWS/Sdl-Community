using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.Community.PostEdit.Compare.Core.Comparison
{

    public partial class Comparer
    {
        public class FileUnitProperties
        {

            public string SourceLanguageIdOriginal { get; set; }
            public string TargetLanguageIdOriginal { get; set; }

            public string SourceLanguageIdUpdated { get; set; }
            public string TargetLanguageIdUpdated { get; set; }

            public string FilePathOriginal { get; set; }
            public string FilePathUpdated { get; set; }

            public int TotalSegmentsOriginal { get; set; }
            public int TotalSegmentsUpdated { get; set; }

            public int TotalContentChanges { get; set; }
            public decimal TotalContentChangesPercentage { get; private set; }

            public int TotalStatusChanges { get; set; }
            public decimal TotalStatusChangesPercentage { get; private set; }

            public int TotalComments { get; set; }

            public int TotalSegments { get; set; }

            public int TotalNotTranslatedOriginal { get; set; }
            public int TotalDraftOriginal { get; set; }
            public int TotalTranslatedOriginal { get; set; }
            public int TotalTranslationRejectedOriginal { get; set; }
            public int TotalTranslationApprovedOriginal { get; set; }
            public int TotalSignOffRejectedOriginal { get; set; }
            public int TotalSignedOffOriginal { get; set; }

            public int TotalNotTranslatedUpdated { get; set; }
            public int TotalDraftUpdated { get; set; }
            public int TotalTranslatedUpdated { get; set; }
            public int TotalTranslationRejectedUpdated { get; set; }
            public int TotalTranslationApprovedUpdated { get; set; }
            public int TotalSignOffRejectedUpdated { get; set; }
            public int TotalSignedOffUpdated { get; set; }

            public int TotalChangesPmSegments { get; set; }
            public int TotalChangesCmSegments { get; set; }
            public int TotalChangesExactSegments { get; set; }
            public int TotalChangesRepsSegments { get; set; }
            public int TotalChangesAtSegments { get; set; }
            public int TotalChangesFuzzy99Segments { get; set; }
            public int TotalChangesFuzzy94Segments { get; set; }
            public int TotalChangesFuzzy84Segments { get; set; }
            public int TotalChangesFuzzy74Segments { get; set; }
            public int TotalChangesNewSegments { get; set; }

            public int TotalChangesPmWords { get; set; }
            public int TotalChangesCmWords { get; set; }
            public int TotalChangesExactWords { get; set; }
            public int TotalChangesRepsWords { get; set; }
            public int TotalChangesAtWords { get; set; }
            public int TotalChangesFuzzy99Words { get; set; }
            public int TotalChangesFuzzy94Words { get; set; }
            public int TotalChangesFuzzy84Words { get; set; }
            public int TotalChangesFuzzy74Words { get; set; }
            public int TotalChangesNewWords { get; set; }



            public int TotalChangesPmCharacters { get; set; }
            public int TotalChangesCmCharacters { get; set; }
            public int TotalChangesExactCharacters { get; set; }
            public int TotalChangesRepsCharacters { get; set; }
            public int TotalChangesAtCharacters { get; set; }
            public int TotalChangesFuzzy99Characters { get; set; }
            public int TotalChangesFuzzy94Characters { get; set; }
            public int TotalChangesFuzzy84Characters { get; set; }
            public int TotalChangesFuzzy74Characters { get; set; }
            public int TotalChangesNewCharacters { get; set; }


            public int TotalSourcePmSegments { get; set; }
            public int TotalSourceCmSegment { get; set; }
            public int TotalSourceExactSegments { get; set; }
            public int TotalSourceRepsSegments { get; set; }
            public int TotalSourceAtSegments { get; set; }
            public int TotalSourceFuzzy99Segments { get; set; }
            public int TotalSourceFuzzy94Segments { get; set; }
            public int TotalSourceFuzzy84Segments { get; set; }
            public int TotalSourceFuzzy74Segments { get; set; }
            public int TotalSourceNewSegments { get; set; }


            public decimal TotalSourcePmWords { get; set; }
            public decimal TotalSourceCmWords { get; set; }
            public decimal TotalSourceExactWords { get; set; }
            public decimal TotalSourceRepsWords { get; set; }
            public decimal TotalSourceAtWords { get; set; }
            public decimal TotalSourceFuzzy99Words { get; set; }
            public decimal TotalSourceFuzzy94Words { get; set; }
            public decimal TotalSourceFuzzy84Words { get; set; }
            public decimal TotalSourceFuzzy74Words { get; set; }
            public decimal TotalSourceNewWords { get; set; }


            public decimal TotalSourcePmCharacters { get; set; }
            public decimal TotalSourceCmCharacters { get; set; }
            public decimal TotalSourceExactCharacters { get; set; }
            public decimal TotalSourceRepsCharacters { get; set; }
            public decimal TotalSourceAtCharacters { get; set; }
            public decimal TotalSourceFuzzy99Characters { get; set; }
            public decimal TotalSourceFuzzy94Characters { get; set; }
            public decimal TotalSourceFuzzy84Characters { get; set; }
            public decimal TotalSourceFuzzy74Characters { get; set; }
            public decimal TotalSourceNewCharacters { get; set; }


            public decimal TotalSourcePmTags { get; set; }
            public decimal TotalSourceCmTags { get; set; }
            public decimal TotalSourceExactTags { get; set; }
            public decimal TotalSourceRepsTags { get; set; }
            public decimal TotalSourceAtTags { get; set; }
            public decimal TotalSourceFuzzy99Tags { get; set; }
            public decimal TotalSourceFuzzy94Tags { get; set; }
            public decimal TotalSourceFuzzy84Tags { get; set; }
            public decimal TotalSourceFuzzy74Tags { get; set; }
            public decimal TotalSourceNewTags { get; set; }

            private Dictionary<string, Dictionary<string, ComparisonParagraphUnit>> ComparisonFileParagraphUnits { get; set; }
            public FileUnitProperties(FileUnitProperties fileUnitProperties, Dictionary<string, Dictionary<string, ComparisonParagraphUnit>> comparisonFileParagraphUnits)
            {
                SourceLanguageIdOriginal = fileUnitProperties.SourceLanguageIdOriginal;
                TargetLanguageIdOriginal = fileUnitProperties.TargetLanguageIdOriginal;

                SourceLanguageIdUpdated = fileUnitProperties.SourceLanguageIdUpdated;
                TargetLanguageIdUpdated = fileUnitProperties.TargetLanguageIdUpdated;

                FilePathOriginal = fileUnitProperties.FilePathOriginal;
                FilePathUpdated = fileUnitProperties.FilePathUpdated;

                TotalSegmentsOriginal = fileUnitProperties.TotalSegmentsOriginal;
                TotalSegmentsUpdated = fileUnitProperties.TotalSegmentsUpdated;

                ComparisonFileParagraphUnits = comparisonFileParagraphUnits;

                Process();
            }

            public FileUnitProperties(string filePathOriginal, string filePathUpdated
                , int totalSegmentsOriginal, int totalSegmentsUpdated, Dictionary<string, Dictionary<string, ComparisonParagraphUnit>> comparisonFileParagraphUnits
                , string sourceLanguageIdOriginal, string targetLanguageIdOriginal, string sourceLanguageIdUpdated, string targetLanguageIdUpdated)
            {
                SourceLanguageIdOriginal = sourceLanguageIdOriginal;
                TargetLanguageIdOriginal = targetLanguageIdOriginal;

                SourceLanguageIdUpdated = sourceLanguageIdUpdated;
                TargetLanguageIdUpdated = targetLanguageIdUpdated;

                FilePathOriginal = filePathOriginal;
                FilePathUpdated = filePathUpdated;

                TotalSegmentsOriginal = totalSegmentsOriginal;
                TotalSegmentsUpdated = totalSegmentsUpdated;

                ComparisonFileParagraphUnits = comparisonFileParagraphUnits;

                Process();
            }

            private void Process()
            {

                TotalContentChanges = 0;
                TotalContentChangesPercentage = 0;

                TotalStatusChanges = 0;
                TotalStatusChangesPercentage = 0;

                TotalComments = 0;

                TotalNotTranslatedOriginal = 0;
                TotalDraftOriginal = 0;
                TotalTranslatedOriginal = 0;
                TotalTranslationRejectedOriginal = 0;
                TotalTranslationApprovedOriginal = 0;
                TotalSignOffRejectedOriginal = 0;
                TotalSignedOffOriginal = 0;

                TotalNotTranslatedUpdated = 0;
                TotalDraftUpdated = 0;
                TotalTranslatedUpdated = 0;
                TotalTranslationRejectedUpdated = 0;
                TotalTranslationApprovedUpdated = 0;
                TotalSignOffRejectedUpdated = 0;
                TotalSignedOffUpdated = 0;

                TotalChangesPmSegments = 0;
                TotalChangesCmSegments = 0;
                TotalChangesExactSegments = 0;
                TotalChangesRepsSegments = 0;
                TotalChangesAtSegments = 0;
                TotalChangesFuzzy99Segments = 0;
                TotalChangesFuzzy94Segments = 0;
                TotalChangesFuzzy84Segments = 0;
                TotalChangesFuzzy74Segments = 0;
                TotalChangesNewSegments = 0;

                TotalSourcePmSegments = 0;
                TotalSourceCmSegment = 0;
                TotalSourceExactSegments = 0;
                TotalSourceRepsSegments = 0;
                TotalSourceAtSegments = 0;
                TotalSourceFuzzy99Segments = 0;
                TotalSourceFuzzy94Segments = 0;
                TotalSourceFuzzy84Segments = 0;
                TotalSourceFuzzy74Segments = 0;
                TotalSourceNewSegments = 0;

                TotalSourcePmWords = 0;
                TotalSourceCmWords = 0;
                TotalSourceExactWords = 0;
                TotalSourceAtWords = 0;
                TotalSourceFuzzy99Words = 0;
                TotalSourceFuzzy94Words = 0;
                TotalSourceFuzzy84Words = 0;
                TotalSourceFuzzy74Words = 0;
                TotalSourceNewWords = 0;

                TotalSourcePmCharacters = 0;
                TotalSourceCmCharacters = 0;
                TotalSourceExactCharacters = 0;
                TotalSourceRepsCharacters = 0;
                TotalSourceAtCharacters = 0;
                TotalSourceFuzzy99Characters = 0;
                TotalSourceFuzzy94Characters = 0;
                TotalSourceFuzzy84Characters = 0;
                TotalSourceFuzzy74Characters = 0;
                TotalSourceNewCharacters = 0;

                TotalSourcePmTags = 0;
                TotalSourceCmTags = 0;
                TotalSourceExactTags = 0;
                TotalSourceRepsTags = 0;
                TotalSourceAtTags = 0;
                TotalSourceFuzzy99Tags = 0;
                TotalSourceFuzzy94Tags = 0;
                TotalSourceFuzzy84Tags = 0;
                TotalSourceFuzzy74Tags = 0;
                TotalSourceNewTags = 0;

                if (ComparisonFileParagraphUnits != null)
                {
                    foreach (var comparisonFileParagraphUnitList in ComparisonFileParagraphUnits)
                    {
                        foreach (var comparisonFileParagraphUnit in comparisonFileParagraphUnitList.Value)
                        {
                            var comparisonParagraphUnit = comparisonFileParagraphUnit.Value;

                            foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                            {                               
                                TotalSegments++;


                                if (comparisonSegmentUnit.SegmentStatusOriginal == string.Empty)
                                    comparisonSegmentUnit.SegmentStatusOriginal = "Unspecified";
                                if (comparisonSegmentUnit.SegmentStatusUpdated == string.Empty)
                                    comparisonSegmentUnit.SegmentStatusUpdated = "Unspecified";


                                if (comparisonSegmentUnit.SegmentStatusOriginal == "Unspecified")
                                {
                                    TotalSourceNewSegments++;
                                    TotalSourceNewWords += comparisonSegmentUnit.SourceWordsOriginal;
                                    TotalSourceNewCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                    TotalSourceNewTags += comparisonSegmentUnit.SourceTagsOriginal;
                                }
                                else if (string.Compare(comparisonSegmentUnit.TranslationOriginTypeOriginal, "auto-propagated", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    TotalSourceRepsSegments++;
                                    TotalSourceRepsWords += comparisonSegmentUnit.SourceWordsOriginal;
                                    TotalSourceRepsCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                    TotalSourceRepsTags += comparisonSegmentUnit.SourceTagsOriginal;
                                }
                                else
                                {
                                    switch (comparisonSegmentUnit.TranslationStatusOriginal)
                                    {
                                        case "PM":
                                            {
                                                TotalSourcePmSegments++;
                                                TotalSourcePmWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                TotalSourcePmCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                                TotalSourcePmTags += comparisonSegmentUnit.SourceTagsOriginal;
                                            }
                                            break;
                                        case "CM":
                                            {
                                                TotalSourceCmSegment++;
                                                TotalSourceCmWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                TotalSourceCmCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                                TotalSourceCmTags += comparisonSegmentUnit.SourceTagsOriginal;
                                            }
                                            break;
                                        case "100%":
                                            {
                                                TotalSourceExactSegments++;
                                                TotalSourceExactWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                TotalSourceExactCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                                TotalSourceExactTags += comparisonSegmentUnit.SourceTagsOriginal;
                                            }
                                            break;
                                        case "AT":
                                            {
                                                TotalSourceAtSegments++;
                                                TotalSourceAtWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                TotalSourceAtCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                                TotalSourceAtTags += comparisonSegmentUnit.SourceTagsOriginal;
                                            }
                                            break;
                                        default:
                                            {
                                                var matchPercentage = GetMatchValue(comparisonSegmentUnit.TranslationStatusOriginal);

                                                if (matchPercentage >= 100)
                                                {
                                                    TotalSourceExactSegments++;
                                                    TotalSourceExactWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalSourceExactCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                                    TotalSourceExactTags += comparisonSegmentUnit.SourceTagsOriginal;
                                                }
                                                else if (matchPercentage >= 95 && matchPercentage < 100)
                                                {
                                                    TotalSourceFuzzy99Segments++;
                                                    TotalSourceFuzzy99Words += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalSourceFuzzy99Characters += comparisonSegmentUnit.SourceCharsOriginal;
                                                    TotalSourceFuzzy99Tags += comparisonSegmentUnit.SourceTagsOriginal;
                                                }
                                                else if (matchPercentage >= 85 && matchPercentage < 95)
                                                {
                                                    TotalSourceFuzzy94Segments++;
                                                    TotalSourceFuzzy94Words += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalSourceFuzzy94Characters += comparisonSegmentUnit.SourceCharsOriginal;
                                                    TotalSourceFuzzy94Tags += comparisonSegmentUnit.SourceTagsOriginal;
                                                }
                                                else if (matchPercentage >= 75 && matchPercentage < 85)
                                                {
                                                    TotalSourceFuzzy84Segments++;
                                                    TotalSourceFuzzy84Words += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalSourceFuzzy84Characters += comparisonSegmentUnit.SourceCharsOriginal;
                                                    TotalSourceFuzzy84Tags += comparisonSegmentUnit.SourceTagsOriginal;
                                                }
                                                else if (matchPercentage >= 50 && matchPercentage < 75)
                                                {
                                                    TotalSourceFuzzy74Segments++;
                                                    TotalSourceFuzzy74Words += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalSourceFuzzy74Characters += comparisonSegmentUnit.SourceCharsOriginal;
                                                    TotalSourceFuzzy74Tags += comparisonSegmentUnit.SourceTagsOriginal;
                                                }
                                                else
                                                {
                                                    TotalSourceNewSegments++;
                                                    TotalSourceNewWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalSourceNewCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                                    TotalSourceNewTags += comparisonSegmentUnit.SourceTagsOriginal;
                                                }                                
                                            }
                                            break;
                                    }

                                }

                                var filtered = true;
                                if (Processor.Settings.CalculateSummaryAnalysisBasedOnFilteredRows)
                                {
                                    if ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                                       && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                                       && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                       && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments))
                                        filtered = false;

                                    if (filtered)
                                    {
                                        if (((!comparisonSegmentUnit.SegmentIsLocked || !Processor.Settings.ReportFilterLockedSegments) && comparisonSegmentUnit.SegmentIsLocked)
                                           || !IsFilterSegmentMatchPercentage(Processor.Settings.ReportFilterTranslationMatchValuesOriginal, Processor.Settings.ReportFilterTranslationMatchValuesUpdated, comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated)
                                           || ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                                               && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                                               && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                               && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments)))
                                            filtered = false;
                                    }
                                }

                                if (!filtered)
                                    continue;

                                switch (comparisonSegmentUnit.SegmentStatusOriginal)
                                {
                                    case "Unspecified": TotalNotTranslatedOriginal++; break;
                                    case "Draft": TotalDraftOriginal++; break;
                                    case "Translated": TotalTranslatedOriginal++; break;
                                    case "RejectedTranslation": TotalTranslationRejectedOriginal++; break;
                                    case "ApprovedTranslation": TotalTranslationApprovedOriginal++; break;
                                    case "RejectedSignOff": TotalSignOffRejectedOriginal++; break;
                                    case "ApprovedSignOff": TotalSignedOffOriginal++; break;
                                }



                                switch (comparisonSegmentUnit.SegmentStatusUpdated)
                                {
                                    case "Unspecified": TotalNotTranslatedUpdated++; break;
                                    case "Draft": TotalDraftUpdated++; break;
                                    case "Translated": TotalTranslatedUpdated++; break;
                                    case "RejectedTranslation": TotalTranslationRejectedUpdated++; break;
                                    case "ApprovedTranslation": TotalTranslationApprovedUpdated++; break;
                                    case "RejectedSignOff": TotalSignOffRejectedUpdated++; break;
                                    case "ApprovedSignOff": TotalSignedOffUpdated++; break;
                                }

                                if (comparisonSegmentUnit.SegmentHasComments)
                                    TotalComments += comparisonSegmentUnit.Comments.Count;

                                if (comparisonSegmentUnit.SegmentTextUpdated)
                                    TotalContentChanges++;

                                if (comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                    TotalStatusChanges++;


                                if (!comparisonSegmentUnit.SegmentTextUpdated && !comparisonSegmentUnit.SegmentSegmentStatusUpdated && !comparisonSegmentUnit.SegmentHasComments)
                                    continue;
                                if (comparisonSegmentUnit.SegmentStatusOriginal == "Unspecified")
                                {
                                    TotalChangesNewSegments++;
                                    TotalChangesNewWords += comparisonSegmentUnit.SourceWordsOriginal;
                                    TotalChangesNewCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                }
                                else if (string.Compare(comparisonSegmentUnit.TranslationOriginTypeOriginal, "auto-propagated", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    TotalChangesRepsSegments++;
                                    TotalChangesRepsWords += comparisonSegmentUnit.SourceWordsOriginal;
                                    TotalChangesRepsCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                }
                                else
                                {
                                    switch (comparisonSegmentUnit.TranslationStatusOriginal)
                                    {
                                        case "PM":
                                            {
                                                TotalChangesPmSegments++;
                                                TotalChangesPmWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                TotalChangesPmCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                            }
                                            break;
                                        case "CM":
                                            {
                                                TotalChangesCmSegments++;
                                                TotalChangesCmWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                TotalChangesCmCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                            }
                                            break;
                                        case "100%":
                                            {
                                                TotalChangesExactSegments++;
                                                TotalChangesExactWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                TotalChangesExactCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                            }
                                            break;
                                        case "AT":
                                            {
                                                TotalChangesAtSegments++;
                                                TotalChangesAtWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                TotalChangesAtCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                            }
                                            break;
                                        default:
                                            {
                                                var matchPercentage = GetMatchValue(comparisonSegmentUnit.TranslationStatusOriginal);


                                                if (matchPercentage >= 100)
                                                {
                                                    TotalChangesExactSegments++;
                                                    TotalChangesExactWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalChangesExactCharacters += comparisonSegmentUnit.SourceCharsOriginal; 
                                                }
                                                else if (matchPercentage >= 95 && matchPercentage < 100)
                                                {
                                                    TotalChangesFuzzy99Segments++;
                                                    TotalChangesFuzzy99Words += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalChangesFuzzy99Characters += comparisonSegmentUnit.SourceCharsOriginal; 
                                                }
                                                else if (matchPercentage >= 85 && matchPercentage < 95)
                                                {
                                                    TotalChangesFuzzy94Segments++;
                                                    TotalChangesFuzzy94Words += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalChangesFuzzy94Characters += comparisonSegmentUnit.SourceCharsOriginal; 
                                                }
                                                else if (matchPercentage >= 75 && matchPercentage < 85)
                                                {
                                                    TotalChangesFuzzy84Segments++;
                                                    TotalChangesFuzzy84Words += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalChangesFuzzy84Characters += comparisonSegmentUnit.SourceCharsOriginal; 
                                                }
                                                else if (matchPercentage >= 50 && matchPercentage < 75)
                                                {
                                                    TotalChangesFuzzy74Segments++;
                                                    TotalChangesFuzzy74Words += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalChangesFuzzy74Characters += comparisonSegmentUnit.SourceCharsOriginal;  
                                                }
                                                else
                                                {
                                                    TotalChangesNewSegments++;
                                                    TotalChangesNewWords += comparisonSegmentUnit.SourceWordsOriginal;
                                                    TotalChangesNewCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                                }  
                                           
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (TotalSegmentsUpdated > 0 && TotalContentChanges > 0)
                    TotalContentChangesPercentage = Math.Round(Convert.ToDecimal(TotalContentChanges) / Convert.ToDecimal(TotalSegments) * 100, 2);

                if (TotalSegmentsUpdated > 0 && TotalStatusChanges > 0)
                    TotalStatusChangesPercentage = Math.Round(Convert.ToDecimal(TotalStatusChanges) / Convert.ToDecimal(TotalSegments) * 100, 2);
            }

            private static decimal GetMatchValue(string matchPercentage)
            {
                decimal value = 0;
                if (string.IsNullOrEmpty(matchPercentage))
                    return value;
                var mRegexPercentage = RegexPercentage.Match(matchPercentage);
                if (mRegexPercentage.Success)
                    value = NormalizeTerpStringValueToDecimal(mRegexPercentage.Groups["x1"].Value);

                return value;
            }
            private static decimal NormalizeTerpStringValueToDecimal(string str)
            {
                var strOut = str.Trim();
                decimal decOut;

                var hasDot = str.Contains(".");
                var hasComma = str.Contains(",");

                if (hasDot && hasComma)
                {
                    var dotIndex = str.IndexOf(".", StringComparison.Ordinal);
                    var commaIndex = str.IndexOf(",", StringComparison.Ordinal);

                    strOut = strOut.Replace(dotIndex < commaIndex ? "." : ",", string.Empty);
                    strOut = strOut.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                }
                else if (hasDot)
                {
                    NormalizeValue(ref strOut, '.');
                }
                else if (hasComma)
                {
                    NormalizeValue(ref strOut, ',');
                    strOut = strOut.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                }

                decimal.TryParse(strOut, NumberStyles.Any, CultureInfo.InvariantCulture, out decOut);

                return decOut;
            }
            private static void NormalizeValue(ref string strOut, char seperator)
            {
                var split = strOut.Split(seperator);
                if (split.Length != 2)
                    return;

                strOut = split[0].Trim().TrimStart('0');
                var end = split[1].Trim().TrimEnd('0');
                strOut += end != string.Empty
                    ? seperator + end
                    : string.Empty;

                if (strOut.Trim() == string.Empty)
                    strOut = "0";
            }

            private static readonly Regex RegexPercentage = new Regex(@"(?<x1>[\d]+)\%");

            private static bool IsFilterSegmentMatchPercentage(string filterOriginal, string filterUpdated, string matchOriginal, string matchUpdated)
            {
                var b = true;

                switch (filterOriginal)
                {
                    case "{All}":
                        {
                            //do nothing
                        } break;
                    case "PM {Perfect Match}":
                        {
                            if (string.Compare(matchOriginal, "PM", StringComparison.OrdinalIgnoreCase) != 0)
                                b = false;
                        } break;
                    case "CM {Context Match}":
                        {
                            if (string.Compare(matchOriginal, "CM", StringComparison.OrdinalIgnoreCase) != 0)
                                b = false;
                        } break;
                    case "AT {Automated Translation}":
                        {
                            if (string.Compare(matchOriginal, "AT", StringComparison.OrdinalIgnoreCase) != 0)
                                b = false;
                        } break;
                    case "Exact Match":
                        {
                            if (string.Compare(matchOriginal, "100%", StringComparison.OrdinalIgnoreCase) != 0)
                                b = false;
                        } break;
                    case "Fuzzy Match":
                        {
                            if (!RegexPercentage.Match(matchOriginal.Trim()).Success)
                                b = false;
                        } break;
                    case "No Match":
                        {
                            if (matchOriginal.Trim() != string.Empty)
                                b = false;
                        } break;
                }

                if (!b) return b;
                switch (filterUpdated)
                {
                    case "{All}":
                        {
                            //do nothing
                        } break;
                    case "PM {Perfect Match}":
                        {
                            if (string.Compare(matchUpdated, "PM", StringComparison.OrdinalIgnoreCase) != 0)
                                b = false;
                        } break;
                    case "CM {Context Match}":
                        {
                            if (string.Compare(matchUpdated, "CM", StringComparison.OrdinalIgnoreCase) != 0)
                                b = false;
                        } break;
                    case "AT {Automated Translation}":
                        {
                            if (string.Compare(matchUpdated, "AT", StringComparison.OrdinalIgnoreCase) != 0)
                                b = false;
                        } break;
                    case "Exact Match":
                        {
                            if (string.Compare(matchUpdated, "100%", StringComparison.OrdinalIgnoreCase) != 0)
                                b = false;
                        } break;
                    case "Fuzzy Match":
                        {
                            if (!RegexPercentage.Match(matchUpdated.Trim()).Success)
                                b = false;
                        } break;
                    case "No Match":
                        {
                            if (matchUpdated.Trim() != string.Empty)
                                b = false;
                        } break;
                }


                return b;
            }

            public int GetWordsCount(string text)
            {
                var iWords = 0;

                var rWords = new Regex(@"\s", RegexOptions.Singleline);

                foreach (var word in rWords.Split(text))
                {
                    var wortTmp = string.Empty;
                    foreach (var _char in word.ToCharArray())
                    {

                        if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2) //aisian characters
                        {
                            if (wortTmp != string.Empty)
                                iWords++;

                            iWords++;
                            wortTmp = string.Empty;
                        }
                        else
                            wortTmp += _char.ToString();


                    }
                    if (wortTmp != string.Empty)
                        iWords++;
                }

                return iWords;
            }
        }
    }


}
