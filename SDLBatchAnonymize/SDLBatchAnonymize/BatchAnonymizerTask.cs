using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.SDLBatchAnonymize
{
	[AutomaticTask("SDL Batch Anonymizer",
		"SDL Batch Anonymizer",
		"Supports the anonymization of Machine Translation use by removing the MT or NMT markers.",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(BatchAnonymizerSettings), typeof(BatchAnonymizerSettingsPage))]

	public class BatchAnonymizerTask: AbstractFileContentProcessingAutomaticTask
	{
		private BatchAnonymizerSettings _settings;

		protected override void OnInitializeTask()
		{
			_settings = GetSetting<BatchAnonymizerSettings>();
		}
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new AnonymizerProcessor(_settings)));
		}
		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}
	}
}
