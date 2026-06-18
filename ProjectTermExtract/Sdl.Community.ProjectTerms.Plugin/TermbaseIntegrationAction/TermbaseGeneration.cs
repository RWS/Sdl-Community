using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Telemetry;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.Terminology.TerminologyProvider.Core;
using Sdl.Terminology.TerminologyProvider.Core.Termbase;
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
		/// Full path of the project termbase (.ttb) created in &lt;project&gt;\Tb.
		/// </summary>
		public string TermbasePath => _termbasePath;

		/// <summary>
		/// Create a file-based (.ttb) termbase in the project's Tb folder using the
		/// managed Terminology Provider API.
		/// </summary>
		/// <returns>The created <see cref="IStudioTermbase"/>, or null if it already exists.</returns>
		public IStudioTermbase CreateTermbase()
		{
			try
			{
				_telemetryTracker.StartTrackRequest("Creating termbase");
				_telemetryTracker.TrackEvent("Creating termbase");

				// Initializes settings (_project, _selectedFile, _termbasePath) and _languages.
				GetProjectTargetLanguages();

				if (File.Exists(_termbasePath) && ExistsProjectTermbase())
				{
					return null;
				}

				// Remove a stale file that is not part of the project so creation can proceed.
				if (File.Exists(_termbasePath))
				{
					File.Delete(_termbasePath);
				}

				var request = new CreateTermbaseRequest
				{
					Name = Path.GetFileNameWithoutExtension(_termbasePath),
					Description = "Project terms termbase",
					Path = Path.GetDirectoryName(_termbasePath),
					Languages = BuildDefinitionLanguages()
				};

				var result = System.Threading.Tasks.Task.Run(() => TerminologyProviderManager.Instance.CreateTermbaseAsync(request))
					.GetAwaiter().GetResult();
				if (result == null || !result.Success)
				{
					var errorMessage = result?.Error ?? string.Empty;
					throw new TermbaseGenerationException(PluginResources.Error_CreateTermbase + errorMessage);
				}

				VerifyTermbasePath();

				return GetStudioTermbase();
			}
			catch (Exception e)
			{
				_telemetryTracker.TrackException(new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message));
				_telemetryTracker.TrackTrace((new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message);
			}
		}

		/// <summary>
		/// Map the project languages to <see cref="DefinitionLanguage"/> entries for termbase creation.
		/// </summary>
		private List<DefinitionLanguage> BuildDefinitionLanguages()
		{
			return _languages.Select(language => new DefinitionLanguage
			{
				Locale = CultureInfo.GetCultureInfo(language.Value).Name,
				Name = language.Key,
				IsBidirectional = true,
				TargetOnly = false
			}).ToList();
		}

		/// <summary>
		/// Confirm the .ttb was created at the expected path; if not, probe the folder for it.
		/// </summary>
		private void VerifyTermbasePath()
		{
			if (File.Exists(_termbasePath))
			{
				return;
			}

			var directory = Path.GetDirectoryName(_termbasePath);
			if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
			{
				return;
			}

			var termbaseName = Path.GetFileNameWithoutExtension(_termbasePath);
			var producedFile = Directory.GetFiles(directory, "*.ttb")
				.FirstOrDefault(file => Path.GetFileNameWithoutExtension(file)
					.Equals(termbaseName, StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty(producedFile))
			{
				_termbasePath = producedFile;
			}
		}

		/// <summary>
		/// Get the shared terminology provider for the created .ttb and cast it to <see cref="IStudioTermbase"/>.
		/// Never dispose the returned provider: it is a shared instance owned by Studio (guide §9).
		/// </summary>
		private IStudioTermbase GetStudioTermbase()
		{
			var termbaseUri = new Uri("ttb." + new Uri(_termbasePath).AbsoluteUri);
			var provider = TerminologyProviderManager.Instance.GetTerminologyProvider(termbaseUri);
			if (provider == null || !provider.Initialize())
			{
				return null;
			}

			return provider as IStudioTermbase;
		}

		/// <summary>
		/// Add entries to a given termbase
		/// </summary>
		/// <param name="termbase"></param>
		public void PopulateTermbase(IStudioTermbase termbase)
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
						termbase?.AddEntry(entry);
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

                _project = StudioContext.ProjectsController.CurrentProject;
                _selectedFile = SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles.FirstOrDefault();
                _termbasePath = Path.Combine(Path.Combine(Path.GetDirectoryName(_project.FilePath), "Tb"), Path.GetFileNameWithoutExtension(_selectedFile?.LocalFilePath) + ".ttb");
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

		// Check if the project termbase exists
		private bool ExistsProjectTermbase()
		{
			if (_project != null)
			{
				var termbaseName = Path.GetFileNameWithoutExtension(_termbasePath);
				var projectTermbases = _project.GetTermbaseConfiguration()?.Termbases;
				if (projectTermbases != null && projectTermbases.Any(t =>
					t.Name.Equals(termbaseName) ||
					t.Name.Equals(Path.GetFileName(_termbasePath))))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Create a typed termbase entry for the source text and its target translations.
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="sourceLang"></param>
		/// <param name="targets"></param>
		/// <returns></returns>
		private Entry CreateEntry(string sourceText, Language sourceLang, List<KeyValuePair<string, string>> targets)
		{
			try
			{
				_telemetryTracker.StartTrackRequest("Creating entry in order to populate the termbase");
				_telemetryTracker.TrackEvent("Creating entry in order to populate the termbase", null);

				var languages = new List<EntryLanguage>
				{
					new EntryLanguage
					{
						Locale = CultureInfo.GetCultureInfo(sourceLang.IsoAbbreviation).Name,
						Name = sourceLang.DisplayName,
						Terms = new List<EntryTerm> { new EntryTerm { Value = sourceText } }
					}
				};

				var targetLanguages = targets
					.GroupBy(item => CultureInfo.GetCultureInfo(_languages[item.Value]).Name)
					.Select(group => new EntryLanguage
					{
						Locale = group.Key,
						Name = group.First().Value,
						Terms = group.Select(item => new EntryTerm { Value = item.Key }).ToList()
					});
				languages.AddRange(targetLanguages);

				return new Entry
				{
					Languages = languages,
					Transactions = new List<EntryTransaction>
					{
						new EntryTransaction { Type = TransactionType.Origination, Date = DateTime.Now }
					}
				};
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