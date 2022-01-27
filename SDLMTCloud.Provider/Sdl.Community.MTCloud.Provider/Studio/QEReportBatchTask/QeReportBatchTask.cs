using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel;
using Sdl.Community.MTCloud.Provider.Service.QEReportCreator;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.MTCloud.Provider.Studio.QEReportBatchTask
{
	[AutomaticTask("MT QE Report",
		"MT QE Report",
		"Creates a Quality Evaluation Report.",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	public class QeReportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		private readonly Dictionary<string, QeFileReport> _qeFileReports = new();
		private readonly QeReportCreator _qeReportCreator = new();

		public override void TaskComplete()
		{
			var groupedByLanguage =
				_qeFileReports.Values.ToList().GroupBy(fr => fr.LanguageDirection.TargetLanguage);
			var fileReportsByLanguage = groupedByLanguage.ToDictionary(frg => frg.Key, frg => frg.ToList());

			foreach (var frg in fileReportsByLanguage)
			{
				CreateReport(frg.Value[0].LanguageDirection, frg.Value.ToList());
			}

			base.TaskComplete();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var showAllQEs = GetStateVariable<bool>("ShowAllQEs");

			var wordCounter = GetWordCounter(projectFile);
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new QeLabelExtractor(projectFile, _qeFileReports, wordCounter, showAllQEs)));
		}

		private T GetStateVariable<T>(string variableName)
		{
			var lwcState = GetLWCProviderState();
			var variable = default(T);
			if (lwcState is null) return variable;

			var showAllQes = JObject.Parse(lwcState)[variableName];
			variable = showAllQes.ToObject<T>();

			return variable;
		}

		private string GetLWCProviderState()
		{
			var tpConfig = Project.GetTranslationProviderConfiguration();

			var lwcTpEntry = tpConfig.Entries.FirstOrDefault(e =>
				e.MainTranslationProvider.Uri.ToString() == PluginResources.SDLMTCloudUri);

			return lwcTpEntry?.MainTranslationProvider.State;
		}

		private void CreateReport(LanguageDirection languageDirection, List<QeFileReport> qeFileReports)
		{
			var xmlReport = _qeReportCreator.CreateXmlReport(Project, TaskFiles, qeFileReports);
			CreateReport("Segments", "Segments' evaluations", xmlReport, languageDirection);
		}
	}
}