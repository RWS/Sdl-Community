using System;
using NLog;

namespace TMX_Lib.Db
{
    public class TmxException : Exception
    {
	    private static readonly Logger log = LogManager.GetCurrentClassLogger();
       
	    public TmxException(string s) : base(s)
        {
	        log.Error($"{s}");
	        LogManager.Flush();
        }
		public TmxException(string s, Exception e) : base(s, e)
        {
	        log.Error($"Exception {s} : {(e != null ? "original msg:" + e.ToString() : "")}");
	        LogManager.Flush();
        }
	}
}
