using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Text;
using Sdl.Community.ProjectTerms.Plugin.ExtractTerms;
using System.Linq;
using System;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Telemetry;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class ProjectTermsExtractor
    {
        private ITelemetryTracker telemetryTracker;
        private List<string> sourceTerms;
        private Dictionary<string, List<KeyValuePair<string, string>>> bilingualContentPair;
        public IFileTypeManager FileTypeManager { get; set; }

        public event EventHandler<Utils.ProgressEventArgs> Progress;

        public ProjectTermsExtractor()
        {
            Initialize();
        }

        public void Initialize()
        {
            telemetryTracker = new TelemetryTracker();

            sourceTerms = new List<string>();
            bilingualContentPair = new Dictionary<string, List<KeyValuePair<string, string>>>();
            FileTypeManager = DefaultFileTypeManager.CreateInstance(true);
        }

        private void OnProgress(int percent)
        {
            if (Progress != null)
            {
                Progress(this, new Utils.ProgressEventArgs { Percent = percent });
            }
        }

        private void AddItemToBilingualContentPair(string sourceText, string targetText, string targetLang)
        {
            if (bilingualContentPair.ContainsKey(sourceText))
            {
                var targetTerms = bilingualContentPair[sourceText];
                if (targetTerms.Where(x => x.Key.Equals(targetText) && x.Value.Equals(targetLang)) == null) return;

                if(targetTerms.Where(x => x.Key.Equals(targetText)) != null)
                {
                    targetTerms.Add(new KeyValuePair<string, string>(targetText, targetLang));
                }
            }
            else
            {
                var targetTermsList = new List<KeyValuePair<string, string>>();
                var targetContent = new KeyValuePair<string, string>(targetText, targetLang);
                targetTermsList.Add(targetContent);
                bilingualContentPair.Add(sourceText, targetTermsList);
            }
        }

        public void ExtractBilingualContent(ProjectFile[] targetFiles)
        {
            try
            {
                telemetryTracker.StartTrackRequest("Extracting content from bilingual files");
                telemetryTracker.TrackEvent("Extracting content from bilingual files", null);

                foreach (var file in targetFiles)
                {
                    var converter = FileTypeManager.GetConverter(file.LocalFilePath, (sender, e) => { });
                    var extractor = new TextExtractionBilingualContentHandler();
                    converter.AddBilingualProcessor(new Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.BilingualContentHandlerAdapter(extractor));
                    converter.Parse();

                    for (int i = 0; i < extractor.SourceText.Count; i++)
                    {
                        if (string.IsNullOrWhiteSpace(extractor.SourceText[i]) || string.IsNullOrWhiteSpace(extractor.TargetText[i])) continue;
                        AddItemToBilingualContentPair(extractor.SourceText[i], extractor.TargetText[i], file.Language.DisplayName);
                    }
                }
            }
            catch (Exception e)
            {
                telemetryTracker.TrackException(new ProjectTermsException(PluginResources.Error_ExtractContent + e.Message));
                telemetryTracker.TrackTrace((new ProjectTermsException(PluginResources.Error_ExtractContent + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new ProjectTermsException(PluginResources.Error_ExtractContent + e.Message);
            }
        }

        public void ExtractProjectFilesTerms(List<ProjectFile> projectFiles)
        {
            try
            {
                telemetryTracker.StartTrackRequest("Extracting project terms");
                telemetryTracker.TrackEvent("Extracting project terms", null);

                sourceTerms.Clear();
                var count = 0;
                foreach (ProjectFile file in projectFiles)
                {
                    if (file.Role != FileRole.Translatable) return;

                    FileTypeManager.SettingsBundle = Core.Settings.SettingsUtil.CreateSettingsBundle(null);
                    // disable xliff validation to speed up things
                    FileTypeManager.SettingsBundle.GetSettingsGroup("SDL XLIFF 1.0 v 1.0.0.0").GetSetting<bool>("ValidateXliff").Value = false;
                    var converter = FileTypeManager.GetConverter(file.LocalFilePath, (sender, e) => { });
                    var extractor = new TextExtractionBilingualSourceContentHandler();
                    converter.AddBilingualProcessor(new Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.BilingualContentHandlerAdapter(extractor));
                    converter.Parse();

                    foreach (var text in extractor.SourceText)
                    {
                        foreach (var term in GetTerms(text))
                        {
                            if (term != "")
                            {
                                sourceTerms.Add(term);
                            }
                        }
                    }

                    count++;
                    OnProgress((int)(100.0 * count / (double)projectFiles.Count));
                }
            }
            catch (Exception e)
            {
                telemetryTracker.TrackException(new ProjectTermsException(PluginResources.Error_ExtractContent + e.Message));
                telemetryTracker.TrackTrace((new ProjectTermsException(PluginResources.Error_ExtractContent + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new ProjectTermsException(PluginResources.Error_ExtractContent + e.Message);
            }
        }

        protected virtual IEnumerable<string> GetTerms(string text)
        {
            var term = new StringBuilder();
            var terms = new List<string>();

            foreach (char ch in text)
            {
                if (char.IsLetter(ch))
                {
                    term.Append(ch);
                }
                else if (char.IsDigit(ch))
                {
                    term.Append(ch);
                }
                else
                {
                    if (!term.Equals(""))
                    {
                        terms.Add(term.ToString());
                    }
                    term.Clear();
                }
            }

            if (!term.ToString().Equals("")) terms.Add(term.ToString());

            return terms;
        }

        public List<string> GetSourceTerms()
        {
            return sourceTerms;
        }

        public Dictionary<string, List<KeyValuePair<string, string>>> GetBilingualContentPair()
        {
            return bilingualContentPair;
        }
    }
}
