using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper.Mappers;

namespace TMProvider
{
    public enum ErrorTypes
    {
        TooManySimilarWords = 0,
        TooManySegments = 1,
        Unknown = 3
    }

    public class ConcordanceError
    {
        public ErrorTypes ErrorType { get; private set; }

        public string QueryPart { get; private set; }

        public ConcordanceError(ErrorTypes type, string queryPartField)
        {
            this.ErrorType = type;
            this.QueryPart = queryPartField;
        }


        // memoQ server API uses the same type
    }
}
