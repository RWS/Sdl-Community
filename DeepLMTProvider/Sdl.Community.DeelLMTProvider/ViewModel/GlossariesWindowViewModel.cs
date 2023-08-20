using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.ViewModel
{
    public class GlossariesWindowViewModel : ViewModel
    {
        private ObservableCollection<GlossaryInfo> _glossaries;
        private bool _isLoading;

        public GlossariesWindowViewModel(IDeepLGlossaryClient deepLGlossaryClient, IMessageService messageService, IGlossaryBrowserService glossaryBrowserService, IGlossaryReaderWriterService glossaryReaderWriterService)
        {
            DeepLGlossaryClient = deepLGlossaryClient;
            MessageService = messageService;
            GlossaryBrowserService = glossaryBrowserService;
            GlossaryReaderWriterService = glossaryReaderWriterService;
            LoadGlossaries();
        }

        public ICommand CancelCommand => new ParameterlessCommand(CancelOperation);
        public bool CancellationRequested { get; set; }
        public ICommand DeleteGlossariesCommand => new AsyncParameterlessCommand(async () => await ExecuteLongMethod(DeleteGlossaries));

        public ICommand ExportGlossariesCommand => new ParameterlessCommand(ExportGlossaries);

        public ObservableCollection<GlossaryInfo> Glossaries
        {
            get => _glossaries;
            set
            {
                SetField(ref _glossaries, value);
                value.ForEach(gi => gi.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(GlossaryInfo.IsChecked)) OnPropertyChanged(nameof(IsCheckAll));
                });
            }
        }

        public ICommand ImportGlossaryCommand => new AsyncParameterlessCommand(async () => await ExecuteLongMethod(ImportGlossaries));

        public bool IsCheckAll
        {
            get => Glossaries.All(g => g.IsChecked);
            set => Glossaries.ForEach(g => g.IsChecked = value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        private IDeepLGlossaryClient DeepLGlossaryClient { get; set; }

        private IGlossaryBrowserService GlossaryBrowserService { get; }
        private IGlossaryReaderWriterService GlossaryReaderWriterService { get; }

        private IMessageService MessageService { get; }
        private List<GlossaryInfo> SelectedGlossaries => Glossaries.Where(g => g.IsChecked).ToList();

        private void CancelOperation()
        {
            CancellationRequested = true;
        }

        private async Task DeleteGlossaries()
        {
            foreach (var glossaryInfo in SelectedGlossaries)
            {
                if (IsCancellationRequested()) break;

                var (success, _, message) = await DeepLGlossaryClient.DeleteGlossary(DeepLTranslationProviderClient.ApiKey, glossaryInfo.Id);
                if (HandleErrorIfFound(success, message)) continue;
                Glossaries.Remove(glossaryInfo);
            }
        }

        /// <summary>
        /// Wrapper for executing methods that need a progress bar
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private async Task ExecuteLongMethod(Func<Task> method)
        {
            IsLoading = true;
            await method.Invoke();
            IsLoading = false;
        }

        private void ExportGlossaries()
        {
            foreach (var selectedGlossary in SelectedGlossaries)
            {
                GlossaryReaderWriterService.WriteGlossary(selectedGlossary);
            }
        }

        private void HandleError(string message, string failingMethod)
        {
            MessageService.ShowWarning(message, failingMethod);
        }

        private bool HandleErrorIfFound(bool success, string message, [CallerMemberName] string failingMethod = null)
        {
            if (success) return false;
            HandleError(message, failingMethod);
            return true;
        }

        private async Task ImportGlossaries()
        {
            var (success, result, message) = await DeepLGlossaryClient.GetGlossarySupportedLanguagePairs(DeepLTranslationProviderClient.ApiKey);

            if (HandleErrorIfFound(success, message))
            {
                IsLoading = false;
                return;
            }

            var glossarySupportedLanguages = result.Select(glp => glp.SourceLanguage).Distinct().ToList();

            if (GlossaryBrowserService.Browse(glossarySupportedLanguages, out var glossaries))
            {
                foreach (var glossaryItem in glossaries)
                {
                    if (IsCancellationRequested()) break;

                    var selectedFilePath = glossaryItem.Path;
                    (success, var glossaryFile, message) = GlossaryReaderWriterService.ReadGlossary(selectedFilePath);

                    if (HandleErrorIfFound(success, message)) continue;

                    glossaryFile.SourceLanguage = glossaryItem.SourceLanguage;
                    glossaryFile.TargetLanguage = glossaryItem.TargetLanguage;
                    glossaryFile.Name = Path.GetFileNameWithoutExtension(selectedFilePath);

                    (success, var glossary, message) =
                        await DeepLGlossaryClient.ImportGlossary(glossaryFile, DeepLTranslationProviderClient.ApiKey);

                    if (HandleErrorIfFound(success, message)) continue;

                    Glossaries.Add(glossary);
                }
            }
        }

        private bool IsCancellationRequested()
        {
            if (!CancellationRequested) return false;
            CancellationRequested = false;
            return true;
        }

        private async void LoadGlossaries()
        {
            var (success, result, message) = await DeepLGlossaryClient.GetGlossaries(DeepLTranslationProviderClient.ApiKey);
            if (HandleErrorIfFound(success, message)) return;
            Glossaries = new ObservableCollection<GlossaryInfo>(result);
        }
    }
}