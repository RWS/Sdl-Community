using Sdl.Community.AntidoteVerifier.Connectix;
using Xunit;

namespace Sdl.Community.AntidoteVerifier.Tests
{
    /// <summary>
    /// Locks the AgentConnectix --api parse contract. The live daemon prints {"port":null} until its
    /// WebSocket server is up; that is a JSON Null token (not C# null), so ParsePort must return null
    /// and let the caller retry instead of throwing "Can not convert Null to Int32".
    /// </summary>
    public class ConnectixPortLocatorTests
    {
        [Fact]
        public void ParsePort_returns_value_when_port_present()
        {
            Assert.Equal(59004, ConnectixPortLocator.ParsePort("{\"port\":59004}"));
        }

        [Fact]
        public void ParsePort_returns_null_when_port_is_json_null()
        {
            Assert.Null(ConnectixPortLocator.ParsePort("{\"port\":null}"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("not json")]
        [InlineData("{}")]
        public void ParsePort_returns_null_for_empty_or_malformed_output(string output)
        {
            Assert.Null(ConnectixPortLocator.ParsePort(output));
        }

        // --api only queries a running daemon; when none is listening we must start a session with
        // the same command the login Run key uses. Lock that command shape so a wrong exe/arg (which
        // would silently fail at runtime) is caught here instead.
        [Fact]
        public void BuildSessionStartInfo_uses_the_login_run_key_command()
        {
            var startInfo = ConnectixPortLocator.BuildSessionStartInfo(@"C:\Connectix\Bin64");

            Assert.Equal(@"C:\Connectix\Bin64\ServiceConnectixAntidote.exe", startInfo.FileName);
            Assert.Equal("/LancementSession", startInfo.Arguments);
            Assert.False(startInfo.UseShellExecute);
            Assert.True(startInfo.CreateNoWindow);
        }
    }
}
