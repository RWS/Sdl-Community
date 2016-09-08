using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AntidoteVerifier.Utils
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy =
            new Lazy<Logger>(()=>new Logger());
        public Logger()
        {

        }

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
                @"SDL Community\Antidote Verifier\logs\antidoteVerifier-{Date}.log");
            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(logFilePath)
                //.MinimumLevel.Verbose()
                .CreateLogger();
        }
    }
}
