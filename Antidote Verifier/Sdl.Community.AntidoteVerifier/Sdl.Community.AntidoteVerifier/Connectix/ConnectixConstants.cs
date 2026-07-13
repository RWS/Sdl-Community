namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>
    /// Constants for locating and talking to the AgentConnectix process. The agent runs a local
    /// WebSocket server; its install folder is read from the registry and its console is asked for the
    /// live port via <c>AgentConnectixConsole.exe --api</c>. Tool ids live in
    /// <see cref="Protocol.AntidoteTool"/>.
    /// </summary>
    public static class ConnectixConstants
    {
        public const string RegistryKey = @"SOFTWARE\Druide informatique inc.\Connectix";
        public const string RegistryFolderValue = "DossierConnectix";
        public const string ConsoleExecutable = "AgentConnectixConsole.exe";
        public const string ApiArgument = "--api";
        // AgentConnectix exposes no WebSocket server until a session is started; this is the
        // command the login Run key (ServiceConnectixAntidote64) uses to launch one.
        public const string ServiceExecutable = "ServiceConnectixAntidote.exe";
        public const string LaunchSessionArgument = "/LancementSession";
    }
}
