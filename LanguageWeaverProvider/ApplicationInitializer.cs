using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace LanguageWeaverProvider
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
		private const string BatchProcessing = "batch processing";
		private const string CreateNewProject = "create a new project";

		public void Execute()
		{
			RatedSegments.Segments = new List<RatedSegment>();
		}

		public static Window GetBatchTaskWindow() =>
			Application
		   .Current
			.Windows
			.Cast<Window>()
			.FirstOrDefault(window => window.Title.ToLower() == BatchProcessing
								   || window.Title.ToLower().Contains(CreateNewProject));
	}
}