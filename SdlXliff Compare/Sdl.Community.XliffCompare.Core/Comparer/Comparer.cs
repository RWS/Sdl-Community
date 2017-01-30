using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.XliffCompare.Core.SDLXLIFF;

namespace Sdl.Community.XliffCompare.Core.Comparer
{
    internal partial class Comparer : TextComparer.TextComparer
    {

        internal delegate void ChangedEventHandler(int maximum, int current, int percent, string message);

        internal event ChangedEventHandler Progress;


        internal Dictionary<string,Dictionary<string, ComparisonParagraphUnit>> GetComparisonParagraphUnits(Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnitsOriginal
            , Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnitsUpdated)
        {
            var comparisonFileParagraphUnits = new Dictionary<string, Dictionary<string, ComparisonParagraphUnit>>();


            var iProgressCurrent = 0;

            var iProgressMaximum = fileParagraphUnitsOriginal.Sum(fileParagraphUnits => fileParagraphUnits.Value.Count);


            foreach (var fileParagraphUnitOriginal in fileParagraphUnitsOriginal) 
            {
                var comparisonParagraphUnits = new Dictionary<string, ComparisonParagraphUnit>();

                if (!fileParagraphUnitsUpdated.ContainsKey(fileParagraphUnitOriginal.Key))
                    throw new Exception("Error: Structure mismatch; unable to locate the corresponding file : '" 
                        + fileParagraphUnitOriginal.Key + "' in the updated file");



                var fileParagraphUnitUdpated = fileParagraphUnitsUpdated[fileParagraphUnitOriginal.Key];
                foreach (var xParagraphUnitOriginalPair in fileParagraphUnitOriginal.Value)
                {
                    OnProgress(iProgressMaximum, iProgressCurrent++, "");

                    var comparisonParagraphUnit = new ComparisonParagraphUnit
                    {
                        ParagraphId = xParagraphUnitOriginalPair.Key,
                        ParagraphIsUpdated = false,
                        ParagraphStatusChanged = false,
                        ParagraphHasComments = false
                    };



                    var paragraphUnitOriginal = xParagraphUnitOriginalPair.Value;
                    ParagraphUnit paragraphUnitUpdated;



                    if (fileParagraphUnitUdpated.ContainsKey(paragraphUnitOriginal.ParagraphUnitId))
                        paragraphUnitUpdated = fileParagraphUnitUdpated[paragraphUnitOriginal.ParagraphUnitId];
                    else
                        throw new Exception("Error: Structure mismatch; unable to locate the corresponding paragraph ID: '" 
                            + paragraphUnitOriginal.ParagraphUnitId + "' in the updated file");


                    if (paragraphUnitOriginal.SegmentPairs.Count >= paragraphUnitUpdated.SegmentPairs.Count)
                    {
                        //if segment count has not changed (or greater than the segment count in the updated file)
                        for (var i = 0; i < paragraphUnitOriginal.SegmentPairs.Count; i++)
                        {
                            var segmentPairOriginal = paragraphUnitOriginal.SegmentPairs[i];

                            var segmentPairUpdated = new SegmentPair();
                            if (paragraphUnitUpdated.SegmentPairs.Count > i)
                                segmentPairUpdated = paragraphUnitUpdated.SegmentPairs[i];

                            var comparisonSegmentUnit = new ComparisonSegmentUnit(segmentPairOriginal.Id, segmentPairOriginal.SourceSections, segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections);

                            AddToComparision(ref comparisonParagraphUnit, comparisonSegmentUnit, segmentPairOriginal, segmentPairUpdated);
                        }
                    }
                    else if (paragraphUnitOriginal.SegmentPairs.Count < paragraphUnitUpdated.SegmentPairs.Count)
                    {
                        //if more segments now exist in the updated file
                        for (var i = 0; i < paragraphUnitUpdated.SegmentPairs.Count; i++)
                        {
                            var segmentPairOriginal = new SegmentPair();
                            if (paragraphUnitOriginal.SegmentPairs.Count > i)
                                segmentPairOriginal = paragraphUnitOriginal.SegmentPairs[i];

                            var segmentPairUpdated = paragraphUnitUpdated.SegmentPairs[i];

                            var comparisonSegmentUnit = new ComparisonSegmentUnit(segmentPairUpdated.Id, segmentPairOriginal.SourceSections, segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections);

                            AddToComparision(ref comparisonParagraphUnit, comparisonSegmentUnit, segmentPairOriginal, segmentPairUpdated);
                        }
                    }

                    comparisonParagraphUnits.Add(comparisonParagraphUnit.ParagraphId, comparisonParagraphUnit);
                }

                comparisonFileParagraphUnits.Add(fileParagraphUnitOriginal.Key, comparisonParagraphUnits);

            }

            return comparisonFileParagraphUnits;
        }


        private void AddToComparision(ref ComparisonParagraphUnit comparisonParagraphUnit
         , ComparisonSegmentUnit comparisonSegmentUnit
         , SegmentPair segmentPairOriginal
         , SegmentPair segmentPairUpdated)
        {

       
            comparisonSegmentUnit.SegmentStatusOriginal = segmentPairOriginal.SegmentStatus;
            comparisonSegmentUnit.SegmentStatusUpdated = segmentPairUpdated.SegmentStatus;

            comparisonSegmentUnit.TranslationStatusOriginal = GetTranslationStatus(segmentPairOriginal);
            comparisonSegmentUnit.TranslationStatusUpdated = GetTranslationStatus(segmentPairUpdated);

            if (segmentPairOriginal.TranslationOrigin!=null)
                comparisonSegmentUnit.TranslationOriginTypeOriginal = segmentPairOriginal.TranslationOrigin.OriginType;
            if (segmentPairUpdated.TranslationOrigin != null)
                comparisonSegmentUnit.TranslationOriginTypeUpdated = segmentPairUpdated.TranslationOrigin.OriginType;


            comparisonSegmentUnit.TranslationSectionsWordsIdentical = 0;
            comparisonSegmentUnit.TranslationSectionsCharactersIdentical = 0;
            comparisonSegmentUnit.TranslationSectionsTagsIdentical = 0;

            comparisonSegmentUnit.TranslationSectionsWordsNew = 0;
            comparisonSegmentUnit.TranslationSectionsCharactersNew = 0;
            comparisonSegmentUnit.TranslationSectionsTagsNew = 0;

            comparisonSegmentUnit.TranslationSectionsWordsRemoved = 0;
            comparisonSegmentUnit.TranslationSectionsCharactersRemoved = 0;
            comparisonSegmentUnit.TranslationSectionsTagsRemoved = 0;


            comparisonSegmentUnit.TranslationSectionsWords = 0;
            comparisonSegmentUnit.TranslationSectionsCharacters = 0;
            comparisonSegmentUnit.TranslationSectionsTags = 0;

            comparisonSegmentUnit.TranslationSectionsChangedWords=0;
            comparisonSegmentUnit.TranslationSectionsChangedCharacters = 0;
            comparisonSegmentUnit.TranslationSectionsChangedTags=0;

       
            if (string.Compare(segmentPairOriginal.Target, segmentPairUpdated.Target, StringComparison.OrdinalIgnoreCase) != 0)
            {
                try
                {
                    comparisonSegmentUnit.ComparisonTextUnits = GetComparisonTextUnits(segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections);


                
                    foreach (var comparisonTextUnit in comparisonSegmentUnit.ComparisonTextUnits)
                    {
                        switch (comparisonTextUnit.ComparisonTextUnitType)
                        {
                            case ComparisonTextUnitType.Identical:
                                foreach (var segmentSection in comparisonTextUnit.TextSections)
                                {
                                    if (segmentSection.Content.Trim() != string.Empty)
                                    {
                                        if (segmentSection.IsText)
                                        {
                                            comparisonSegmentUnit.TranslationSectionsWordsIdentical++;                                        
                                        }
                                        else
                                            comparisonSegmentUnit.TranslationSectionsTagsIdentical++;
                                    }
                                    if (segmentSection.IsText)
                                        comparisonSegmentUnit.TranslationSectionsCharactersIdentical += segmentSection.Content.ToCharArray().Length;
                                }
                                break;
                            case ComparisonTextUnitType.New:
                                foreach (var segmentSection in comparisonTextUnit.TextSections)
                                {
                                    if (segmentSection.Content.Trim() != string.Empty)
                                    {
                                        if (segmentSection.IsText)
                                        {
                                            comparisonSegmentUnit.TranslationSectionsWordsNew++;                                        
                                        }
                                        else
                                            comparisonSegmentUnit.TranslationSectionsTagsNew++;
                                    }
                                    if (segmentSection.IsText)
                                        comparisonSegmentUnit.TranslationSectionsCharactersNew += segmentSection.Content.ToCharArray().Length;
                                }
                                break;
                            case ComparisonTextUnitType.Removed:

                                foreach (var segmentSection in comparisonTextUnit.TextSections)
                                {
                                    if (segmentSection.Content.Trim() != string.Empty)
                                    {
                                        if (segmentSection.IsText)
                                        {
                                            comparisonSegmentUnit.TranslationSectionsWordsRemoved++;                                      
                                        }
                                        else
                                            comparisonSegmentUnit.TranslationSectionsTagsRemoved++;
                                    }
                                    if (segmentSection.IsText)
                                        comparisonSegmentUnit.TranslationSectionsCharactersRemoved += segmentSection.Content.ToCharArray().Length;
                                }
                                break;
                        }
                    }

                    comparisonSegmentUnit.TranslationSectionsWords = comparisonSegmentUnit.TranslationSectionsWordsIdentical + comparisonSegmentUnit.TranslationSectionsWordsRemoved;
                    comparisonSegmentUnit.TranslationSectionsCharacters = comparisonSegmentUnit.TranslationSectionsCharactersIdentical + comparisonSegmentUnit.TranslationSectionsCharactersRemoved;
                    comparisonSegmentUnit.TranslationSectionsTags = comparisonSegmentUnit.TranslationSectionsTagsIdentical + comparisonSegmentUnit.TranslationSectionsTagsRemoved;

                    var totalChangesWords = comparisonSegmentUnit.TranslationSectionsWordsNew + comparisonSegmentUnit.TranslationSectionsWordsRemoved;
                    var totalChangesCharacters = comparisonSegmentUnit.TranslationSectionsCharactersNew + comparisonSegmentUnit.TranslationSectionsCharactersRemoved;
                    var totalChangesTags = comparisonSegmentUnit.TranslationSectionsTagsNew + comparisonSegmentUnit.TranslationSectionsTagsRemoved;



                    comparisonSegmentUnit.TranslationSectionsChangedWords = totalChangesWords;
                    comparisonSegmentUnit.TranslationSectionsChangedCharacters = totalChangesCharacters;
                    comparisonSegmentUnit.TranslationSectionsChangedTags = totalChangesTags;
     
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                comparisonSegmentUnit.SegmentTextUpdated = true;
                comparisonParagraphUnit.ParagraphIsUpdated = true;
            }



            if (string.Compare(comparisonSegmentUnit.SegmentStatusOriginal, comparisonSegmentUnit.SegmentStatusUpdated, StringComparison.OrdinalIgnoreCase) != 0)
            {
                comparisonSegmentUnit.SegmentSegmentStatusUpdated = true;
                comparisonParagraphUnit.ParagraphStatusChanged = true;
            }

            if (segmentPairUpdated.Comments != null && segmentPairUpdated.Comments.Count > 0)
            {
                comparisonSegmentUnit.Comments = segmentPairUpdated.Comments;
                comparisonSegmentUnit.SegmentHasComments = true;

                comparisonParagraphUnit.ParagraphHasComments = true;
            }


            comparisonParagraphUnit.ComparisonSegmentUnits.Add(comparisonSegmentUnit);
        }

        internal int GetPartsCount(string text)
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

        private static string GetTranslationStatus(SegmentPair segmentPair)
        {
           
            var match = string.Empty;
            if (segmentPair.TranslationOrigin == null) return match;
            if (segmentPair.TranslationOrigin.MatchPercentage >= 100)
            {
                if (string.Compare(segmentPair.TranslationOrigin.OriginType, "document-match", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    match = "PM";
                }
                else if (string.Compare(segmentPair.TranslationOrigin.TextContextMatchLevel, "SourceAndTarget", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    match = "CM";
                }
                else if (string.Compare(segmentPair.TranslationOrigin.OriginType, "mt", StringComparison.OrdinalIgnoreCase) == 0
                    || string.Compare(segmentPair.TranslationOrigin.OriginType, "amt", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    match = "AT";
                }
                else
                {
                    match = segmentPair.TranslationOrigin.MatchPercentage + "%";
                }
            }
            else if (string.Compare(segmentPair.TranslationOrigin.OriginType, "mt", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(segmentPair.TranslationOrigin.OriginType, "amt", StringComparison.OrdinalIgnoreCase) == 0)
            {
                match = "AT";
            }
            else if (segmentPair.TranslationOrigin.MatchPercentage > 0)
            {
                match = segmentPair.TranslationOrigin.MatchPercentage + "%";
            }
            else
            {
                match = "";
            }
            return match;
        }


        internal void OnProgress(int maximum, int current, string message)
        {
            if (Progress == null) return;
            var percent = Convert.ToInt32(current <= maximum && maximum > 0 ? Convert.ToDecimal(current) / Convert.ToDecimal(maximum) * Convert.ToDecimal(100) : maximum);

            Progress(maximum, current, percent, message);
        }
        
    }

}
