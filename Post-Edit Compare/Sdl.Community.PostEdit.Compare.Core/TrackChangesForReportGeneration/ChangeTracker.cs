using Newtonsoft.Json;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration.Components;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Community.PostEdit.Compare.Core.TrackChangesForReportGeneration
{
    public static class ChangeTracker
    {
        private static EditorEventListener EditorEventListener { get; set; } = new();

        public static void TrackChosenTUsFromTMs()
        {
            EditorEventListener.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
            EditorEventListener.ActiveSegmentConfirmationLevelChanged += ActiveDocument_ActiveSegmentConfirmationLevelChanged;
            EditorEventListener.StartListening();
        }

        /// <summary>
        /// Scenario: After pretranslation, when the user opens a segment to edit, we must record its data first.
        /// </summary>
        private static void ActiveDocument_ActiveSegmentChanged() => AddChosenTuToMetadata();



        private static void ActiveDocument_ActiveSegmentConfirmationLevelChanged() => AddChosenTuToMetadata();

        private static void AddChosenTuToMetadata()
        {
            var activeDocument = EditorEventListener.ActiveDocument;

            var activeSegmentPair = activeDocument.GetActiveSegmentPair();

            var isLocked = activeSegmentPair?.Properties.IsLocked ?? true;
            if (isLocked) return;

            var translationOrigin = activeSegmentPair?.Properties.TranslationOrigin;
            if (translationOrigin is null) return;

            if (translationOrigin.OriginType?.ToUpperInvariant() != "TM" ||
                translationOrigin.MetaDataContainsKey(Constants.OriginalTuKey)) return;

            var projController = SdlTradosStudio.Application.GetController<ProjectsController>();
            var currentProject = projController.CurrentProject;

            var tmPath = FileBasedTmHelper.GetTmPath(translationOrigin.OriginSystem, currentProject);
            if (tmPath is null)
            {
                List<string> variables = [];
                variables.AddVariable("Origin System", translationOrigin.OriginSystem);
                variables.AddVariable("Current Project", currentProject.FilePath);
                ErrorHandler.Log("TM Path is null", []);
                return;
            }

            var difference = GetTuSourceDocSourceDifference(activeSegmentPair, translationOrigin.MatchPercent, tmPath, currentProject);

            translationOrigin.SetMetaData(Constants.OriginalTuKey, JsonConvert.SerializeObject(difference));
            activeDocument.UpdateSegmentPairProperties(activeSegmentPair, activeSegmentPair.Properties);
        }





        private static List<DiffSegment> GetTuSourceDocSourceDifference(ISegmentPair segmentPair, int matchPercent,
            string tmPath, FileBasedProject currentProject)
        {
            var searchResults = FileBasedTmHelper.GetTmMatches(segmentPair.Source.ToString(),
                segmentPair.GetProjectFile().Language, tmPath, currentProject);
            var searchResult =
                FileBasedTmHelper.GetMostProbableMatch(searchResults, segmentPair.Target.ToString(), matchPercent);
            return searchResult == null ? [] : FileBasedTmHelper.GetTuWithTrackedChanges(searchResult, segmentPair.Source.ToString());
        }


    }
}