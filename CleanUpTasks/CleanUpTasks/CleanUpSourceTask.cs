using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using SDLCommunityCleanUpTasks.Utilities;
using Timer = System.Timers.Timer;

namespace SDLCommunityCleanUpTasks
{
	[AutomaticTask("Cleanup Source",
	"Cleanup Source",
	"Cleans up source segments before translation",
	GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(CleanUpSourceSettings), typeof(CleanUpSourceSettingsPage))]
	public class CleanUpSourceTask : AbstractFileContentProcessingAutomaticTask
	{
		private readonly List<string> reports = new List<string>();
		private IXmlReportGenerator reportGenerator = null;
		private CleanUpSourceSettings settings = null;

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}

		public override void TaskComplete()
		{
			CreateReport("Cleanup Source Report", "Cleanup Source Batch Task Results", reportGenerator.ToString());
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			SegmentProcessor segProcessor = new SegmentProcessor(settings);
			segProcessor.Run(multiFileConverter, Project, projectFile, reportGenerator);
		}

		protected override void OnInitializeTask()
		{
			settings = GetSetting<CleanUpSourceSettings>();

			var logFolder = Path.Combine(GetProjectFolder(), "Cleanup Logs");
			reportGenerator = new XmlReportGenerator(logFolder);
		}

		private string GetProjectFolder()
		{
			var first = TaskFiles.FirstOrDefault();

			return Path.GetDirectoryName(first.LocalFilePath);
		}
	}
}