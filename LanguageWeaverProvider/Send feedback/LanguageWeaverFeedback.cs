using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using Newtonsoft.Json;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Send_feedback
{
    public abstract class LanguageWeaverFeedback
    {
        public abstract Task<bool> Send();

        protected static AccessToken GetAccessToken(string pluginVersion)
        {
            var translationOptions = GetTranslationOptions(pluginVersion);
            if (translationOptions is null) return null;

            CredentialManager.GetCredentials(translationOptions, true);
            Service.ValidateTokenAsync(translationOptions, false);

            var accessToken = translationOptions.AccessToken;
            return accessToken;
        }

        private static ITranslationOptions GetTranslationOptions(string pluginVersion)
        {
            var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
            var filesController = SdlTradosStudio.Application.GetController<FilesController>();

            string state;

            if (filesController is not null)
            {
                var project = filesController.CurrentProject;
                state = project?.GetTranslationProviderConfiguration().Entries
                    .FirstOrDefault(e => e.MainTranslationProvider.Uri.ToString() == pluginVersion)?
                    .MainTranslationProvider.State;

                if (state is not null) return JsonConvert.DeserializeObject<TranslationOptions>(state);
            }

            if (projectController is not null)
            {
                var project = projectController.SelectedProjects.FirstOrDefault();
                state = project?.GetTranslationProviderConfiguration().Entries
                    .FirstOrDefault(e => e.MainTranslationProvider.Uri.ToString() == pluginVersion)?
                    .MainTranslationProvider.State;

                if (state is not null) return JsonConvert.DeserializeObject<TranslationOptions>(state);
            }

            return null;
        }
    }
}