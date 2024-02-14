using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper.Mappers;

namespace TMProvider
{
    public class TranslationRangeInfo
    {

        public int Length { get; private set; }

        public double Score { get; private set; }

        public int Start { get; private set; }

        /// <summary>
        /// For serialization.
        /// </summary>
        public TranslationRangeInfo() { }


        public TranslationRangeInfo(int start, int length, double score)
        {
            this.Start = start;
            this.Length = length;
            this.Score = score;
        }

        // the memoQ Server API uses the same class
    }
}
