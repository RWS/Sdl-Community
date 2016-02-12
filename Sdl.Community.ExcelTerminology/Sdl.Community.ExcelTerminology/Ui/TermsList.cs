using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Terminology.TerminologyProvider.Core;


namespace Sdl.Community.ExcelTerminology.Ui
{
    public partial class TermsList : UserControl
    {
        private readonly ExcelTermProviderService _excelTermProviderService;
        private readonly ProviderSettings _providerSettings;
        private readonly EntryTransformerService _transformerService;
        private List<ExcelEntry> _terms;

        public TermsList()
        {
            InitializeComponent();

            var persistenceService = new PersistenceService();
            _providerSettings = persistenceService.Load();
            if (string.IsNullOrEmpty(_providerSettings.ApprovedColumn))
            {
                this.Approved.Visible = false;
            }
            var excelTermLoaderService = new ExcelTermLoaderService(_providerSettings);
            var parser = new Parser(_providerSettings);
            _transformerService = new EntryTransformerService(parser);
            _terms = new List<ExcelEntry>();
            _excelTermProviderService = new ExcelTermProviderService(excelTermLoaderService,
                _transformerService);
        }

        public TermsList(List<ExcelEntry> terms):this()
        {
            _terms = terms;
        }

        public void SetTerms(List<ExcelEntry> terms)
        {
            _terms = terms;
            sourceListView.SetObjects(_terms);
            sourceListView.SelectedIndex = 0;
        }

        protected override void OnLoad(EventArgs e)
        {
            sourceListView.ShowGroups = false;
            sourceListView.FullRowSelect = true;
            sourceListView.HeaderStyle = ColumnHeaderStyle.None;
            sourceListView.HideSelection = false;

            sourceListView.SetObjects(_terms);
            sourceListView.SelectedIndex = 0;
            targetGridView.ColumnHeadersVisible = false;
            targetGridView.EditMode = DataGridViewEditMode.EditOnEnter;


            sourceColumn.IsEditable = true;

        }

       

        public void JumpToTerm(IEntry entry)
        {
          
            var selectedItem = sourceListView.Objects.Cast<ExcelEntry>().FirstOrDefault(s => s.Id == entry.Id);
           
            if (selectedItem != null)
            {
                sourceListView.DeselectAll();
                sourceListView.Focus();
                sourceListView.EnsureModelVisible(selectedItem);
                sourceListView.SelectObject(selectedItem, true);
            }
            
        }

        public void AddTerm(string source, string target)
        {
            var excelTerm = new ExcelTerm
            {
                SourceCulture = _providerSettings.SourceLanguage,
                TargetCulture = _providerSettings.TargetLanguage,
                Source = source,
                Target = target
            };

            var entryLanguages = _transformerService.CreateEntryLanguages(excelTerm);

            var maxId = 0;
            if (_terms.Count > 0)
            {
                maxId = _terms.Max(term => term.Id);
            }

            var excelEntry = new ExcelEntry
            {
                Id = maxId+1,
                Fields = new List<IEntryField>(),
                Languages = entryLanguages,
                SearchText = source

            };

            sourceListView.AddObject(excelEntry);
            _terms.Add(excelEntry);
            JumpToTerm(excelEntry);
        }

        public void AddAndEdit(IEntry entry, ExcelDataGrid excelDataGrid)
        {
            
            var selectedTerm = _terms.FirstOrDefault(item => item.Id == entry.Id);
            var termToAdd = new EntryTerm
            {
                Value = excelDataGrid.Term
            };

            var excelTerm = new ExcelTerm
            {
                SourceCulture = entry.Languages[0].Locale,
                TargetCulture = entry.Languages[1].Locale,
                Target = excelDataGrid.Term
            };
            var source = (ExcelEntry) entry;
            excelTerm.Source = source.SearchText;

            var exist = false;
            if (selectedTerm != null)
            {
                foreach (var term in selectedTerm.Languages[1].Terms)
                {
                    if (term.Value == excelDataGrid.Term)
                    {
                        exist = true;
                    }
                   

                }

                if (exist == false)
                {
                    selectedTerm.Languages[1].Terms.Add(termToAdd);

                    _terms[entry.Id].Languages = selectedTerm.Languages;
                }
                
            }

            JumpToTerm(entry);
           
        }

        private async void confirmBtn_Click(object sender, EventArgs e)
        {
            if (sourceListView.SelectedObject == null) return;
            var entry = new ExcelTerm();


            var source = (ExcelEntry) sourceListView.SelectedObject;
            var entryId = source.Id;
            entry.Source = source.SearchText;

            foreach (var cultureCast in source.Languages.Cast<ExcelEntryLanguage>())
            {
                if (cultureCast.IsSource)
                {
                    entry.SourceCulture = cultureCast.Locale;
                }
                else
                {
                    entry.TargetCulture = cultureCast.Locale;
                }
            }

            var targetTerms = targetGridView.DataSource as List<ExcelDataGrid>;
            if (targetTerms != null)
            {
                var termValue = string.Join("|", targetTerms.Select(x=>x.Term));
                var approvedValue = string.Join("|", targetTerms.Select(x => x.Approved));
                entry.Target = termValue;
                entry.Approved = approvedValue;
            }

            await _excelTermProviderService.AddOrUpdateEntry(entryId, entry);

        }

        private void sourceListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var rowIndex = e.ItemIndex;
            var result = new List<ExcelDataGrid>();

            if (rowIndex == 0 && _terms.Count == 0)
            {
                targetGridView.DataSource = result;
                targetGridView.AllowUserToAddRows = false;
            }
            if (rowIndex >= _terms.Count) return;
            var item = _terms[rowIndex];
            foreach (var target in item.Languages)
            {
                var targetCast = (ExcelEntryLanguage)target;

                if (!targetCast.IsSource)
                {
                    result.AddRange(targetCast.Terms.Select(term => new ExcelDataGrid
                    {
                        Term = term.Value,
                        Approved = string.Join(string.Empty,term.Fields.Select(x=>x.Value))
                    }));
                   
                    
                }
            }

            targetGridView.DataSource = result;
            targetGridView.AllowUserToAddRows = true;

        }

        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (sourceListView.SelectedObject == null) return;
             var source = (ExcelEntry)sourceListView.SelectedObject;
            sourceListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;

            _terms.Remove(source);
            sourceListView.RemoveObject(source); //remove from listview
            sourceListView.SelectedIndex = 0;
            sourceListView.Focus();
            sourceListView.EnsureModelVisible(sourceListView.SelectedItem);


            await _excelTermProviderService.DeleteEntry(source.Id);

        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            
            

        }
    }


}
