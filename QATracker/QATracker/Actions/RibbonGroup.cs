using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace QATracker.Actions;

[RibbonGroup("QaTrackerGroup", Name = "QA Tracker")]
[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
public class RibbonGroup : AbstractRibbonGroup
{
}