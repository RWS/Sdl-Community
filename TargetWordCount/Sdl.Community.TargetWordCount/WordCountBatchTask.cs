using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.TargetWordCount
{
	[AutomaticTask("TargetWordCountID",
		"Target Word Count",
		"Counts number of words in target",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(WordCountBatchTaskSettings), typeof(WordCountBatchTaskSettingsPage))]
	public class WordCountBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		private readonly Dictionary<string, List<ISegmentWordCounter>> counters = new Dictionary<string, List<ISegmentWordCounter>>();
		private readonly Dictionary<string, LanguageDirection> keys = new Dictionary<string, LanguageDirection>();
		private IWordCountBatchTaskSettings settings = null;
		private SegmentWordCounter tempCounter = null;

		private string CreateKey(LanguageDirection langDir)
		{
			return langDir.SourceLanguage.IsoAbbreviation + langDir.TargetLanguage.IsoAbbreviation;
		}

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var key = CreateKey(projectFile.GetLanguageDirection());

			if (counters.ContainsKey(key))
			{
				counters[key].Add(tempCounter);
			}
			else
			{
				counters.Add(key, new List<ISegmentWordCounter>() { tempCounter });
				keys.Add(key, projectFile.GetLanguageDirection());
			}

			return false;
		}

		public override void TaskComplete()
		{
			base.TaskComplete();

			foreach (var key in counters.Keys)
			{
				var report = ReportGenerator.Generate(counters[key], settings);

				var langDirection = keys[key];

				CreateReport(CreateReportName(langDirection), "Count for each file", report, langDirection);
				ReportGenerator.GenerateHelixReport(langDirection);
			}

		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			tempCounter = new SegmentWordCounter(projectFile.Name, settings, GetWordCounter(projectFile));
			multiFileConverter.AddBilingualProcessor(tempCounter);
		}

		protected override void OnInitializeTask()
		{
			settings = GetSetting<WordCountBatchTaskSettings>();
		}

		private string CreateReportName(LanguageDirection langDirection)
		{
			return $"Target Word Count {langDirection.SourceLanguage.IsoAbbreviation}_{langDirection.TargetLanguage.IsoAbbreviation}";
		}
	}
}