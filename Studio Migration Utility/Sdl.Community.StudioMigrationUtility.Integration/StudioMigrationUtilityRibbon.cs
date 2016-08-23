using Sdl.Community.StudioMigrationUtility.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.StudioMigrationUtility.Integration
{
    [RibbonGroup("Sdl.Community.StudioMigrationUtility", Name = "Studio Migration Utility")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class StudioMigrationUtilityRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.StudioMigrationUtility", Name = "Studio Migration Utility", Icon = "migrate", Description = "Studio Migration Utility")]
    [ActionLayout(typeof(StudioMigrationUtilityRibbon), 20, DisplayType.Large)]
    class StudioMigrationUtilityViewPartAction:AbstractAction
    {
        protected override void Execute()
        {
            var migrateUtility = new MigrateUtility(new StudioVersionService());
            migrateUtility.ShowDialog();

        }
    }
}
