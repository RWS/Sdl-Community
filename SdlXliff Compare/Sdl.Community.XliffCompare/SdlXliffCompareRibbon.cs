using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace Sdl.Community.XliffCompare
{

    [ViewPart(Id = "SdlXliffCompare.Editor", Name = "SdlXliffCompare", Description = "SdlXliffCompare", Icon = "SDLXLIFFCompare")]
    [ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
    class SdlXliffCompareRibbon : AbstractViewPartController
    {
        private static readonly Lazy<FormMain> _control = new Lazy<FormMain>(() => new FormMain());
        protected override void Initialize()
        {

        }

        protected override Control GetContentControl()
        {
            return _control.Value;
        }
    }

    [ViewPart(Id = "SdlXliffCompare.Projects", Name = "SdlXliffCompare", Description = "SdlXliffCompare", Icon = "SDLXLIFFCompare")]
    [ViewPartLayout(typeof(ProjectsController), Dock = DockType.Bottom)]
    class SdlXliffCompareProjectsViewPart : AbstractViewPartController
    {
        private static readonly Lazy<FormMain> _control = new Lazy<FormMain>(() => new FormMain());
        protected override void Initialize()
        {

        }

        protected override Control GetContentControl()
        {
            return _control.Value;
        }
    }

    [Action("SdlXliffCompare.Projects", typeof(ProjectsController), Name = "SdlXliffCompare", Icon = "SDLXLIFFCompare", Description = "SdlXliffCompare")]
    [ActionLayout(typeof(SdlXliffCompareRibbon), 20, DisplayType.Large)]
    class SdlXliffCompareViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlXliffCompareProjectsViewPart>().Show();
        }
    }

    [ViewPart(Id = "SdlXliffCompare.Files", Name = "SdlXliffCompare", Description = "SdlXliffCompare", Icon = "SDLXLIFFCompare")]
    [ViewPartLayout(typeof(FilesController), Dock = DockType.Bottom)]
    class SdlXliffCompareFilesViewPart : AbstractViewPartController
    {
        private static readonly Lazy<FormMain> _control = new Lazy<FormMain>(() => new FormMain());
        protected override void Initialize()
        {

        }

        protected override Control GetContentControl()
        {
            return _control.Value;
        }
    }

    [Action("SdlXliffCompare.Files", typeof(FilesController), Name = "SdlXliffCompare", Icon = "SDLXLIFFCompare", Description = "SdlXliffCompare")]
    [ActionLayout(typeof(SdlXliffCompareRibbon), 20, DisplayType.Large)]
    class SdlXliffCompareFilesViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlXliffCompareFilesViewPart>().Show();
        }
    }
}
