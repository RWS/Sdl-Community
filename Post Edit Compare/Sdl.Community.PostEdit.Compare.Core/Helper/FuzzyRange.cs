using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Sdl.Community.PostEdit.Compare.Core.Helper;

public class FuzzyRange
{
    public static List<string> GetFuzzyRangesFromProjectId(string projectId)
    {
        var ranges = new List<string> { "All" };
        if (string.IsNullOrWhiteSpace(projectId))
        {
            MessageBox.Show(
                "Fuzzy ranges cannot be used for this comparison\nGet project analysis bands failed: projectId is not set.",
                "Comparison", MessageBoxButton.OK, MessageBoxImage.Warning);
            return ranges;
        }

        var analysisBands = GetProjectAnalysisBandsFromId(projectId);
        ranges.AddRange(FuzzyRange.GetFuzzyRanges(analysisBands));

        return ranges;
    }

    public static List<string> GetFuzzyRangesFromProjectPath(string originalProjectPath)
    {
        var ranges = new List<string> { "All" };
        if (string.IsNullOrWhiteSpace(originalProjectPath))
        {
            MessageBox.Show(
                "Fuzzy ranges cannot be used for this comparison\nGet project analysis bands failed: original project path is not set.",
                "Comparison", MessageBoxButton.OK, MessageBoxImage.Warning);
            return ranges;
        }

        var analysisBands = GetProjectAnalysisBandsFromProjectPath(originalProjectPath);
        ranges.AddRange(FuzzyRange.GetFuzzyRanges(analysisBands));

        return ranges;
    }

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

    public static bool IsInFuzzyRange(int segmentPercentage, string fuzzyRange)
    {
        if (fuzzyRange.Contains("All"))
            return true;
        GetLimits(fuzzyRange, out var min, out var max);

        if (min > -1 && max > -1)
        {
            if (segmentPercentage >= min &&
                segmentPercentage <= max)
                return true;
        }
        else if (segmentPercentage >= min)
            return true;

        return false;
    }

    private static List<string> GetFuzzyRanges(List<string> analysisBands)
    {
        var ranges = new List<string>();
        for (var index = 0; index < analysisBands.Count - 1; index++)
            ranges.Add($"{analysisBands[index]} - {analysisBands[index + 1]}");
        ranges.Add($"> {analysisBands[analysisBands.Count - 1]}");
        return ranges;
    }

    private static List<string> GetProjectAnalysisBandsFromId(string projectId)
    {
        var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
        var projectPath = projectsController.GetAllProjects()
            .FirstOrDefault(proj => proj.GetProjectInfo().Id.ToString() == projectId)
            ?.FilePath;

        return GetProjectAnalysisBandsFromProjectPath(projectPath);
    }

    private static List<string> GetProjectAnalysisBandsFromProjectPath(string originalProjectPath)
    {
        if (string.IsNullOrWhiteSpace(originalProjectPath))
            return null;
        var doc = XDocument.Load(originalProjectPath);

        var analysisBands = doc.Descendants("AnalysisBands")
            .SelectMany(band => band.Elements("AnalysisBand"))
            .Select(b => b.FirstAttribute.Value)
            .ToList();

        return analysisBands;
    }
}