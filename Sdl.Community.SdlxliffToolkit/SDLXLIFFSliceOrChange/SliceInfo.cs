using System;
using System.Collections.Generic;

namespace Sdl.Community.SDLXLIFFSliceOrChange
{
    public class SliceInfo
    {
        public String File { get; set; }
        /// <summary>
        /// list of (trans unit ID and list of segments ID)
        /// </summary>
        public List<KeyValuePair<string, List<string>>> Segments { get; set; }
    }
}