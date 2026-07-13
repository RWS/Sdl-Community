using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using NLog;

namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>Raised when the AgentConnectix install folder cannot be found in the registry.</summary>
    public sealed class ConnectixNotFoundException : Exception
    {
        public ConnectixNotFoundException(string message) : base(message) { }
    }

    /// <summary>Raised when the AgentConnectix console did not return a usable WebSocket port.</summary>
    public sealed class ConnectixPortUnavailableException : Exception
    {
        public ConnectixPortUnavailableException(string message, Exception inner = null) : base(message, inner) { }
    }

    /// <summary>
    /// Discovers the AgentConnectix WebSocket port the way Druide's reference does: read the install
    /// folder from <c>HKLM\SOFTWARE\Druide informatique inc.\Connectix\DossierConnectix</c>, run
    /// <c>AgentConnectixConsole.exe --api</c> hidden, and parse the <c>{"port": N}</c> it prints. This is
    /// an I/O boundary (registry + process), so it is covered by integration/manual testing.
    /// </summary>
    public sealed class ConnectixPortLocator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // A single --api probe gets this long to answer before we treat it as "no port".
        private const int ProbeTimeoutMs = 5000;
        // After starting a session, how long to wait between probes and how many times to retry
        // (~15 s total) while AgentConnectix brings its WebSocket server up.
        private const int PollIntervalMs = 1500;
        private const int StartRetries = 10;

        public int LocatePort()
        {
            var folder = GetInstallFolder();
            var consolePath = Path.Combine(folder, ConnectixConstants.ConsoleExecutable);
            if (!File.Exists(consolePath))
                throw new ConnectixNotFoundException(PluginResources.AntidoteRegistryKeyNotFound_Error);

            // Fast path: AgentConnectix may already be listening.
            var port = Probe(consolePath);
            if (port.HasValue)
                return port.Value;

            // Parity with the old COM launcher: --api only queries a running daemon, it never
            // starts one. When nothing is listening, start a Connectix session ourselves (the
            // same command the login Run key uses) and poll until its WebSocket server is up.
            TryStartSession(folder);
            for (var attempt = 0; attempt < StartRetries; attempt++)
            {
                Thread.Sleep(PollIntervalMs);
                port = Probe(consolePath);
                if (port.HasValue)
                    return port.Value;
            }

            throw new ConnectixPortUnavailableException(PluginResources.AntidoteCannotBeStarted_Error);
        }

        private static int? Probe(string consolePath)
        {
            try
            {
                return RunConsole(consolePath, ProbeTimeoutMs);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "AgentConnectix port probe failed.");
                return null;
            }
        }

        private static string GetInstallFolder()
        {
            string folder;
            using (var key = Registry.LocalMachine.OpenSubKey(ConnectixConstants.RegistryKey))
            {
                folder = key?.GetValue(ConnectixConstants.RegistryFolderValue) as string;
            }

            if (string.IsNullOrEmpty(folder))
                throw new ConnectixNotFoundException(PluginResources.AntidoteRegistryKeyNotFound_Error);

            return folder;
        }

        // Fire-and-forget the same session launcher the login Run key uses
        // (ServiceConnectixAntidote.exe /LancementSession). The command shape is covered by
        // BuildSessionStartInfo; the launch itself is an I/O boundary tested manually.
        private static void TryStartSession(string folder)
        {
            try
            {
                var startInfo = BuildSessionStartInfo(folder);
                if (!File.Exists(startInfo.FileName))
                {
                    Logger.Warn("Connectix session launcher not found at {0}.", startInfo.FileName);
                    return;
                }

                using (Process.Start(startInfo)) { }
                Logger.Info("No AgentConnectix port was available; started a Connectix session.");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not start a Connectix session.");
            }
        }

        // Exposed for unit testing the launch command; building the ProcessStartInfo has no I/O.
        public static ProcessStartInfo BuildSessionStartInfo(string folder)
        {
            return new ProcessStartInfo
            {
                FileName = Path.Combine(folder, ConnectixConstants.ServiceExecutable),
                Arguments = ConnectixConstants.LaunchSessionArgument,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
        }

        private static int? RunConsole(string consolePath, int timeoutMs)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = consolePath,
                Arguments = ConnectixConstants.ApiArgument,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                    return null;

                var output = process.StandardOutput.ReadToEnd();
                if (!process.WaitForExit(timeoutMs))
                {
                    try { process.Kill(); } catch { /* best effort */ }
                    return null;
                }

                return ParsePort(output);
            }
        }

        // Exposed for unit testing the null-port contract; parsing has no I/O so it is safe to test directly.
        public static int? ParsePort(string output)
        {
            if (string.IsNullOrWhiteSpace(output))
                return null;

            try
            {
                var token = JObject.Parse(output)["port"];
                // Daemon-not-ready prints {"port":null}: that is a JValue of type Null, not C# null,
                // so guard on the token type or ToObject<int>() throws "Can not convert Null to Int32".
                if (token != null && token.Type != JTokenType.Null)
                    return token.ToObject<int>();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not parse AgentConnectix port from output: {0}", output);
            }

            return null;
        }
    }
}
