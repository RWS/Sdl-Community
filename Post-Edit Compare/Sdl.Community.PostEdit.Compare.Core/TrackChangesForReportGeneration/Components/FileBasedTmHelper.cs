using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.FileBased;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration.Components
{
    public static class FileBasedTmHelper
    {
        public static SearchResult GetMostProbableMatch(SearchResults searchResults, string target, int matchPercent)
        {
            if (searchResults.Count == 0) return null;
            if (searchResults.Count == 1) return searchResults[0];

            List<int> difference = [];
            foreach (var searchResult in searchResults)
            {
                if (searchResult.MemoryTranslationUnit.TargetSegment.ToString() != target) continue;
                difference.Add(Math.Abs(searchResult.ScoringResult.Match - matchPercent));
            }

            var minDiff = (100, 0);
            for (var i = 0; i < difference.Count; i++)
            {
                var diff = difference[i];
                minDiff = (Math.Min(minDiff.Item1, diff), i);
            }

            return searchResults[minDiff.Item2];
        }

        public static SearchResults GetTmMatches(string source, Language targetLanguage, string tmPath, FileBasedProject currentProject)
        {
            var sourceLanguage = currentProject.GetProjectInfo().SourceLanguage;

            var tmLangDir = GetLanguageDirection(tmPath, sourceLanguage, targetLanguage);
            var minimumScore = GetSettings(currentProject, targetLanguage);

            var hasScore = int.TryParse(minimumScore.Value, out var minScore);
            return tmLangDir.SearchText(new SearchSettings { MinScore = hasScore ? minScore : 70 }, source);
        }

        public static string GetTmPath(string originSystem, FileBasedProject currentProject)
        {
            var segTransOrigName = originSystem;
            var translationProviderCascadeEntries = currentProject.GetTranslationProviderConfiguration().Entries;
            var tms = translationProviderCascadeEntries.Select(tm => tm.MainTranslationProvider.Uri.LocalPath).ToList();
            var tmPath = tms.FirstOrDefault(tm => Path.GetFileNameWithoutExtension(tm) == segTransOrigName);
            return tmPath;
        }

        public static List<DiffSegment> GetTuWithTrackedChanges(SearchResult searchResult, string source) =>
                                    DiffMerger.MergeSegments(searchResult.MemoryTranslationUnit.SourceSegment.ToString(),
                source);

        private static ITranslationMemoryLanguageDirection GetLanguageDirection(string tmPath, Language sourceLanguage,
                            Language targetLanguage)
        {
            var tm = new FileBasedTranslationMemory(tmPath);
            var tmLangDir = tm.GetLanguageDirection(new LanguagePair(sourceLanguage, targetLanguage));
            return tmLangDir;
        }

        private static Setting<string> GetSettings(FileBasedProject currentProject, Language targetLanguage)
        {
            var settings = currentProject.GetSettings(targetLanguage);
            var searchSettings = settings.GetSettingsGroup("TranslationMemorySettings");
            var minimumScore = searchSettings.GetSetting<string>("TranslationMinimumMatchValue");
            return minimumScore;
        }
    }
}