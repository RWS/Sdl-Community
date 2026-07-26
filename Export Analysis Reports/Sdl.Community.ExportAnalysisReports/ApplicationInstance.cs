using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.ExportAnalysisReports
{
	[ApplicationInitializer]
	internal class ApplicationInstance : IApplicationInitializer
	{
		public void Execute()
		{
			Helpers.Log.Setup();
		}
	}
}
