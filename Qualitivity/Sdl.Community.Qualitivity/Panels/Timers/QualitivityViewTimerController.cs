using System;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;

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
        protected override IUIControl GetContentControl()
        {
            return Control.Value;
        }

        public void InitializeController() => Initialize();

        protected override void Initialize()
        {
            
        }

        public Lazy<QualitivityViewTimerControl> Control = new Lazy<QualitivityViewTimerControl>(() => new QualitivityViewTimerControl());

        public void StopTracking()
        {
	        Control.Value.StopTracking();
        }
    }
}