using System.Collections.Generic;

namespace Sdl.Community.StudioMigrationUtility.Model
{
    public class TaskArgument
    {
        public List<Project> Projects { get; set; }

        public List<Project> ProjectToBeMoved{ get; set; }

        public StudioVersion DestinationStudioVersion { get; set; }

        public StudioVersion SourceStudioVersion { get; set; }

        public bool MigrateTranslationMemories { get; set; }
        
        public bool MigrateCustomers { get; set; }
    }
}
