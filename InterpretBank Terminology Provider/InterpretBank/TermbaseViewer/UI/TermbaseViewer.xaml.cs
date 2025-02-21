using InterpretBank.TermbaseViewer.ViewModel;
using Sdl.Core.Globalization;
using System.Collections.Generic;
using System.Windows.Input;

namespace InterpretBank.TermbaseViewer.UI
{
    /// <summary>
    /// Interaction logic for TermbaseViewer.xaml
    /// </summary>
    public partial class TermbaseViewer
    {
        public TermbaseViewer(TermbaseViewerViewModel termbaseViewerViewModel)
        {
            InitializeComponent();
            DataContext = termbaseViewerViewModel;
        }

        private TermbaseViewerViewModel ViewModel => (TermbaseViewerViewModel)DataContext;

        public void AddTerm(string source, string target)
        {
            ViewModel.OpenAddTermPopup(source, target);
        }

        public void LoadTerms(Language sourceLanguage, Language targetLanguage, List<string> glossaries, List<string> tags, bool useTags, string databaseFilepath)
        {
            ViewModel.Setup(sourceLanguage, targetLanguage, glossaries, tags, useTags, databaseFilepath);
        }

        public void ReloadDb(string filepath)
        {
            ViewModel.ReloadDb(filepath);
        }

        public void ReloadTerms(Language sourceLanguage, Language targetLanguage)
        {
            ViewModel.ReloadTerms(sourceLanguage, targetLanguage);
        }

        private void DeleteCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataContext is not TermbaseViewerViewModel viewModel) return;
            viewModel.RemoveSelectedEntryCommand.Execute(viewModel.SelectedEntry);
        }
    }
}