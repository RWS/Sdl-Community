using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TMProvider
{
    public class RangeInfo
    {
        public int Start { get; private set; }
        public int Length { get; private set; }

        /// <summary>
        /// For serialization.
        /// </summary>
        public RangeInfo() { }

        public RangeInfo(int start, int length)
        {
            this.Start = start;
            this.Length = length;
        }


        // the memoQ Server API uses the same class

    }
}
