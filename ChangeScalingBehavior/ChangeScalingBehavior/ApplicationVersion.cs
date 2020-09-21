using System.Collections.Generic;
using Sdl.Community.Toolkit.Core;

namespace ChangeScalingBehavior
{
    public class ApplicationVersion
    {
        public string InstallPath { get; set; }
        public string PublicVersion { get; set; }
        public string  ResourceFileName { get; set; }
        public List<StudioVersion> StudioVersions  { get; set; }
        public List<MultiTermVersion> MultiTermVersions { get; set; }        
    }
}
