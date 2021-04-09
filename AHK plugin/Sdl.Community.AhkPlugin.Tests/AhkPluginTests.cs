using System;
using System.IO;
using System.Linq;
using NSubstitute;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.Interface;
using Sdl.Community.AhkPlugin.ViewModels;
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

        [Fact]
        public void AddFiles_PopulatesCollectionWithScripts()
        {
			// Arrange

			var dialogService = Substitute.For<IDialogService>();
			var pageViewModel = new ImportScriptPageViewModel(Substitute.For<MainWindowViewModel>(), dialogService);
			var filter = "AHK Scripts(*.ahk) | *.ahk";
			dialogService.ShowDialog(filter).Returns(_scripts.ToList());

			// Assume
			Assert.Empty(pageViewModel.ScriptsCollection);

			// Act
			pageViewModel.AddFilesCommand.Execute(null);

			// Assert
			Assert.NotEmpty(pageViewModel.ScriptsCollection);
        }

		private string GetScript(string scriptName)
        {
	        var scriptPath = _scripts.FirstOrDefault(x => x.Contains(scriptName));
	        return scriptPath;
        }
    }
}
