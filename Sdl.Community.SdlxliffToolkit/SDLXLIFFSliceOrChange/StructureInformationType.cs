using System;
using System.Collections.Generic;

namespace Sdl.Community.SDLXLIFFSliceOrChange
{
    public class StructureInformationType
    {
        public List<KeyValuePair<string, List<string>>> IDs { get; set; }
        public String InternalName { get; set; }
        public String DisplayName { get; set; }
    }
}