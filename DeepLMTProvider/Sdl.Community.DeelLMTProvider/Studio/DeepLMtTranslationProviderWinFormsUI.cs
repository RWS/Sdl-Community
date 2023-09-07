using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Helpers.GlossaryReadersWriters;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Service;
using Sdl.Community.DeepLMTProvider.UI;
using Sdl.Community.DeepLMTProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Sdl.Community.DeepLMTProvider.Studio
{
    [TranslationProviderWinFormsUi(
        Id = "DeepLMtTranslationProviderWinFormsUI",
        Name = "DeepLMtTranslationProviderWinFormsUI",
        Description = "DeepLMtTranslationProviderWinFormsUI")]
    public class DeepLMtTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        public bool SupportsEditing => true;
        public string TypeDescription => "DeepL MT Translation Provider";
        public string TypeName => "DeepL MT Translation Provider";

        private DeepLWindowViewModel DeepLWindowViewModel { get; set; }

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var options = new DeepLTranslationOptions();

            //get credentials
            var credentials = GetCredentials(credentialStore, PluginResources.DeeplTranslationProviderScheme);

            DeepLWindowViewModel = new DeepLWindowViewModel(options, new DeepLGlossaryClient(), credentials, languagePairs, new MessageService());
            var dialog = new DeepLWindow(DeepLWindowViewModel);

            DeepLWindowViewModel.ManageGlossaries += ViewModel_ManageGlossaries;

            ElementHost.EnableModelessKeyboardInterop(dialog);
            dialog.ShowDialog();

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value)
                return null;

            var provider = new DeepLMtTranslationProvider(options, new DeepLTranslationProviderClient(options.ApiKey), languagePairs)
            {
                Options = DeepLWindowViewModel.Options
            };
            var apiKey = DeepLWindowViewModel.Options.ApiKey;
            SetDeeplCredentials(credentialStore, apiKey, true);

            return new ITranslationProvider[] { provider };
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            var editProvider = translationProvider as DeepLMtTranslationProvider;

            if (editProvider == null)
            {
                return false;
            }

            //get saved key if there is one and put it into options
            var savedCredentials = GetCredentials(credentialStore, PluginResources.DeeplTranslationProviderScheme);
            if (savedCredentials != null)
            {
                editProvider.Options.ApiKey = savedCredentials.Credential;
            }

            DeepLWindowViewModel = new DeepLWindowViewModel(editProvider.Options, new DeepLGlossaryClient(), savedCredentials, languagePairs, new MessageService());
            var dialog = new DeepLWindow(DeepLWindowViewModel);

            DeepLWindowViewModel.ManageGlossaries += ViewModel_ManageGlossaries;

            ElementHost.EnableModelessKeyboardInterop(dialog);
            dialog.ShowDialog();

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value)
                return false;

            var apiKey = editProvider.Options.ApiKey;
            SetDeeplCredentials(credentialStore, apiKey, true);

            return true;
        }

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            throw new NotImplementedException();
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderDisplayInfo
            {
                Name = "DeepL Translation provider",
                TooltipText = "DeepL Translation provider",
                //SearchResultImage = PluginResources.DeepL,
                TranslationProviderIcon = PluginResources.deepLIcon
            };
            return info;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException(nameof(translationProviderUri));
            }

            var supportsProvider = string.Equals(translationProviderUri.Scheme, DeepLMtTranslationProvider.ListTranslationProviderScheme,
                StringComparison.OrdinalIgnoreCase);
            return supportsProvider;
        }

        private static string GetDateTimeToString(DateTime dateTime)
        {
            var value = dateTime.Year +
                        dateTime.Month.ToString().PadLeft(2, '0') +
                        dateTime.Day.ToString().PadLeft(2, '0') +
                        "-" +
                        dateTime.Hour.ToString().PadLeft(2, '0') +
                        dateTime.Minute.ToString().PadLeft(2, '0') +
                        dateTime.Second.ToString().PadLeft(2, '0');
            return value;
        }

        private static bool UserHasWritePermission(DirectoryInfo directoryInfo)
        {
            try
            {
                // Attempt to create a temporary file in the directory
                string tempFilePath = Path.Combine(directoryInfo.FullName, Guid.NewGuid().ToString("N") + ".tmp");
                using (FileStream fs = File.Create(tempFilePath)) { }

                // Delete the temporary file
                File.Delete(tempFilePath);

                return true; // User has write permission
            }
            catch (UnauthorizedAccessException)
            {
                return false; // User does not have write permission
            }
        }

        private TranslationProviderCredential GetCredentials(ITranslationProviderCredentialStore credentialStore, string uri)
        {
            var providerUri = new Uri(uri);
            TranslationProviderCredential cred = null;

            if (credentialStore.GetCredential(providerUri) != null)
            {
                //get the credential to return
                cred = new TranslationProviderCredential(credentialStore.GetCredential(providerUri).Credential, credentialStore.GetCredential(providerUri).Persist);
            }

            return cred;
        }

        private void SetDeeplCredentials(ITranslationProviderCredentialStore credentialStore, string apiKey, bool persistKey)
        {
            //used to set credentials
            // we are only setting and getting credentials for the uri with no parameters...kind of like a master credential
            var uri = new Uri(PluginResources.DeeplTranslationProviderScheme);
            var credentials = new TranslationProviderCredential(apiKey, persistKey);
            credentialStore.RemoveCredential(uri);
            credentialStore.AddCredential(uri, credentials);
        }

        private void ViewModel_ManageGlossaries(bool easeOfAccess)
        {
            var glossaryReaderWriterFactory = new GlossaryReaderWriterFactory();
            var messageService = new MessageService();

            var glossariesWindowViewModel = new GlossariesWindowViewModel(new DeepLGlossaryClient(),
                messageService, new GlossaryImportExportService(new DialogWrapper()),
                new GlossaryReaderWriterService(glossaryReaderWriterFactory), new ProcessStarter(), new EditGlossaryService());

            var (success, glossaryWriter, _) = glossaryReaderWriterFactory.CreateGlossaryWriter(GlossaryReaderWriterService.Format.CSV);
            if (!success) messageService.ShowWarning("Backup service could not be initialized.\nUse glossary manager carefully!", "Manage glossaries");

            var backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Trados AppStore",
                $@"DeepLLogs\Backup");

            if (!UserHasWritePermission(new DirectoryInfo(backupPath))) messageService.ShowWarning($"User not authorized to write to backup location {backupPath}");

            glossariesWindowViewModel.ShouldBackUp += glossary =>
            {
                var backupDirectory = $@"{backupPath}\{GetDateTimeToString(DateTime.Now)}";
                Directory.CreateDirectory(backupDirectory);

                glossaryWriter.WriteGlossary(glossary,
                    $@"{backupPath}\{GetDateTimeToString(DateTime.Now)}\{glossary.Name}_{glossary.SourceLanguage}-{glossary.TargetLanguage}.csv");
            };

            var glossariesWindow = new GlossariesWindow { DataContext = glossariesWindowViewModel };
            glossariesWindow.ShowDialog();

            DeepLWindowViewModel.LoadLanguagePairSettings();
        }
    }
}