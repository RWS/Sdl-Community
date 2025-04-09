using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration.Components;

public class DiffSegment
{
    public string Type { get; set; }
    public string Content { get; set; }
}

public class DiffMerger
{
    public static List<DiffSegment> MergeSegments(string segment1, string segment2)
    {
        var tokens1 = Tokenize(segment1);
        var tokens2 = Tokenize(segment2);
        var lcs = ComputeLCS(tokens1, tokens2);

        var diffSegments = new List<DiffSegment>();
        int i = 0, j = 0;

        foreach (var token in lcs)
        {
            // Tokens in segment1 before the common token are considered removed.
            var removed = new List<string>();
            while (i < tokens1.Count && tokens1[i] != token)
            {
                removed.Add(tokens1[i]);
                i++;
            }
            if (removed.Count > 0)
                AddDiffSegment(diffSegments, "textRemoved", string.Join(" ", removed));

            // Tokens in segment2 before the common token are considered new.
            var added = new List<string>();
            while (j < tokens2.Count && tokens2[j] != token)
            {
                added.Add(tokens2[j]);
                j++;
            }
            if (added.Count > 0)
                AddDiffSegment(diffSegments, "textNew", string.Join(" ", added));

            // The common token remains unchanged.
            AddDiffSegment(diffSegments, "text", token);
            i++;
            j++;
        }

        // Process any remaining tokens.
        if (i < tokens1.Count)
        {
            var remainingRemoved = tokens1.GetRange(i, tokens1.Count - i);
            AddDiffSegment(diffSegments, "textRemoved", string.Join(" ", remainingRemoved));
        }
        if (j < tokens2.Count)
        {
            var remainingAdded = tokens2.GetRange(j, tokens2.Count - j);
            AddDiffSegment(diffSegments, "textNew", string.Join(" ", remainingAdded));
        }

        // Build the final result by merging adjacent segments of the same type.
        //var result = new StringBuilder();
        //foreach (var seg in diffSegments)
        //{
        //    result.AppendFormat("<{0}>{1}</{0}> ", seg.Type, seg.Content);
        //}
        //return result.ToString().Trim();

        return diffSegments;
    }

    private static void AddDiffSegment(List<DiffSegment> segments, string type, string content)
    {
        if (segments.Count > 0 && segments[segments.Count - 1].Type == type)
            segments[segments.Count - 1].Content += " " + content;
        else
            segments.Add(new DiffSegment { Type = type, Content = content });
    }

    private static List<string> Tokenize(string segment)
    {
        var tokens = new List<string>();
        // Matches tags (e.g., <tag>) or any sequence of non-whitespace characters.
        var regex = new Regex(@"<[^>]+>|[^\s]+");
        foreach (Match match in regex.Matches(segment))
            tokens.Add(match.Value);
        return tokens;
    }

    private static List<string> ComputeLCS(List<string> tokens1, List<string> tokens2)
    {
        int m = tokens1.Count, n = tokens2.Count;
        var dp = new int[m + 1, n + 1];

        // Build LCS matrix.
        for (var i = 1; i <= m; i++)
            for (var j = 1; j <= n; j++)
                dp[i, j] = tokens1[i - 1] == tokens2[j - 1]
                    ? dp[i - 1, j - 1] + 1
                    : Math.Max(dp[i - 1, j], dp[i, j - 1]);

        // Backtrack to extract the LCS tokens.
        var lcs = new List<string>();
        int a = m, b = n;
        while (a > 0 && b > 0)
            if (tokens1[a - 1] == tokens2[b - 1])
            {
                lcs.Add(tokens1[a - 1]);
                a--;
                b--;
            }
            else if (dp[a - 1, b] >= dp[a, b - 1])
                a--;
            else
                b--;
        lcs.Reverse();
        return lcs;
    }
}