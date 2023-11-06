using InterpretBank.GlossaryService.Interface;
using InterpretBank.Model;
using InterpretBank.TermbaseViewer.ViewModel;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
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

//            DataContext.AnyEditedTermsChanged += b => AnyEditedTermsChanged?.Invoke(b);
        }

        //public event Action<bool> AnyEditedTermsChanged;

        //public bool AnyEditedTerms => DataContext.AnyEditedTerms;
        private TermbaseViewerViewModel DataContext => (TermbaseViewerViewModel)TermbaseViewer.DataContext;
        private TermbaseViewer TermbaseViewer { get; set; }

        //public void AddNewTerm()
        //{
        //    TermbaseViewer.AddNewTermButton_Click(null, null);
        //}

        //public void AddTerm(string source, string target)
        //{
        //    TermbaseViewer.AddNewTermButton_Click(
        //        new TermModel { SourceTerm = source, TargetTerm = target }, null);
        //}

        //public void CommitToDatabase()
        //{
        //    DataContext.CommitAllToDatabaseCommand.Execute(null);
        //}

        //public void EditTerm(IEntry term) => DataContext.EditTerm(term);

        //public void JumpToTerm(IEntry entry) => DataContext.JumpToTerm(entry);

        public void LoadTerms(Language sourceLanguage, Language targetLanguage, List<string> glossaries, string databaseFilepath)
        {
            TermbaseViewer.LoadTerms(sourceLanguage, targetLanguage, glossaries, databaseFilepath);
        }

        public void ReloadTerms(Language sourceLanguage, Language targetLanguage)
        {
            TermbaseViewer.ReloadTerms(sourceLanguage, targetLanguage);
        }

        public void ReloadDb(string filepath)
        {
            TermbaseViewer.ReloadDb(filepath);
        }
    }
}