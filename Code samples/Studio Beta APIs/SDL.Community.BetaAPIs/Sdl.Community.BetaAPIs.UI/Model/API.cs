using System.Collections.Generic;

namespace Sdl.Community.BetaAPIs.UI.Model
{
    public class API
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<string> Assemblies { get; set; }

        public bool Enabled { get; set; }
    }
}
