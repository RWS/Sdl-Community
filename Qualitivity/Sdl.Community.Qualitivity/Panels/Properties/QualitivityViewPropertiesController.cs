using System;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Qualitivity.Panels.Properties
{
    

    [ViewPart(
        Id = "QualitivityViewPropertiesController",
        Name = "Activity Properties",
        Description = "Activity Properties",
        Icon = "QualitivityPropertiesController_Icon"
        )]
    [ViewPartLayout(Dock = DockType.Right, ZIndex = 2, LocationByType = typeof(QualitivityViewController))]
    public class QualitivityViewPropertiesController : AbstractViewPartController
    {
        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            
        }

        public static Lazy<QualitivityViewPropertiesControl> Control = new Lazy<QualitivityViewPropertiesControl>(() => new QualitivityViewPropertiesControl());


        public static TreeView NavigationTreeView { get; set; }
        public static ObjectListView ObjectListView { get; set; }

        public static void UpdateActivityPropertiesViewer()
        {

            Control.Value.UpdateActivityPropertiesViewer();

        }
    }
}