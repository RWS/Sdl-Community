using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Services;

namespace ProjectAutomationDataProtectionSuiteSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
	        const bool ENCRYPT_ALL = false;

			// create simple project
	        var localSett = ProjectUtil.DefaultSettings();
			Console.WriteLine($"creating project {localSett.ProjectName} from {localSett.InputFolder} to {localSett.OutputFolder}");
			var project = ProjectUtil.CreateProject(localSett);

			// create the settings for Protect Data
			var settBundle = project.GetSettings();
			var protectDataSett = settBundle.GetSettingsGroup<AnonymizerSettings>();
			protectDataSett.EnableAll = true;
			protectDataSett.ShouldAnonymize = true;
			protectDataSett.EncryptAll = ENCRYPT_ALL;
			protectDataSett.EncryptionKey = "titiduru";
			var patterns = Constants.GetDefaultRegexPatterns();
			foreach (var pattern in patterns)
			{
				pattern.ShouldEnable = true;
				pattern.ShouldEncrypt = ENCRYPT_ALL;
			}
			protectDataSett.RegexPatterns = patterns;
			project.UpdateSettings(settBundle);
			project.Save();

			// run automatic tasks
			project.RunAutomaticTask(project.GetSourceLanguageFiles().Select(f => f.Id).ToArray(), "SDPSAnonymizerTask");
			project.RunAutomaticTask(project.GetTargetLanguageFiles().Select(f => f.Id).ToArray(), "SDPSAnonymizerTask");
			project.Save();
			Console.WriteLine("project creation complete");
        }
	}
}
