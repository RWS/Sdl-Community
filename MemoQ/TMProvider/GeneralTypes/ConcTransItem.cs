using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper.Mappers;

namespace TMProvider
{
    public class ConcTransItem
    {
        public string Expression { get; private set; }

        public double Score { get; private set; }

        public ConcTransItem() { }

        public ConcTransItem(string expression, double score)
        {
            this.Expression = expression;
            this.Score = score;
        }

        // the memoQ Server API uses the same class
    }
}
