using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class EditGlossaryService : IEditGlossaryService
    {
        public EditGlossaryService(
            IGlossaryReaderWriterService glossaryReaderWriterService,
            IUserInteractionService glossaryBrowserService,
            IMessageService messageService)
        {
            GlossaryReaderWriterService = glossaryReaderWriterService;
            GlossaryBrowserService = glossaryBrowserService;
            MessageService = messageService;
        }

        public List<GlossaryEntry> GlossaryEntries
        {
            get
            {
                var entries = EditGlossaryWindow.GlossaryEntries;
                entries.ForEach(e => e.Trim());
                return new(entries);
            }
        }

        public string GlossaryName => EditGlossaryWindow.GlossaryName.Trim();
        private EditGlossaryWindow EditGlossaryWindow { get; set; }
        private IUserInteractionService GlossaryBrowserService { get; }
        private IGlossaryReaderWriterService GlossaryReaderWriterService { get; }
        private IMessageService MessageService { get; }

        public bool EditGlossary(List<GlossaryEntry> glossaryEntries, string glossaryName)
        {
            SetGlossaryWindow(glossaryEntries, glossaryName);
            EditGlossaryWindow.ImportEntriesRequested += EditGlossaryWindow_ImportEntriesRequested;
            return EditGlossaryWindow.ShowDialog() ?? false;
        }

        private void AddRangeOfEntries(List<GlossaryEntry> glossaryEntries)
        {
            var entries = EditGlossaryWindow.GlossaryEntries.ToList();
            entries.AddRange(glossaryEntries);

            EditGlossaryWindow.GlossaryEntries = new ObservableCollection<GlossaryEntry>(entries);
        }

        private void EditGlossaryWindow_ImportEntriesRequested()
        {
            GlossaryBrowserService.OpenImportEntriesDialog(out var glossariesAndDelimiters);

            foreach (var glossaryAndDelimiter in glossariesAndDelimiters)
            {
                var (success, glossary, message) = GlossaryReaderWriterService.ReadGlossary(glossaryAndDelimiter.Filepath, glossaryAndDelimiter.Delimiter);
                if (HandleErrorIfFound(success, message)) continue;
                AddRangeOfEntries(glossary.Entries);
            }
        }

        private bool HandleErrorIfFound(bool success, string message, [CallerMemberName] string failingMethod = null)
        {
            if (success) return false;
            MessageService.ShowWarning(message, failingMethod);
            return true;
        }

        private void SetGlossaryWindow(List<GlossaryEntry> glossaryEntries, string glossaryName)
        {
            if (EditGlossaryWindow is not null)
                EditGlossaryWindow.ImportEntriesRequested -= EditGlossaryWindow_ImportEntriesRequested;

            EditGlossaryWindow = new EditGlossaryWindow(glossaryEntries, glossaryName);
        }
    }
}