using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.SDLBatchAnonymize
{
	[AutomaticTask("SDL BatchAnonymize",
		"SDL BatchAnonymize",
		"Supports the anonymization of Machine Translation use by removing the MT or NMT markers.",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	public class BatchAnonymizerTask: AbstractFileContentProcessingAutomaticTask
	{
		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new AnonymizerProcessor()));
		}
		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}
	}
}
