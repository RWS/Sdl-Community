using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services;


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

            sourceListView.SetObjects(_terms);
            targetGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            targetGridView.ColumnHeadersVisible = false;
        }

        private void sourceListView_CellClick(object sender, BrightIdeasSoftware.CellClickEventArgs e)
        {
            var rowIndex = e.RowIndex;
            var item = _terms[rowIndex];
            var result = new List<ExcelDataGrid>();
            var approved = new List<string>();
            foreach (var target in item.Languages)
            {
                var targetCast = (ExcelEntryLanguage) target;

                if (!targetCast.IsSource)
                {
                    result.AddRange(targetCast.Terms.Select(term => new ExcelDataGrid
                    {
                        Term = term.Value,
                        Approved = null
                    }));
                    approved.AddRange(from approvedField in targetCast.Terms
                        from approvedTerm in approvedField.Fields
                        select approvedTerm.Value);
                }
            }

            for (var i = 0; i < result.Count; i++)
            {
                result[i].Approved = approved[i];
            }

            targetGridView.DataSource = result;
            approved.Clear();

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
                entry.Target = value.ToString();
            }

            _excelTermProviderService.UpdateEntry(entry, entryId);
        }
    }


}
