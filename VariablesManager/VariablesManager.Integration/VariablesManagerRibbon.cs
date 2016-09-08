using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VariablesManager.Integration
{
    [RibbonGroup("Sdl.Community.VariablesManager", Name = "Variables Manager")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class VariablesManagerRibbon: AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.VariablesManager", Name = "Variables Manager",Icon = "variables manager - 128", Description = "Variables Manager")]
    [ActionLayout(typeof(VariablesManagerRibbon), 20, DisplayType.Large)]
    class VariablesManagerViewPartAction : AbstractAction
    {
        protected override void Execute()
        {
            var main = new Form1();
            main.ShowDialog();
        }
    }
}
