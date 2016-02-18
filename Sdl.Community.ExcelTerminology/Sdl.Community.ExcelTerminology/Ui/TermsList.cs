using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.ExcelTerminology.Insights;
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
        private List<ExcelEntry> _terms = new List<ExcelEntry>();
        private Uri _uri;

        public TermsList()
        {
            InitializeComponent();

            
        }

        public TermsList(List<ExcelEntry> terms,Uri uri):this()
        {
            _terms = terms;
            _uri = uri;

            var persistenceService = new PersistenceService();
            _providerSettings = persistenceService.Load(_uri);
            if (string.IsNullOrEmpty(_providerSettings.ApprovedColumn))
            {
                this.Approved.Visible = false;
            }
            var excelTermLoaderService = new ExcelTermLoaderService(_providerSettings);
            var parser = new Parser(_providerSettings);
            _transformerService = new EntryTransformerService(parser);

            _excelTermProviderService = new ExcelTermProviderService(excelTermLoaderService,
                _transformerService);
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
            sourceListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClickAlways;
            sourceListView.CellEditUseWholeCell = true;

            sourceListView.SetObjects(_terms);
            sourceListView.SelectedIndex = 0;
            targetGridView.ColumnHeadersVisible = false;
            targetGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            targetGridView.AllowUserToAddRows = true;

            sourceColumn.IsEditable = true;
            TelemetryService.Instance.TrackPage("Terms viewer");

        }



        public void JumpToTerm(IEntry entry)
        {
            try
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
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }

        }

        public void AddTerm(string source, string target)
        {
            AddTermInternal(source, target);
            Task.Run(Save);
        }

        private void AddTermInternal(string source, string target)
        {
            try
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
                    Id = maxId + 1,
                    Fields = new List<IEntryField>(),
                    Languages = entryLanguages,
                    SearchText = source

                };
                
                sourceListView.AddObject(excelEntry);
                _terms.Add(excelEntry);
                JumpToTerm(excelEntry);
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }
        }
     
        public void AddAndEdit(IEntry entry, ExcelDataGrid excelDataGrid)
        {
            try
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
                Task.Run(Save);
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            Task.Run(Save);
        }

        private async Task Save()
        {
            try
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

                var targetTerms = bsTarget.DataSource as List<ExcelDataGrid>;
                if (targetTerms != null)
                {
                    var termValue = string.Join("|", targetTerms.Select(x => x.Term));
                    var approvedValue = string.Join("|", targetTerms.Select(x => x.Approved));
                    entry.Target = termValue;
                    entry.Approved = approvedValue;
                }



                var entryLanguage = _transformerService.CreateEntryLanguages(entry);
                var entryToUpdate = _terms.Find(item => item.Id == entryId);
                
                if (entryToUpdate != null)
                {

                    entryToUpdate.Languages = entryLanguage;

                }
                await _excelTermProviderService.AddOrUpdateEntry(entryId, entry);
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }
        }

        private void sourceListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                var rowIndex = e.ItemIndex;


                var result = new List<ExcelDataGrid>();
                if (rowIndex == 0 && _terms.Count == 0)
                {
                    bsTarget.DataSource = result;
                    bsTarget.AllowNew = false;
                }
                if (rowIndex >= _terms.Count) return;
                var item = _terms[rowIndex];
                foreach (var target in item.Languages)
                {
                    var targetCast = (ExcelEntryLanguage) target;

                    if (!targetCast.IsSource)
                    {
                        result.AddRange(targetCast.Terms.Select(term => new ExcelDataGrid
                        {
                            Term = term.Value,
                            Approved = string.Join(string.Empty, term.Fields.Select(x => x.Value))
                        }));


                    }
                }
                bsTarget.DataSource = result;
                bsTarget.AllowNew = true;
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }
        }

        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (sourceListView.SelectedObject == null) return;
                var source = (ExcelEntry) sourceListView.SelectedObject;
                sourceListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;

                _terms.Remove(source);
                sourceListView.RemoveObject(source); //remove from listview
                sourceListView.SelectedIndex = 0;
                sourceListView.Focus();
                sourceListView.EnsureModelVisible(sourceListView.SelectedItem);


                await _excelTermProviderService.DeleteEntry(source.Id);
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }

        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            try
            {
                AddTermInternal(string.Empty, string.Empty);
                var item = sourceListView.GetLastItemInDisplayOrder();

                sourceListView.StartCellEdit(item, 0);
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }
        }

    }


}
