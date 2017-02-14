using System;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Qualitivity.Panels.DQF
{
    

    [ViewPart(
        Id = "QualitivityViewDQFController",
        Name = "TAUS DQF",
        Description = "TAUS DQF",
        Icon = "QualitivityDQFController_Icon"
        )]
    [ViewPartLayout(Dock = DockType.Bottom, ZIndex = 1, LocationByType = typeof(QualitivityViewController))]
    public class QualitivityViewDqfController : AbstractViewPartController
    {
        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            
        }

        public static readonly Lazy<QualitivityViewDqfControl> Control = new Lazy<QualitivityViewDqfControl>(() => new QualitivityViewDqfControl());

     
        
    }
}