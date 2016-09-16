using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdlXliff.Toolkit.Integration.Data;

namespace SdlXliff.Toolkit.Integration.Controls
{
    public class WarningCellData
    {
        /// <summary>
        /// indexes of match the error appeared in
        /// </summary>
        public List<IndexData> MatchIndexes
        {
            get;
            private set;
        }

        /// <summary>
        /// warning message
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        public WarningCellData(IndexData matchIndex, string message)
        {
            MatchIndexes = new List<IndexData>();
            MatchIndexes.Add(matchIndex);

            Message = message;
        }
        public WarningCellData(List<IndexData> matchIndex, string message)
        {
            MatchIndexes = matchIndex;
            Message = message;
        }

        /// <summary>
        /// make deep copy
        /// </summary>
        /// <returns></returns>
        public WarningCellData Clone()
        {
            List<IndexData> indexes = new List<IndexData>();
            foreach (var ind in MatchIndexes)
                indexes.Add(new IndexData(ind.IndexStart, ind.Length));

            return new WarningCellData(indexes, Message);
        }
    }
}
