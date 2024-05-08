using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XliffCompare
{
	public abstract class SDLXLIFFViewPartController : AbstractViewPartController
	{
		public FormMain Control => (FormMain)GetContentControl();
	}

	[ViewPart(Id = "SdlXliffCompare.Files", Name = "SdlXliffCompare", Description = "SdlXliffCompare", Icon = "SDLXLIFFCompare")]
	[ViewPartLayout(typeof(FilesController), Dock = DockType.Bottom)]
	public class SdlXliffCompareFilesViewPart : SDLXLIFFViewPartController
	{
		private static readonly Lazy<FormMain> _control = new Lazy<FormMain>(() => new FormMain());

		protected override IUIControl GetContentControl()
		{
			return _control.Value;
		}

		protected override void Initialize()
		{
		}
	}

	[ViewPart(Id = "SdlXliffCompare.Projects", Name = "SdlXliffCompare", Description = "SdlXliffCompare", Icon = "SDLXLIFFCompare")]
	[ViewPartLayout(typeof(ProjectsController), Dock = DockType.Bottom)]
	public class SdlXliffCompareProjectsViewPart : SDLXLIFFViewPartController
	{
		private static readonly Lazy<FormMain> _control = new Lazy<FormMain>(() => new FormMain());

		protected override IUIControl GetContentControl()
		{
			return _control.Value;
		}

		protected override void Initialize()
		{
		}
	}

	[ViewPart(Id = "SdlXliffCompare.Editor", Name = "SdlXliffCompare", Description = "SdlXliffCompare", Icon = "SDLXLIFFCompare")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public class SdlXliffCompareEditorViewPart : SDLXLIFFViewPartController
	{
		private static readonly Lazy<FormMain> _control = new Lazy<FormMain>(() => new FormMain());

		protected override IUIControl GetContentControl()
		{
			return _control.Value;
		}

		protected override void Initialize()
		{
		}
	}
}