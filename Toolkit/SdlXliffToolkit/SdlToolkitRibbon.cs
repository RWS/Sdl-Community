using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;


namespace SdlXliffToolkit
{


    [ViewPart(Id = "Toolkit",Name = "Toolkit",Description = "Toolkit",Icon = "toolkit- 128")]
    [ViewPartLayout(typeof(EditorController),Dock = DockType.Bottom  )]
    class SdlToolkitEditorViewPart : AbstractViewPartController
    {
        protected override void Initialize()
        {
           
        }

        protected override Control GetContentControl()
        {

            return _control.Value;
        }

       
        private static readonly Lazy<global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange> _control = new Lazy<global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange>(() => new global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange());
    }

    [Action("Sdl.Community.SdlToolkit.Editor", typeof(EditorController), Name = "SdlToolkit", Icon = "toolkit- 128", Description = "SdlToolkit")]
    [ActionLayout(typeof(SdlToolkitRibbon), 20, DisplayType.Large)]
    class SdlToolkitEditorViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitEditorViewPart>().Show();
        }

    }

    [ViewPart(Id = "Toolkit", Name = "Toolkit", Description = "Toolkit", Icon = "toolkit- 128")]
    [ViewPartLayout(typeof(ProjectsController), Dock = DockType.Bottom)]
    class SdlToolkitProjectsViewPart : AbstractViewPartController
    {
        protected override void Initialize()
        {
           
        }

        protected override Control GetContentControl()
        {
            return _control.Value;
        }

        private static readonly Lazy<global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange> _control = new Lazy<global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange>(() => new global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange());
    }


    [Action("Sdl.Community.SdlToolkit.Projects",typeof(ProjectsController), Name = "SdlToolkit", Icon = "toolkit- 128", Description = "SdlToolkit")]
    [ActionLayout(typeof(SdlToolkitRibbon), 20, DisplayType.Large)]
    class SdlToolkitProjectsViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitProjectsViewPart>().Show();
        }

    }

    [ViewPart(Id = "Toolkit", Name = "Toolkit", Description = "Toolkit", Icon = "toolkit- 128")]
    [ViewPartLayout(typeof(FilesController), Dock = DockType.Bottom)]
    class SdlToolkitFilesViewPart : AbstractViewPartController
    {
        protected override void Initialize()
        {

        }

        protected override Control GetContentControl()
        {
            return _control.Value;
        }

        private static readonly Lazy<global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange> _control = new Lazy<global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange>(() => new global::SDLXLIFFSliceOrChange.SDLXLIFFSliceOrChange());
    }

    [Action("Sdl.Community.SdlToolkit.Files",typeof(FilesController), Name = "SdlToolkit", Icon = "toolkit- 128", Description = "SdlToolkit")]
    [ActionLayout(typeof(SdlToolkitRibbon), 20, DisplayType.Large)]
    class SdlToolkitFilesViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            ApplicationHost<SdlTradosStudioApplication>.Application.GetController<SdlToolkitFilesViewPart>().Show();
        }

    }


    [RibbonGroup("Sdl.Community.SdlToolkit", Name = "SdlToolkit",Icon = "toolkit- 128")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class SdlToolkitRibbon : AbstractRibbonGroup
    {
    }

 
}
