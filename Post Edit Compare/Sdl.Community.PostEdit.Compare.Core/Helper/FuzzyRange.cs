using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Compare.Core.Helper;

public class FuzzyRange
{
    public static void GetLimits(string range, out int min, out int max)
    {
        var limits = range.Split('-');
        if (limits.Length == 1)
        {
            min = int.Parse(range.Trim('>').Trim());
            max = -1;
        }
        else
        {
            min = int.Parse(limits[0].Trim());
            max = int.Parse(limits[1].Trim());
        }
    }

    public static List<string> GetFuzzyRanges(List<string> analysisBands)
    {
        var ranges = new List<string>();
        for (var index = 0; index < analysisBands.Count - 1; index++)
            ranges.Add($"{analysisBands[index]} - {analysisBands[index + 1]}");
        ranges.Add($"> {analysisBands[analysisBands.Count - 1]}");
        return ranges;
    }

    public static bool IsInFuzzyRange(int segmentPercentage, string fuzzyRange)
    {
        GetLimits(fuzzyRange, out var min, out var max);

        if (min > -1 && max > -1)
        {
            if (segmentPercentage >= min &&
                segmentPercentage <= max) return true;
        }
        else
        {
            if (segmentPercentage >= min) return true;
        }

        return false;
    }
}