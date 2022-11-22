using System;

namespace TMX_Lib.Db
{
    public class TmxException : Exception
    {
        public TmxException(string s) : base(s)
        {
        }
        public TmxException(string s, Exception e) : base(s, e)
        {
        }
    }
}
