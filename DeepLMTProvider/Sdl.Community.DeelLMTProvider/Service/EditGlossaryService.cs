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
    public class EditGlossaryService(
        IGlossaryReaderWriterService glossaryReaderWriterService,
        IUserInteractionService glossaryBrowserService,
        IMessageService messageService)
        : IEditGlossaryService
    {
        public List<GlossaryEntry> GlossaryEntries
        {
            get
            {
                var entries = EditGlossaryWindow.GlossaryEntries;
                entries.ForEach(e => e.Trim());
                return [..entries];
            }
        }

        public string GlossaryName => EditGlossaryWindow.GlossaryName.Trim();
        private EditGlossaryWindow EditGlossaryWindow { get; set; }
        private IUserInteractionService GlossaryBrowserService { get; } = glossaryBrowserService;
        private IGlossaryReaderWriterService GlossaryReaderWriterService { get; } = glossaryReaderWriterService;
        private IMessageService MessageService { get; } = messageService;

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
            if (!GlossaryBrowserService.OpenImportEntriesDialog(out var glossariesAndDelimiters)) return;

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