using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using FolderSelect;
using log4net;
using SDLXLIFFSliceOrChange.Data;
using SDLXLIFFSliceOrChange.ResourceManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Sdl.Community.Toolkit.Core.Services;
using Sdl.Core.Globalization;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using SdlXliff.Toolkit.Integration;
using SdlXliff.Toolkit.Integration.Controls;
using SdlXliff.Toolkit.Integration.Data;
using SdlXliff.Toolkit.Integration.File;

namespace SDLXLIFFSliceOrChange
{
	public partial class SDLXLIFFSliceOrChange : UserControl
	{
		private ErrorProvider _errorProvider;

		public SDLXLIFFSliceOrChange()
		{
			InitializeComponent();
			_sliceManager = new SliceManager(this);
			_updateManager = new UpdateManager(this);
			_errorProvider = new ErrorProvider();
		}

		private bool _saveCultrue = true;
		private ILog logger = LogManager.GetLogger(typeof(SDLXLIFFSliceOrChange));

		public SliceManager SliceManager
		{
			get { return _sliceManager; }
		}

		public UpdateManager UpdateManager
		{
			get { return _updateManager; }
		}

		private bool _setFormSizeChanged = true;
		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			_setFormSizeChanged = false;
			groupSlice.Enabled = groupChange.Enabled = groupClear.Enabled = tabControl1.SelectedIndex != 2;
			if (tabControl1.SelectedIndex == 1)
				BindSearchResults();
			if (tabControl1.SelectedIndex == 2)
			{
				panelCommands.Visible = false;
				BindReplaceResults();
			}
			else
			{
				panelCommands.Visible = true;
			}
			_setFormSizeChanged = true;

			if (tabControl1.SelectedIndex == 1)
			{
				var resources = new UIResources(Settings.GetSavedCulture());
				var selIndex = cmbOperator.SelectedIndex;
				cmbOperator.Items.Clear();
				cmbOperator.Items.Add(resources.OR);
				cmbOperator.Items.Add(resources.AND);
				cmbOperator.SelectedIndex = selIndex == -1 ? 0 : selIndex;
			}
		}

		private void HideProcess()
		{
			_setFormSizeChanged = false;
			if (tabControl1.SelectedIndex == 2)
			{
				panelCommands.Visible = false;
			}
			else
			{
				panelCommands.Visible = true;
			}


			panelStatus.Visible = false;
			_setFormSizeChanged = true;
		}

		public void ShowProcess()
		{
			_setFormSizeChanged = false;
			panelStatus.Visible = true;
			progressBar.Value = progressBar.Minimum = 0;
			progressBar.Maximum = 100;
			if (tabControl1.SelectedIndex == 2)
			{
				panelCommands.Visible = false;
			}

			_setFormSizeChanged = true;
		}

		public void StepProcess(String message, bool lastStep = false)
		{
			Invoke((MethodInvoker)delegate
			   {
				   try
				   {

					   progressBar.Focus();
					   if (progressBar.Value + 1 < progressBar.Maximum)
						   progressBar.Value++;

					   if (lastStep)
					   {
						   progressBar.Value = progressBar.Maximum;
						   HideProcess();
					   }
				   }
				   catch (Exception ex)
				   {
					   if (lastStep)
					   {
						   HideProcess();
					   }
					   logger.Error(ex.Message, ex);
				   }
			   });
		}

		private void SDLXLIFFSliceOrChange_Load(object sender, EventArgs e)
		{
			HideProcess();
			BindDDL(cmbLanguages, GetAllAvailableLanguages());
			_saveCultrue = true;
			cmbLanguages.SelectedValue = Settings.GetSavedCulture();
		}

		private List<KeyValuePair<String, String>> GetAllAvailableLanguages()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var dir = Path.GetDirectoryName(assembly.Location);
			if (string.IsNullOrEmpty(dir))
				return new List<KeyValuePair<String, string>>();
			var languageFiles = Directory.GetFiles(dir, "*.resx");

			var cultures = new List<KeyValuePair<string, string>>();
			foreach (var languageFile in languageFiles)
			{
				var langs = languageFile.Split('.');
				var cultureID = langs[langs.Count() - 2];
				var cultureName = new CultureInfo(cultureID).NativeName.Split(' ')[0];
				cultures.Add(new KeyValuePair<string, string>(cultureID, cultureName));
			}
			return cultures;
		}

		private void BindDDL(ComboBox comboBox, List<KeyValuePair<String, String>> items)
		{
			comboBox.ValueMember = "Key";
			comboBox.DisplayMember = "Value";
			comboBox.DataSource = items;
		}

		private void cmbLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			var culture = cmbLanguages.SelectedValue.ToString();
			if (_saveCultrue)
				Settings.SaveSelectedCulture(culture);
			EnsureUIBasedOnCultureInfo(culture);
		}

		private void EnsureUIBasedOnCultureInfo(string culture)
		{
			var resources = new UIResources(culture);

			btnSelectSDLXLIFFFiles.Text = resources.SelectXLIFF;
			toolStripButton1.Text = resources.ClearFiles;
			ckMerge.Text = resources.Merge;
			ckCopySourceToTarget.Text = resources.CopySourceToTarget;
			groupReplace.Text = resources.ReplaceGroup;
			groupReplaceOptions.Text = resources.Options;
			ckReplaceMatchCase.Text = resources.SearchMatchCase;
			ckReplaceMatchWholeWord.Text = resources.SearchMatchWholeWord;
			ckReplaceUseRegEx.Text = resources.SearchRegEx;
			lblReplaceSourceSearch.Text = resources.SearchSource;
			lblReplaceTargetSearch.Text = resources.SearchTarget;
			lblReplaceSourceReplace.Text = lblReplaceTargetReplace.Text = resources.ReplaceWith;
			btnFindAllInReplace.Text = resources.FindAll;
			btnPreview.Text = resources.Preview;
			btnReplace.Text = resources.Replace;


			pageStatuses.Text = resources.Statuses;
			pageSearch.Text = resources.Search;
			groupChange.Text = resources.Change;
			groupSlice.Text = resources.Slice;
			labelSliceComments.Text = resources.SliceComments;
			btnSliceit.Text = resources.Sliceit;
			btnChangeit.Text = resources.Changeit;
			labelChangeComments.Text = resources.ChangeComments;
			groupChangeToStatusOr.Text = resources.ChangeToStatusOr;
			groupChangeLocked.Text = resources.ChangeLocked;
			groupChangeTranslationStatus.Text = resources.ChangeTranslationStatus;
			ckChangeToSignedOff.Text = resources.ChangeToSignedOff;
			ckChangeToSignOffRejected.Text = resources.ChangeToSignOffRejected;
			ckChangeToTranslationApproved.Text = resources.ChangeToTranslationApproved;
			ckChangeToTranslationRejected.Text = resources.ChangeToTranslationRejected;
			ckChangeToTranslated.Text = resources.ChangeToTranslated;
			ckChangeToDraft.Text = resources.ChangeToDraft;
			ckChangeToNotTranslated.Text = resources.ChangeToNotTranslated;
			ckChangeToUnlocked.Text = resources.ChangeToUnlocked;
			ckChangeToLocked.Text = resources.ChangeToLocked;
			btnSelectFolder.Text = resources.SelectFolder;
			btnSelectProjectFile.Text = resources.ProjectFile;
			groupScore.Text = resources.Score;
			ckMatchValues.Text = resources.MatchValues;
			ckContextMatch.Text = resources.ContextMatch;
			ckPerfectMatch.Text = resources.PerfectMatch;
			groupStatusesLocked.Text = resources.StatusesLocked;
			ckUnlocked.Text = resources.Unlocked;
			ckLocked.Text = resources.Locked;
			groupStatusesTranslationStatus.Text = resources.StatusesTranslationStatus;
			ckSignedOff.Text = resources.ApprovedSignOff;
			ckSignOffRejected.Text = resources.RejectedSignOff;
			ckTranslationApproved.Text = resources.ApprovedTranslation;
			ckTranslationRejected.Text = resources.RejectedTranslation;
			ckTranslated.Text = resources.Translated;
			ckDraft.Text = resources.Draft;
			ckNotTranslated.Text = resources.NotTranslated;
			groupTranslationOrigin.Text = resources.TranslationOrigin;
			ckAutoPropagated.Text = resources.AutoPropagated;
			ckAutomatedTranslation.Text = resources.AutomatedTranslation;
			ckInteractive.Text = resources.Interactive;
			ckTranslationMemory.Text = resources.TranslationMemory;
			groupSystem.Text = resources.System;
			ckPropagated.Text = resources.Propagated;
			ckSystemTranslationMemory.Text = resources.SystemTranslationMemory;
			ckSystemMachineTranslation.Text = resources.SystemMachineTranslation;
			groupDocumentStructure.Text = resources.DocumentStructure;
			btnGenerateDSI.Text = resources.GenerateDSI;
			labelGenerateDSIComments.Text = resources.GenerateDSIComments;
			colPath.HeaderText = resources.FileLocation;
			colName.HeaderText = resources.Name;
			colSize.HeaderText = resources.Size;
			colDate.HeaderText = resources.Date;
			groupOptions.Text = resources.Options;
			ckSearchInTags.Text = resources.SearchInTags;
			ckSearchRegEx.Text = resources.SearchRegEx;
			ckSearchMatchWholeWord.Text = resources.SearchMatchWholeWord;
			ckSearchMatchCase.Text = resources.SearchMatchCase;
			groupSearch.Text = resources.Search;
			labelSearchTarget.Text = resources.SearchTarget;
			labelSearchSource.Text = resources.SearchSource;
			btnFindAll.Text = resources.FindAll;
			btnReverseSelection.Text = resources.ReverseSelection;

			var selIndex = cmbOperator.SelectedIndex;

			cmbOperator.Items.Clear();
			cmbOperator.Items.Add(resources.OR);
			cmbOperator.Items.Add(resources.AND);
			cmbOperator.SelectedIndex = selIndex == -1 ? 0 : selIndex;

			groupClear.Text = resources.Clear;
			btnClearit.Text = resources.Clearit;
			lblClearit.Text = resources.ClearitDescription;
		}

		private void btnReverseSelection_Click(object sender, EventArgs e)
		{
			foreach (var c in pageStatuses.Controls.OfType<GroupBox>())
			{
				foreach (var ck in c.Controls.OfType<CheckBox>())
				{
					ck.Checked = !ck.Checked;
				}
				foreach (var list in c.Controls.OfType<ListBox>())
				{
					var selectedItems =
						list.Items.Cast<object>().Where(item => !list.SelectedItems.Contains(item)).ToList();
					list.SelectedItems.Clear();
					foreach (var selItem in selectedItems)
						list.SelectedItems.Add(selItem);
				}
			}
		}

		private string GetProjectSourceLanguageCode(string projectPath)
		{
			var document = new XmlDocument();
			document.PreserveWhitespace = true;
			document.Load(projectPath);
			var selectSingleNode = document.SelectSingleNode("//SourceLanguageCode");
			if (selectSingleNode != null)
				return selectSingleNode.InnerText;

			return string.Empty;
		}

		private string GetStudioVersion()
		{
			var studioVersionService = new StudioVersionService();
			var studioVersion = studioVersionService.GetStudioVersion();

			var studioFolder = string.Empty;
			if (studioVersion != null)
			{
				if (studioVersion.Version.Equals("Studio15"))
				{
					studioFolder = "Studio 2019";
				}
			}

			return studioFolder;
		}

		private void btnSelectFolder_Click(object sender, EventArgs e)
		{
			var selectFolder = new FolderSelectDialog();
			selectFolder.InitialDirectory = GetProjectPath();
			if (selectFolder.ShowDialog())
			{
				var dsFiles = new XLIFFFiles();
				var files = Directory.GetFiles(selectFolder.FileName, "*.sdlxliff");
				foreach (var file in files)
				{
					var fileRow = dsFiles.Files.NewFilesRow();
					fileRow.Path = file;
					fileRow.Name = Path.GetFileName(file);
					fileRow.Size = ((decimal)(new FileInfo(file)).Length / 1000).ToString(CultureInfo.InvariantCulture);
					fileRow.Date = (new FileInfo(file)).CreationTime.ToString(CultureInfo.InvariantCulture);
					dsFiles.Files.AddFilesRow(fileRow);
				}
				BindFilesGrid(dsFiles);
			}
		}

		private void btnSelectSDLXLIFFFiles_Click(object sender, EventArgs e)
		{
			selectProjectFile.InitialDirectory = GetProjectPath();
			selectProjectFile.CheckFileExists = true;
			selectProjectFile.Filter = "SDLXLIFF Files (.sdlxliff)|*.sdlxliff";
			selectProjectFile.Multiselect = true;
			if (selectProjectFile.ShowDialog() == DialogResult.OK)
			{
				var dsFiles = new XLIFFFiles();
				var files = selectProjectFile.FileNames;
				foreach (var file in files)
				{
					var fileRow = dsFiles.Files.NewFilesRow();
					fileRow.Path = file;
					fileRow.Name = Path.GetFileName(file);
					fileRow.Size = ((decimal)(new FileInfo(file)).Length / 1000).ToString(CultureInfo.InvariantCulture);
					fileRow.Date = (new FileInfo(file)).CreationTime.ToString(CultureInfo.InvariantCulture);
					dsFiles.Files.AddFilesRow(fileRow);
				}
				BindFilesGrid(dsFiles);
			}
		}

		private void BindFilesGrid(XLIFFFiles dsFiles)
		{
			gridXLIFFFiles.DataSource = dsFiles;
			gridXLIFFFiles.DataMember = dsFiles.Files.TableName;
		}

		private void btnSelectProjectFile_Click(object sender, EventArgs e)
		{
			selectProjectFile.InitialDirectory = GetProjectPath();
			selectProjectFile.CheckFileExists = true;
			selectProjectFile.Filter = "SDL Project Files (.sdlproj)|*.sdlproj";
			selectProjectFile.Multiselect = false;
			if (selectProjectFile.ShowDialog() == DialogResult.OK)
			{
				var phisicalFilesPath = GetPhisicalFilesPathFromProjectFile(selectProjectFile.FileName);

				var dsFiles = new XLIFFFiles();
				foreach (var file in phisicalFilesPath)
				{
					var fileRow = dsFiles.Files.NewFilesRow();
					fileRow.Path = string.Format("{0}\\{1}", Path.GetDirectoryName(selectProjectFile.FileName), file);
					fileRow.Name = file;
					fileRow.Size =
						((decimal)(new FileInfo(fileRow.Path)).Length / 1000).ToString(CultureInfo.InvariantCulture);
					fileRow.Date = (new FileInfo(fileRow.Path)).CreationTime.ToString(CultureInfo.InvariantCulture);
					dsFiles.Files.AddFilesRow(fileRow);
				}
				BindFilesGrid(dsFiles);
			}
		}

		private IEnumerable<string> GetPhisicalFilesPathFromProjectFile(string projectFile)
		{
			var sourceCulture = GetProjectSourceLanguageCode(projectFile) + "\\";
			var phisicalFilesPath = new List<string>();
			try
			{
				var projectsFile = new XPathDocument(projectFile);
				var nav = projectsFile.CreateNavigator();
				const string expression = "//FileVersion";
				var fileVersions = nav.Select(expression);
				while (fileVersions.MoveNext())
				{
					var filePath = fileVersions.Current.SelectSingleNode("@PhysicalPath");
					if (filePath != null)
					{
						var fileName = filePath.Value;
						if (fileName.IndexOf(".sdlxliff", StringComparison.Ordinal) != -1 &&
							!phisicalFilesPath.Contains(fileName) && !fileName.Contains(sourceCulture))
							phisicalFilesPath.Add(fileName);
					}
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message, ex);
			}
			return phisicalFilesPath;
		}

		private List<StructureInformationType> _structureInformationTypes = new List<StructureInformationType>();

		private void btnGenerateDSI_Click(object sender, EventArgs e)
		{
			ShowProcess();
			var files =
				(from DataGridViewRow row in gridXLIFFFiles.SelectedRows select row.Cells[0].Value.ToString()).ToList();
			_structureInformationTypes.Clear();
			StepProcess("Reading Document Structure information from files...", false);
			var trd = new Thread(() =>
				{
					foreach (var file in files)
					{
						StepProcess(
							"Reading Document Structure information from file: " + Path.GetFileName(file) + ".", false);
						using (var reader = XmlReader.Create(file))
						{
							while (reader.Read())
							{
								if (reader.Name == "cxt-defs")
								{
									var cxtDoc = new XmlDocument();
									cxtDoc.PreserveWhitespace = true;
									cxtDoc.LoadXml(reader.ReadOuterXml());
									var cxtDefs = cxtDoc.DocumentElement.GetElementsByTagName("cxt-def");

									foreach (var cxtDef in cxtDefs.OfType<XmlElement>())
									{
										var type = cxtDef.Attributes["type"].Value;
										var id = cxtDef.Attributes["id"].Value;

										var cxt =
											_structureInformationTypes.FirstOrDefault(t => t.InternalName == type);
										if (cxt == null)
											_structureInformationTypes.Add(new StructureInformationType()
											{
												IDs =
														new List<KeyValuePair<string, List<string>>>()
															{
																new KeyValuePair<string, List<string>>(file,
																									   new List<string>()
																										   {
																											   id
																										   })
															},
												InternalName = type,
												DisplayName = type
											});
										else
										{
											if (cxt.IDs.Any(f => f.Key == file))
											{
												var idsPerFile =
													cxt.IDs.FirstOrDefault(f => f.Key == file);
												if (!idsPerFile.Value.Contains(id))
													idsPerFile.Value.Add(id);
											}
											else
											{
												cxt.IDs.Add(new KeyValuePair<string, List<string>>(file,
																								   new List<string>()
																									   {
																										   id
																									   }));
											}
										}
									}
								}
							}
						}
					}
					StepProcess("Displaying Document Structure information ...", true);
					DisplayDSI();
				});
			trd.Start();
		}

		private void DisplayDSI()
		{
			Invoke((MethodInvoker)delegate
			   {
				   listDocumentStructure.Items.Clear();
				   listDocumentStructure.Items.AddRange(
					   _structureInformationTypes.Select(type => type.DisplayName).ToArray());
			   });
		}

		private void btnSliceit_Click(object sender, EventArgs e)
		{
			var studioFolder = GetStudioVersion();
			var folder = new FolderSelectDialog
			{
				InitialDirectory = string.Format(@"{0}\{1}\Projects",
					Environment.GetFolderPath(
						Environment.SpecialFolder.MyDocuments), studioFolder)
			};
			if (folder.ShowDialog())
			{
				_folderForSlicedFiles = folder.FileName;

				var files =
					(from DataGridViewRow row in gridXLIFFFiles.SelectedRows select row.Cells[0].Value.ToString())
						.ToList();
				if (files.Count == 0) return;
				ShowProcess();
				StepProcess("Sliceing files ...", false);

				var selectedTabIndex = tabControl1.SelectedIndex;
				var selectedDSIndexes = listDocumentStructure.SelectedIndices.Cast<int>().ToList();
				var doMerge = ckMerge.Checked;
				var doOR = cmbOperator.SelectedIndex == 0;
				var t = new Thread(() => DoSliceNow(selectedTabIndex, selectedDSIndexes, doMerge, doOR));
				t.Start();
			}
		}

		public string _folderForSlicedFiles;

		private void SliceFiles(bool doMerge)
		{
			if (_segmentsToBeSliced.Count == 0)
				return;

			var tempFolderForSlicedFiles = Path.Combine(_folderForSlicedFiles, Guid.NewGuid().ToString());
			if (!Directory.Exists(tempFolderForSlicedFiles))
				Directory.CreateDirectory(tempFolderForSlicedFiles);

			var filesPerLanguage = new List<KeyValuePair<string, List<string>>>();
			foreach (var sliceInfo in _segmentsToBeSliced)
			{
				var destinationDirectory = Path.Combine(tempFolderForSlicedFiles, Path.GetFileName(Path.GetDirectoryName(sliceInfo.File)));
				if (!Directory.Exists(destinationDirectory))
					Directory.CreateDirectory(destinationDirectory);
				var destinationFile = Path.Combine(destinationDirectory, Path.GetFileName(sliceInfo.File));
				File.Copy(sliceInfo.File, destinationFile, true);

				var language = string.Empty;
				try
				{
					language = Path.GetFileName(Path.GetDirectoryName(sliceInfo.File));
				}
				catch (Exception ex)
				{
					logger.Error(ex.Message, ex);
				}
				if (filesPerLanguage.Any(f => f.Key == language))
				{
					var languageItem = filesPerLanguage.FirstOrDefault(f => f.Key == language);
					if (!languageItem.Value.Contains(destinationFile))
						languageItem.Value.Add(destinationFile);
				}
				else
					filesPerLanguage.Add(new KeyValuePair<string, List<string>>(language,
																				new List<string>() { destinationFile }));

				SliceManager.SliceFile(destinationFile, sliceInfo);
			}

			if (doMerge)
			{
				StepProcess("Merging sliced files ...", false);
				SliceManager.MergeSplitFiles(filesPerLanguage);
			}
			else
			{
				CopyFilesFromTempFolder(filesPerLanguage);
			}
			Directory.Delete(tempFolderForSlicedFiles, true);
		}

		private void CopyFilesFromTempFolder(List<KeyValuePair<string, List<string>>> filesPerLanguage)
		{
			foreach (var keyValuePair in filesPerLanguage)
				foreach (var file in keyValuePair.Value)
				{
					File.Copy(file,
							  Path.Combine(_folderForSlicedFiles,
										   Path.GetFileName(file).Replace(".sdlxliff", "_sliced.sdlxliff")));
				}
		}

		private void DoSliceNow(int selectedTab, IEnumerable<int> indexes, bool doMerge, bool doOR)
		{
			StepProcess("Processing files based on slice information ...", false);
			var filesToBeSliced = SplitMergedXliffFiles();
			if (!filesToBeSliced.Any()) return;
			if (selectedTab == 1)
			{
				_doUpdateStatus = false;
				FindInFiles(doOR, filesToBeSliced.Values.ToList());

				_segmentsToBeSliced.Clear();
				foreach (var searchResult in _searchResults)
				{
					foreach (var searchSourceResult in searchResult.SearchSourceResults)
					{
						var SegmentId = searchSourceResult.Value.SegmentId.ToString(CultureInfo.InvariantCulture);
						var transUnitID = searchSourceResult.Value.SegmentContent.ParentParagraphUnit.Properties.ParagraphUnitId.Id;
						var file = searchResult.FilePath;

						if (transUnitID != String.Empty && SegmentId != String.Empty)
						{
							var fileSliceInfo = _segmentsToBeSliced.FirstOrDefault(slice => slice.File == file);
							if (fileSliceInfo == null)
							{
								_segmentsToBeSliced.Add(new SliceInfo()
								{
									File = file,
									Segments = new List<KeyValuePair<string, List<string>>>() { new KeyValuePair<string, List<string>>(transUnitID, new List<String>() { SegmentId }) }
								});
							}
							else
							{
								if (fileSliceInfo.Segments.All(s => s.Key != transUnitID))
									fileSliceInfo.Segments.Add(new KeyValuePair<string, List<string>>(transUnitID, new List<String>() { SegmentId }));
								else
								{
									var segSliceInfo = fileSliceInfo.Segments.FirstOrDefault(s => s.Key == transUnitID);
									if (!segSliceInfo.Value.Contains(SegmentId))
										segSliceInfo.Value.Add(SegmentId);
								}
							}
						}
					}
				}
			}
			else
			{
				_segmentsToBeSliced.Clear();
				ProcessFileBasedOnStatuses(indexes, true, filesToBeSliced);
			}

			StepProcess("Slice files ...", false);
			SliceFiles(doMerge);

			var folder = Path.GetDirectoryName(filesToBeSliced.Values.ToList()[0]);
			if (folder != null) Directory.Delete(folder, true);

			StepProcess("Files were successfully sliced.", true);

		}

		private Dictionary<string, string> SplitMergedXliffFiles()
		{
			var files = (from DataGridViewRow row in gridXLIFFFiles.SelectedRows select row.Cells[0].Value.ToString()).ToList();
			var splitedFiles = new Dictionary<String, String>();
			var folder = Path.Combine(_folderForSlicedFiles, Guid.NewGuid().ToString());
			foreach (var file in files)
			{
				var result = SplitMergedXliffFile(file, folder);
				foreach (var key in result.Keys) splitedFiles.Add(key, result[key]);
			}

			return splitedFiles;
		}

		private Dictionary<string, string> SplitMergedXliffFile(string filePath, string folder)
		{
			var splitedFiles = new Dictionary<string, string>();
			var xDoc = new XmlDocument();
			xDoc.PreserveWhitespace = true;
			xDoc.Load(filePath);
			var docInfoText = string.Empty;
			var docInfos = xDoc.DocumentElement.GetElementsByTagName("doc-info");
			if (docInfos.Count > 0)
			{
				foreach (var docInfo in docInfos.OfType<XmlElement>())
				{
					docInfoText += docInfo.OuterXml;
				}
			}
			var fileList = xDoc.DocumentElement.GetElementsByTagName("file");

			var xliffBeginText = @"<?xml version=""1.0"" encoding=""utf-8""?><xliff xmlns:sdl=""http://sdl.com/FileTypes/SdlXliff/1.0"" xmlns=""urn:oasis:names:tc:xliff:document:1.2"" version=""1.2"" sdl:version=""1.0"">";
			var xliffEndText = @"</xliff>";

			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			var fileNameTemplate = Path.Combine(folder, string.Format("{0}{1}.{2}", Path.GetFileNameWithoutExtension(filePath), "_{0}", Path.GetExtension(filePath)));
			foreach (var file in fileList.OfType<XmlElement>())
			{
				var fileText = file.OuterXml;
				var fileName = string.Format(fileNameTemplate, Guid.NewGuid().ToString());
				using (var sw = new StreamWriter(fileName, false))
				{
					sw.Write(xliffBeginText);
					sw.Write(docInfoText);
					sw.Write(fileText);
					sw.Write(xliffEndText);
				}
				splitedFiles.Add(filePath, fileName);
			}
			fileList = null;
			xDoc = null;
			return splitedFiles;
		}

		private void btnChangeit_Click(object sender, EventArgs e)
		{
			ShowProcess();
			var selectedTabIndex = tabControl1.SelectedIndex;
			var indexes = listDocumentStructure.SelectedIndices.Cast<int>().ToList();
			var doCopySourceToTarget = ckCopySourceToTarget.Checked;
			var doOR = cmbOperator.SelectedIndex == 0;
			StepProcess("Changing files based on selected options...");
			var t = new Thread(() => DoChangeNow(selectedTabIndex, indexes, doCopySourceToTarget, doOR));
			t.Start();
		}

		private void DoChangeNow(int selectedTabIndex, IEnumerable<int> indexes, bool doCopySourceToTarget, bool doOR)
		{
			if (gridXLIFFFiles.SelectedRows.Count == 0)
				return;
			if (selectedTabIndex == 1)
			{
				StartFind(true, doOR);
			}
			else
			{
				ProcessFileBasedOnStatuses(indexes);
			}

			if (doCopySourceToTarget)
			{
				StepProcess("Copying source to target in selected files ...");
				DoCopySourceNow(selectedTabIndex, indexes, doOR);
			}

			StepProcess("Files are changed based on selected options.", true);
		}

		private void DoCopySourceNow(int selectedTabIndex, IEnumerable<int> selectedDSIndexes, bool doOR)
		{
			StepProcess("Processing files ...");
			if (selectedTabIndex == 1)
			{
				_doUpdateStatus = false;
				FindInFiles(doOR);
				_segmentsToBeSliced.Clear();
				foreach (var searchResult in _searchResults)
				{
					foreach (var searchSourceResult in searchResult.SearchSourceResults)
					{
						var SegmentId = searchSourceResult.Value.SegmentId.ToString(CultureInfo.InvariantCulture);
						var transUnitID =
							searchSourceResult.Value.SegmentContent.ParentParagraphUnit.Properties.ParagraphUnitId.Id;
						var file = searchResult.FilePath;

						if (transUnitID != String.Empty && SegmentId != string.Empty)
						{
							var fileSliceInfo = _segmentsToBeSliced.FirstOrDefault(slice => slice.File == file);
							if (fileSliceInfo == null)
							{
								_segmentsToBeSliced.Add(new SliceInfo()
								{
									File = file,
									Segments = new List<KeyValuePair<string, List<string>>>()
											{
												new KeyValuePair<string, List<string>>(transUnitID,
																					   new List<String>() {SegmentId})
											}
								});
							}
							else
							{
								if (fileSliceInfo.Segments.All(s => s.Key != transUnitID))
									fileSliceInfo.Segments.Add(new KeyValuePair<string, List<string>>(transUnitID,
																									  new List<String>()
																										  {
																											  SegmentId
																										  }));
								else
								{
									var segSliceInfo = fileSliceInfo.Segments.FirstOrDefault(s => s.Key == transUnitID);
									if (!segSliceInfo.Value.Contains(SegmentId))
										segSliceInfo.Value.Add(SegmentId);
								}
							}
						}
					}
				}
			}
			else
			{
				_segmentsToBeSliced.Clear();
				ProcessFileBasedOnStatuses(selectedDSIndexes, true);
			}
			StepProcess("Copying source to target ...");
			CopyFiles();
			StepProcess("Source segments from selected files are copied to target.", true);
		}

		private void CopyFiles()
		{
			if (_segmentsToBeSliced.Count == 0)
				return;

			var threads = new List<Thread>();
			foreach (var sliceInfo in _segmentsToBeSliced)
			{
				StepProcess("Copying in file: " + Path.GetFileName(sliceInfo.File) + ".");

				var t = new Thread(() => ClearManager.CopyFile(sliceInfo, this));
				t.Start();
				threads.Add(t);
			}
			foreach (var thread in threads)
				thread.Join();
		}

		private List<SliceInfo> _segmentsToBeSliced = new List<SliceInfo>();

		private void ProcessFileBasedOnStatuses(IEnumerable<int> indexes, bool forSlice = false, Dictionary<string, string> filesToBeSliced = null)
		{
			var files = filesToBeSliced != null ? filesToBeSliced.Values.ToList() : (from DataGridViewRow row in gridXLIFFFiles.SelectedRows select row.Cells[0].Value.ToString()).ToList();
			var threads = new List<Thread>();
			foreach (var file in files)
			{
				var t = new Thread(() => ProcessOneFileBasedOnStatuses(forSlice, file, indexes, filesToBeSliced));
				t.Start();
				threads.Add(t);
			}
			foreach (var thread in threads)
			{
				thread.Join();
			}
		}

		private void ProcessOneFileBasedOnStatuses(bool forSlice, string file, IEnumerable<int> DSSelectedIndexes, Dictionary<string, string> filesToBeSliced)
		{
			StepProcess("Processing file: " + Path.GetFileName(file) + ". ", false);
			var xDoc = new XmlDocument();
			xDoc.PreserveWhitespace = true;
			xDoc.Load(file);
			var xmlEncoding = "utf-8";
			try
			{
				if (xDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
				{
					// Get the encoding declaration.
					var decl = (XmlDeclaration)xDoc.FirstChild;
					xmlEncoding = decl.Encoding;
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message, ex);
			}
			var originalFile = file;
			if (filesToBeSliced != null)
			{
				originalFile = filesToBeSliced.FirstOrDefault(f => f.Value == file).Key;
				if (String.IsNullOrEmpty(originalFile))
					originalFile = file;
			}
			var fileList = xDoc.DocumentElement.GetElementsByTagName("file");
			foreach (var fileElement in fileList.OfType<XmlElement>())
			{
				var bodyElement = (XmlElement)(fileElement.GetElementsByTagName("body")[0]);
				var groupElements = bodyElement.GetElementsByTagName("group");
				foreach (var groupElement in groupElements.OfType<XmlElement>())
				{
					ProcessOnFileBasedOnStatusesInBody(forSlice, file, DSSelectedIndexes, originalFile, groupElement);
				}
				ProcessOnFileBasedOnStatusesInBody(forSlice, file, DSSelectedIndexes, originalFile, bodyElement);
				bodyElement = null;
				groupElements = null;
			}
			if (!forSlice)
			{
				Encoding encoding = new UTF8Encoding();
				if (!String.IsNullOrEmpty(xmlEncoding))
					encoding = Encoding.GetEncoding(xmlEncoding);
				using (var writer = new XmlTextWriter(file, encoding))
				{
					xDoc.Save(writer);
				}
			}

			xDoc = null;
			fileList = null;
		}

		private void ProcessOnFileBasedOnStatusesInBody(bool forSlice, string file, IEnumerable<int> DSSelectedIndexes,
														string originalFile, object groupElement)
		{
			try
			{
				var doUpdateElement = false;
				//look in cxts
				if (DSSelectedIndexes.Any())
				{
					var selectedIDs = new List<string>();
					foreach (var index in DSSelectedIndexes)
					{
						if (_structureInformationTypes[index].IDs.Any(f => f.Key == originalFile))
						{
							var fileIDs = _structureInformationTypes[index].IDs.FirstOrDefault(f => f.Key == originalFile);
							selectedIDs.AddRange(fileIDs.Value);
						}
					}
					var cxtDefs = ((XmlNode)groupElement).ChildNodes;
					foreach (var cxtDef in cxtDefs.OfType<XmlElement>())
					{
						if (((XmlNode)cxtDef).Name != "sdl:cxts")
							continue;

						var cxts = ((XmlElement)cxtDef).ChildNodes;
						foreach (var cxt in cxts.OfType<XmlElement>())
						{
							if (((XmlNode)cxt).Name != "sdl:cxt")
								continue;

							var id = ((XmlElement)cxt).Attributes["id"].Value;
							if (selectedIDs.Contains(id))
							{
								doUpdateElement = true;
								break;
							}
						}
						cxts = null;
						if (doUpdateElement)
							break;
					}
					cxtDefs = null;
				}
				else
					doUpdateElement = true;

				//if structure type not in the slected ones ... go to the next element
				if (!doUpdateElement)
					return;

				//look in segments
				var transUnits = ((XmlElement)groupElement).ChildNodes; ;
				foreach (var transUnit in transUnits.OfType<XmlElement>())
				{
					if (((XmlNode)transUnit).Name != "trans-unit")
						continue;

					var transUnitID = String.Empty;
					transUnitID = ((XmlElement)transUnit).Attributes["id"].Value;

					var segDefs = ((XmlElement)transUnit).ChildNodes;
					foreach (var segDef in segDefs.OfType<XmlElement>())
					{
						if (((XmlNode)segDef).Name != "sdl:seg-defs")
							continue;

						var segments = ((XmlElement)segDef).ChildNodes;

						#region segments

						foreach (var segment in segments.OfType<XmlElement>())
						{
							if (((XmlNode)segment).Name != "sdl:seg")
								continue;

							var SegmentId = String.Empty;
							SegmentId = ((XmlElement)segment).Attributes["id"].Value;
							doUpdateElement = true;
							try
							{
								//look in tranlsation statuses
								if (GroupHasCheckedCheckBoxes(groupStatusesTranslationStatus))
									doUpdateElement = UpdateManager.DoUpdateElementBasedOnTranslatioStatus(segment);

								//look if locked / unlocked
								if (GroupHasCheckedCheckBoxes(groupStatusesLocked))
									doUpdateElement = UpdateManager.DoUpdateElementBasedOnLockedInformation(segment);
								//look in score
								if (GroupHasCheckedCheckBoxes(groupScore))
									doUpdateElement = UpdateManager.DoUpdateElementBasedOnScoreInformation(segment);
								//look in translation origin
								if (GroupHasCheckedCheckBoxes(groupTranslationOrigin))
									doUpdateElement = UpdateManager.DoUpdateElementBasedOnTranslationOriginInformation(segment);
								//look in system
								if (GroupHasCheckedCheckBoxes(groupSystem))
									doUpdateElement = UpdateManager.DoUpdateElementBasedOnSystemInformation(segment);

								if (doUpdateElement)
								{
									if (forSlice && transUnitID != String.Empty && SegmentId != String.Empty)
									{
										var fileSliceInfo = _segmentsToBeSliced.FirstOrDefault(slice => slice.File == file);
										if (fileSliceInfo == null)
										{
											_segmentsToBeSliced.Add(new SliceInfo()
											{
												File = file,
												Segments =
														new List<KeyValuePair<string, List<string>>>()
															{
																new KeyValuePair<string, List<string>>(transUnitID,
																									   new List<String>()
																										   {
																											   SegmentId
																										   })
															}
											});
										}
										else
										{
											if (fileSliceInfo.Segments.All(s => s.Key != transUnitID))
												fileSliceInfo.Segments.Add(new KeyValuePair<string, List<string>>(transUnitID,
																												  new List
																													  <String>()
																													  {
																														  SegmentId
																													  }));
											else
											{
												var segSliceInfo =
													fileSliceInfo.Segments.FirstOrDefault(s => s.Key == transUnitID);
												if (!segSliceInfo.Value.Contains(SegmentId))
													segSliceInfo.Value.Add(SegmentId);
											}
										}
									}
									else
									{
										//update the element
										if (ckChangeToUnlocked.Checked &&
											((XmlElement)segment).HasAttribute("locked"))
											((XmlElement)segment).RemoveAttribute("locked");
										if (ckChangeToLocked.Checked)
											((XmlElement)segment).SetAttribute("locked", "true");

										if (GetTranslationStatus() != null)
										{
											var translationStatus = GetTranslationStatus().Value.ToString();
											if (String.IsNullOrEmpty(translationStatus))
											{
												if (((XmlElement)segment).HasAttribute("conf"))
													((XmlElement)segment).RemoveAttribute("conf");
											}
											else
											{
												((XmlElement)segment).SetAttribute("conf", translationStatus);
											}
										}
									}
								}
							}
							catch (Exception ex)
							{
								logger.Error(ex.Message, ex);
							}
						}

						#endregion

						segments = null;
					}
					segDefs = null;
				}
				transUnits = null;
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message, ex);
			}
		}

		private bool GroupHasCheckedCheckBoxes(GroupBox groupBox)
		{
			var checkBoxes = groupBox.Controls.OfType<CheckBox>();
			foreach (var checkBox in checkBoxes)
			{
				if (checkBox.Checked)
					return true;
			}

			return false;
		}

		private bool GroupHasCheckedRadioButtons(GroupBox groupBox)
		{
			var checkBoxes = groupBox.Controls.OfType<RadioButton>();
			foreach (var checkBox in checkBoxes)
			{
				if (checkBox.Checked)
					return true;
			}

			return false;
		}

		private void btnFindAll_Click(object sender, EventArgs e)
		{
			if (gridXLIFFFiles.SelectedRows.Count == 0 ||
				(String.IsNullOrEmpty(txtSourceSearch.Text) && String.IsNullOrEmpty(txtTargetSearch.Text)))
				return;
			ShowProcess();
			StepProcess("Finding in files ...");
			var doOR = cmbOperator.SelectedIndex == 0;
			var t = new Thread(() => DoFindNow(doOR));
			t.Start();
		}

		private void DoFindNow(bool doOR)
		{
			StartFind(false, doOR);
			StepProcess("Find completed.", true);
		}

		private void StartFind(bool updateStatus, bool doOR)
		{
			_doUpdateStatus = updateStatus;
			FindInFiles(doOR);
		}

		private void BindSearchResults(DataGridView grView)
		{
			grView.ReadOnly = true;
			grView.Columns.Clear();
			grView.Columns.Add("ID", "#");
			grView.Columns.Add("Status", "#");
			grView.Columns.Add(new ControlColumn());
			grView.Columns.Add("File", "#");
			SetDetailGridView(grView);

			grView.DataSource = _searchDataManager == null || _searchDataManager.DetailFilteredData == null
									? null
									: _searchDataManager.DetailFilteredData;
		}

		private void SetDetailGridView(DataGridView grView)
		{
			var resources = new UIResources(Settings.GetSavedCulture());

			grView.Columns[0].HeaderText = resources.Seg;
			grView.Columns[1].HeaderText = resources.Status;
			grView.Columns[2].HeaderText = resources.SearchSource;
			grView.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			((ControlColumn)grView.Columns[2]).FileFilter = false;
			((ControlColumn)grView.Columns[2]).IsSearch = true;
			((ControlColumn)grView.Columns[2]).SearchInTags = ckSearchInTags.Checked;
			((ControlColumn)grView.Columns[2]).SearchInSource = true;
			((ControlColumn)grView.Columns[2]).SearchInTarget = true;
			grView.Columns[3].HeaderText = "File";

			grView.CurrentCell = null;
			grView.RowHeadersWidth = 20;
			grView.Columns[0].MinimumWidth = 40;
			grView.Columns[0].Width = 40;
			grView.Columns[1].MinimumWidth = 60;
			grView.Columns[1].Width = 60;
			grView.Columns[2].MinimumWidth = 150;
			grView.Columns[2].Width = 300;
			grView.Columns[3].MinimumWidth = 150;
			grView.Columns[3].Width = 500;
		}

		private FileDataManager _searchDataManager;
		private FileDataManager _replaceDataManager;

		private void gridSearchResults_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			try
			{
				if ((_searchDataManager.DetailFilteredData != null) && (_searchDataManager.DetailFilteredData.Count > 0))
				{
					((DataGridView)sender).Rows[e.RowIndex].MinimumHeight = 0x38;
					if (e.ColumnIndex == 0)
					{
						e.Value = _searchDataManager.DetailFilteredData[e.RowIndex].SegID;
					}
					else if (e.ColumnIndex == 1)
					{
						e.Value =
							(new UIResources(Settings.GetSavedCulture())).GetString(
								_searchDataManager.DetailFilteredData[e.RowIndex].SegStatus);
					}
					else if (e.ColumnIndex == 2)
					{
						e.Value = _searchDataManager.DetailFilteredData[e.RowIndex];
					}
					else
					{
						e.Value = String.Format("{0}\\{1}",
							Path.GetFileName(Path.GetDirectoryName(_searchDataManager.DetailFilteredData[e.RowIndex].FileName)),
							Path.GetFileName(_searchDataManager.DetailFilteredData[e.RowIndex].FileName));
					}
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message, ex);
			}
		}

		private void gridSearchResults_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			if (((DataGridView)sender).ColumnCount > 3)
			{
				for (var i = 4; i < ((DataGridView)sender).ColumnCount; i++)
				{
					((DataGridView)sender).Columns[i].Visible = false;
				}
				((DataGridView)sender).Columns[1].Width = 80;
				((DataGridView)sender).Columns[2].Width = ((DataGridView)sender).Width -
														   ((DataGridView)sender).Columns[1].Width -
														   ((DataGridView)sender).Columns[0].Width - 55;
			}
		}

		private SearchResults _searchResultsForm;

		private void btnExpandSearchResults_Click(object sender, EventArgs e)
		{
			var grView = new DataGridView();
			grView.AllowUserToAddRows = false;
			grView.AllowUserToDeleteRows = false;
			grView.AllowUserToResizeRows = false;
			grView.BackgroundColor = System.Drawing.SystemColors.Window;
			grView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			grView.GridColor = System.Drawing.SystemColors.Control;
			grView.Location = new System.Drawing.Point(166, 92);
			grView.Name = "grView";
			grView.RowHeadersVisible = false;
			grView.Size = new System.Drawing.Size(676, 151);
			grView.TabIndex = 7;
			grView.VirtualMode = true;
			grView.CellValueNeeded += gridSearchResults_CellValueNeeded;
			grView.DataBindingComplete += gridSearchResults_DataBindingComplete;
			BindSearchResults(grView);


			var location = new Point();
			var size = new Size();
			var setSizeAndLocation = false;
			if (_searchResultsForm != null && !_searchResultsForm.IsDisposed)
			{
				location = _searchResultsForm.Location;
				size = _searchResultsForm.Size;
				setSizeAndLocation = true;
				_searchResultsForm.Close();
			}

			_searchResultsForm = new SearchResults(grView);
			if (setSizeAndLocation)
			{
				_searchResultsForm.Location = location;
				_searchResultsForm.Size = size;
			}
			_searchResultsForm.Show();
		}

		private bool _doUpdateStatus = false;
		private List<FileData> _searchResults;
		private List<FileData> _replaceResults;

		private readonly SliceManager _sliceManager;
		private readonly UpdateManager _updateManager;

		private void FindInFiles(bool doOR, List<string> filesToBeSliced = null)
		{
			var files = filesToBeSliced ?? (from DataGridViewRow row in gridXLIFFFiles.SelectedRows select row.Cells[0].Value.ToString()).ToList();
			if (_searchResults != null)
				_searchResults.Clear();
			StepProcess("Looking in source segments ...");
			var settings = new SearchSettings()
			{
				SearchText = txtSourceSearch.Text,
				ReplaceText = txtSourceSearch.Text,
				SearchInSource = true,
				SearchInTarget = false,
				MatchCase = ckSearchMatchCase.Checked,
				MatchWholeWord = ckSearchMatchWholeWord.Checked,
				UseRegex = ckSearchRegEx.Checked,
				SearchInTag = ckSearchInTags.Checked,
				SearchInLocked = true,
				NotSearchStatus = new List<ConfirmationLevel>(),
				LockSegment = ckChangeToLocked.Checked,
				UnlockContent = ckChangeToUnlocked.Checked
			};
			if (GetTranslationStatus() != null)
			{
				settings.NewStatus = GetTranslationStatus().Value;
				settings.UpdateStatus = _doUpdateStatus;
			}
			else
			{
				settings.UpdateStatus = false;
			}
			SearchSettings sourceSettings = null;
			if (!string.IsNullOrEmpty(settings.SearchText))
			{
				sourceSettings = settings;
				var analyzer = new FilesAnalyzer(files);
				analyzer.SearchInFiles(settings);
				_searchResults = analyzer.FileResults;
			}
			var searchInTarget = !string.IsNullOrEmpty(txtTargetSearch.Text);
			SearchSettings targetSettings = null;
			if (searchInTarget)
			{
				StepProcess("Looking in target segments ...");
				settings.SearchText = txtTargetSearch.Text;
				settings.ReplaceText = txtTargetSearch.Text;
				settings.SearchInSource = false;
				settings.SearchInTarget = true;

				targetSettings = settings;

				var analyzer = new FilesAnalyzer(files);
				analyzer.SearchInFiles(settings);
				var targetResult = analyzer.FileResults;

				if (_searchResults == null || _searchResults.Count == 0)
					_searchResults = targetResult;
				else
				{
					StepProcess("Merging source and target segments ...");
					var fileDataToBeAdded = new List<FileData>();
					var fileDataToBeRemoved = new List<FileData>();
					foreach (var fileData in _searchResults)
					{
						var file = fileData.FilePath;
						var targetFileData = targetResult.FirstOrDefault(data => data.FilePath == file);
						if (targetFileData != null)
						{
							//create new FileData containing source result search from source and target result search from target
							var fileToBeAdded = doOR
													? UnionSourceAndTarget(fileData, targetFileData, file)
													: IntersectSourceAndTarget(fileData, targetFileData, file);
							if (fileToBeAdded != null)
								fileDataToBeAdded.Add(fileToBeAdded);
							fileDataToBeRemoved.Add(fileData);
						}
					}

					var lenghtOfFilesToBeAdded = fileDataToBeAdded.Count;
					for (var i = 0; i < fileDataToBeRemoved.Count; i++)
					{
						_searchResults.Remove(fileDataToBeRemoved[i]);
						if (i < lenghtOfFilesToBeAdded)
							_searchResults.Insert(i, fileDataToBeAdded[i]);
					}
				}
			}
			PostProcessSearchResult(_searchResults, sourceSettings, targetSettings);

			if (_doUpdateStatus)
			{
				UpdateFileBasedOnResults(_searchResults);
				_doUpdateStatus = false;
				FindInFiles(doOR);
				PostProcessSearchResult(_searchResults, sourceSettings, targetSettings);
			}
			else
				BindSearchResults();
		}

		private void BindSearchResults()
		{
			Invoke((MethodInvoker)delegate
			   {
				   gridSearchResults.DataSource = null;
				   if (_searchResults != null)
				   {
					   _searchDataManager = new FileDataManager(_searchResults);
					   gridSearchResults.RowCount = _searchDataManager.SetDetailDataSearch();
					   if (_searchResultsForm != null && !_searchResultsForm.IsDisposed)
					   {
						   _searchResultsForm.SearchResultsGrid.DataSource = null;
						   _searchResultsForm.SearchResultsGrid.RowCount = gridSearchResults.RowCount;
					   }
					   if (!_searchDataManager.IsSearchResultEmpty())
					   {
						   BindSearchResults(gridSearchResults);
						   if (_searchResultsForm != null && !_searchResultsForm.IsDisposed)
						   {
							   BindSearchResults(_searchResultsForm.SearchResultsGrid);
						   }
					   }
				   }
			   });
		}

		private void UpdateFileBasedOnResults(List<FileData> searchResult)
		{
			foreach (var fileData in searchResult)
			{
				StepProcess("Updating file: " + Path.GetFileName(fileData.FilePath) + " ...");

				if (fileData.SearchSourceResults.Count == 0)
					continue;

				var filePath = fileData.FilePath;
				var xDoc = new XmlDocument();
				xDoc.PreserveWhitespace = true;
				xDoc.Load(filePath);
				var xmlEncoding = "utf-8";
				try
				{
					if (xDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
					{
						// Get the encoding declaration.
						var decl = (XmlDeclaration)xDoc.FirstChild;
						xmlEncoding = decl.Encoding;
					}
				}
				catch (Exception ex)
				{
					logger.Error(ex.Message, ex);
				}
				var fileList = xDoc.DocumentElement.GetElementsByTagName("file");
				foreach (var fileElement in fileList.OfType<XmlElement>())
				{
					var bodyElement = (XmlElement)(fileElement.GetElementsByTagName("body")[0]);
					var groupElements = bodyElement.GetElementsByTagName("group");
					foreach (var groupElement in groupElements.OfType<XmlElement>())
					{
						UpdateFileBasedOnResultsInBody(groupElement, fileData);
					}
					UpdateFileBasedOnResultsInBody(bodyElement, fileData);
				}
				Encoding encoding = new UTF8Encoding();
				if (!String.IsNullOrEmpty(xmlEncoding))
					encoding = Encoding.GetEncoding(xmlEncoding);
				using (var writer = new XmlTextWriter(filePath, encoding))
				{
					xDoc.Save(writer);
				}
				StepProcess("File: " + Path.GetFileName(fileData.FilePath) + " was updated.");
			}
		}

		private void UpdateFileBasedOnResultsInBody(object groupElement, FileData fileData)
		{
			var elements = ((XmlElement)groupElement).ChildNodes;
			foreach (var element in elements.OfType<XmlElement>())
			{
				try
				{
					if (!((XmlElement)element).HasAttribute("id"))
						continue;

					var id = ((XmlElement)element).Attributes["id"].Value;
					var segmentData =
						fileData.SearchSourceResults.FirstOrDefault(
							result =>
							result.Value.SegmentContent.ParentParagraphUnit.Properties.ParagraphUnitId.Id ==
							id)
								.Value;
					if (segmentData != null)
					{
						var SegmentId = segmentData.SegmentId;
						var segDefs = ((XmlElement)element).ChildNodes;
						foreach (var segDef in segDefs.OfType<XmlElement>())
						{
							if (((XmlNode)segDef).Name != "sdl:seg-defs")
								continue;
							var segments = ((XmlElement)segDef).ChildNodes;
							foreach (var segment in segments.OfType<XmlElement>())
							{
								if (((XmlNode)segment).Name != "sdl:seg")
									continue;
								try
								{
									if (Convert.ToInt32(((XmlElement)segment).Attributes["id"].Value) ==
										SegmentId)
									{
										if (ckChangeToUnlocked.Checked &&
											((XmlElement)segment).HasAttribute("locked"))
											((XmlElement)segment).RemoveAttribute("locked");
										if (ckChangeToLocked.Checked)
											((XmlElement)segment).SetAttribute("locked", "true");

										if (GetTranslationStatus() != null)
										{
											var translationStatus = GetTranslationStatus().Value.ToString();
											if (String.IsNullOrEmpty(translationStatus))
											{
												if (((XmlElement)segment).HasAttribute("conf"))
													((XmlElement)segment).RemoveAttribute("conf");
											}
											else
											{
												((XmlElement)segment).SetAttribute("conf", translationStatus);
											}
										}
									}
								}
								catch (Exception ex)
								{
									logger.Error(ex.Message, ex);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					logger.Error(ex.Message, ex);
				}
			}
		}

		private ConfirmationLevel? GetTranslationStatus()
		{
			if (!GroupHasCheckedRadioButtons(groupChangeTranslationStatus)) return null;
			return ckChangeToNotTranslated.Checked
					   ? ConfirmationLevel.Unspecified
					   : ckChangeToDraft.Checked
							 ? ConfirmationLevel.Draft
							 : ckChangeToTranslated.Checked
								   ? ConfirmationLevel.Translated
								   : ckChangeToTranslationRejected.Checked
										 ? ConfirmationLevel.RejectedTranslation
										 : ckChangeToTranslationApproved.Checked
											   ? ConfirmationLevel.ApprovedTranslation
											   : ckChangeToSignOffRejected.Checked
													 ? ConfirmationLevel.RejectedSignOff
													 : ckChangeToSignedOff.Checked
														   ? ConfirmationLevel.ApprovedSignOff
														   : ConfirmationLevel.Unspecified;
		}

		public List<String> GetTranslationStatusForSearch()
		{
			var statuses = new List<string>();

			if (ckNotTranslated.Checked) statuses.Add(ConfirmationLevel.Unspecified.ToString());
			if (ckDraft.Checked) statuses.Add(ConfirmationLevel.Draft.ToString());
			if (ckTranslated.Checked) statuses.Add(ConfirmationLevel.Translated.ToString());
			if (ckTranslationRejected.Checked) statuses.Add(ConfirmationLevel.RejectedTranslation.ToString());
			if (ckTranslationApproved.Checked) statuses.Add(ConfirmationLevel.ApprovedTranslation.ToString());
			if (ckSignOffRejected.Checked) statuses.Add(ConfirmationLevel.RejectedSignOff.ToString());
			if (ckSignedOff.Checked) statuses.Add(ConfirmationLevel.ApprovedSignOff.ToString());

			return statuses;
		}

		private FileData UnionSourceAndTarget(FileData fileData, FileData targetFileData, string filePath)
		{
			//add all non existing segmens from target (sourceSearchResult) into source
			var sourceSegments = AddNonExistingSegments(fileData.SearchSourceResults.Values.ToList(),
																	  targetFileData.SearchSourceResults.Values.ToList());

			//add all non existing segmens from source (targetSearchResult) into target
			var targetSegments = AddNonExistingSegments(
				targetFileData.SearchTargetResults.Values.ToList(), fileData.SearchTargetResults.Values.ToList());

			var fileToBeAdded = new FileData(filePath,
												  sourceSegments.OrderBy(seg => seg.SegmentId).ToList(),
												  targetSegments.OrderBy(seg => seg.SegmentId).ToList());
			return fileToBeAdded;
		}

		private List<SegmentData> AddNonExistingSegments(List<SegmentData> sourceSegments,
														 List<SegmentData> targetSegments)
		{
			foreach (var targetSegment in targetSegments)
			{
				if (sourceSegments.All(segment => segment.SegmentId != targetSegment.SegmentId))
				{
					var maxSID = sourceSegments.Count == 0 ? -1 : sourceSegments.Select(segData => segData.Sid).Max();

					var sData = new SegmentData(maxSID + 1, targetSegment.SegmentId,
												targetSegment.SegmentText, targetSegment.SegmentStatus,
												targetSegment.SegmentContent)
					{
						Tags = targetSegment.Tags,
						SearchResults = targetSegment.SearchResults
					};

					sourceSegments.Add(sData);
				}
			}

			return sourceSegments;
		}

		private FileData IntersectSourceAndTarget(FileData fileData, FileData targetFileData, string filePath)
		{
			//add all non existing segmens from target (sourceSearchResult) into source
			var sourceSegments = RemoveNotExistingSegments(fileData.SearchSourceResults.Values.ToList(),
																		 targetFileData.SearchSourceResults.Values
																					   .ToList());

			//add all non existing segmens from source (targetSearchResult) into target
			var targetSegments =
				RemoveNotExistingSegments(targetFileData.SearchTargetResults.Values.ToList(),
										  fileData.SearchTargetResults.Values.ToList());

			targetSegments = UpdateSIDsInTargetSegments(sourceSegments, targetSegments);
			var fileToBeAdded = new FileData(filePath,
												  sourceSegments.OrderBy(seg => seg.SegmentId).ToList(),
												  targetSegments.OrderBy(seg => seg.SegmentId).ToList());
			return fileToBeAdded;
		}

		private List<SegmentData> UpdateSIDsInTargetSegments(List<SegmentData> sourceSegments,
															 List<SegmentData> targetSegments)
		{
			var segments = new List<SegmentData>();

			foreach (var targetSegment in targetSegments)
			{
				var sourceSegment =
					sourceSegments.FirstOrDefault(seg => seg.SegmentId == targetSegment.SegmentId);
				if (sourceSegment == null)
					throw new InvalidDataException("sourceSegment");

				var sData = new SegmentData(sourceSegment.Sid, targetSegment.SegmentId,
											targetSegment.SegmentText, targetSegment.SegmentStatus,
											targetSegment.SegmentContent)
				{
					Tags = targetSegment.Tags,
					SearchResults = targetSegment.SearchResults
				};

				segments.Add(sData);
			}

			return segments;
		}

		private List<SegmentData> RemoveNotExistingSegments(List<SegmentData> sourceSegments,
															List<SegmentData> targetSegments)
		{
			var segmentsToBeRemoved = new List<SegmentData>();
			foreach (var sourceSegment in sourceSegments)
			{
				if (targetSegments.Any(segment => segment.SegmentId == sourceSegment.SegmentId))
					continue;
				segmentsToBeRemoved.Add(sourceSegment);
			}
			foreach (var segmentData in segmentsToBeRemoved)
				sourceSegments.Remove(segmentData);

			return sourceSegments;
		}

		private void gridXLIFFFiles_DragDrop(object sender, DragEventArgs e)
		{
			var data = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			var dsFiles = new XLIFFFiles();
			var projectFiles = data.Where(file => file.Contains(".sdlproj"));
			foreach (var projectFile in projectFiles)
			{
				var phisicalFilesPath = GetPhisicalFilesPathFromProjectFile(projectFile);

				foreach (var file in phisicalFilesPath)
				{
					var fileRow = dsFiles.Files.NewFilesRow();
					fileRow.Path = String.Format("{0}\\{1}", Path.GetDirectoryName(projectFile), file);
					fileRow.Name = file;
					fileRow.Size =
						((decimal)(new FileInfo(fileRow.Path)).Length / 1000).ToString(CultureInfo.InvariantCulture);
					fileRow.Date = (new FileInfo(fileRow.Path)).CreationTime.ToString(CultureInfo.InvariantCulture);
					dsFiles.Files.AddFilesRow(fileRow);
				}
			}

			var xliffFiles = data.Where(file => file.Contains(".sdlxliff"));
			foreach (var xliffFile in xliffFiles)
			{
				var fileRow = dsFiles.Files.NewFilesRow();
				fileRow.Path = xliffFile;
				fileRow.Name = Path.GetFileName(xliffFile);
				fileRow.Size = ((decimal)(new FileInfo(xliffFile)).Length / 1000).ToString(CultureInfo.InvariantCulture);
				fileRow.Date = (new FileInfo(xliffFile)).CreationTime.ToString(CultureInfo.InvariantCulture);
				dsFiles.Files.AddFilesRow(fileRow);
			}
			BindFilesGrid(dsFiles);
		}

		private void gridXLIFFFiles_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void gridXLIFFFiles_SelectionChanged(object sender, EventArgs e)
		{
			listDocumentStructure.Items.Clear();
		}

		private void btnClearit_Click(object sender, EventArgs e)
		{
			ShowProcess();
			StepProcess("Clearing target segments from selected files ...");
			var selectedTabIndex = tabControl1.SelectedIndex;
			var selectedDSIndexes = listDocumentStructure.SelectedIndices.Cast<int>().ToList();
			var doOR = cmbOperator.SelectedIndex == 0;
			var t = new Thread(() => DoClearDow(selectedTabIndex, selectedDSIndexes, doOR));
			t.Start();
		}

		private void DoClearDow(int selectedTabIndex, List<int> selectedDSIndexes, bool doOR)
		{
			StepProcess("Processing files ...");
			if (selectedTabIndex == 1)
			{
				_doUpdateStatus = false;
				FindInFiles(doOR);
				_segmentsToBeSliced.Clear();
				foreach (var searchResult in _searchResults)
				{
					foreach (var searchSourceResult in searchResult.SearchSourceResults)
					{
						var SegmentId = searchSourceResult.Value.SegmentId.ToString(CultureInfo.InvariantCulture);
						var transUnitID =
							searchSourceResult.Value.SegmentContent.ParentParagraphUnit.Properties.ParagraphUnitId.Id;
						var file = searchResult.FilePath;

						if (transUnitID != String.Empty && SegmentId != String.Empty)
						{
							var fileSliceInfo = _segmentsToBeSliced.FirstOrDefault(slice => slice.File == file);
							if (fileSliceInfo == null)
							{
								_segmentsToBeSliced.Add(new SliceInfo()
								{
									File = file,
									Segments = new List<KeyValuePair<string, List<string>>>()
											{
												new KeyValuePair<string, List<string>>(transUnitID,
																					   new List<String>() {SegmentId})
											}
								});
							}
							else
							{
								if (fileSliceInfo.Segments.All(s => s.Key != transUnitID))
									fileSliceInfo.Segments.Add(new KeyValuePair<string, List<string>>(transUnitID,
																									  new List<String>()
																										  {
																											  SegmentId
																										  }));
								else
								{
									var segSliceInfo = fileSliceInfo.Segments.FirstOrDefault(s => s.Key == transUnitID);
									if (!segSliceInfo.Value.Contains(SegmentId))
										segSliceInfo.Value.Add(SegmentId);
								}
							}
						}
					}
				}
			}
			else
			{
				_segmentsToBeSliced.Clear();
				ProcessFileBasedOnStatuses(selectedDSIndexes, true);
			}
			StepProcess("Clearing files ...");
			ClearFiles();
			StepProcess("Target segments from selected files are clear.", true);
		}

		private void ClearFiles()
		{
			if (_segmentsToBeSliced.Count == 0)
				return;

			var threads = new List<Thread>();
			foreach (var sliceInfo in _segmentsToBeSliced)
			{
				StepProcess("Clearing file: " + Path.GetFileName(sliceInfo.File) + ".");

				var t = new Thread(() => ClearManager.ClearFile(sliceInfo, this));
				t.Start();
				threads.Add(t);
			}
			foreach (var thread in threads)
				thread.Join();
		}

		private void btnReplace_Click(object sender, EventArgs e)
		{
			var files =
				(from DataGridViewRow row in gridXLIFFFiles.SelectedRows select row.Cells[0].Value.ToString());
			if (!files.Any())
				return;
			ShowProcess();
			StepProcess("Replacing in files ...");
			var t = new Thread(() => DoReplaceNow(files));
			t.Start();
		}

		private void DoReplaceNow(IEnumerable<string> files)
		{
			if (_replaceResults == null || _replaceResults.Count == 0)
				DoReplacePreviewOperation();
			ChangeInFiles(files);
			BindReplaceResults();
			StepProcess("Replace completed.", true);
		}

		private void ChangeInFiles(IEnumerable<string> files)
		{
			var settings = new ReplaceSettings()
			{
				MatchCase = ckReplaceMatchCase.Checked,
				MatchWholeWord = ckReplaceMatchWholeWord.Checked,
				UseRegEx = ckReplaceUseRegEx.Checked,
				SourceSearchText = txtReplaceSourceSearch.Text,
				SourceReplaceText = txtReplaceSourceReplace.Text,
				TargetSearchText = txtReplaceTargetSearch.Text,
				TargetReplaceText = txtReplaceTargetReplace.Text
			};
			var threads = new List<Thread>();
			foreach (var file in files)
			{
				var t = new Thread(() => ReplaceManager.DoReplaceInFile(file, settings, this));
				t.Start();
				threads.Add(t);
			}

			foreach (var thread in threads)
				thread.Join();
		}

		private void btnExpandReplaceResults_Click(object sender, EventArgs e)
		{
			var grView = new DataGridView();
			grView.AllowUserToAddRows = false;
			grView.AllowUserToDeleteRows = false;
			grView.AllowUserToResizeRows = false;
			grView.BackgroundColor = System.Drawing.SystemColors.Window;
			grView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			grView.GridColor = System.Drawing.SystemColors.Control;
			grView.Location = new System.Drawing.Point(166, 92);
			grView.Name = "grView";
			grView.RowHeadersVisible = false;
			grView.Size = new System.Drawing.Size(676, 151);
			grView.TabIndex = 7;
			grView.VirtualMode = true;
			grView.CellValueNeeded += gridReplaceResults_CellValueNeeded;
			grView.DataBindingComplete += gridReplaceResults_DataBindingComplete;

			BindReplaceResults(grView);


			var location = new Point();
			var size = new Size();
			var setSizeAndLocation = false;
			if (_searchResultsForm != null && !_searchResultsForm.IsDisposed)
			{
				location = _searchResultsForm.Location;
				size = _searchResultsForm.Size;
				setSizeAndLocation = true;
				_searchResultsForm.Close();
			}

			_searchResultsForm = new SearchResults(grView) { Dock = DockStyle.Fill };
			if (setSizeAndLocation)
			{
				_searchResultsForm.Location = location;
				_searchResultsForm.Size = size;
			}
			_searchResultsForm.Show();
		}

		private void btnFindAllInReplace_Click(object sender, EventArgs e)
		{
			if (gridXLIFFFiles.SelectedRows.Count == 0)
				return;
			if (String.IsNullOrEmpty(txtReplaceSourceSearch.Text) && String.IsNullOrEmpty(txtReplaceTargetSearch.Text))
				return;

			_doUpdateStatus = false;
			ShowProcess();
			StepProcess("Finding in files before replace ...");

			var sourceSettings = String.IsNullOrEmpty(txtReplaceSourceSearch.Text)
									 ? null
									 : new SearchSettings()
									 {
										 SearchText = txtReplaceSourceSearch.Text,
										 ReplaceText = txtReplaceSourceSearch.Text,
										 SearchInSource = true,
										 SearchInTarget = false,
										 MatchCase = ckReplaceMatchCase.Checked,
										 MatchWholeWord = ckReplaceMatchWholeWord.Checked,
										 UseRegex = ckReplaceUseRegEx.Checked,
										 SearchInTag = false,
										 SearchInLocked = true,
										 NotSearchStatus = new List<ConfirmationLevel>(),
										 LockSegment = false,
										 UnlockContent = false,
										 NewStatus = ConfirmationLevel.Unspecified,
										 UpdateStatus = false
									 };

			var targetSettings = String.IsNullOrEmpty(txtReplaceTargetSearch.Text)
									 ? null
									 : new SearchSettings()
									 {
										 SearchText = txtReplaceTargetSearch.Text,
										 ReplaceText = txtReplaceTargetSearch.Text,
										 SearchInSource = false,
										 SearchInTarget = true,
										 MatchCase = ckReplaceMatchCase.Checked,
										 MatchWholeWord = ckReplaceMatchWholeWord.Checked,
										 UseRegex = ckReplaceUseRegEx.Checked,
										 SearchInTag = false,
										 SearchInLocked = true,
										 NotSearchStatus = new List<ConfirmationLevel>(),
										 LockSegment = false,
										 UnlockContent = false,
										 NewStatus = ConfirmationLevel.Unspecified,
										 UpdateStatus = false
									 };
			var t = new Thread(() => DoFindForReplaceNow(sourceSettings, targetSettings));
			t.Start();
		}

		private void DoFindForReplaceNow(SearchSettings sourceSettings, SearchSettings targetSettings)
		{
			FindInFilesForReplace(sourceSettings, targetSettings);

			BindReplaceResults();
			StepProcess("Find before replace completed.", true);
		}

		private void btnPreview_Click(object sender, EventArgs e)
		{
			if (gridXLIFFFiles.SelectedRows.Count == 0)
				return;
			if (string.IsNullOrEmpty(txtReplaceSourceSearch.Text) && String.IsNullOrEmpty(txtReplaceTargetSearch.Text))
				return;

			_doUpdateStatus = false;
			ShowProcess();
			StepProcess("Generating replace preview ...");
			var t = new Thread(() =>
				{
					DoReplacePreviewOperation();

					BindReplaceResults();
					StepProcess("Preview generated.", true);
				});
			t.Start();
		}

		private void gridReplaceResults_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			try
			{
				if ((_replaceDataManager.DetailFilteredData != null) && (_replaceDataManager.DetailFilteredData.Count > 0))
				{
					((DataGridView)sender).Rows[e.RowIndex].MinimumHeight = 0x38;
					if (e.ColumnIndex == 0)
					{
						e.Value = _replaceDataManager.DetailFilteredData[e.RowIndex].SegID;
					}
					else if (e.ColumnIndex == 1)
					{
						e.Value = (new UIResources(Settings.GetSavedCulture())).GetString(_replaceDataManager.DetailFilteredData[e.RowIndex].SegStatus);
					}
					else if (e.ColumnIndex == 2)
					{
						e.Value = _replaceDataManager.DetailFilteredData[e.RowIndex];
					}
					else
					{
						e.Value = string.Format("{0}\\{1}",
							Path.GetFileName(Path.GetDirectoryName(_replaceDataManager.DetailFilteredData[e.RowIndex].FileName)),
							Path.GetFileName(_replaceDataManager.DetailFilteredData[e.RowIndex].FileName));

					}
				}
			}
			catch (Exception ex)
			{
				logger.Error(ex.Message, ex);
			}
		}

		private void gridReplaceResults_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			if (((DataGridView)sender).ColumnCount > 3)
			{
				for (var i = 4; i < ((DataGridView)sender).ColumnCount; i++)
				{
					((DataGridView)sender).Columns[i].Visible = false;
				}
				((DataGridView)sender).Columns[1].Width = 80;
				((DataGridView)sender).Columns[2].Width = ((DataGridView)sender).Width -
														   ((DataGridView)sender).Columns[1].Width -
														   ((DataGridView)sender).Columns[0].Width - 55;
			}
		}

		private void BindReplaceResults(DataGridView grView)
		{
			grView.ReadOnly = true;
			grView.Columns.Clear();
			grView.Columns.Add("ID", "#");
			grView.Columns.Add("Status", "#");
			grView.Columns.Add(new ControlColumn());
			grView.Columns.Add("File", "#");
			SetDetailReplaceGridView(grView);

			grView.DataSource = _replaceDataManager == null || _replaceDataManager.DetailFilteredData == null
									? null
									: _replaceDataManager.DetailFilteredData;
		}

		private void SetDetailReplaceGridView(DataGridView grView)
		{
			var resources = new UIResources(Settings.GetSavedCulture());

			grView.Columns[0].HeaderText = resources.Seg;
			grView.Columns[1].HeaderText = resources.Status;
			grView.Columns[2].HeaderText = resources.SearchSource;
			grView.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			((ControlColumn)grView.Columns[2]).FileFilter = false;
			((ControlColumn)grView.Columns[2]).IsSearch = true;
			((ControlColumn)grView.Columns[2]).SearchInTags = false;
			((ControlColumn)grView.Columns[2]).SearchInSource = true;
			((ControlColumn)grView.Columns[2]).SearchInTarget = true;
			grView.Columns[3].HeaderText = "File";

			grView.CurrentCell = null;
			grView.RowHeadersWidth = 20;
			grView.Columns[0].MinimumWidth = 40;
			grView.Columns[0].Width = 40;
			grView.Columns[1].MinimumWidth = 60;
			grView.Columns[1].Width = 60;
			grView.Columns[2].MinimumWidth = 150;
			grView.Columns[2].Width = 300;
			grView.Columns[3].MinimumWidth = 150;
			grView.Columns[3].Width = 500;
		}

		private void BindReplaceResults()
		{
			Invoke((MethodInvoker)delegate
			   {
				   gridReplaceResults.DataSource = null;
				   if (_replaceResults != null)
				   {
					   _replaceDataManager = new FileDataManager(_replaceResults);
					   gridReplaceResults.RowCount = _replaceDataManager.SetDetailDataSearch();
					   if (_searchResultsForm != null && !_searchResultsForm.IsDisposed)
					   {
						   _searchResultsForm.SearchResultsGrid.DataSource = null;
						   _searchResultsForm.SearchResultsGrid.RowCount = gridReplaceResults.RowCount;
					   }
					   if (!_replaceDataManager.IsSearchResultEmpty())
					   {
						   BindReplaceResults(gridReplaceResults);
						   if (_searchResultsForm != null && !_searchResultsForm.IsDisposed)
						   {
							   BindReplaceResults(_searchResultsForm.SearchResultsGrid);
						   }
					   }
				   }
			   });
		}

		private void FindInFilesForReplace(SearchSettings sourceSettings = null, SearchSettings targeSettings = null)
		{
			var files =
				(from DataGridViewRow row in gridXLIFFFiles.SelectedRows select row.Cells[0].Value.ToString()).ToList();
			if (_replaceResults != null)
				_replaceResults.Clear();
			StepProcess("Processing source segments ...");

			if (sourceSettings != null && !String.IsNullOrEmpty(sourceSettings.SearchText))
			{
				var analyzer = new FilesAnalyzer(files);
				analyzer.SearchInFiles(sourceSettings);
				_replaceResults = analyzer.FileResults;
			}
			var searchInTarget = false;
			searchInTarget = targeSettings != null && !String.IsNullOrEmpty(targeSettings.SearchText);
			if (searchInTarget)
			{
				StepProcess("Processing target segments ...");
				var analyzer = new FilesAnalyzer(files);
				analyzer.SearchInFiles(targeSettings);
				var targetResult = analyzer.FileResults;

				if (_replaceResults == null || _replaceResults.Count == 0)
					_replaceResults = targetResult;
				else
				{
					var fileDataToBeAdded = new List<FileData>();
					var fileDataToBeRemoved = new List<FileData>();
					foreach (var fileData in _replaceResults)
					{
						var file = fileData.FilePath;
						var targetFileData = targetResult.FirstOrDefault(data => data.FilePath == file);
						if (targetFileData != null)
						{
							//create new FileData containing source result search from source and target result search from target
							var fileToBeAdded = UnionSourceAndTarget(fileData, targetFileData, file);
							if (fileToBeAdded != null)
								fileDataToBeAdded.Add(fileToBeAdded);
							fileDataToBeRemoved.Add(fileData);
						}
					}

					var lenghtOfFilesToBeAdded = fileDataToBeAdded.Count;
					for (var i = 0; i < fileDataToBeRemoved.Count; i++)
					{
						_replaceResults.Remove(fileDataToBeRemoved[i]);
						if (i < lenghtOfFilesToBeAdded)
							_replaceResults.Insert(i, fileDataToBeAdded[i]);
					}
				}
			}

			PostProcessSearchResult(_replaceResults, sourceSettings, targeSettings);
		}

		private void PostProcessSearchResult(IEnumerable<FileData> results, SearchSettings sourceSettings = null, SearchSettings targetSettings = null)
		{
			StepProcess("Post processing search result ...");

			foreach (var fileData in results)
			{
				StepProcess("Post processing search result for file: " + Path.GetFileName(fileData.FilePath) + " ...");
				if (sourceSettings != null && sourceSettings.SearchInTag && fileData.SearchSourceResults.Any(ssr => ssr.Value.Tags.Count > 0))
				{
					var sResults = fileData.SearchSourceResults.Where(ssr => ssr.Value.Tags.Count > 0);
					foreach (var sResult in sResults)
					{
						for (var i = 0; i < sResult.Value.Tags.Count; i++)
						{
							var tag = sResult.Value.Tags[i];
							if (tag.TagText.StartsWith("</"))
								continue;
							if (tag.SearchResults == null || tag.SearchResults.Count == 0)
								continue;

							var nextTag = tag;
							if (i + 1 < sResult.Value.Tags.Count)
								nextTag = sResult.Value.Tags[i + 1];

							if (sResult.Value.SearchResults == null) sResult.Value.SearchResults = new List<IndexData>();
							sResult.Value.SearchResults.Add(new IndexData(tag.TagPosition, nextTag.TagPosition - tag.TagPosition));
						}
					}
				}

				if (sourceSettings != null && sourceSettings.UseRegex &&
					fileData.SearchSourceResults.Any(sr => sr.Value.MatchesCount > 0))
				{
					StepProcess("Post processing source result ...");
					var sResults = fileData.SearchSourceResults.Where(sr => sr.Value.MatchesCount > 0);
					foreach (var sResult in sResults)
					{
						var sResultText = sResult.Value.SegmentText;
						var searchResultsToRemove = new List<IndexData>();
						foreach (var searchResult in sResult.Value.SearchResults)
						{
							if (!Regex.IsMatch(sResultText, sourceSettings.SearchText, !sourceSettings.MatchCase ? RegexOptions.IgnoreCase : RegexOptions.None))
							{
								searchResultsToRemove.Add(searchResult);
							}
						}

						foreach (var indexData in searchResultsToRemove)
						{
							sResult.Value.SearchResults.Remove(indexData);
						}
					}
				}

				if (targetSettings != null && targetSettings.SearchInTag && fileData.SearchTargetResults.Any(ssr => ssr.Value.Tags.Count > 0))
				{
					var sResults = fileData.SearchTargetResults.Where(ssr => ssr.Value.Tags.Count > 0);
					foreach (var sResult in sResults)
					{
						for (var i = 0; i < sResult.Value.Tags.Count; i++)
						{
							var tag = sResult.Value.Tags[i];
							if (tag.TagText.StartsWith("</"))
								continue;
							if (tag.SearchResults == null || tag.SearchResults.Count == 0)
								continue;

							var nextTag = sResult.Value.Tags[i + 1];

							if (sResult.Value.SearchResults == null) sResult.Value.SearchResults = new List<IndexData>();
							sResult.Value.SearchResults.Add(new IndexData(tag.TagPosition, nextTag.TagPosition - tag.TagPosition));
						}
					}
				}

				if (targetSettings != null && targetSettings.UseRegex &&
					fileData.SearchTargetResults.Any(sr => sr.Value.MatchesCount > 0))
				{
					StepProcess("Post processing target result ...");
					var sResults = fileData.SearchTargetResults.Where(sr => sr.Value.MatchesCount > 0);
					foreach (var sResult in sResults)
					{
						var tResultText = sResult.Value.SegmentText;
						var searchResultsToRemove = new List<IndexData>();
						foreach (var searchResult in sResult.Value.SearchResults)
						{
							if (!Regex.IsMatch(tResultText, targetSettings.SearchText, !targetSettings.MatchCase ? RegexOptions.IgnoreCase : RegexOptions.None))
							{
								searchResultsToRemove.Add(searchResult);
							}
						}

						foreach (var indexData in searchResultsToRemove)
						{
							sResult.Value.SearchResults.Remove(indexData);
						}
					}
				}
			}
			StepProcess("Post processing completed.");
		}

		private void ReplaceInResultsForReplacePreview()
		{
			var fileDataToBeAdded = new List<FileData>();
			var fileDataToBeRemoved = new List<FileData>();
			foreach (var fileData in _replaceResults)
			{
				StepProcess("Preparing replace information for file:" + Path.GetFileName(fileData.FilePath) + " ...");
				var sourceSegments = new List<SegmentData>();
				var targetSegments = new List<SegmentData>();
				foreach (var sourceSegment in fileData.SearchSourceResults)
				{
					var newText = ReplaceSegmentTextForPreview(sourceSegment.Value.SegmentText,
																  txtReplaceSourceSearch.Text,
																  txtReplaceSourceReplace.Text);
					var sData = new SegmentData(sourceSegment.Value.Sid, sourceSegment.Value.SegmentId,
												newText,
												sourceSegment.Value.SegmentStatus,
												sourceSegment.Value.SegmentContent)
					{
						Tags = sourceSegment.Value.Tags,
						SearchResults = sourceSegment.Value.SearchResults,
					};
					if (sData.SearchResults != null && sData.SearchResults.Count > 0)
					{
						var lenDif = (newText.Length - sourceSegment.Value.SegmentText.Length) / sData.SearchResults.Count;

						for (var index = 0; index < sData.SearchResults.Count; index++)
						{
							var searchResult = sData.SearchResults[index];
							searchResult.Length += lenDif;
							searchResult.IndexStart += (index * lenDif);
						}
					}
					sourceSegments.Add(sData);
				}
				foreach (var targetSegment in fileData.SearchTargetResults)
				{
					var newText = ReplaceSegmentTextForPreview(targetSegment.Value.SegmentText,
																  txtReplaceTargetSearch.Text,
																  txtReplaceTargetReplace.Text);
					var sData = new SegmentData(targetSegment.Value.Sid, targetSegment.Value.SegmentId,
												newText, targetSegment.Value.SegmentStatus,
												targetSegment.Value.SegmentContent)
					{
						Tags = targetSegment.Value.Tags,
						SearchResults = targetSegment.Value.SearchResults
					};
					if (sData.SearchResults != null && sData.SearchResults.Count > 0)
					{
						var lenDif = (newText.Length - targetSegment.Value.SegmentText.Length) / sData.SearchResults.Count;

						for (var index = 0; index < sData.SearchResults.Count; index++)
						{
							var searchResult = sData.SearchResults[index];
							searchResult.Length += lenDif;
							searchResult.IndexStart += (index * lenDif);
						}
					}
					targetSegments.Add(sData);
				}
				var fileToBeAdded = new FileData(fileData.FilePath,
													  sourceSegments.OrderBy(seg => seg.SegmentId).ToList(),
													  targetSegments.OrderBy(seg => seg.SegmentId).ToList());

				fileDataToBeAdded.Add(fileToBeAdded);
				fileDataToBeRemoved.Add(fileData);
				StepProcess("File:" + Path.GetFileName(fileData.FilePath) + " ready.");
			}

			StepProcess("Preparing the view ...");
			var lenghtOfFilesToBeAdded = fileDataToBeAdded.Count;
			for (var i = 0; i < fileDataToBeRemoved.Count; i++)
			{
				_replaceResults.Remove(fileDataToBeRemoved[i]);
				if (i < lenghtOfFilesToBeAdded)
					_replaceResults.Insert(i, fileDataToBeAdded[i]);
			}
		}

		private string ReplaceSegmentTextForPreview(string segmentText, string pattern, string replaceString)
		{
			if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(replaceString))
				return segmentText;

			var text = segmentText;
			if (ckReplaceUseRegEx.Checked)
			{
				var options = RegexOptions.None;
				if (!ckReplaceMatchCase.Checked)
					options = RegexOptions.IgnoreCase;
				return Regex.Replace(text, pattern, replaceString, options);
			}

			var remove = Regex.Escape(pattern);
			var replacePattern = ckReplaceMatchWholeWord.Checked
										? string.Format(@"(\b(?<!\w){0}\b|(?<=^|\s){0}(?=\s|$))", remove)
										: remove;

			return Regex.Replace(text, replacePattern, replaceString,
								 !ckReplaceMatchCase.Checked ? RegexOptions.IgnoreCase : RegexOptions.None);
		}

		private void DoReplacePreviewOperation()
		{
			StepProcess("Preparing files for replace operation ...");
			var sourceSettings = string.IsNullOrEmpty(txtReplaceSourceSearch.Text)
									 ? null
									 : new SearchSettings()
									 {
										 SearchText = txtReplaceSourceSearch.Text,
										 ReplaceText = txtReplaceSourceSearch.Text,
										 SearchInSource = true,
										 SearchInTarget = false,
										 MatchCase = ckReplaceMatchCase.Checked,
										 MatchWholeWord = ckReplaceMatchWholeWord.Checked,
										 UseRegex = ckReplaceUseRegEx.Checked,
										 SearchInTag = false,
										 SearchInLocked = true,
										 NotSearchStatus = new List<ConfirmationLevel>(),
										 LockSegment = false,
										 UnlockContent = false,
										 NewStatus = ConfirmationLevel.Unspecified,
										 UpdateStatus = false
									 };

			var targetSettings = string.IsNullOrEmpty(txtReplaceTargetSearch.Text)
									 ? null
									 : new SearchSettings()
									 {
										 SearchText = txtReplaceTargetSearch.Text,
										 ReplaceText = txtReplaceTargetSearch.Text,
										 SearchInSource = false,
										 SearchInTarget = true,
										 MatchCase = ckReplaceMatchCase.Checked,
										 MatchWholeWord = ckReplaceMatchWholeWord.Checked,
										 UseRegex = ckReplaceUseRegEx.Checked,
										 SearchInTag = false,
										 SearchInLocked = true,
										 NotSearchStatus = new List<ConfirmationLevel>(),
										 LockSegment = false,
										 UnlockContent = false,
										 NewStatus = ConfirmationLevel.Unspecified,
										 UpdateStatus = false
									 };

			FindInFilesForReplace(sourceSettings, targetSettings);
			ReplaceInResultsForReplacePreview();
		}

		private void ckChangeToNotTranslated_Click(object sender, EventArgs e)
		{
			if (!_checkedChangedBeforeClick && ((RadioButton)sender).Checked)
				((RadioButton)sender).Checked = false;
			_checkedChangedBeforeClick = false;
		}

		private bool _checkedChangedBeforeClick = false;

		private void ckChangeToNotTranslated_CheckedChanged(object sender, EventArgs e)
		{
			_checkedChangedBeforeClick = true;
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (gridXLIFFFiles.DataSource != null)
			{
				var dsFiles = (XLIFFFiles)((XLIFFFiles)gridXLIFFFiles.DataSource).Copy();
				var files =
					(from DataGridViewRow row in gridXLIFFFiles.SelectedRows select row.Cells[0].Value.ToString()).ToList();

				var rowsToBeRemoved = new List<XLIFFFiles.FilesRow>();
				foreach (var fileRow in dsFiles.Files)
				{
					if (files.Contains(fileRow.Path))
						rowsToBeRemoved.Add(fileRow);
				}

				foreach (var filesRow in rowsToBeRemoved)
					dsFiles.Files.RemoveFilesRow(filesRow);

				BindFilesGrid(dsFiles);
			}
		}

		private void SDLXLIFFSliceOrChange_FormClosed(object sender, FormClosedEventArgs e)
		{

			//Close();

		}

		private bool _formSizeChanged = false;
		private void SDLXLIFFSliceOrChange_SizeChanged(object sender, EventArgs e)
		{
			if (_setFormSizeChanged)
				_formSizeChanged = true;
		}

		private void txtReplaceSourceSearch_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var pattern = txtReplaceSourceSearch.Text;
			try
			{
				new Regex(pattern);
				_errorProvider.SetError(txtReplaceSourceSearch, "");
			}
			catch (Exception ex)
			{
				_errorProvider.SetError(txtReplaceSourceSearch, "Invalid regular expression");
			}
		}

		private void txtReplaceTargetSearch_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var pattern = txtReplaceTargetSearch.Text;
			try
			{
				new Regex(pattern);
				_errorProvider.SetError(txtReplaceTargetSearch, "");
			}
			catch (Exception ex)
			{
				_errorProvider.SetError(txtReplaceTargetSearch, "Invalid regular expression");
			}
		}

		protected override bool ProcessCmdKey(ref Message message, Keys keys)
		{
			if (ControlHasFocus() && keys == (Keys.Control | Keys.Shift | Keys.Space))
			{
				UpdateFocusedTextBox(new[] { '\u00A0' });

				return true;
			}

			return base.ProcessCmdKey(ref message, keys);
		}

		private bool ControlHasFocus()
		{
			return txtSourceSearch.Focused ||
				   txtTargetSearch.Focused ||
				   txtReplaceSourceSearch.Focused ||
				   txtReplaceSourceReplace.Focused ||
				   txtReplaceTargetSearch.Focused ||
				   txtReplaceTargetReplace.Focused;
		}

		private void UpdateFocusedTextBox(IEnumerable chars)
		{
			var str = chars.Cast<object>().Aggregate(string.Empty, (current, c) => current + c.ToString());

			if (txtSourceSearch.Focused)
			{
				txtSourceSearch.SelectedText = str;
			}
			else if (txtTargetSearch.Focused)
			{
				txtTargetSearch.SelectedText = str;
			}
			else if (txtReplaceSourceSearch.Focused)
			{
				txtReplaceSourceSearch.SelectedText = str;
			}
			else if (txtReplaceSourceReplace.Focused)
			{
				txtReplaceSourceReplace.SelectedText = str;
			}
			else if (txtReplaceTargetSearch.Focused)
			{
				txtReplaceTargetSearch.SelectedText = str;
			}
			else if (txtReplaceTargetReplace.Focused)
			{
				txtReplaceTargetReplace.SelectedText = str;
			}
		}

		private static ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		private string GetProjectPath()
		{
			var projectsController = GetProjectController();
			if (projectsController?.CurrentProject != null)
			{
				var currentProject = projectsController.CurrentProject;
				var projectPath = Path.GetDirectoryName(currentProject.FilePath);
				return projectPath;
			}

			return $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{GetStudioVersion()}\Projects";
		}
	}
}