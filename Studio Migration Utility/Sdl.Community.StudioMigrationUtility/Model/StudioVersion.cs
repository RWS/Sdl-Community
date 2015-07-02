using System;

namespace Sdl.Community.StudioMigrationUtility.Model
{
    public class StudioVersion
    {
        public string Version { get; set; }
        public string PublicVersion { get; set; }
        public string InstallPath { get; set; }
        public Version ExecutableVersion { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} - {1}", PublicVersion, ExecutableVersion);
        }

        public override bool Equals(object obj)
        {
            var targetStudioVersion = (StudioVersion) obj;
            
            return this.Version.Equals(targetStudioVersion.Version);
        }
    }
}
