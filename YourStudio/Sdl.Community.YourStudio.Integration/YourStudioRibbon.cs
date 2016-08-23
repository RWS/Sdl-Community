using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.YourStudio.Integration
{
    [RibbonGroup("Sdl.Community.YourStudio", Name = "YourStudio")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class YourStudioRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.YourStudio", Name = "YourStudio", Icon = "yoursdlstudio", Description = "YourStudio")]
    [ActionLayout(typeof(YourStudioRibbon), 20, DisplayType.Large)]
    class YourStudioViewPartAction:AbstractAction
    {
        protected override void Execute()
        {
            var yourStudio = new Form1();
            yourStudio.ShowDialog();
        }
    }
}
