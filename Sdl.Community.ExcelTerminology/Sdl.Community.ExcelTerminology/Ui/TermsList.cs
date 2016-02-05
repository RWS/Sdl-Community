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
        private List<ExcelEntry> _terms;

        public TermsList()
        {
            InitializeComponent();

            var persistenceService = new PersistenceService();
            var providerSettings = persistenceService.Load();
            var excelTermLoaderService = new ExcelTermLoaderService(providerSettings);
            var parser = new Parser(providerSettings);
            var transformerService = new EntryTransformerService(parser);

            _excelTermProviderService = new ExcelTermProviderService(excelTermLoaderService, transformerService);
        }
        
        protected override void OnLoad(EventArgs e)
        {
            _terms = _excelTermProviderService.LoadEntries();
            sourceListView.ShowGroups = false;
            sourceListView.FullRowSelect = true;
            sourceListView.HeaderStyle = ColumnHeaderStyle.None;
            sourceListView.HideSelection = false;

            sourceListView.SetObjects(_terms);
           // targetGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
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
                sourceListView.SelectObject(selectedItem);
                
            }
            
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
            _excelTermProviderService.AddEntry(excelTerm,entry.Id);
            
           
        }
         private void confirmBtn_Click(object sender, EventArgs e)
        {
            var entry = new ExcelTerm();
            var entryId = new int();
            if (sourceListView.SelectedItem != null)
            {
                
                var source = (ExcelEntry)sourceListView.SelectedItem.RowObject;
                entryId = source.Id;
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
            }

           
            if (targetGridView.CurrentCell != null)
            {
                var value = targetGridView.CurrentCell.Value;
                if (value != null)
                {
                    entry.Target = value.ToString();
                }
                
            }

            _excelTermProviderService.UpdateEntry(entry, entryId);
        }

        private void sourceListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            
            var rowIndex = e.ItemIndex;
            var item = _terms[rowIndex];
             var result = new List<ExcelDataGrid>();
            //var approved = new List<string>();
            foreach (var target in item.Languages)
            {
                var targetCast = (ExcelEntryLanguage)target;

                if (!targetCast.IsSource)
                {
                    result.AddRange(targetCast.Terms.Select(term => new ExcelDataGrid
                    {
                        Term = term.Value,
                        Approved = null
                    }));
                    //approved.AddRange(from approvedField in targetCast.Terms
                    //                  from approvedTerm in approvedField.Fields
                    //                  select approvedTerm.Value);
                }
            }

          
      

            //for (var i = 0; i < result.Count; i++)
            //{
            //    result[i].Approved = approved[i];
            //}

            targetGridView.DataSource = result;
           // approved.Clear();
           
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
             var source = (ExcelEntry)sourceListView.SelectedItem.RowObject;
            sourceListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;

            _terms.Remove(source);
            sourceListView.RemoveObject(source); //remove from listview
            sourceListView.SetObjects(_terms);
            var nextTerm = _terms.Where(s => s.Id == source.Id + 1); 
            sourceListView.SelectObject(nextTerm.FirstOrDefault());// set the focus on next object

            _excelTermProviderService.DeleteEntry(source.Id);
        }
    }


}
