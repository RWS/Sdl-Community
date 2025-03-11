using Newtonsoft.Json;
using Sdl.Community.PostEdit.Compare.Core.SDLXLIFF;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration
{
    public static class ChangeTracker
    {
        private static EditorController EditorController => SdlTradosStudio.Application.GetController<EditorController>();

        public static void TrackChosenTUsFromTMs() =>
            EditorController.TranslationResultsController.TranslationFinished += OnSegmentsConfirmationLevelChanged;

        private static void OnSegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            var activeDocument = EditorController.ActiveDocument;

            var activeSegmentPair = activeDocument.GetActiveSegmentPair();
            var translationOrigin = activeSegmentPair?.Properties.TranslationOrigin;
            if (translationOrigin is null) return;

            if (translationOrigin.OriginType?.ToUpperInvariant() != "TM" ||
                translationOrigin.MetaDataContainsKey(Constants.OriginalTuKey)) return;

            var difference = GetTuSourceDocSourceDifference(activeSegmentPair, translationOrigin);

            translationOrigin.SetMetaData(Constants.OriginalTuKey, JsonConvert.SerializeObject(difference));
            activeDocument.UpdateSegmentPairProperties(activeSegmentPair,
                activeSegmentPair.Properties);
        }

        private static List<DiffSegment> GetTuSourceDocSourceDifference(ISegmentPair segmentPair, ITranslationOrigin translationOrigin)
        {
            var searchResults = GetTmMatches(segmentPair, translationOrigin);
            var searchResult = GetMostProbableMatch(searchResults, segmentPair, translationOrigin);
            return searchResult == null ? [] : GetTuWithTrackedChanges(searchResult, segmentPair);
        }


        //Since we cannot access the TM Results windows, we must infer what the user chose by redoing the search on the same TM and comparing the results
        private static SearchResult GetMostProbableMatch(SearchResults searchResults, ISegmentPair segmentPair, ITranslationOrigin translationOrigin)
        {
            foreach (var searchResult in searchResults)
            {
                if (searchResult.MemoryTranslationUnit.TargetSegment.ToString() !=
                    segmentPair.Target.ToString()) continue;

                if (translationOrigin.MatchPercent == searchResult.ScoringResult.Match)
                    return  searchResult;
            }

            return null;
        }

        private static SearchResults GetTmMatches(ISegmentPair segmentPair,
            ITranslationOrigin activeSegmentTranslationOrigin)
        {
            var projController = SdlTradosStudio.Application.GetController<ProjectsController>();
            var currentProject = projController.CurrentProject;

            var sourceLanguage = currentProject.GetProjectInfo().SourceLanguage;
            var targetLanguage = segmentPair.GetProjectFile().Language;

            var tmLangDir = GetLanguageDirection(activeSegmentTranslationOrigin, currentProject, sourceLanguage, targetLanguage);
            var minimumScore = GetSettings(currentProject, targetLanguage);

            return tmLangDir.SearchText(new SearchSettings {MinScore = int.Parse(minimumScore.Value)}, segmentPair.Source.ToString());
        }

        private static List<DiffSegment> GetTuWithTrackedChanges(SearchResult searchResult, ISegmentPair pair) =>
            DiffMerger.MergeSegments(searchResult.MemoryTranslationUnit.SourceSegment.ToString(),
                pair.Source.ToString());

        private static ITranslationMemoryLanguageDirection GetLanguageDirection(
            ITranslationOrigin activeSegmentTranslationOrigin, FileBasedProject currentProject, Language sourceLanguage,
            Language targetLanguage)
        {
            var segTransOrigName = activeSegmentTranslationOrigin.OriginSystem;
            var translationProviderCascadeEntries = currentProject.GetTranslationProviderConfiguration().Entries;
            var tms = translationProviderCascadeEntries.Select(tm => tm.MainTranslationProvider.Uri.LocalPath).ToList();
            var tmPath = tms.FirstOrDefault(tm => Path.GetFileNameWithoutExtension(tm) == segTransOrigName);

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