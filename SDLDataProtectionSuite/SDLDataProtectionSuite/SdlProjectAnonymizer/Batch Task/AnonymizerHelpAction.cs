using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

/* if you run into a build error during building the plugin (the dll gets built, you get an msbuild error pfe103)
 * remove Sdl.TranslationStudioAutomation.IntegrationApi.dll and \Sdl.TranslationStudioAutomation.IntegrationApi.Extensions.dll
 * from the project's references, and then manually add them (via Visual Studio's UI).
 */
namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task
{
	[Action("SDPSProjectAnonymizerHelpAction",
		Name = "Trados Project Anonymizer Help",
		Description = "Help",
		Icon = "question"
	)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class AnonymizerHelpAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3199/trados-project-anonymizer");
		}
	}
}
