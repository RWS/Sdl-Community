using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

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
}