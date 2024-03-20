using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer
{
	[ApplicationInitializer]
	public class DsiViewerInitializer : IApplicationInitializer
	{
		private static EditorController _editorController;

		public static EditorController EditorController => _editorController ??= SdlTradosStudio.Application.GetController<EditorController>();

		public void Execute()
		{
			
		}
	}
}