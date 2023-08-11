using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using Sdl.Community.DeepLMTProvider.ViewModel;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLSettingsAction : AbstractTellMeAction
    {
        public DeepLSettingsAction()
        {
            Name = "DeepL MT Provider options";
        }

        public override string Category => "DeepL results";

        public override Icon Icon => PluginResources.Settings;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

            if (currentProject == null)
            {
                MessageBox.Show(@"No project is set as active");
            }
            else
            {
				//TODO: Don't forget to fix this
				var settings = currentProject.GetTranslationProviderConfiguration();

                if (!settings.Entries.Any(entry =>
                    entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider")))
                {
                    MessageBox.Show(
                        @"DeepL is not set on this project\nPlease set it in project settings before using TellMe to access it");
                }
                else
                {
                    var translationProvider = settings.Entries.FirstOrDefault(entry =>
                        entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider"));
                    if (translationProvider != null)
                    {
                        var uri = translationProvider.MainTranslationProvider.Uri;
                        var state = translationProvider.MainTranslationProvider.State;
                        var options = new DeepLTranslationOptions(uri, state);

                        var viewModel = new DeepLWindowViewModel(options, isTellMeAction: true);
                        var dialog = new DeepLWindow(viewModel);

                        dialog.ShowDialog();

                        if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                        {
                            translationProvider.MainTranslationProvider.Uri = options.Uri;
                            currentProject.UpdateTranslationProviderConfiguration(settings);
                        }
                    }
                }
            }
        }
    }
}