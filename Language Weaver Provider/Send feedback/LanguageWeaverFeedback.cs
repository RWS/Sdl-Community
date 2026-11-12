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

        protected static async Task<AccessToken> GetAccessToken(string pluginVersion)
        {
            var translationOptions = GetTranslationOptions(pluginVersion);
            if (translationOptions is null) return null;

            // Prefer the provider's live options: translation keeps their token refreshed in memory, while the
            // credential store can still hold one that has since expired (CRQ-46950: 401 code 57).
            if (translationOptions.Id is not null
             && ApplicationInitializer.TranslationOptions?.TryGetValue(translationOptions.Id, out var liveOptions) == true)
            {
                translationOptions = liveOptions;
            }
            else
            {
                CredentialManager.GetCredentials(translationOptions, true);
            }

            // Await the refresh: it replaces AccessToken, so reading it earlier returns the expired token.
            // The guarded path swallows an expired EdgeSSO session, which cannot be renewed without UI.
            await Service.ValidateAndUpdateTokenAsync(translationOptions, () =>
            {
                if (ApplicationInitializer.CredentialStore is not null)
                    CredentialManager.UpdateCredentials(ApplicationInitializer.CredentialStore, translationOptions);
            }).ConfigureAwait(false);

            return translationOptions.AccessToken;
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