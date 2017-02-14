using System;
using System.Linq;
using Sdl.Community.Comparison;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.Projects.Activities;

namespace Sdl.Community.Qualitivity
{
    public class EditDistance
    {

        public long SourceWordCount { get; private set; }
        public decimal Edits { get; private set; }
        public decimal EditDistanceRelative { get; private set; }
        public decimal PemPercentage { get; private set; }


        public EditDistance(Record record, Activity tpa)
        {         
            SetDamerauLevenshteinDistance(record, tpa);
        }

        private void SetDamerauLevenshteinDistance(Record record, Activity tpa)
        {
            var textComparer = new TextComparer();

            SourceWordCount = 0;
            Edits = 0;
            EditDistanceRelative = 0;
            PemPercentage = 0;

         
            decimal p1 = 100;

            var targetOriginalForCompareLen = 0;
            var targetUpdatedForCompareLen = 0;

            #region  |  get string len  |



            foreach (var scr in record.ContentSections.TargetOriginalSections)
            {
                if (scr.RevisionMarker != null && scr.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore
                }
                else if (scr.CntType == ContentSection.ContentType.Text)
                {
                    var strLists = Helper.GetTextSections(scr.Content);
                    targetOriginalForCompareLen += strLists.SelectMany(part => part).Count();
                }
                else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                {
                    targetOriginalForCompareLen++;
                }
            }
            foreach (var scr in record.ContentSections.TargetUpdatedSections)
            {
                if (scr.RevisionMarker != null && scr.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore
                }
                else if (scr.CntType == ContentSection.ContentType.Text)
                {
                    var strLists = Helper.GetTextSections(scr.Content);
                    targetUpdatedForCompareLen += strLists.SelectMany(part => part).Count();
                }
                else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                {
                    targetUpdatedForCompareLen++;
                }
            }
            #endregion

            #region  |  get distance  |

            var charsTargetO = 0;
            var charsTargetU = 0;
            decimal edits = textComparer.DamerauLevenshteinDistance_fromObject(record.ContentSections.TargetOriginalSections, record.ContentSections.TargetUpdatedSections
                , out charsTargetO, out charsTargetU, tpa.ComparisonOptions.IncludeTagsInComparison);
            decimal pemp = 100;

            decimal maxChars = targetOriginalForCompareLen > targetUpdatedForCompareLen ? targetOriginalForCompareLen : targetUpdatedForCompareLen;

            if (targetOriginalForCompareLen > 0)
            {
                var charsDistPerTmp = edits / maxChars;
                pemp = 1.0m - charsDistPerTmp;

                edits = Math.Round(edits, 2);
                pemp = Math.Round(pemp * 100, 2);
            }
            if (targetOriginalForCompareLen == 0)
            {
                pemp = 0;
                edits = 0;
            }

            p1 = pemp;

            #endregion


  
            SourceWordCount = record.WordCount;
            Edits = edits;
            EditDistanceRelative = maxChars;
            PemPercentage = p1;

            

        }
    }
}
