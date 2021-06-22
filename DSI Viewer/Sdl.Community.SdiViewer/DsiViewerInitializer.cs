using Sdl.Community.DsiViewer.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer
{
	[ApplicationInitializer]
	public class DsiViewerInitializer : IApplicationInitializer
	{
		public const string AdequateQuality = "Adequate";
		public const string GoodQuality = "Good";
		public const string NoneAvailable = "N/a";
		public const string PoorQuality = "Poor";

		public static EditorController EditorController { get; private set; }

		public static FilterApplier FilterApplier { get; } = new FilterApplier();

		public void Execute()
		{
			EditorController = SdlTradosStudio.Application.GetController<EditorController>();
		}
	}
}