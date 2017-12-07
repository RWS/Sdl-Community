using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Telemetry;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Sdl.Community.ProjectTerms.Plugin.TermbaseIntegrationAction
{
    public class TermbaseGeneration : AbstractBilingualContentHandler
    {
        private ITelemetryTracker telemetryTracker;

        private FileBasedProject project;
        private ProjectFile selectedFile;
        private string termbasePath;
        private Dictionary<string, string> langs;

        public TermbaseGeneration()
        {
            telemetryTracker = new TelemetryTracker();
            langs = new Dictionary<string, string>();
        }

        private void CheckedTermbaseDirectoryExists(string termbasePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(termbasePath)))
            {
                Utils.Utils.CreateDirectory(Path.GetDirectoryName(termbasePath));
            }
        }

        private void Settings()
        {
            try
            {
                telemetryTracker.StartTrackRequest("Termbase settings");
                telemetryTracker.TrackEvent("Termbase settings", null);

                project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
                selectedFile = SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles.FirstOrDefault();
                termbasePath = Path.Combine(Path.GetTempPath() + "\\Tb", Path.GetFileNameWithoutExtension(selectedFile.LocalFilePath) + ".sdltb");
                if(!Directory.Exists(Path.GetDirectoryName(termbasePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(termbasePath));
                }
            }
            catch (Exception e)
            {
                telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_Settings + e.Message));
                telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_Settings + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);

                throw new TermbaseGenerationException(PluginResources.Error_Settings + e.Message);
            }
        }

        /// <summary>
        /// Connect to local termbase repository
        /// </summary>
        /// <returns></returns>
        private Termbases ConnectToTermbaseLocalRepository()
        {
            try
            {
                telemetryTracker.StartTrackRequest("Connecting to the local repository");
                telemetryTracker.TrackEvent("Connecting to the local repository", null);

                var multiTermClientObject = new Application();
                var localRepository = multiTermClientObject.LocalRepository;
                localRepository.Connect("", "");
                return localRepository.Termbases;
            } catch(Exception e)
            {
                telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_ConnectToTermbaseLocalRepository + e.Message));
                telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_ConnectToTermbaseLocalRepository + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new TermbaseGenerationException(PluginResources.Error_ConnectToTermbaseLocalRepository + e.Message);
            }
        }

        /// <summary>
        /// Create a termbase and add it to Termbases file in sdl project.
        /// </summary>
        /// <param name="termbaseDefinitionPath"></param>
        /// <returns></returns>
        public ITermbase CreateTermbase(string termbaseDefinitionPath)
        {
            try
            {
                telemetryTracker.StartTrackRequest("Creating termbase");
                telemetryTracker.TrackEvent("Creating termbase", null);

                var termbases = ConnectToTermbaseLocalRepository();

                if (File.Exists(termbasePath)) return null;

                var termbase = termbases.New(Path.GetFileNameWithoutExtension(selectedFile.LocalFilePath), "Optional Description", termbaseDefinitionPath, termbasePath);

                Utils.Utils.RemoveDirectory(Path.GetDirectoryName(termbaseDefinitionPath));
                return termbase;
            } catch (Exception e)
            {
                telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message));
                telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message);
            }
        }

        /// <summary>
        /// Create xml entry as string
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="sourceLang"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private string CreateEntry(string sourceText, Language sourceLang, List<KeyValuePair<string, string>> targets)
        {
            try
            {
                telemetryTracker.StartTrackRequest("Creating entry xml element in order to populate the termbase");
                telemetryTracker.TrackEvent("Creating entry xml element in order to populate the termbase", null);

                return new XElement("conceptGrp",
                    // Add source text
                    new XElement("languageGrp",
                        new XElement("language", new XAttribute("lang", sourceLang.IsoAbbreviation.ToUpper()), new XAttribute("type", sourceLang.DisplayName.ToUpper())),
                        new XElement("termGrp", new XElement("term", sourceText))),
                    // Add target texts
                    targets.Select(item =>
                         new XElement("languageGrp",
                            new XElement("language", new XAttribute("lang", langs[item.Value]), new XAttribute("type", item.Value)),
                            new XElement("termGrp", new XElement("term", item.Key)))
                    )
                ).ToString();
            } catch(Exception e)
            {
                telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_CreateEntry + e.Message));
                telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_CreateEntry + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new TermbaseGenerationException(PluginResources.Error_CreateEntry + e.Message);
            }
        }

        /// <summary>
        /// Add entries to a given termbase
        /// </summary>
        /// <param name="oTb"></param>
        public void PopulateTermbase(ITermbase termbase)
        {
            try
            {
                telemetryTracker.StartTrackRequest("Population of the termbase");
                telemetryTracker.TrackEvent("Population of the termbase", null);

                var extractor = new ProjectTermsExtractor();

                var targetProjectFiles = project.GetTargetLanguageFiles();
                var targetFilesReportedToSelectedFile = targetProjectFiles.Where(file => file.Name.Equals(selectedFile.Name));

                extractor.ExtractBilingualContent(targetFilesReportedToSelectedFile.ToArray());

                Dictionary<string, List<KeyValuePair<string, string>>> bilingualContentPair = extractor.GetBilingualContentPair();
                foreach (var item in bilingualContentPair.Keys)
                {
                    var entry = CreateEntry(item, selectedFile.SourceFile.Language, bilingualContentPair[item]);
                    var oEntries = termbase.Entries;
                    oEntries.New(entry, true);
                }
            }
            catch (Exception e)
            {
                telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_PopulateTermbase + e.Message));
                telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_PopulateTermbase + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new TermbaseGenerationException(PluginResources.Error_PopulateTermbase + e.Message);
            }
        }

        /// <summary>
        /// Extract the project languages to include them in termbase
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetProjectLanguages()
        {
            try
            {
                telemetryTracker.StartTrackRequest("Extracting project languages in order to complete the .xdt file");
                telemetryTracker.TrackEvent("Extracting project languages in order to complete the .xdt file", null);

				if (project == null)
				{
					Settings();
				}

				var sourceLanguage = project.GetProjectInfo().SourceLanguage;
                langs[sourceLanguage.DisplayName] = sourceLanguage.IsoAbbreviation.ToUpper();
                var targetLanguages = project.GetProjectInfo().TargetLanguages;
                foreach (var lang in targetLanguages)
                {
                    langs[lang.DisplayName] = lang.IsoAbbreviation.ToUpper();
                }

                return langs;
            }
            catch (Exception e)
            {
                telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message));
                telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message);
            }
        }

		/// <summary>
		/// Extract project languages names (ISOLanguageName (eg: EN) and first name from the entire language (eg: English) to include them in the .xdt schema)
		/// </summary>
		/// <returns>dictionary with all source and target languages of current project</returns>
		public Dictionary<string, string> GetLanguagesForXdt()
		{
			try
			{
				Dictionary<string, string> languages = new Dictionary<string, string> ();

				if (project == null)
				{
					Settings();
				}

				telemetryTracker.StartTrackRequest("Extracting project languages in order to complete the .xdt file");
				telemetryTracker.TrackEvent("Extracting project languages in order to complete the .xdt file", null);

				var sourceLanguage = project.GetProjectInfo().SourceLanguage;
				var targetLanguages = project.GetProjectInfo().TargetLanguages;

				var shortSourceLanguageName = sourceLanguage.CultureInfo != null ? sourceLanguage.CultureInfo.TwoLetterISOLanguageName.ToUpper() : string.Empty;

				languages.Add(sourceLanguage.DisplayName.Split(' ')[0], shortSourceLanguageName);
				foreach(var targetLanguage in targetLanguages)
				{
					string shortTargetLanguage = targetLanguage.CultureInfo != null ? targetLanguage.CultureInfo.TwoLetterISOLanguageName.ToUpper() : string.Empty;
					languages.Add(targetLanguage.DisplayName.Split(' ')[0], shortTargetLanguage);
				}

				return languages;
			}
			catch (Exception e)
			{
				telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message));
				telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message);
			}
		}
	}
}
