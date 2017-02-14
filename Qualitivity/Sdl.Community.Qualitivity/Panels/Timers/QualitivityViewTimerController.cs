using System;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Qualitivity.Panels.Timers
{
    

    [ViewPart(
        Id = "QualitivityViewTimeTrackerController",
        Name = "Activity Tracker",
        Description = "Activity Tracker",
        Icon = "QualitivityTimerController_Icon"
        )]
    [ViewPartLayout(Dock = DockType.Right, ZIndex = 3, LocationByType = typeof(QualitivityViewController))]
    public class QualitivityViewTimerController : AbstractViewPartController
    {
        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            
        }

        public Lazy<QualitivityViewTimerControl> Control = new Lazy<QualitivityViewTimerControl>(() => new QualitivityViewTimerControl());
    }
}