using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Command;
using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using IMessageService = Sdl.Community.DeepLMTProvider.Interface.IMessageService;

namespace Sdl.Community.DeepLMTProvider.ViewModel
{
    public class GlossariesWindowViewModel : ViewModel
    {
        private string _filterQuery;
        private ObservableCollection<GlossaryInfo> _glossaries;
        private bool _isLoading;
        private GlossaryInfo _selectedGlossary;
        private GlossaryLanguagePair _selectedLanguagePair;

        public GlossariesWindowViewModel(
            IDeepLGlossaryClient deepLGlossaryClient,
            IMessageService messageService,
            IGlossaryBrowserService glossaryBrowserService,
            IGlossaryReaderWriterService glossaryReaderWriterService,
            IProcessStarter processStarter,
            IEditGlossaryService editGlossaryService)
        {
            //TODO: remove peripheral dependencies -> use events to handle those interactions instead
            DeepLGlossaryClient = deepLGlossaryClient;
            MessageService = messageService;
            GlossaryBrowserService = glossaryBrowserService;
            GlossaryReaderWriterService = glossaryReaderWriterService;
            ProcessStarter = processStarter;
            EditGlossaryService = editGlossaryService;

            LoadGlossaries();

            var (success, result, message) = DeepLGlossaryClient.GetGlossarySupportedLanguagePairs(DeepLTranslationProviderClient.ApiKey, false).Result;
            if (HandleErrorIfFound(success, message)) return;

            SupportedLanguagePairs = result;
        }

        public event Action<Glossary> ShouldBackUp;

        public ICommand AddNewGlossaryCommand => new AsyncParameterlessCommand(AddNewGlossary);
        public ICommand CancelCommand => new ParameterlessCommand(CancelOperation);
        public bool CancellationRequested { get; set; }
        public ICommand DeleteGlossariesCommand => new AsyncParameterlessCommand(async () => await ExecuteLongMethod(DeleteGlossaries));

        public ICommand EditGlossaryCommand => new AsyncParameterlessCommand(async () => await ExecuteLongMethod(EditGlossary));

        public ICommand ExportGlossariesCommand => new AsyncCommandWithParameter(async f => await ExecuteLongMethod(() => ExportGlossaries(f)));

        public string FilterQuery
        {
            get => _filterQuery;
            set
            {
                SetField(ref _filterQuery, value);
                FilterByQuery(value);
            }
        }

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

        public ICommand ImportEntriesCommand => new AsyncParameterlessCommand(async () => await ExecuteLongMethod(ImportEntries));

        public ICommand ImportGlossariesCommand => new AsyncParameterlessCommand(async () => await ExecuteLongMethod(ImportGlossaries));

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

        public GlossaryInfo SelectedGlossary
        {
            get => _selectedGlossary;
            set => SetField(ref _selectedGlossary, value);
        }

        public GlossaryLanguagePair SelectedLanguagePair
        {
            get => _selectedLanguagePair;
            set
            {
                SetField(ref _selectedLanguagePair, value);
                Filter(value);
            }
        }

        public List<GlossaryLanguagePair> SupportedLanguagePairs { get; set; }

        private IDeepLGlossaryClient DeepLGlossaryClient { get; set; }

        private IEditGlossaryService EditGlossaryService { get; }

        private IGlossaryBrowserService GlossaryBrowserService { get; }

        private IGlossaryReaderWriterService GlossaryReaderWriterService { get; }

        private IMessageService MessageService { get; }

        private IProcessStarter ProcessStarter { get; }

        private List<GlossaryInfo> SelectedGlossaries => Glossaries.Where(g => g.IsChecked).ToList();

        private async Task AddNewGlossary()
        {
            var glossarySupportedLanguages = SupportedLanguagePairs.Select(glp => glp.SourceLanguage).Distinct().ToList();
            var existingGlossaryNames = Glossaries.Select(g => g.Name).ToList();

            if (GlossaryBrowserService.OpenNewGlossaryDialog(existingGlossaryNames, glossarySupportedLanguages, out var newGlossary))
            {
                var (success, glossary, message) =
                    await DeepLGlossaryClient.ImportGlossary(
                        new Glossary
                        {
                            Entries = new List<GlossaryEntry> { new() { SourceTerm = "new entry", TargetTerm = "new entry" } },
                            Name = newGlossary.Name,
                            SourceLanguage = newGlossary.SourceLanguage,
                            TargetLanguage = newGlossary.TargetLanguage
                        }, DeepLTranslationProviderClient.ApiKey);

                if (!HandleErrorIfFound(success, message)) Glossaries.Add(glossary);
            }
        }

        private async Task AddRangeOfEntriesToSelectedGlossaries(List<GlossaryEntry> glossaryEntries)
        {
            ValidateEntriesList(glossaryEntries);

            foreach (var selectedGlossary in SelectedGlossaries)
            {
                var (success, originalEntries, message) = await DeepLGlossaryClient.RetrieveGlossaryEntries(selectedGlossary.Id, DeepLTranslationProviderClient.ApiKey);
                if (HandleErrorIfFound(success, message)) continue;

                originalEntries.AddRange(glossaryEntries);
                RemoveDuplicates(originalEntries);

                (success, var glossaryInfo, message) = await DeepLGlossaryClient.UpdateGlossary(
                    new Glossary
                    {
                        Name = selectedGlossary.Name,
                        SourceLanguage = selectedGlossary.SourceLanguage,
                        TargetLanguage = selectedGlossary.TargetLanguage,
                        Entries = originalEntries
                    }, selectedGlossary.Id, DeepLTranslationProviderClient.ApiKey);

                if (HandleErrorIfFound(success, message))
                {
                    LoadGlossaries();
                    continue;
                }

                Glossaries.Remove(selectedGlossary);
                Glossaries.Add(glossaryInfo);
            }
        }

        private void CancelOperation()
        {
            CancellationRequested = true;
        }

        private async Task DeleteGlossaries()
        {
            var selectedGlossaries = SelectedGlossaries.ToList();

            if (!selectedGlossaries.Any())
            {
                if (SelectedGlossary != null) selectedGlossaries.Add(SelectedGlossary);
                else
                {
                    MessageService.ShowWarning("No glossaries selected");
                    return;
                }
            }

            if (!MessageService.ShowDialog("Are you sure you want to delete the selected glossaries from DeepL?")) return;

            foreach (var glossaryInfo in selectedGlossaries)
            {
                if (IsCancellationRequested()) break;

                var (success, _, message) = await DeepLGlossaryClient.DeleteGlossary(DeepLTranslationProviderClient.ApiKey, glossaryInfo.Id);
                if (HandleErrorIfFound(success, message)) continue;
                Glossaries.Remove(glossaryInfo);
            }
        }

        private async Task EditGlossary()
        {
            if (SelectedGlossary == null) return;

            var (success, originalEntries, message) = await DeepLGlossaryClient.RetrieveGlossaryEntries(SelectedGlossary.Id, DeepLTranslationProviderClient.ApiKey);

            if (HandleErrorIfFound(success, message)) return;

            RaiseBackUp(new Glossary
            {
                Name = SelectedGlossary.Name,
                Entries = originalEntries,
                SourceLanguage = SelectedGlossary.SourceLanguage,
                TargetLanguage = SelectedGlossary.TargetLanguage
            });

            if (!EditGlossaryService.EditGlossary(originalEntries, SelectedGlossary.Name)) return;
            var newEntries = EditGlossaryService.GlossaryEntries;
            var newGlossaryName = EditGlossaryService.GlossaryName;

            (success, var glossaryInfo, message) = await DeepLGlossaryClient.UpdateGlossary(
                new Glossary
                {
                    Name = newGlossaryName,
                    SourceLanguage = SelectedGlossary.SourceLanguage,
                    TargetLanguage = SelectedGlossary.TargetLanguage,
                    Entries = newEntries
                }, SelectedGlossary.Id, DeepLTranslationProviderClient.ApiKey);

            if (HandleErrorIfFound(success, message))
            {
                LoadGlossaries();
                return;
            }

            Glossaries.Remove(SelectedGlossary);
            Glossaries.Add(glossaryInfo);
            SelectedGlossary = glossaryInfo;
        }

        /// <summary>
        /// Wrapper for executing methods that need a progress bar
        /// </summary>
        private async Task ExecuteLongMethod(Func<Task> method)
        {
            IsLoading = true;
            await method.Invoke();
            IsLoading = false;
        }

        private async Task ExportGlossaries(object parameter)
        {
            if (!SelectedGlossaries.Any())
            {
                MessageService.ShowWarning("No glossaries selected");
                return;
            }

            if (!Enum.TryParse<GlossaryReaderWriterService.Format>(parameter.ToString(), out var format)) return;

            var (success, folderPath) = GlossaryBrowserService.OpenExportDialog();
            if (!success) return;

            foreach (var selectedGlossary in SelectedGlossaries)
            {
                (success, var entries, var message) = await DeepLGlossaryClient.RetrieveGlossaryEntries(selectedGlossary.Id, DeepLTranslationProviderClient.ApiKey);
                if (HandleErrorIfFound(success, message)) continue;

                (success, _, message) = GlossaryReaderWriterService.WriteGlossary(new Glossary { Entries = entries }, format, $"{folderPath}\\{selectedGlossary.Name}.{parameter.ToString().ToLower()}");
                if (HandleErrorIfFound(success, message)) return;

                ProcessStarter.StartInFileExplorer(folderPath);
            }
        }

        private void Filter(GlossaryLanguagePair value)
        {
            var collectionView = CollectionViewSource.GetDefaultView(Glossaries);
            Predicate<object> collectionViewFilter = value.Label == PluginResources.AllLanguagePairs_Label
                ? null
                : gi =>
                    ((GlossaryInfo)gi).SourceLanguage == value.SourceLanguage &&
                    ((GlossaryInfo)gi).TargetLanguage == value.TargetLanguage;

            collectionView.Filter = null;
            collectionView.Filter = collectionViewFilter;
        }

        private void FilterByQuery(string value)
        {
            var collectionView = CollectionViewSource.GetDefaultView(Glossaries);
            if (string.IsNullOrWhiteSpace(value)) { collectionView.Filter = null; return; }

            collectionView.Filter = null;
            collectionView.Filter = glossary =>
                ((GlossaryInfo)glossary).Name.ToLower().Contains(value.ToLower());
        }

        private bool HandleErrorIfFound(bool success, string message, [CallerMemberName] string failingMethod = null)
        {
            if (success) return false;
            MessageService.ShowWarning(message, failingMethod);
            return true;
        }

        private async Task ImportEntries()
        {
            if (!SelectedGlossaries.Any())
            {
                MessageService.ShowWarning("No glossaries selected");
                return;
            }

            GlossaryBrowserService.OpenImportEntriesDialog(out var filePaths);
            foreach (var filePath in filePaths)
            {
                var (success, glossary, message) = GlossaryReaderWriterService.ReadGlossary(filePath);
                if (HandleErrorIfFound(success, message)) continue;
                await AddRangeOfEntriesToSelectedGlossaries(glossary.Entries);
            }
        }

        private async Task ImportGlossaries()
        {
            var glossarySupportedLanguages = SupportedLanguagePairs.Select(glp => glp.SourceLanguage).Distinct().ToList();
            if (GlossaryBrowserService.OpenImportDialog(glossarySupportedLanguages, out var glossaries))
            {
                foreach (var glossaryItem in glossaries)
                {
                    if (IsCancellationRequested()) break;

                    var selectedFilePath = glossaryItem.Path;
                    if (Glossaries.Select(g => g.Name).Contains(glossaryItem.Name)) continue;

                    var (success, glossaryFile, message) = GlossaryReaderWriterService.ReadGlossary(selectedFilePath);
                    if (HandleErrorIfFound(success, message)) continue;

                    ValidateEntriesList(glossaryFile.Entries);
                    RemoveDuplicates(glossaryFile.Entries);

                    glossaryFile.SourceLanguage = glossaryItem.SourceLanguage;
                    glossaryFile.TargetLanguage = glossaryItem.TargetLanguage;
                    glossaryFile.Name = glossaryItem.Name;

                    (success, var glossary, message) =
                        await DeepLGlossaryClient.ImportGlossary(glossaryFile, DeepLTranslationProviderClient.ApiKey);

                    if (HandleErrorIfFound(success, message))
                    {
                        LoadGlossaries();
                        continue;
                    }

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

        private void LoadGlossaries()
        {
            var (success, result, message) = DeepLGlossaryClient.GetGlossaries(DeepLTranslationProviderClient.ApiKey, false).Result;
            if (HandleErrorIfFound(success, message)) return;
            Glossaries = new ObservableCollection<GlossaryInfo>(result);
        }

        private void RaiseBackUp(Glossary glossary)
        {
            ShouldBackUp?.Invoke(glossary);
        }

        private void RemoveDuplicates(List<GlossaryEntry> originalEntries)
        {
            var duplicates = originalEntries
                .GroupBy(e => e.SourceTerm)
                .Where(g => g.Count() > 1)
                .Select(g => g.FirstOrDefault())
                .ToList();

            originalEntries.RemoveAll(oe => duplicates.Select(d => d.SourceTerm).Contains(oe.SourceTerm));
            originalEntries.AddRange(duplicates);
        }

        private void ValidateEntriesList(List<GlossaryEntry> glossaryEntries)
        {
            var toBeRemoved = new List<GlossaryEntry>();
            foreach (var glossaryEntry in glossaryEntries)
            {
                glossaryEntry.Trim();
                if (glossaryEntry.IsInvalid()) toBeRemoved.Add(glossaryEntry);
            }

            glossaryEntries.RemoveAll(toBeRemoved.Contains);
        }
    }
}