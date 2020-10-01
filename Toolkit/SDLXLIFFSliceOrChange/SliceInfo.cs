using System.Collections.Generic;

namespace SDLXLIFFSliceOrChange
{
    public class SliceInfo
    {
        public string File { get; set; }

        /// <summary>
        /// list of (trans unit ID and list of segments ID)
        /// </summary>
        public List<KeyValuePair<string, List<string>>> Segments { get; set; }
    }
}