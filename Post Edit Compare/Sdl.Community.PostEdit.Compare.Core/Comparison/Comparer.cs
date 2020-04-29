using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.PostEdit.Compare.Core.Comparison.Text;
using Sdl.Community.PostEdit.Compare.Core.Reports;
using Sdl.Community.PostEdit.Compare.Core.SDLXLIFF;

namespace Sdl.Community.PostEdit.Compare.Core.Comparison
{
    public partial class Comparer : TextComparer
    {

        internal delegate void ChangedEventHandler(int maximum, int current, int percent, string message);

        internal event ChangedEventHandler Progress;


        private Dictionary<string, ParagraphUnit> GetUpdatedParagraphsIds(Dictionary<string, ParagraphUnit> original, Dictionary<string, ParagraphUnit> updated)
        {
            var updatedUnit = new Dictionary<string, ParagraphUnit>();


            var indexOriginal = 0;
            foreach (var kvpOriginal in original)
            {
                var indexUpdated = 0;
                foreach (var kvpUpdated in updated)
                {
                    if (indexOriginal == indexUpdated)
                    {
                        updatedUnit.Add(kvpOriginal.Key, kvpUpdated.Value);
                        break;
                    }
                    indexUpdated++;
                }
                indexOriginal++;
            }




            return updatedUnit;
        }

        internal Dictionary<string, Dictionary<string, ComparisonParagraphUnit>> GetComparisonParagraphUnits(Dictionary<string, Dictionary<string, ParagraphUnit>> xFileParagraphUnitsOriginal
            , Dictionary<string, Dictionary<string, ParagraphUnit>> xFileParagraphUnitsUpdated)
        {
            var comparisonFileParagraphUnits = new Dictionary<string, Dictionary<string, ComparisonParagraphUnit>>();


            var iProgressCurrent = 0;

            var iProgressMaximum = xFileParagraphUnitsOriginal.Sum(xFileParagraphUnits => xFileParagraphUnits.Value.Count());


            var errorMatchingFileLevel01 = false;
            var errorMatchingFileLevel = false;
            var errorMatchingFileLevelMessage = string.Empty;

            var errorMatchingParagraphLevel = false;

            #region  |  check for initial errors  |
            foreach (var fileParagraphUnitOriginal in xFileParagraphUnitsOriginal)
            {


                if (!xFileParagraphUnitsUpdated.ContainsKey(fileParagraphUnitOriginal.Key))
                {
                    errorMatchingFileLevel = true;
                    errorMatchingFileLevelMessage =
                        string.Format(
                            "Error: Structure mismatch; unable to locate the corresponding file : '{0}' in the updated file",
                            fileParagraphUnitOriginal.Key);


                    var fileNameOriginal = Path.GetFileName(fileParagraphUnitOriginal.Key);

                    errorMatchingFileLevel01 = xFileParagraphUnitsUpdated.Select(kvp => Path.GetFileName(kvp.Key)).All(fileNameUpdated => string.Compare(fileNameOriginal, fileNameUpdated, StringComparison.OrdinalIgnoreCase) != 0);

                    break;
                }

                var fileParagraphUnitUpdated = xFileParagraphUnitsUpdated[fileParagraphUnitOriginal.Key];
                if ((from paragraphUnitOriginalPair in fileParagraphUnitOriginal.Value 
                     select paragraphUnitOriginalPair.Value into paragraphUnitOriginal 
                     let paragraphUnitUpdated = new ParagraphUnit(paragraphUnitOriginal.ParagraphUnitId, new List<SegmentPair>()) 
                     select paragraphUnitOriginal).Any(paragraphUnitOriginal => !fileParagraphUnitUpdated.ContainsKey(paragraphUnitOriginal.ParagraphUnitId)))
                {
                    errorMatchingParagraphLevel = true;
                }
            }
            #endregion

            if (errorMatchingFileLevel01)
            {
                //not possible to compare these files; excluding the file path information, the file names are different...
                throw new Exception(errorMatchingFileLevelMessage);
            }
            if (errorMatchingFileLevel || errorMatchingParagraphLevel)
            {
                #region  |  compare structure missmatch  |


                foreach (var fileParagraphUnitOriginal in xFileParagraphUnitsOriginal)
                {
                    var fileName = Path.GetFileName(fileParagraphUnitOriginal.Key);

                    var comparisonParagraphUnits = new Dictionary<string, ComparisonParagraphUnit>();


                    var fileParagraphUnitOriginalTmp = fileParagraphUnitOriginal.Value;


                    var nameOriginal = Path.GetFileName(fileParagraphUnitOriginal.Key);
                    var fileParagraphUnitUpdated = (from kvp in xFileParagraphUnitsUpdated let fileNameUpdated = Path.GetFileName(kvp.Key) 
                                                    where string.Compare(nameOriginal, fileNameUpdated, StringComparison.OrdinalIgnoreCase) == 0 
                                                    select kvp.Value).FirstOrDefault();

                    if (fileParagraphUnitUpdated == null)
                    {
                        throw new Exception(
                            string.Format(
                                "Error: Structure mismatch; unable to locate the corresponding file : '{0}' in the updated file",
                                fileParagraphUnitOriginal.Key));
                    }
                    //update paragraph ids in the updated object xFileParagraphUnit_updated
                    fileParagraphUnitUpdated = GetUpdatedParagraphsIds(fileParagraphUnitOriginalTmp, fileParagraphUnitUpdated);


                    foreach (var paragraphUnitOriginalPair in fileParagraphUnitOriginalTmp)
                    {

                        OnProgress(iProgressMaximum, iProgressCurrent++, fileName);

                        var comparisonParagraphUnit = new ComparisonParagraphUnit
                        {
                            ParagraphId = paragraphUnitOriginalPair.Key,
                            ParagraphIsUpdated = false,
                            ParagraphStatusChanged = false,
                            ParagraphHasComments = false
                        };



                        var paragraphUnitOriginal = paragraphUnitOriginalPair.Value;
                        var paragraphUnitUpdated = new ParagraphUnit(paragraphUnitOriginal.ParagraphUnitId, new List<SegmentPair>());



                        if (fileParagraphUnitUpdated.ContainsKey(paragraphUnitOriginal.ParagraphUnitId))
                            paragraphUnitUpdated = fileParagraphUnitUpdated[paragraphUnitOriginal.ParagraphUnitId];


                        if (paragraphUnitUpdated != null)
                        {
                            if (paragraphUnitOriginal.SegmentPairs.Count >= paragraphUnitUpdated.SegmentPairs.Count)
                            {
                                #region  |  xParagraphUnit_original.xSegmentPairs.Count >= xParagraphUnit_updated.xSegmentPairs.Count  |

                                //if segment count has not changed (or greater than the segment count in the updated file)
                                for (var i = 0; i < paragraphUnitOriginal.SegmentPairs.Count; i++)
                                {
                                    var segmentPairOriginal = paragraphUnitOriginal.SegmentPairs[i];

                                    var segmentPairUpdated = new SegmentPair();
                                    if (paragraphUnitUpdated.SegmentPairs.Count > i)
                                        segmentPairUpdated = paragraphUnitUpdated.SegmentPairs[i];

                                    var comparisonSegmentUnit = new ComparisonSegmentUnit(segmentPairOriginal.Id, segmentPairOriginal.SourceSections, segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections, segmentPairOriginal.IsLocked);

                                    AddToComparision(ref comparisonParagraphUnit, comparisonSegmentUnit, segmentPairOriginal, segmentPairUpdated);
                                }
                                #endregion
                            }
                            else if (paragraphUnitOriginal.SegmentPairs.Count < paragraphUnitUpdated.SegmentPairs.Count)
                            {
                                #region  |  xParagraphUnit_original.xSegmentPairs.Count < xParagraphUnit_updated.xSegmentPairs.Count  |
                                //if more segments now exist in the updated file
                                for (var i = 0; i < paragraphUnitUpdated.SegmentPairs.Count; i++)
                                {
                                    var segmentPairOriginal = new SegmentPair();
                                    if (paragraphUnitOriginal.SegmentPairs.Count > i)
                                        segmentPairOriginal = paragraphUnitOriginal.SegmentPairs[i];

                                    var segmentPairUpdated = paragraphUnitUpdated.SegmentPairs[i];

                                    var comparisonSegmentUnit = new ComparisonSegmentUnit(segmentPairUpdated.Id, segmentPairOriginal.SourceSections, segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections, segmentPairOriginal.IsLocked);

                                    AddToComparision(ref comparisonParagraphUnit, comparisonSegmentUnit, segmentPairOriginal, segmentPairUpdated);
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            #region  |  else  |

                            foreach (var segmentPairOriginal in paragraphUnitOriginal.SegmentPairs)
                            {
                                var segmentPairUpdated = new SegmentPair();
                                var comparisonSegmentUnit = new ComparisonSegmentUnit(segmentPairOriginal.Id, segmentPairOriginal.SourceSections, segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections, segmentPairOriginal.IsLocked);
                                AddToComparision(ref comparisonParagraphUnit, comparisonSegmentUnit, segmentPairOriginal, segmentPairUpdated);
                            }
                            #endregion
                        }

                        comparisonParagraphUnits.Add(comparisonParagraphUnit.ParagraphId, comparisonParagraphUnit);
                    }

                    comparisonFileParagraphUnits.Add(fileParagraphUnitOriginal.Key, comparisonParagraphUnits);
                }


                #endregion
            }
            else
            {
                #region  |  compare normal  |

                foreach (var fileParagraphUnitOriginal in xFileParagraphUnitsOriginal)
                {
                    var fileName = Path.GetFileName(fileParagraphUnitOriginal.Key);

                    var comparisonParagraphUnits = new Dictionary<string, ComparisonParagraphUnit>();

                    if (!xFileParagraphUnitsUpdated.ContainsKey(fileParagraphUnitOriginal.Key))
                        throw new Exception(
                            string.Format(
                                "Error: Structure mismatch; unable to locate the corresponding file : '{0}' in the updated file",
                                fileParagraphUnitOriginal.Key));

                    var fileParagraphUnitUpdated = xFileParagraphUnitsUpdated[fileParagraphUnitOriginal.Key];
                    foreach (var paragraphUnitOriginalPair in fileParagraphUnitOriginal.Value)
                    {

                        OnProgress(iProgressMaximum, iProgressCurrent++, fileName);

                        var comparisonParagraphUnit = new ComparisonParagraphUnit
                        {
                            ParagraphId = paragraphUnitOriginalPair.Key,
                            ParagraphIsUpdated = false,
                            ParagraphStatusChanged = false,
                            ParagraphHasComments = false
                        };



                        var paragraphUnitOriginal = paragraphUnitOriginalPair.Value;
                        ParagraphUnit paragraphUnitUpdated;

                        if (fileParagraphUnitUpdated.ContainsKey(paragraphUnitOriginal.ParagraphUnitId))
                            paragraphUnitUpdated = fileParagraphUnitUpdated[paragraphUnitOriginal.ParagraphUnitId];
                        else
                            throw new Exception(
                                string.Format(
                                    "Error: Structure mismatch; unable to locate the corresponding paragraph ID: '{0}' in the updated file",
                                    paragraphUnitOriginal.ParagraphUnitId));

                        if (paragraphUnitOriginal.SegmentPairs.Count >= paragraphUnitUpdated.SegmentPairs.Count)
                        {
                            #region  |  xParagraphUnit_original.xSegmentPairs.Count >= xParagraphUnit_updated.xSegmentPairs.Count  |

                            //if segment count has not changed (or greater than the segment count in the updated file)
                            for (var i = 0; i < paragraphUnitOriginal.SegmentPairs.Count; i++)
                            {
                                var segmentPairOriginal = paragraphUnitOriginal.SegmentPairs[i];

                                var segmentPairUpdated = new SegmentPair();
                                if (paragraphUnitUpdated.SegmentPairs.Count > i)
                                    segmentPairUpdated = paragraphUnitUpdated.SegmentPairs[i];

                                var comparisonSegmentUnit = new ComparisonSegmentUnit(segmentPairOriginal.Id, segmentPairOriginal.SourceSections, segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections, segmentPairOriginal.IsLocked);

                                AddToComparision(ref comparisonParagraphUnit, comparisonSegmentUnit, segmentPairOriginal, segmentPairUpdated);
                            }
                            #endregion
                        }
                        else if (paragraphUnitOriginal.SegmentPairs.Count < paragraphUnitUpdated.SegmentPairs.Count)
                        {
                            #region  |  xParagraphUnit_original.xSegmentPairs.Count < xParagraphUnit_updated.xSegmentPairs.Count  |
                            //if more segments now exist in the updated file
                            for (var i = 0; i < paragraphUnitUpdated.SegmentPairs.Count; i++)
                            {
                                var segmentPairOriginal = new SegmentPair();
                                if (paragraphUnitOriginal.SegmentPairs.Count > i)
                                    segmentPairOriginal = paragraphUnitOriginal.SegmentPairs[i];

                                var segmentPairUpdated = paragraphUnitUpdated.SegmentPairs[i];

                                var comparisonSegmentUnit = new ComparisonSegmentUnit(segmentPairUpdated.Id, segmentPairOriginal.SourceSections, segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections, segmentPairOriginal.IsLocked);

                                AddToComparision(ref comparisonParagraphUnit, comparisonSegmentUnit, segmentPairOriginal, segmentPairUpdated);
                            }
                            #endregion
                        }

                        comparisonParagraphUnits.Add(comparisonParagraphUnit.ParagraphId, comparisonParagraphUnit);
                    }

                    comparisonFileParagraphUnits.Add(fileParagraphUnitOriginal.Key, comparisonParagraphUnits);

                }
                #endregion
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

            if (segmentPairOriginal.TranslationOrigin != null)
                comparisonSegmentUnit.TranslationOriginTypeOriginal = segmentPairOriginal.TranslationOrigin.OriginType;
            if (segmentPairUpdated.TranslationOrigin != null)
                comparisonSegmentUnit.TranslationOriginTypeUpdated = segmentPairUpdated.TranslationOrigin.OriginType;



            if (string.CompareOrdinal(segmentPairOriginal.Target, segmentPairUpdated.Target) != 0)
            {
                comparisonSegmentUnit.ComparisonTextUnits = GetComparisonTextUnits(segmentPairOriginal.TargetSections, segmentPairUpdated.TargetSections);

                comparisonSegmentUnit.SegmentTextUpdated = true;
                comparisonParagraphUnit.ParagraphIsUpdated = true;   
            }


            comparisonSegmentUnit.SourceWordsOriginal = segmentPairOriginal.SourceWords;
            comparisonSegmentUnit.SourceCharsOriginal = segmentPairOriginal.SourceChars;
            comparisonSegmentUnit.SourceTagsOriginal = segmentPairOriginal.SourceTags;

            comparisonSegmentUnit.SourceWordsUpdated = segmentPairUpdated.SourceWords;
            comparisonSegmentUnit.SourceCharsUpdated = segmentPairUpdated.SourceChars;
            comparisonSegmentUnit.SourceTagsUpdated = segmentPairUpdated.SourceTags;


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

            comparisonSegmentUnit.TargetOriginalRevisionMarkers = new List<SegmentSection>();
            foreach (var section in segmentPairOriginal.TargetSections)
            {
                if (section.RevisionMarker != null)
                {
                    comparisonSegmentUnit.TargetOriginalRevisionMarkers.Add(section);
                }
            }

            comparisonSegmentUnit.TargetUpdatedRevisionMarkers = new List<SegmentSection>();
            foreach (var section in segmentPairUpdated.TargetSections)
            {
                if (section.RevisionMarker != null)
                {
                    comparisonSegmentUnit.TargetUpdatedRevisionMarkers.Add(section);
                }
            }
            comparisonParagraphUnit.ComparisonSegmentUnits.Add(comparisonSegmentUnit);
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
					|| string.Compare(segmentPair.TranslationOrigin.OriginType, "nmt", StringComparison.OrdinalIgnoreCase) == 0
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
				|| string.Compare(segmentPair.TranslationOrigin.OriginType, "nmt", StringComparison.OrdinalIgnoreCase) == 0
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


        internal static int LevenshteinDistance(string s, string t)
        {
            // degenerate cases
            if (s == t) return 0;
            if (s.Length == 0) return t.Length;
            if (t.Length == 0) return s.Length;

            // create two work vectors of integer distances
            var v0 = new int[t.Length + 1];
            var v1 = new int[t.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (var i = 0; i < v0.Length; i++)
                v0[i] = i;

            for (var i = 0; i < s.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (var j = 0; j < t.Length; j++)
                {
                    var cost = s[i] == t[j] ? 0 : 1;
                    //v1[j + 1] = minimumn(v1[j] + 1, v0[j + 1] + 1, v0[j] + cost);
                    v1[j + 1] = Math.Min(v1[j] + 1, Math.Min(v0[j + 1] + 1, v0[j] + cost)) + 1;
                }

                // copy v1 (current row) to v0 (previous row) for next interation
                for (var j = 0; j < v0.Length; j++)
                    v0[j] = v1[j];
            }

            return v1[t.Length];
        }



        private static List<string> GetSectionsList(IReadOnlyList<SegmentSection> segmentSections, ref int charsLength)
        {
            var items = new List<string>();

            foreach (var section in segmentSections)
            {
                if (section.RevisionMarker != null && section.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                {
                    //ignore from the comparison process
                }
                else
                {
                    if (section.Type == SegmentSection.ContentType.Text)
                    {
                        var strLists = ReportUtils.GetTextSections(section.Content);

                        foreach (var part in strLists)
                        {
                            foreach (var letter in part)
                            {
                                items.Add(letter.ToString());
                                charsLength++;
                            }
                        }
                    }
                    else
                    {
                        charsLength++;
                        items.Add(section.Content);
                    }
                }
            }
            return items;
            
        }
        internal static int DamerauLevenshteinDistanceFromObject(List<SegmentSection> source, List<SegmentSection> target)
        {

            var sourceLen = 0;
            var sourceItems = GetSectionsList(source, ref sourceLen);
          
            var targetLen = 0;
            var targetItems = GetSectionsList(target, ref targetLen);
           

            if (sourceLen == 0)            
                return targetLen == 0 ? 0 : targetLen;
            
            if (targetLen == 0)            
                return sourceLen;
            

            var score = new int[sourceLen + 2, targetLen + 2];

            var inf = sourceLen + targetLen;
            score[0, 0] = inf;
            for (var i = 0; i <= sourceLen; i++) { score[i + 1, 1] = i; score[i + 1, 0] = inf; }
            for (var j = 0; j <= targetLen; j++) { score[1, j + 1] = j; score[0, j + 1] = inf; }

            var sd = new SortedDictionary<string, int>();

            var fullList = new List<string>();
            fullList.AddRange(sourceItems);
            fullList.AddRange(targetItems);

            foreach (var item in fullList)
            {
                if (!sd.ContainsKey(item))
                    sd.Add(item, 0);
            }


            for (var i = 1; i <= sourceLen; i++)
            {
                var db = 0;
                for (var j = 1; j <= targetLen; j++)
                {
                    var i1 = sd[targetItems[j - 1]];
                    var j1 = db;

                    if (sourceItems[i - 1] == targetItems[j - 1])
                    {
                        score[i + 1, j + 1] = score[i, j];
                        db = j;
                    }
                    else
                    {
                        score[i + 1, j + 1] = Math.Min(score[i, j], Math.Min(score[i + 1, j], score[i, j + 1])) + 1;
                    }

                    score[i + 1, j + 1] = Math.Min(score[i + 1, j + 1], score[i1, j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
                }

                sd[sourceItems[i - 1]] = i;
            }

            return score[sourceLen + 1, targetLen + 1];
        }


        public static int DamerauLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }
            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            var score = new int[source.Length + 2, target.Length + 2];

            var inf = source.Length + target.Length;
            score[0, 0] = inf;
            for (var i = 0; i <= source.Length; i++) { score[i + 1, 1] = i; score[i + 1, 0] = inf; }
            for (var j = 0; j <= target.Length; j++) { score[1, j + 1] = j; score[0, j + 1] = inf; }

            var sd = new SortedDictionary<char, int>();
            foreach (var letter in source + target)
            {
                if (!sd.ContainsKey(letter))
                    sd.Add(letter, 0);
            }

            for (var i = 1; i <= source.Length; i++)
            {
                var db = 0;
                for (var j = 1; j <= target.Length; j++)
                {
                    var i1 = sd[target[j - 1]];
                    var j1 = db;

                    if (source[i - 1] == target[j - 1])
                    {
                        score[i + 1, j + 1] = score[i, j];
                        db = j;
                    }
                    else
                    {
                        score[i + 1, j + 1] = Math.Min(score[i, j], Math.Min(score[i + 1, j], score[i, j + 1])) + 1;
                    }

                    score[i + 1, j + 1] = Math.Min(score[i + 1, j + 1], score[i1, j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
                }

                sd[source[i - 1]] = i;
            }

            return score[source.Length + 1, target.Length + 1];
        }

    }

}
