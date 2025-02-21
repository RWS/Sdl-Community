using Sdl.Community.PostEdit.Compare.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model
{
    public class SegmentFilter
    {
        public static SegmentFilter Empty => new();
        public List<string> FuzzyPercentage { get; set; }
        public bool IsEmpty => MatchTypes == 0 && Statuses == 0;
        public MatchTypes MatchTypes { get; set; }
        public Operator Operator { get; set; }
        public Statuses Statuses { get; set; }

        public bool IsMatch(ReportSegment segment)
        {
            if (segment == null)
                throw new ArgumentNullException(nameof(segment));

            if (Statuses != 0)
            {
                if (!EnumHelper.TryGetStatus(segment.Status, out var confirmationStatus))
                    throw new ArgumentException("Invalid confirmation status");
                if (!Statuses.HasFlag(confirmationStatus))
                {
                    if (Operator == Operator.And)
                        return false;
                }
                else if (Operator == Operator.Or) return true;
            }

            if (MatchTypes != 0)
            {
                if (string.IsNullOrWhiteSpace(segment.MatchType)) return MatchTypes.HasFlag(MatchTypes.NoMatch);

                if (!Enum.TryParse<MatchTypes>(segment.MatchType, out var matchType))
                    if (!segment.MatchType.Contains("%")) throw new ArgumentException("Invalid match type");
                    else
                    {
                        if (!int.TryParse(segment.MatchType.Split('%')[0], out var fuzzyPercentage))
                            throw new ArgumentException("Invalid fuzzy percentage");
                        if (!FuzzyRange.IsInFuzzyRanges(fuzzyPercentage, FuzzyPercentage))
                            return !FuzzyPercentage.Any();
                        if (Operator == Operator.Or) return true;
                    }
                else if (!MatchTypes.HasFlag(matchType)) return false;
            }

            return Operator == Operator.And;
        }
    }
}