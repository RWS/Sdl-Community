using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Telemetry;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ProjectTerms.Plugin.TermbaseIntegrationAction
{
	public class TermbaseGeneration : AbstractBilingualContentHandler
    {
        private readonly ITelemetryTracker _telemetryTracker;
        private readonly Dictionary<string, string> _languages;

		private FileBasedProject _project;
        private ProjectFile _selectedFile;
        private string _termbasePath;

        public TermbaseGeneration()
        {
            _telemetryTracker = new TelemetryTracker();
            _languages = new Dictionary<string, string>();
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
		        _telemetryTracker.StartTrackRequest("Creating termbase");
		        _telemetryTracker.TrackEvent("Creating termbase");

		        var termbases = ConnectToTermbaseLocalRepository();

		        if (File.Exists(_termbasePath) && ExistsProjectTermbase())
		        {
			        return null;
		        }
		        var termbase = termbases.New(Path.GetFileNameWithoutExtension(_selectedFile.LocalFilePath), "Optional Description", termbaseDefinitionPath, _termbasePath);

		        Utils.Utils.RemoveDirectory(Path.GetDirectoryName(termbaseDefinitionPath));
		        return termbase;
	        }
	        catch (Exception e)
	        {
		        _telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message));
		        _telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
		        throw new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message);
	        }
        }

		/// <summary>
		/// Add entries to a given termbase
		/// </summary>
		/// <param name="termbase"></param>
		public void PopulateTermbase(ITermbase termbase)
		{
			try
			{
				_telemetryTracker.StartTrackRequest("Population of the termbase");
				_telemetryTracker.TrackEvent("Population of the termbase");

				var extractor = new ProjectTermsExtractor();

				var targetProjectFiles = _project.GetTargetLanguageFiles();
				var targetFilesReportedToSelectedFile = targetProjectFiles.Where(file => file.Name.Equals(_selectedFile.Name));

				extractor.ExtractBilingualContent(targetFilesReportedToSelectedFile.ToArray());

				var bilingualContentPair = extractor.GetBilingualContentPair();
				if (bilingualContentPair != null)
				{
					foreach (var item in bilingualContentPair.Keys)
					{
						var entry = CreateEntry(item, _selectedFile.SourceFile.Language, bilingualContentPair[item]);
						var oEntries = termbase?.Entries;
						if (oEntries == null) continue;
						oEntries.New(entry, true);
					}
				}
			}
			catch (Exception e)
			{
				_telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_PopulateTermbase + e.Message));
				_telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_PopulateTermbase + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseGenerationException(PluginResources.Error_PopulateTermbase + e.Message);
			}
		}

		/// <summary>
		/// Extract the project languages to include them in termbase
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, string> GetProjectTargetLanguages()
		{
			try
			{
				_telemetryTracker.StartTrackRequest("Extracting project languages in order to complete the .xdt file");
				_telemetryTracker.TrackEvent("Extracting project languages in order to complete the .xdt file", null);

				if (_project == null)
				{
					GetSettings();
				}

				var projectInfo = _project.GetProjectInfo();
				if (projectInfo != null)
				{
					_languages[projectInfo.SourceLanguage.DisplayName] = projectInfo.SourceLanguage.IsoAbbreviation.ToUpper();
					foreach (var targetLang in projectInfo.TargetLanguages)
					{
						_languages[targetLang.DisplayName] = targetLang.IsoAbbreviation.ToUpper();
					}
				}

				return _languages;
			}
			catch (Exception e)
			{
				_telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message));
				_telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message);
			}
		}

		private void GetSettings()
        {
            try
            {
                _telemetryTracker.StartTrackRequest("Termbase settings");
                _telemetryTracker.TrackEvent("Termbase settings", null);

                _project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
                _selectedFile = SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles.FirstOrDefault();
                _termbasePath = Path.Combine(Path.GetTempPath() + "\\Tb", Path.GetFileNameWithoutExtension(_selectedFile?.LocalFilePath) + ".sdltb");
                CreateDirectory();
            }
            catch (Exception e)
            {
                _telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_Settings + e.Message));
                _telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_Settings + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);

                throw new TermbaseGenerationException(PluginResources.Error_Settings + e.Message);
            }
        }

        private void CreateDirectory()
        {
	        if (!Directory.Exists(Path.GetDirectoryName(_termbasePath)))
	        {
		        var directoryName = Path.GetDirectoryName(_termbasePath);
		        if (!string.IsNullOrEmpty(directoryName))
		        {
			        Directory.CreateDirectory(directoryName);
		        }
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
                _telemetryTracker.StartTrackRequest("Connecting to the local repository");
                _telemetryTracker.TrackEvent("Connecting to the local repository");

                var multiTermClientObject = new Application();
                var localRepository = multiTermClientObject.LocalRepository;
                localRepository.Connect(string.Empty, string.Empty);
                return localRepository.Termbases;
            }
            catch(Exception e)
            {
                _telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_ConnectToTermbaseLocalRepository + e.Message));
                _telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_ConnectToTermbaseLocalRepository + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new TermbaseGenerationException(PluginResources.Error_ConnectToTermbaseLocalRepository + e.Message);
            }
        }

		// Check if the project termbase exists
        private bool ExistsProjectTermbase()
        {
	        if (_project != null)
	        {
		        var termbaseName = Path.GetFileNameWithoutExtension(_termbasePath);
		        var projectTermbases = _project.GetTermbaseConfiguration()?.Termbases;
				if(projectTermbases !=null && projectTermbases.Any(t=>t.Name.Equals(termbaseName)))
				{
					return true;
				}
	        }
	        return false;
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
                _telemetryTracker.StartTrackRequest("Creating entry xml element in order to populate the termbase");
                _telemetryTracker.TrackEvent("Creating entry xml element in order to populate the termbase", null);

				return new XElement("conceptGrp",
                    // Add source text
                    new XElement("languageGrp",
                        new XElement("language", new XAttribute("lang", sourceLang.IsoAbbreviation.ToUpper()), new XAttribute("type", sourceLang.DisplayName.ToUpper())),
                        new XElement("termGrp", new XElement("term", sourceText))),
                    // Add target texts
                    targets.Select(item =>
                         new XElement("languageGrp",
                            new XElement("language", new XAttribute("lang", _languages[item.Value]), new XAttribute("type", item.Value)),
                            new XElement("termGrp", new XElement("term", item.Key)))
                    )).ToString();
            } 
            catch(Exception e)
            {
                _telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_CreateEntry + e.Message));
                _telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_CreateEntry + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new TermbaseGenerationException(PluginResources.Error_CreateEntry + e.Message);
            }
        }
    }
}