using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using TMX_UI.View;

namespace TMX_TranslationProvider.Tellme
{
	public class TmxProviderSettingsAction : AbstractTellMeAction
    {
        public TmxProviderSettingsAction()
        {
            Name = "TmxProvider MT Provider options";
        }

        public override string Category => "TmxProvider results";

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
                var settings = currentProject.GetTranslationProviderConfiguration();

                if (!settings.Entries.Any(entry =>
                    entry.MainTranslationProvider.Uri.OriginalString.Contains(TmxTranslationProvider.ProviderScheme)))
                {
                    MessageBox.Show(
                        @"TmxProvider is not set on this project\nPlease set it in project settings before using Tell Me to access it");
                }
                else
                {
                    var translationProvider = settings.Entries.FirstOrDefault(entry =>
                        entry.MainTranslationProvider.Uri.OriginalString.Contains(TmxTranslationProvider.ProviderScheme));
                    if (translationProvider != null)
                    {
                        var uri = translationProvider.MainTranslationProvider.Uri;
                        var options = new TmxTranslationsOptions(uri);
                        var form = new OptionsView(options.Databases, options.CareForLocale);
						var interopHelper = new System.Windows.Interop.WindowInteropHelper(form);
						interopHelper.Owner = TmxTranslationProviderWinFormsUI.GetParentForm().Handle;
                        if (form.ShowDialog() == true)
                        {
							options.CopyFrom(form.ViewModel);
							translationProvider.MainTranslationProvider.Uri = options.Uri();
                            currentProject.UpdateTranslationProviderConfiguration(settings);
                        }
                    }
                }
            }
        }
    }
}