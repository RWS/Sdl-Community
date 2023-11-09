using InterpretBank.TermbaseViewer.UI.Controls;
using InterpretBank.TermbaseViewer.ViewModel;
using Sdl.Core.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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

        public void LoadTerms(Language sourceLanguage, Language targetLanguage, List<string> glossaries, string databaseFilepath)
        {
            ViewModel.Setup(sourceLanguage, targetLanguage, glossaries, databaseFilepath);
        }

        public void ReloadTerms(Language sourceLanguage, Language targetLanguage)
        {
            ViewModel.ReloadTerms(sourceLanguage, targetLanguage);
        }

        public void ReloadDb(string filepath)
        {
            ViewModel.ReloadDb(filepath);
        }
    }
}