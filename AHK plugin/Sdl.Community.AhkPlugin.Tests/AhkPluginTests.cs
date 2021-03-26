using System;
using System.IO;
using System.Linq;
using Sdl.Community.AhkPlugin.Helpers;
using Xunit;

namespace Sdl.Community.AhkPlugin.Tests
{
    public class AhkPluginTests
    {
	    private readonly string[] _scripts;

	    public AhkPluginTests()
	    {
		    _scripts = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"TestScripts"));
	    }

		[InlineData("AhkMasterScript.ahk")]
		[InlineData("ScriptForTesting.ahk")]
		[InlineData("RandomTextAtTheEnd.ahk")]
		[Theory]
        public void IsGeneratedByAhk_ReturnsTrue(string scriptName)
        {
			// Arrange
			var scriptPath = GetScript(scriptName);

			// Act
			var isScript = ProcessScript.IsGeneratedByAhkPlugin(scriptPath);

			// Assert
			Assert.True(isScript);
        }

        [InlineData("FakeScript.ahk")]
		[InlineData("CombinedScript.ahk")]
		[InlineData("DoubleContent.ahk")]
		[Theory]
        public void IsNotGeneratedByAhk_ReturnsFalse(string scriptName)
        {
	        // Arrange
	        var scriptPath = GetScript(scriptName);

	        // Act
	        var isScript = ProcessScript.IsGeneratedByAhkPlugin(scriptPath);

	        // Assert
	        Assert.False(isScript);
        }

		private string GetScript(string scriptName)
        {
	        var scriptPath = _scripts.FirstOrDefault(x => x.Contains(scriptName));
	        return scriptPath;
        }
    }
}
