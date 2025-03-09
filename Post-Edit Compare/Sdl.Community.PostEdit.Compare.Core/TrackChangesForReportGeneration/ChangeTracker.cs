using Oasis.Xliff12;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
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

            var tmSource = GetTmSource(activeSegmentPair, translationOrigin);

            translationOrigin.SetMetaData(Constants.OriginalTuKey, tmSource);
            activeDocument.UpdateSegmentPairProperties(activeSegmentPair,
                activeSegmentPair.Properties);
        }

        private static string GetTmSource(ISegmentPair segmentPair, ITranslationOrigin activeSegmentTranslationOrigin)
        {
            var projController = SdlTradosStudio.Application.GetController<ProjectsController>();
            var currentProject = projController.CurrentProject;

            var sourceLanguage = currentProject.GetProjectInfo().SourceLanguage;
            var targetLanguage = segmentPair.GetProjectFile().Language;

            var minimumScore = GetSettings(currentProject, targetLanguage);

            var tmLangDir = GetLanguageDirection(activeSegmentTranslationOrigin, currentProject, sourceLanguage, targetLanguage);

            var searchResults = tmLangDir.SearchText(new SearchSettings {MinScore = int.Parse(minimumScore.Value)}, segmentPair.Source.ToString());

            foreach (var searchResult in searchResults)
            {
                if (searchResult.MemoryTranslationUnit.TargetSegment.ToString() == segmentPair.Target.ToString())
                {
                    if (activeSegmentTranslationOrigin.MatchPercent == searchResult.ScoringResult.Match)
                        return searchResult.MemoryTranslationUnit.SourceSegment.ToString();
                }
            }

            //x[0].MemoryTranslationUnit
            //ITranslationMemoryLanguageDirection tmLanguageDirection = GetTranslationMemoryLanguageDirection();

            //// Create search options using the source text
            //var searchOptions = new TranslationUnitSearchOptions { SourceSegment = sourceText };

            //// Search the TM for matching TUs
            //var results = tmLanguageDirection.SearchTranslationUnits(searchOptions);
            //var appliedTU = results.FirstOrDefault();

            return null;
        }

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