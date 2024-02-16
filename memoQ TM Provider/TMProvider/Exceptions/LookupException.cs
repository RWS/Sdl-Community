using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace TMProvider
{
    [Serializable]
    public class LookupException: Exception
    {
        public LookupException() : base() { }
        public LookupException(string message) : base(message) { }
        public LookupException(string message, Exception innerException) : base(message, innerException) { }
        protected LookupException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }
}
