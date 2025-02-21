using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.SdlFreshstart.Model
{
    public class MultiTermInstalledVersion
    {
        public string Version { get; set; }
        public string PublicVersion { get; set; }
        public string InstallPath { get; set; }
        public Version ExecutableVersion { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} - {1}", PublicVersion, ExecutableVersion);
        }
    }
}
