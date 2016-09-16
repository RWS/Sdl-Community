namespace SdlXliff.Toolkit.Integration.Data
{
    public class IndexData
    {
        /// <summary>
        /// start index of match
        /// </summary>
        public int IndexStart
        {
            get;
            set;
        }

        /// <summary>
        /// number of chars in match
        /// </summary>
        public int Length
        {
            get;
            set;
        }

        /// <summary>
        /// true - these indexes are overlapping with another match in same segment
        /// </summary>
        public bool IsIndexOverlap
        {
            get;
            set;
        }

        /// <summary>
        /// real start index of match in IContainer
        /// </summary>
        public int RealStartIndex
        {
            get;
            set;
        }

        public IndexData(int indexStart, int length)
        {
            IndexStart = indexStart;
            Length = length;
        }
    }
}
