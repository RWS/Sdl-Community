using NLog.Config;
using NLog.Targets;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AntidoteVerifier
{
    public static class Log
    {
        public static void Setup()
        {
            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }

            var config = LogManager.Configuration;

            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore",
                "AntidoteVerifier", "Logs");
            Directory.CreateDirectory(logDirectoryPath);

            var target = new FileTarget
            {
                Name = "AntidoteVerifier",
                FileName = Path.Combine(logDirectoryPath, "AntidoteVerifier.Logs.txt"),
                ArchiveEvery = FileArchivePeriod.Day,
                Encoding = Encoding.UTF8,
                Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
            };

            config.AddTarget(target);
            // Debug and above, deliberately NOT Trace: the transport logs whole frames verbatim at
            // Trace, and one getTextZones response holds the text of every segment (~2 MB on a
            // 12,000-segment document), re-sent whenever Antidote's window moves or the editor
            // scrolls. Debug keeps an abbreviated frame instead (see FrameLog). The live harness
            // configures its own Trace rule when a verbatim capture is wanted.
            // Support capture: set ANTIDOTE_VERIFIER_TRACE=1 before starting Studio to get the frames
            // back verbatim for one session.
            var traceRequested = string.Equals(
                Environment.GetEnvironmentVariable("ANTIDOTE_VERIFIER_TRACE"), "1", StringComparison.Ordinal);

            config.AddRule(traceRequested ? LogLevel.Trace : LogLevel.Debug, LogLevel.Fatal, target,
                "*AntidoteVerifier*");

            LogManager.ReconfigExistingLoggers();
        }
    }
}
