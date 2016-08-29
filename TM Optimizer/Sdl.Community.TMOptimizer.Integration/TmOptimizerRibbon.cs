using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.TMOptimizer.Control;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.TMOptimizer.Integration
{
    [RibbonGroup("Sdl.Community.TmOptimizer", Name = "TmOptimizer")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class TmOptimizerRibbon : AbstractRibbonGroup
    {
    }
    [Action("Sdl.Community.TmOptimizer", Name = "TmOptimizer", Icon = "product",Description = "TmOptimizer")]
    [ActionLayout(typeof(TmOptimizerRibbon), 20, DisplayType.Large)]
    class TmOptimizerViewPart:AbstractAction
    {
        protected override void Execute()
        {
            var main = new MainWindow();
            main.ShowDialog();
        }
    }
}
