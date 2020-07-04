using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SpotCheck
{
    [ViewPart(
        Id = "SDL SpotCheck",
        Name = "SDL SpotCheck", 
        Description = "Mark a percentage of segments for spot-checking",
        Icon="spotcheck"
        )]
    [ViewPartLayout(typeof(FilesController), Dock = DockType.Bottom)]
    class SpotCheckFilesViewPart : AbstractViewPartController
    {
        protected override IUIControl GetContentControl()
        {
            return _control.Value;
        }

        protected override void Initialize()
        {            
        }

        private FilesController FilesController
        {
            get { return SdlTradosStudio.Application.GetController<FilesController>(); }
        }
        
        private static readonly Lazy<SpotCheckViewPartControl> _control = new Lazy<SpotCheckViewPartControl>(() => new SpotCheckViewPartControl());                        
    }
}
