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
		private readonly ExcelTermProviderService _excelTermProviderService;
		private readonly ProviderSettings _providerSettings;
		private readonly EntryTransformerService _transformerService;
		private List<ExcelEntry> _terms = new List<ExcelEntry>();
		private bool _listChanged;
		private readonly TerminologyProviderExcel _terminologyProviderExcel;

		public static readonly Log Log = Log.Instance;

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
				confirmBtn.Enabled = false;
				sourceColumn.IsEditable = false;
				sourceListView.CellEditActivation = ObjectListView.CellEditActivateMode.None;
				targetGridView.AllowUserToAddRows = false;
				targetGridView.AllowUserToDeleteRows = false;
				targetGridView.ReadOnly = true;
			}
		}

		public void JumpToTerm(IEntry entry)
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

		public async void AddTerm(string source, string target)
		{
			

			AddTermInternal(source, target);
			await Save();
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

		public async void AddAndEdit(IEntry entry, ExcelData excelDataGrid)
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

		private void EditTerm(IEntry entry, ExcelData excelDataGrid)
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

		private async void confirmBtn_Click(object sender, EventArgs e)
		{
			await Save();
		}

		private async Task Save()
		{
			try
			{
				if (!_listChanged) return;
				_listChanged = false;

				if (sourceListView.SelectedObject == null) return;
				var entry = new ExcelTerm();
				var terms = SourceListViewEntries.ToList();

				var source = (ExcelEntry)sourceListView.SelectedObject;
				if (source.Id == 0)
				{
					var maxId = terms.Max(x => x.Id);
					source.Id = maxId + 1;
				}
				var entryId = source.Id;
				entry.Source = source.SearchText;

				var sourceEntryLanguages = source.Languages.Cast<ExcelEntryLanguage>().ToList();

				var isSourceFirst = sourceEntryLanguages[0].IsSource;
				entry.SourceCulture = sourceEntryLanguages[isSourceFirst ? 0 : 1].Locale;
				entry.TargetCulture = sourceEntryLanguages[isSourceFirst ? 1 : 0].Locale;

				var targetTerms = bsTarget.DataSource as List<ExcelData>;
				if (targetTerms != null)
				{
					var termValue = string.Join(_providerSettings.Separator.ToString(), targetTerms.Select(x => x.Term));
					var approvedValue = string.Join(_providerSettings.Separator.ToString(),
						targetTerms.Select(x => x.Approved));
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
				_terminologyProviderExcel.Terms.Add(source);

			}
			catch (Exception ex)
			{
				Log.Logger.Error($"Save method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
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

		private async void btnSync_Click(object sender, EventArgs e)
		{
			try
			{
				var terms = SourceListViewEntries.ToList();
				//get all terms that are new or have any changes
				var uiAddedTerms = terms.Where(x => x.IsDirty).ToList();
				//load excel entries from file
				await _terminologyProviderExcel.LoadEntries();
				//load in memory the newly loaded terms
				terms = SourceListViewEntries.ToList();
				var maxId = 0;
				if (terms.Count > 0)
				{
					maxId = terms.Max(x => x.Id);
				}
				var termsToBeSaved = new Dictionary<int, ExcelTerm>();
				foreach (var newTerm in uiAddedTerms)
				{
					var ignoreTerm = false;
					if (newTerm.Id == 0)
					{
						//if we have newly added term then the id is 0 so we need to 
						//assign a new id which is the max row numbers
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
								//if we found terms that have the same text and they are coming 
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
							if (newTargetLanguage != null)
							{
								foreach (
									var newTargetTerm in
									newTargetLanguage.Terms.Where(newTargetTerm =>
									{
										return existingTargetLanguage != null && !existingTargetLanguage.Terms.Any(
												   x =>
													   x.Value.Equals(newTargetTerm.Value,
														   StringComparison.InvariantCultureIgnoreCase));
									}))
								{
									existingTargetLanguage?.Terms.Add(newTargetTerm);
								}
								//we want to save in the excel the merge result so we need to override the value to be saved
								if (existingTargetLanguage != null)
								{
									newTargetLanguage.Terms = existingTargetLanguage.Terms;
								}
							}
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
				Log.Logger.Error($"btnSync_Click method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
		}

		private IEnumerable<ExcelEntry> SourceListViewEntries => sourceListView.Objects.Cast<ExcelEntry>();

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

		private void sourceListView_CellEditStarting(object sender, CellEditEventArgs e)
		{
			var textBox = e.Control as CustomTabTextBox;
			if (textBox == null) return;

			textBox.OnTabPressed += TextBox_OnTabPressed;
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