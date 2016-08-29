using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.ReindexTms.Integration
{
    [RibbonGroup("Sdl.Community.ReindexTms", Name = "ReindexTms")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class ReindexTmsRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.ReindexTms", Name = "ReindexTms", Icon = "icon", Description = "ReindexTms")]
    [ActionLayout(typeof(ReindexTmsRibbon), 20, DisplayType.Large)]
    class ReindexTmsViewPartAction:AbstractAction
    {
        protected override void Execute()
        {
            var reindex = new ReindexForm();
            reindex.ShowDialog(); 
        }
    }
}
