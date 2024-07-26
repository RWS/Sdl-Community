using InterpretBank.TermbaseViewer.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace InterpretBank.TermbaseViewer.UI
{
    public partial class TermbaseViewerControl : UserControl
    {
        public TermbaseViewerControl(TermbaseViewer termbaseViewer)
        {
            InitializeComponent();
            var elementHost = new ElementHost
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(elementHost);
            elementHost.Child = termbaseViewer;

            TermbaseViewer = termbaseViewer;
        }

        public string LoadedDatabaseFilepath { get; set; }

        private TermbaseViewerViewModel DataContext => (TermbaseViewerViewModel)TermbaseViewer.DataContext;

        private TermbaseViewer TermbaseViewer { get; set; }

        public void AddTerm(string source, string target)
        {
            TermbaseViewer.AddTerm(source, target);
        }

        public void JumpToTerm(Entry entry) => DataContext.JumpToTerm(entry);

        public void LoadTerms(Language sourceLanguage, Language targetLanguage, List<string> glossaries, string databaseFilepath)
        {
            LoadedDatabaseFilepath = databaseFilepath;
            TermbaseViewer.LoadTerms(sourceLanguage, targetLanguage, glossaries, databaseFilepath);
        }

        public void ReloadDb(string filepath)
        {
            if (!filepath.Equals(LoadedDatabaseFilepath)) return;
            TermbaseViewer.ReloadDb(filepath);
        }

        public void ReloadTerms(Language sourceLanguage, Language targetLanguage)
        {
            TermbaseViewer.ReloadTerms(sourceLanguage, targetLanguage);
        }
    }
}