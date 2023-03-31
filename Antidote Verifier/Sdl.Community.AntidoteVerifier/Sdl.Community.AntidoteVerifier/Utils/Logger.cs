using Serilog;
using System;
using System.IO;

namespace Sdl.Community.AntidoteVerifier.Utils
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy =
            new Lazy<Logger>(()=>new Logger());

	    public static void IntializeLogger()
        {
            if(!lazy.IsValueCreated)
            {
                lazy.Value.Initialize();
            }
        }

	    private void Initialize()
	    {
		    var logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			    @"Trados AppStore\Antidote Verifier\logs\antidoteVerifier-{Date}.log");
		    Log.Logger = new LoggerConfiguration()
			    .WriteTo.RollingFile(logFilePath)
			    .CreateLogger();
	    }
    }
}
