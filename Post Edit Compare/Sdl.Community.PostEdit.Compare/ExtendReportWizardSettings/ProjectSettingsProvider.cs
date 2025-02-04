using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sdl.Community.PostEdit.Compare.ExtendReportWizardSettings;

public static class ProjectSettingsProvider
{

    public static List<string> GetProjectAnalysisBands(string originalProjectPath)
    {
        if (string.IsNullOrWhiteSpace(originalProjectPath)) throw new Exception("Original project path is not set.");

        var doc = XDocument.Load(originalProjectPath);

        var analysisBands = doc.Descendants("AnalysisBands")
            .SelectMany(band => band.Elements("AnalysisBand"))
            .Select(b => b.FirstAttribute.Value)
            .ToList();

        return analysisBands;
    }
}