using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace SdlXliffToolkit
{
	[ViewPart(Id = "Sdl.Community.SdlToolkit", Name = "Toolkit", Description = "Toolkit", Icon = "toolkit- 128")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	internal class SdlToolkitEditorViewPart : AbstractViewPartController
	{
		private static readonly Lazy<global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange> _control = new Lazy<global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange>(() => new global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange());

		protected override IUIControl GetContentControl()
		{
			return _control.Value;
		}

		protected override void Initialize()
		{
		}
	}

	[Action("Sdl.Community.SdlToolkit.Editor", typeof(EditorController), Name = "SdlToolkit", Icon = "toolkit- 128", Description = "SdlToolkit")]
	[ActionLayout(typeof(SdlToolkitRibbon), 20, DisplayType.Large)]
	internal class SdlToolkitEditorViewPartAction : AbstractAction
	{
		protected override void Execute()
		{
			ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitEditorViewPart>().Show();
		}
	}

	[ViewPart(Id = "Sdl.Community.SdlToolkit.FilesView", Name = "Toolkit", Description = "Toolkit", Icon = "toolkit- 128")]
	[ViewPartLayout(typeof(FilesController), Dock = DockType.Bottom)]
	internal class SdlToolkitFilesViewPart : AbstractViewPartController
	{
		private static readonly Lazy<global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange> _control = new Lazy<global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange>(() => new global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange());

		protected override IUIControl GetContentControl()
		{
			return _control.Value;
		}

		protected override void Initialize()
		{
		}
	}

	[Action("Sdl.Community.SdlToolkit.FilesAction", typeof(FilesController), Name = "SdlToolkit", Icon = "toolkit- 128", Description = "SdlToolkit")]
	[ActionLayout(typeof(SdlToolkitRibbon), 20, DisplayType.Large)]
	internal class SdlToolkitFilesViewPartAction : AbstractAction
	{
		protected override void Execute()
		{
			ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitFilesViewPart>().Show();
		}
	}

	[ViewPart(Id = "Sdl.Community.SdlToolkit.ProjectsView", Name = "Toolkit", Description = "Toolkit", Icon = "toolkit- 128")]
	[ViewPartLayout(typeof(ProjectsController), Dock = DockType.Bottom)]
	internal class SdlToolkitProjectsViewPart : AbstractViewPartController
	{
		private static readonly Lazy<global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange> _control = new Lazy<global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange>(() => new global::SDLXLIFFSliceOrChange.SdlxliffSliceOrChange());

		protected override IUIControl GetContentControl()
		{
			return _control.Value;
		}

		protected override void Initialize()
		{
		}
	}

	[Action("Sdl.Community.SdlToolkit.ProjectsAction", typeof(ProjectsController), Name = "SdlToolkit", Icon = "toolkit- 128", Description = "SdlToolkit")]
	[ActionLayout(typeof(SdlToolkitRibbon), 20, DisplayType.Large)]
	internal class SdlToolkitProjectsViewPartAction : AbstractAction
	{
		protected override void Execute()
		{
			ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitProjectsViewPart>().Show();
		}
	}

	[RibbonGroup("Sdl.Community.SdlToolkit", Name = "SdlToolkit", Icon = "toolkit- 128")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	internal class SdlToolkitRibbon : AbstractRibbonGroup
	{
	}
}