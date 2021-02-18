﻿using Sdl.Community.DsiViewer.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer
{
	[ApplicationInitializer]
	public class DsiViewerInitializer : IApplicationInitializer
	{
		public static EditorController EditorController { get; private set; }

		public static FilterApplier FilterApplier { get; } = new();

		public void Execute()
		{
			EditorController = SdlTradosStudio.Application.GetController<EditorController>();
		}
	}
}