using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Services;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Ui
{
	public partial class TermsList : UserControl
	{
		public static readonly Log Log = Log.Instance;
		private readonly ExcelTermProviderService _excelTermProviderService;
		private readonly ProviderSettings _providerSettings;
		private readonly TerminologyProviderExcel _terminologyProviderExcel;
		private readonly EntryTransformerService _transformerService;
		private bool _listChanged;
		private List<ExcelEntry> _terms = new List<ExcelEntry>();

		public TermsList()
		{
			InitializeComponent();
		}

		public TermsList(TerminologyProviderExcel terminologyProviderExcel) : this()
		{
			_terminologyProviderExcel = terminologyProviderExcel;

			_terms = _terminologyProviderExcel.Terms;
			var uri = _terminologyProviderExcel.Uri;

			var persistenceService = new PersistenceService();
			_providerSettings = persistenceService.Load(uri);
			if (string.IsNullOrEmpty(_providerSettings.ApprovedColumn))
			{
				Approved.Visible = false;
			}
			var excelTermLoaderService = new ExcelTermLoaderService(_providerSettings);
			var parser = new Parser(_providerSettings);
			_transformerService = new EntryTransformerService(parser);

			_excelTermProviderService = new ExcelTermProviderService(excelTermLoaderService, _transformerService);
			_terminologyProviderExcel.TermsLoaded += SetTerms;
			SetTerms(_terminologyProviderExcel.Terms);

			sourceListView.CellEditFinishing += SourceListView_CellEditFinished;
		}

		private IEnumerable<ExcelEntry> SourceListViewEntries => sourceListView.Objects.Cast<ExcelEntry>();

		public async void AddAndEdit(Entry entry, ExcelData excelDataGrid)
		{
			if (!string.IsNullOrEmpty(excelDataGrid?.Term))
			{
				EditTerm(entry, excelDataGrid);
				JumpToTerm(entry);

				await Save();
			}
			else
			{
				MessageBox.Show(@"Target selection cannot be empty", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		public async void AddTerm(string source, string target)
		{
			AddTermInternal(source, target);
			await Save();
		}

		public void JumpToTerm(Entry entry)
		{
			try
			{
				var selectedItem = SourceListViewEntries.FirstOrDefault(s => s.Id == entry.Id);

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
				Log.Logger.Error($"JumpToTerm method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		public void SetTerms(List<ExcelEntry> terms)
		{
			_terms = terms;
			sourceListView.SetObjects(terms);
			sourceListView.SelectedIndex = 0;
			SetReadOnlyControls();
		}

		protected override void OnLoad(EventArgs e)
		{
			ObjectListView.EditorRegistry.Register(typeof(string), typeof(CustomTabTextBox));
			sourceListView.ShowGroups = false;
			sourceListView.FullRowSelect = true;
			sourceListView.HeaderStyle = ColumnHeaderStyle.None;
			sourceListView.HideSelection = false;
			sourceListView.SetObjects(_terms);
			sourceListView.SelectedIndex = 0;
			targetGridView.ColumnHeadersVisible = false;
			SetReadOnlyControls();
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
				Log.Logger.Error($"addBtn_Click method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		private void AddTermInternal(string source, string target)
		{
			try
			{
				var excelEntry = _transformerService.CreateExcelEntry(source, target, _providerSettings.SourceLanguage, _providerSettings.TargetLanguage);

				AddToInternalList(excelEntry);
				JumpToTerm(excelEntry);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"AddTermInternal method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		private void AddToInternalList(ExcelEntry excelEntry)
		{
			var entriesSearchTexts = SourceListViewEntries.Select(e => e.SearchText);
			if (entriesSearchTexts.Any(e => e == excelEntry.SearchText)) return;

			sourceListView.AddObject(excelEntry);
			_listChanged = true;
		}

		private void bsTarget_CurrentItemChanged(object sender, EventArgs e)
		{
			if (bsTarget.Count == 0) return;
			var currentSynonimEntry = bsTarget.Current as ExcelData;
			if (currentSynonimEntry == null) return;
			var excelEntry = sourceListView.SelectedObject as ExcelEntry;

			var targetEntryLanguage = excelEntry?.Languages.Cast<ExcelEntryLanguage>().FirstOrDefault(x => !x.IsSource);
			if (targetEntryLanguage == null) return;

			var targetTerm = targetEntryLanguage.Terms.FirstOrDefault(
				x => x.Value.Equals(currentSynonimEntry.Term, StringComparison.InvariantCultureIgnoreCase));

			if (targetTerm == null)
			{
				targetEntryLanguage.Terms.Clear();
				var targetSynonims = bsTarget.DataSource as List<ExcelData>;
				if (targetSynonims != null)
					foreach (var targetSynonim in targetSynonims)
					{
						var targetEntryTerms = _transformerService.CreateEntryTerms(targetSynonim.Term,
							targetSynonim.Approved);
						foreach (var targetEntryTerm in targetEntryTerms)
						{
							targetEntryLanguage.Terms.Add(targetEntryTerm);
						}
					}
				excelEntry.IsDirty = true;
			}
			else
			{
				var approvedField =
					targetTerm.Fields.FirstOrDefault(x => x.Name.Equals(EntryTransformerService.ApprovedFieldName));
				if (approvedField != null && approvedField.Value.Equals(currentSynonimEntry.Approved, StringComparison.InvariantCultureIgnoreCase))
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

		private async void btnSync_Click(object sender, EventArgs e)
		{
			try
			{
				var terms = SourceListViewEntries.ToList();
				var uiAddedTerms = terms.Where(x => x.IsDirty).ToList();
				AssignIdsToNewTerms(terms, uiAddedTerms);

				var excelTerms = uiAddedTerms.ToDictionary(t => t.Id, t => t.ToExcelTerm());
				await _excelTermProviderService.AddOrUpdateEntries(excelTerms);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"btnSync_Click method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		private static void AssignIdsToNewTerms(List<ExcelEntry> terms, List<ExcelEntry> uiAddedTerms)
		{
			var maxId = 0;
			if (terms.Count > 0)
			{
				maxId = terms.Max(x => x.Id);
			}

			uiAddedTerms.ForEach(t =>
			{
				if (t.Id == 0) t.Id = ++maxId;
			});
		}

		private async void confirmBtn_Click(object sender, EventArgs e)
		{
			await Save();
		}

		private async void deleteBtn_Click(object sender, EventArgs e)
		{
			try
			{
				if (sourceListView.SelectedObject == null) return;
				var source = (ExcelEntry)sourceListView.SelectedObject;
				sourceListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;

				sourceListView.RemoveObject(source); //remove from listview
				sourceListView.SelectedIndex = 0;
				sourceListView.Focus();
				sourceListView.EnsureModelVisible(sourceListView.SelectedItem);

				await _excelTermProviderService.DeleteEntry(source.Id);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"deleteBtn_Click method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		private void EditTerm(Entry entry, ExcelData excelDataGrid)
		{
			((ExcelEntry)entry).IsDirty = true;

			var terms = sourceListView.Objects?.Cast<ExcelEntry>().ToList();
			var selectedTerm = terms.FirstOrDefault(item => item.Id == entry.Id);

			var targetLanguage = selectedTerm?.Languages.Cast<ExcelEntryLanguage>()
				.FirstOrDefault(x => !x.IsSource);
			var exist = false;
			if (targetLanguage != null)
			{
				foreach (var term in targetLanguage.Terms)
				{
					if (term.Value == excelDataGrid.Term)
					{
						exist = true;
					}
				}

				if (exist == false)
				{
					_listChanged = true;
					var termToAdd = new EntryTerm
					{
						Value = excelDataGrid.Term
					};
					targetLanguage.Terms.Add(termToAdd);

					var updatingTerm = terms.FirstOrDefault(x => x.Id == entry.Id);
					updatingTerm.Languages = selectedTerm.Languages;
				}
			}
		}

		private async Task Save()
		{
			try
			{
				if (!_listChanged) return;
				_listChanged = false;

				if (sourceListView.SelectedObject == null) return;
				var terms = SourceListViewEntries.ToList();

				var source = (ExcelEntry)sourceListView.SelectedObject;
				if (source.Id == 0)
				{
					var maxId = terms.Max(x => x.Id);
					source.Id = maxId + 1;
				}

				var sourceEntryLanguages = source.Languages.Cast<ExcelEntryLanguage>().ToList();
				var isSourceFirst = sourceEntryLanguages[0].IsSource;

				var targetTerms = (List<ExcelData>)bsTarget.DataSource;
				var termValue = string.Join(_providerSettings.Separator.ToString(), targetTerms.Select(x => x.Term));
				var approvedValue = string.Join(_providerSettings.Separator.ToString(),
					targetTerms.Select(x => x.Approved));

				var entry = _transformerService.GetExcelTerm(source.SearchText, termValue,
					sourceEntryLanguages[isSourceFirst ? 0 : 1].Locale,
					sourceEntryLanguages[isSourceFirst ? 1 : 0].Locale);
				entry.Approved = approvedValue;

				await _excelTermProviderService.AddOrUpdateEntry(source.Id, entry);
				source.IsDirty = false;

				_terminologyProviderExcel.Terms.Add(source);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Save method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		private void SetReadOnlyControls()
		{
			if (!_providerSettings.IsReadOnly)
			{
				targetGridView.EditMode = DataGridViewEditMode.EditOnEnter;
				targetGridView.AllowUserToAddRows = true;
				sourceColumn.IsEditable = true;
				sourceListView.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
				sourceListView.CellEditUseWholeCell = true;
			}
			else
			{
				addBtn.Enabled = false;
				deleteBtn.Enabled = false;
				sourceColumn.IsEditable = false;
				sourceListView.CellEditActivation = ObjectListView.CellEditActivateMode.None;
				targetGridView.AllowUserToAddRows = false;
				targetGridView.AllowUserToDeleteRows = false;
				targetGridView.ReadOnly = true;
			}
		}

		private void sourceListView_CellEditFinished(object sender, CellEditEventArgs e)
		{
			var textBox = e.Control as CustomTabTextBox;
			if (textBox != null)
			{
				textBox.OnTabPressed -= TextBox_OnTabPressed;
			}
			var termName = e.NewValue as string;
			var entryTerm = e.RowObject as ExcelEntry;
			if (entryTerm == null) return;

			var sourceEntryTerms = _transformerService.CreateEntryTerms(termName);
			var sourceEntryLanguage = entryTerm.Languages.Cast<ExcelEntryLanguage>().FirstOrDefault(x => x.IsSource);
			if (sourceEntryLanguage == null)
			{
				var newSourceEntryLanguage = new ExcelEntryLanguage
				{
					Locale = _providerSettings.SourceLanguage,
					Name = _providerSettings.SourceLanguage.EnglishName,
					Terms = sourceEntryTerms,
					Fields = new List<EntryField>(),
					IsSource = true
				};
				entryTerm.Languages.Add(newSourceEntryLanguage);
			}
			else
			{
				sourceEntryLanguage.Terms = sourceEntryTerms;
			}
		}

		private void SourceListView_CellEditFinished(object sender, CellEditEventArgs e)
		{
			var entries = SourceListViewEntries.Select(entry => entry.SearchText);
			if (string.IsNullOrWhiteSpace(e.NewValue.ToString()))
			{
				e.Cancel = true;
			}
			else if (entries.Any(entry => entry == e.NewValue.ToString()))
			{
				MessageBox.Show(@"Cannot have two identical entries. Add the terms to the existing entry.", @"Duplicate entries", MessageBoxButtons.OK);
				e.Cancel = true;
			}
		}

		private void sourceListView_CellEditStarting(object sender, CellEditEventArgs e)
		{
			var textBox = e.Control as CustomTabTextBox;
			if (textBox == null) return;

			textBox.OnTabPressed += TextBox_OnTabPressed;
		}

		private void sourceListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			try
			{
				var rowIndex = e.ItemIndex;
				var terms = SourceListViewEntries.ToList();

				var result = new List<ExcelData>();
				if (rowIndex == 0 && terms.Count == 0)
				{
					bsTarget.DataSource = result;
					bsTarget.AllowNew = false;
				}
				if (rowIndex >= terms.Count) return;
				var item = terms[rowIndex];
				foreach (var target in item.Languages)
				{
					var targetCast = (ExcelEntryLanguage)target;

					if (!targetCast.IsSource)
					{
						result.AddRange(targetCast.Terms.Select(term => new ExcelData
						{
							Term = term.Value,
							Approved = string.Join(string.Empty, term.Fields.Select(x => x.Value))
						}));
					}
				}
				bsTarget.DataSource = result;
				bsTarget.AllowNew = !_providerSettings.IsReadOnly;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"sourceListView_ItemSelectionChanged method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		private void targetGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			var dataGridViewColumn = targetGridView.Columns["Approved"];
			if (dataGridViewColumn != null && ((e.ColumnIndex == dataGridViewColumn.Index)
											   && e.Value != null))
			{
				var cell = targetGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

				cell.ToolTipText = cell.Value.ToString();
			}
		}

		private void TextBox_OnTabPressed(object source, TabPressedEventArgs e)
		{
			var initialCount = bsTarget.Count;
			targetGridView.Focus();
			//if the list is empty bs will add a new item and we don't have to do it
			if (initialCount == bsTarget.Count)
			{
				bsTarget.AddNew();
			}
			bsTarget.Position = bsTarget.Count - 1;
			e.Handled = true;
		}
	}
}