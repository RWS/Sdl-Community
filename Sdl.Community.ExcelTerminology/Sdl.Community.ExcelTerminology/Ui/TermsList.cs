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
        private readonly List<int> _newTerms = new List<int>();
        private readonly TerminologyProviderExcel _terminologyProviderExcel;
        private Uri _uri;

        public TermsList()
        {
            InitializeComponent();


        }

        public TermsList(TerminologyProviderExcel terminologyProviderExcel) : this()
        {
            _terminologyProviderExcel = terminologyProviderExcel;
            _terms = _terminologyProviderExcel.Terms;
            _uri = _terminologyProviderExcel.Uri;

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
            sourceListView.SetObjects(terms);
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
            var excelTerm = new ExcelTerm
            {
                SourceCulture = _providerSettings.SourceLanguage,
                TargetCulture = _providerSettings.TargetLanguage,
                Source = source,
                Target = target
            };
            AddTermInternal(excelTerm);
        }
        private void AddTermInternal(ExcelTerm excelTerm)
        {
            try
            {
                
                var entryLanguages = _transformerService.CreateEntryLanguages(excelTerm);

                var excelEntry = new ExcelEntry
                {
                    Id = 0,
                    Fields = new List<IEntryField>(),
                    Languages = entryLanguages,
                    SearchText = excelTerm.Source,
                    IsDirty = true

                };
        
                sourceListView.AddObject(excelEntry);
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
                var terms = sourceListView.Objects.Cast<ExcelEntry>().ToList();

                var selectedTerm = terms.FirstOrDefault(item => item.Id == entry.Id);
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
                source.IsDirty = true;
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

                        terms[entry.Id].Languages = selectedTerm.Languages;
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
                var terms = sourceListView.Objects.Cast<ExcelEntry>().ToList();

                var source = (ExcelEntry) sourceListView.SelectedObject;
                if (source.Id == 0)
                {
                    var maxId = terms.Max(x => x.Id);
                    source.Id = maxId + 1;
                }
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
                    var termValue = string.Join(_providerSettings.Separator.ToString(), targetTerms.Select(x => x.Term));
                    var approvedValue = string.Join(_providerSettings.Separator.ToString(), targetTerms.Select(x => x.Approved));
                    entry.Target = termValue;
                    entry.Approved = approvedValue;
                }



                var entryLanguage = _transformerService.CreateEntryLanguages(entry);
                var entryToUpdate = terms.Find(item => item.Id == entryId);

                if (entryToUpdate != null)
                {

                    entryToUpdate.Languages = entryLanguage;

                }
                await _excelTermProviderService.AddOrUpdateEntry(entryId, entry);
                source.IsDirty = false;
                if (_newTerms.Contains(entryId))
                {
                    _newTerms.Remove(entryId);
                }
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

                var terms = sourceListView.Objects.Cast<ExcelEntry>().ToList();


                var result = new List<ExcelDataGrid>();
                if (rowIndex == 0 && terms.Count == 0)
                {
                    bsTarget.DataSource = result;
                    bsTarget.AllowNew = false;
                }
                if (rowIndex >= terms.Count) return;
                var item = terms[rowIndex];
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

        private async void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
              
                var terms = sourceListView.Objects.Cast<ExcelEntry>().ToList();
                //get all terms that are new or have any changes
                var uiAddedTerms = terms.Where(x => x.IsDirty).ToList();
                //load excel entries from file
                await _terminologyProviderExcel.LoadEntries();
                //load in memory the newly loaded tersm
                terms = sourceListView.Objects.Cast<ExcelEntry>().ToList();
                var maxId = terms.Max(x => x.Id);
                var termsToBeSaved = new Dictionary<int, ExcelTerm>();
                foreach (var newTerm in uiAddedTerms)
                {
                    var ignoreTerm = false;
                    if (newTerm.Id == 0)
                    {
                        //if we have newly added term then the id is 0 so we need to 
                        //assing a new id which is the max row numbers
                        maxId++;
                        newTerm.Id = maxId;
                        newTerm.IsDirty = false;
                        terms.Add(newTerm);
                    }
                    else
                    {
                        //if we have an existing term that has some changes then we 
                        //need to look into the newly loaded from excel terms
                        // and see if we have the same terms
                        var existingTerms = terms.Where(
                            x => x.SearchText.Equals(newTerm.SearchText, StringComparison.InvariantCultureIgnoreCase))
                            .ToList();

                        foreach (var existingTerm in existingTerms)
                        {
                            if (existingTerm.Id != newTerm.Id)
                            {
                                //if we found terms that have the same text and they are comming 
                                //from excel we ignore the changes made in the viewer (lose them)
                                ignoreTerm = true;
                                continue;
                            }
                            //we merge the terms from excel with the ones from memory
                            newTerm.IsDirty = false;

                            var existingTargetLanguage =
                                existingTerm.Languages.Cast<ExcelEntryLanguage>().FirstOrDefault(x => !x.IsSource);
                            var newTargetLanguage =
                                newTerm.Languages.Cast<ExcelEntryLanguage>().FirstOrDefault(x => !x.IsSource);
                            foreach (var newTargetTerm in newTargetLanguage.Terms.Where(newTargetTerm => !existingTargetLanguage.Terms.Any(
                                x =>
                                    x.Value.Equals(newTargetTerm.Value,
                                        StringComparison.InvariantCultureIgnoreCase))))
                            {
                                existingTargetLanguage.Terms.Add(newTargetTerm);
                            }
                            //we want to save in the excel the merge result so we need to override the value to be saved
                            newTargetLanguage.Terms = existingTargetLanguage.Terms;

                        }
                      
                    }
                    if (!ignoreTerm)
                    {
                        termsToBeSaved.Add(newTerm.Id, newTerm.ToExcelTerm());
                    }
                }
                //save all the entries to excel
                await _excelTermProviderService.AddOrUpdateEntries(termsToBeSaved);
                SetTerms(terms);
                JumpToTerm(terms.FirstOrDefault());
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }
        }

        private void sourceListView_CellEditFinished(object sender, CellEditEventArgs e)
        {
           var termName =  e.NewValue as string;
            var entryTerm = e.RowObject as ExcelEntry;
            if (entryTerm == null) return;

            var sourceEntryTerms = _transformerService.CreateEntryTerms(termName);
            var sourceEntryLanguage = entryTerm.Languages.Cast<ExcelEntryLanguage>().FirstOrDefault(x => x.IsSource);
            if (sourceEntryLanguage == null)
            {
                var newSourceEntryLanguage = new ExcelEntryLanguage
                {
                    Locale =_providerSettings.SourceLanguage,
                    Name = _providerSettings.SourceLanguage.EnglishName,
                    Terms = sourceEntryTerms,
                    Fields = new List<IEntryField>(),
                    IsSource = true
                };
                entryTerm.Languages.Add(newSourceEntryLanguage);

            }
            else
            {
                sourceEntryLanguage.Terms = sourceEntryTerms;
            }
        }

        private void bsTarget_CurrentItemChanged(object sender, EventArgs e)
        {
            if (bsTarget.Count == 0) return;
            var currentSynonimEntry = bsTarget.Current as ExcelDataGrid;
            if (currentSynonimEntry == null) return;
            var excelEntry = sourceListView.SelectedObject as ExcelEntry;

            var targetEntryLanguage = excelEntry?.Languages.Cast<ExcelEntryLanguage>().FirstOrDefault(x => !x.IsSource);
            if (targetEntryLanguage == null) return;

            var targetTerm = targetEntryLanguage.Terms.FirstOrDefault(
                x => x.Value.Equals(currentSynonimEntry.Term, StringComparison.InvariantCultureIgnoreCase));



            if (targetTerm == null)
            {
                targetEntryLanguage.Terms.Clear();
                var targetSynonims = bsTarget.DataSource as List<ExcelDataGrid>;
                foreach (var targetSynonim in targetSynonims)
                {
                    var targetEntryTerms = _transformerService.CreateEntryTerms(targetSynonim.Term, targetSynonim.Approved);
                    foreach (var targetEntryTerm in targetEntryTerms)
                    {
                    targetEntryLanguage.Terms.Add(targetEntryTerm);

                    }
                }
                excelEntry.IsDirty = true;
            }
            else
            {
                var approvedField = targetTerm.Fields.FirstOrDefault(x => x.Name.Equals(EntryTransformerService.ApprovedFieldName));
                if (approvedField == null ||
                    approvedField.Value.Equals(currentSynonimEntry.Approved, StringComparison.InvariantCultureIgnoreCase))
                    return;
                targetTerm.Fields.Clear();
                targetTerm.Fields.Add(new EntryField
                {
                    Name = EntryTransformerService.ApprovedFieldName,
                    Value = currentSynonimEntry.Approved
                });
                excelEntry.IsDirty = true;
            }

         


        }
    }
}
