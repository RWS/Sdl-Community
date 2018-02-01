using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Community.Plugins.AdvancedDisplayFilter;
using Sdl.Community.Plugins.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Models;
using Sdl.Community.Toolkit.Integration.DisplayFilter;
using Sdl.Community.Toolkit.FileType;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
	public partial class DisplayFilterControl : UserControl
    {
        #region  |  Delegates  |

        public delegate void OnApplyFilterHandler(DisplayFilterSettings displayFilterSettings, CustomFilterSettings customSettings,bool reverse,FilteredCountsCallback result);
        public event OnApplyFilterHandler OnApplyDisplayFilter;
        public delegate void FilteredCountsCallback(int filteredSegments, int totalSegments);

	    private bool _uniqueSegments ;
	    private bool _reverseFilter;
        #endregion

        #region  |  Properties  |

        private static EditorController GetEditorController()
        {
            return SdlTradosStudio.Application.GetController<EditorController>();
        }
        private EditorController EditorController { get; set; }
        private Document ActiveDocument { get; set; }

        public DisplayFilter DisplayFilter { get; set; }

        public IList<IContextInfo> ContextInfoList { get; set; }

        private int TotalSegmentPairsCount { get; set; }
        private int FilteredSegmentPairsCount { get; set; }
	    public List<string> AvailableColorsList { get; set; }

	    private CustomFilterSettings _customSettings;
		private CustomFilterSettings CustomFilter
	    {
			get
			{
				_customSettings = new CustomFilterSettings
				{
					OddsNo = oddBtn.Checked,
					EvenNo = evenBtn.Checked,
					Grouped = groupedBtn.Checked,
					UseRegexCommentSearch = commentRegexBox.Checked,
					Colors = new List<string>(),
					FuzzyMin = fuzzyMin.Text,
					FuzzyMax = fuzzyMax.Text,
					SplitSegments = splitCheckBox.Checked,
					MergedSegments = mergedCheckbox.Checked,
					SourceEqualsTarget = sourceSameBox.Checked,
					IsEqualsCaseSensitive = equalsCaseSensitive.Checked,
					Unique = _uniqueSegments,
					MergedAcross = mergedAcross.Checked,
					ContainsTags = containsTagsCheckBox.Checked,
					ModifiedBy = modifiedByBox.Text,
					ModifiedByChecked = modifiedByCheck.Checked,
					CreatedBy = createdByBox.Text,
					CreatedByChecked = createdByCheck.Checked
				};
				foreach (ListViewItem color in colorsListView.SelectedItems)
				{
					var colorCode = color.Text;

					if (!_customSettings.Colors.Contains(colorCode))
					{
						_customSettings.Colors.Add(colorCode);
					}
				}
				if (groupedBtn.Checked)
				{
					_customSettings.GroupedList = segmentsBox.Text;
				}
				if (commentRegexBox.Checked)
				{
					_customSettings.CommentRegex = textBox_commentText.Text;

				}
				return _customSettings;
			}
			set
			{
				if (value == null) return;
				//segments settings 
				
				oddBtn.Checked = value.OddsNo;
				evenBtn.Checked = value.EvenNo;
				groupedBtn.Checked = value.Grouped;
				splitCheckBox.Checked = value.SplitSegments;
				mergedCheckbox.Checked = value.MergedSegments;
				fuzzyMin.Text = value.FuzzyMin;
				fuzzyMax.Text = value.FuzzyMax;
				sourceSameBox.Checked = value.SourceEqualsTarget;
				equalsCaseSensitive.Checked = value.IsEqualsCaseSensitive;
				_uniqueSegments = value.Unique;
				commentRegexBox.Checked = value.UseRegexCommentSearch;
				_customSettings.Colors = value.Colors;
				mergedAcross.Checked = value.MergedAcross;
				containsTagsCheckBox.Checked = value.ContainsTags;
				modifiedByBox.Text = value.ModifiedBy;
				modifiedByCheck.Checked = value.ModifiedByChecked;
				createdByBox.Text = value.CreatedBy;
				createdByCheck.Checked = value.CreatedByChecked;
				foreach (var color in value.Colors)
				{
					foreach (ListViewItem colorItem in colorsListView.Items)
					{
						if (colorItem.Text.Equals(color))
						{
							colorItem.Selected = true;
						}
					}
				}
				if (groupedBtn.Checked)
				{
				 segmentsBox.Text = value.GroupedList;
				}
			}
		}

	    private DisplayFilterSettings DisplayFilterSettings
        {
            get
            {
                #region  |  get settings  |
                var settings = new DisplayFilterSettings()
                {
                    IsRegularExpression = checkBox_regularExpression.Checked,
                    IsCaseSensitive = checkBox_caseSensitive.Checked,
                    SourceText = textBox_source.Text.Trim(),
                    TargetText = textBox_target.Text.Trim(),
                    CommentText = textBox_commentText.Text.Trim(),
                    CommentAuthor = textBox_commentAuthor.Text.Trim(),
                    CommentSeverity = comboBox_commentSeverity.SelectedIndex,
                    ShowAllContent = false
                };
				
                foreach (var contextInfo in listView_contextInfo.SelectedItems
                    .Cast<ListViewItem>().Select(selectedItem => selectedItem.Tag as IContextInfo)
                    .Where(contextInfo => contextInfo != null
                        && !settings.ContextInfoTypes.Contains(contextInfo.ContextType)))
                {
                    settings.ContextInfoTypes.Add(contextInfo.ContextType);
                }

                foreach (ListViewItem item in listView_selected.Items)
                {
                    if (item.Group == GroupGeneralSelected
                        && item.Tag.ToString() == StringResources.DisplayFilterControl_ShowAllContent)
                        settings.ShowAllContent = true;
                    else if (item.Group == GroupOriginSelected)
                        settings.OriginTypes.Add(item.Tag.ToString());
                    else if (item.Group == GroupPreviousOriginSelected)
                        settings.PreviousOriginTypes.Add(item.Tag.ToString());
                    else if (item.Group == GroupStatusSelected)
                        settings.ConfirmationLevels.Add(item.Tag.ToString());
                    else if (item.Group == GroupRepetitionTypesSelected)
                        settings.RepetitionTypes.Add(item.Tag.ToString());
                    else if (item.Group == GroupReviewTypesSelected)
                        settings.SegmentReviewTypes.Add(item.Tag.ToString());
                    else if (item.Group == GroupLockingTypesSelected)
                        settings.SegmentLockingTypes.Add(item.Tag.ToString());
                    else if (item.Group == GroupContentTypesSelected)
                        settings.SegmentContentTypes.Add(item.Tag.ToString());
                }
                #endregion
                return settings;
            }
            set
            {
                if (value == null) return;
                #region  |  set settings  |

                #region  |  content panel  |

                textBox_source.Text = value.SourceText;
                textBox_target.Text = value.TargetText;

                checkBox_regularExpression.Checked = value.IsRegularExpression;
                checkBox_caseSensitive.Checked = value.IsCaseSensitive;

                #endregion

                #region  |  filters panel  |

                try
                {
                    listView_available.BeginUpdate();
                    listView_selected.BeginUpdate();

                    MoveAllListViewItems(listView_selected, listView_available);

                    foreach (var item in listView_available.Items.Cast<ListViewItem>()
                        .Where(
                            item =>
                                item.Group == GroupGeneralAvailable
                                &&
                                item.Tag.ToString() ==
                                StringResources.DisplayFilterControl_ShowAllContent
                                && value.ShowAllContent))
                    {

                        MoveListViewItem(listView_available, item, listView_selected);
                    }


                    if (value.RepetitionTypes.Any())
                    {
                        foreach (var item in listView_available.Items.Cast<ListViewItem>()
                            .Where(
                                item =>
                                    item.Group == GroupRepetitionTypesAvailable &&
                                    value.RepetitionTypes.Contains(item.Tag.ToString())))
                        {

                            MoveListViewItem(listView_available, item, listView_selected);
                        }
                    }

                    if (value.SegmentReviewTypes.Any())
                    {
                        foreach (var item in listView_available.Items.Cast<ListViewItem>()
                            .Where(
                                item =>
                                    item.Group == GroupReviewTypesAvailable &&
                                    value.SegmentReviewTypes.Contains(item.Tag.ToString())))
                        {

                            MoveListViewItem(listView_available, item, listView_selected);
                        }
                    }

                    if (value.SegmentLockingTypes.Any())
                    {
                        foreach (var item in listView_available.Items.Cast<ListViewItem>()
                            .Where(
                                item =>
                                    item.Group == GroupLockingTypesAvailable &&
                                    value.SegmentLockingTypes.Contains(item.Tag.ToString())))
                        {
                            MoveListViewItem(listView_available, item, listView_selected);
                        }
                    }

                    if (value.SegmentContentTypes.Any())
                    {
                        foreach (var item in listView_available.Items.Cast<ListViewItem>()
                            .Where(
                                item =>
                                    item.Group == GroupContentTypesAvailable &&
                                    value.SegmentContentTypes.Contains(item.Tag.ToString())))
                        {
                            MoveListViewItem(listView_available, item, listView_selected);
                        }
                    }


                    if (value.ConfirmationLevels.Any())
                    {
                        foreach (var item in listView_available.Items.Cast<ListViewItem>()
                            .Where(
                                item =>
                                    item.Group == GroupStatusAvailable &&
                                    value.ConfirmationLevels.Contains(item.Tag.ToString())))
                        {
                            MoveListViewItem(listView_available, item, listView_selected);
                        }
                    }


                    if (value.OriginTypes.Any())
                    {
                        foreach (var item in listView_available.Items.Cast<ListViewItem>()
                            .Where(
                                item =>
                                    item.Group == GroupOriginAvailable &&
                                    value.OriginTypes.Contains(item.Tag.ToString())))
                        {
                            MoveListViewItem(listView_available, item, listView_selected);
                        }
                    }

                    if (value.PreviousOriginTypes.Any())
                    {
                        foreach (var item in listView_available.Items.Cast<ListViewItem>()
                            .Where(
                                item =>
                                    item.Group == GroupPreviousOriginAvailable &&
                                    value.PreviousOriginTypes.Contains(item.Tag.ToString())))
                        {
                            MoveListViewItem(listView_available, item, listView_selected);
                        }
                    }
                }
                finally
                {
                    listView_available.EndUpdate();
                    listView_selected.EndUpdate();
                }

                #endregion

                #region  |  comments panel  |

                textBox_commentText.Text = value.CommentText;
                textBox_commentAuthor.Text = value.CommentAuthor;
                comboBox_commentSeverity.SelectedIndex = value.CommentSeverity;


                #endregion

                #region  |  context info panel  |


                foreach (ListViewItem item in listView_contextInfo.Items)
                {
                    var contextInfoItem = item.Tag as IContextInfo;
                    if (contextInfoItem != null && value.ContextInfoTypes.Contains(contextInfoItem.ContextType))
                        item.Selected = true;
                    else
                        item.Selected = false;
                }

                #endregion

                #endregion
            }
        }


        #region  |  ListView Groups  |

        internal static ListViewGroup GroupStatusAvailable { get; set; }
        internal static ListViewGroup GroupOriginAvailable { get; set; }
        internal static ListViewGroup GroupPreviousOriginAvailable { get; set; }
        internal static ListViewGroup GroupGeneralAvailable { get; set; }
        internal static ListViewGroup GroupRepetitionTypesAvailable { get; set; }
        internal static ListViewGroup GroupReviewTypesAvailable { get; set; }
        internal static ListViewGroup GroupLockingTypesAvailable { get; set; }
        internal static ListViewGroup GroupContentTypesAvailable { get; set; }

        private static ListViewGroup GroupStatusSelected { get; set; }
        private static ListViewGroup GroupOriginSelected { get; set; }
        private static ListViewGroup GroupPreviousOriginSelected { get; set; }
        private static ListViewGroup GroupGeneralSelected { get; set; }
        private static ListViewGroup GroupRepetitionTypesSelected { get; set; }
        private static ListViewGroup GroupReviewTypesSelected { get; set; }
        private static ListViewGroup GroupLockingTypesSelected { get; set; }
        private static ListViewGroup GroupContentTypesSelected { get; set; }

        #endregion


        #endregion

        #region  |  Constructor  |
        public DisplayFilterControl()
        {
            InitializeComponent();

            AddGroupsToOriginTypeListview();

            InitializeSettings();
			// colorsListView.View = View.List;
	        colorsListView.CheckBoxes = false;
			colorsListView.View = View.Tile;
	        colorsListView.TileSize = new Size(70, 20);

			listView_available.ListViewItemSorter = new ListViewItemComparer();
            listView_selected.ListViewItemSorter = new ListViewItemComparer();

            EditorController = GetEditorController();
            EditorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

            ActiveDocument = EditorController.ActiveDocument;

            OnApplyDisplayFilter += ApplyDisplayFilter;

            listView_available.SetGroupState(ListViewGroupState.Collapsible | ListViewGroupState.Collapsed);
            listView_selected.SetGroupState(ListViewGroupState.Collapsible | ListViewGroupState.Normal);

            listView_available.SetGroupState(ListViewGroupState.Collapsible | ListViewGroupState.Normal, listView_available.Groups[1]);
            listView_available.SetGroupState(ListViewGroupState.Collapsible | ListViewGroupState.Normal, listView_available.Groups[2]);

	        segmentsBox.Enabled = false;
		}
        #endregion

        private void InitializeSettings()
        {
			#region segments number

	        evenBtn.Checked = false;
	        oddBtn.Checked = false;
	        groupedBtn.Checked = false;
	        segmentsBox.Text = string.Empty;
	        segmentsBox.Enabled = false;
	        fuzzyMin.Text = string.Empty;
	        fuzzyMax.Text = string.Empty;
	        splitCheckBox.Checked = false;
	        mergedCheckbox.Checked = false;
	        mergedAcross.Checked = false;
	        commentRegexBox.Checked = false;
	        sourceSameBox.Checked = false;
	        equalsCaseSensitive.Checked = false;
	        _uniqueSegments = false;
			colorsListView.SelectedItems.Clear();
	        _reverseFilter = false;
			containsTagsCheckBox.Checked=false;
	        modifiedByBox.Text = string.Empty;
	        modifiedByCheck.Checked = false;
	        createdByBox.Text = string.Empty;
	        createdByCheck.Checked = false;
#endregion

			#region  |  content panel  |

			textBox_source.Text = string.Empty;
            textBox_target.Text = string.Empty;

            checkBox_regularExpression.Checked = false;
            checkBox_caseSensitive.Checked = false;
            #endregion

            #region  |  filters panel  |
            try
            {
                listView_available.BeginUpdate();
                listView_selected.BeginUpdate();

                listView_available.Items.Clear();
                listView_selected.Items.Clear();

                var _item =
                    listView_available.Items.Add(
                        StringResources.DisplayFilterControl_Show_All_Content);
                _item.Group = GroupGeneralAvailable;
                _item.Tag = StringResources.DisplayFilterControl_ShowAllContent;

                foreach (var type in Enum.GetValues(typeof(DisplayFilterSettings.RepetitionType)))
                {
                    var item =
                        listView_available.Items.Add(Helper.GetTypeName((DisplayFilterSettings.RepetitionType)type));

                    item.Group = GroupRepetitionTypesAvailable;
                    item.Tag = type;
                }

				//unique 
	            var unique = listView_available.Items.Add("Unique Occurrences");
				unique.Group = GroupRepetitionTypesAvailable;
	            unique.Tag = "Unique";

				foreach (var type in Enum.GetValues(typeof(DisplayFilterSettings.SegmentReviewType)))
                {
                    var item =
                        listView_available.Items.Add(Helper.GetTypeName((DisplayFilterSettings.SegmentReviewType)type));

                    item.Group = GroupReviewTypesAvailable;
                    item.Tag = type;
                }
				
                foreach (var type in Enum.GetValues(typeof(DisplayFilterSettings.SegmentLockingType)))
                {
                    var item =
                        listView_available.Items.Add(Helper.GetTypeName((DisplayFilterSettings.SegmentLockingType)type));

                    item.Group = GroupLockingTypesAvailable;
                    item.Tag = type;
                }
				
                foreach (var type in Enum.GetValues(typeof(DisplayFilterSettings.SegmentContentType)))
                {
                    var item =
                        listView_available.Items.Add(Helper.GetTypeName((DisplayFilterSettings.SegmentContentType)type));

                    item.Group = GroupContentTypesAvailable;
                    item.Tag = type;
                }
				
                foreach (var type in Enum.GetValues(typeof(DisplayFilterSettings.ConfirmationLevel)))
                {
                    var item =
                        listView_available.Items.Add(Helper.GetTypeName((DisplayFilterSettings.ConfirmationLevel)type));

                    item.Group = GroupStatusAvailable;
                    item.Tag = type;
                }

                foreach (var type in Enum.GetValues(typeof(OriginType)))
                {
                    if (type.ToString() == "None")
                        continue;

                    var item =
                        listView_available.Items.Add(Helper.GetTypeName((OriginType)type));

                    item.Group = GroupOriginAvailable;

                    item.Tag = type;
                }
                foreach (var type in Enum.GetValues(typeof(OriginType)))
                {
                    if (type.ToString() == "None")
                        continue;

                    var item =
                        listView_available.Items.Add(Helper.GetTypeName((OriginType)type));

                    item.Group = GroupPreviousOriginAvailable;

                    item.Tag = type;
                }
                listView_available.Items[0].Selected = true;
            }
            finally
            {
                listView_available.EndUpdate();
                listView_selected.EndUpdate();
            }

            #endregion

            #region  |  comments panel  |

            textBox_commentText.Text = string.Empty;
            textBox_commentAuthor.Text = string.Empty;

            comboBox_commentSeverity.Items.Clear();
            foreach (var severity in Enum.GetValues(typeof(DisplayFilterSettings.CommentSeverityType)))
                comboBox_commentSeverity.Items.Add(severity.ToString());

            comboBox_commentSeverity.SelectedIndex = 0;
            #endregion

            #region  |  context info panel  |

            PopulateContextInfoList();

            #endregion

            #region  |  filter status counter  |

            // initialize the filter status counter
            UpdateFilteredCountDisplay(0, 0);

            #endregion

            InitializeTabPageIcons();
			AvailableColorsList = new List<string>();
        }

	    private void AddColor(string color)
	    {
		    if (!string.IsNullOrEmpty(color))
		    {
				if (!AvailableColorsList.Contains(color))
				{
					AvailableColorsList.Add(color);
				}
			}
		   
	    }
	    private void PopulateColorList()
	    {
		    try
		    {
			    AvailableColorsList.Clear();
			    foreach (var segmentPair in ActiveDocument.SegmentPairs)
			    {
				    var colorCodesList = ColorPickerHelper.GetColorsList(segmentPair.Source);
				    foreach (var color in colorCodesList)
				    {
					    AddColor(color);
				    }
				    if (colorCodesList.Count > 0) continue;
				    var contextInfoList = segmentPair.GetParagraphUnitProperties().Contexts.Contexts;
				    var colorCode = ColorPickerHelper.DefaultFormatingColorCode(contextInfoList);
				    AddColor(colorCode);
			    }
			    SetAddColorsToListView();
		    }catch(Exception e) { }

	    }

	    private void SetAddColorsToListView()
	    {
		   colorsListView.Items.Clear();
			if (AvailableColorsList != null)
		    {
			    foreach (var color in AvailableColorsList)
			    {
				    var hexaCode = string.Concat("#", color);
				    var colorItem = new ListViewItem(hexaCode)
				    {
					    BackColor = ColorTranslator.FromHtml(hexaCode),
						ForeColor = Color.White
				    };
				    colorsListView.Items.Add(colorItem);
			    }
		    }
	    }

	    public void ApplyFilter(bool reverseSearch)
	    {
		    if (OnApplyDisplayFilter != null)
		    {
			    var result = new FilteredCountsCallback(UpdateFilteredCountDisplay);
			    OnApplyDisplayFilter(DisplayFilterSettings, CustomFilter, reverseSearch,result);
		    }
	    }

	    public void ClearFilter()
        {
            InitializeSettings();
            ApplyFilter(false);
        }

        public void SaveFilter()
        {
            var saveSettingsDialog = new SaveFileDialog
            {
                Title = StringResources.DisplayFilterControl_SaveFilter_Save_Filter_Settings,
                Filter = StringResources.DisplayFilterControl_Settings_XML_File_sdladfsettings
            };
            if (saveSettingsDialog.ShowDialog() != DialogResult.OK) return;

	        var setting = new SavedSettings
	        {
		        CustomFilterSettings = CustomFilter,
		        DisplayFilterSettings = DisplayFilterSettings
	        };

	        var settingsXml = DisplayFilterSerializer.SerializeSettings(setting);
			using (var sw = new StreamWriter(saveSettingsDialog.FileName, false, Encoding.UTF8))
            {
                sw.Write(settingsXml);
                sw.Flush();
            }
        }
        public void LoadFilter()
        {
            var loadSettingsDialog = new OpenFileDialog
            {
                Title = StringResources.DisplayFilterControl_LoadFilter_Load_Filter_Settings,
                Filter = StringResources.DisplayFilterControl_Settings_XML_File_sdladfsettings
            };

            if (loadSettingsDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                // read in the xml content
                string settingsXml;
                using (var sr = new StreamReader(loadSettingsDialog.FileName, Encoding.UTF8))
                    settingsXml = sr.ReadToEnd();

				var savedSettings = DisplayFilterSerializer.DeserializeSettings<SavedSettings>(settingsXml);
				// deserialize the to the settings xml
	            DisplayFilterSettings = savedSettings.DisplayFilterSettings;
	            CustomFilter = savedSettings.CustomFilterSettings;

                ApplyFilter(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ActiveDocument_DocumentFilterChanged(object sender, DocumentFilterEventArgs e)
        {
            if (e.DisplayFilter == null
                || e.DisplayFilter.GetType() != typeof(DisplayFilter))
                InitializeSettings();
			
            UpdateFilteredCountDisplay(e.FilteredSegmentPairsCount, e.TotalSegmentPairsCount);
        }
        private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
        {
            InitializeSettings();

			if (ActiveDocument != null)
                ActiveDocument.DocumentFilterChanged -= ActiveDocument_DocumentFilterChanged;

            // get a reference to the active document            
            ActiveDocument = e.Document;

            if (ActiveDocument != null)
            {
                ActiveDocument.DocumentFilterChanged += ActiveDocument_DocumentFilterChanged;

                SetContextInfoList();
                PopulateContextInfoList();

                if (ActiveDocument.DisplayFilter != null &&
                    ActiveDocument.DisplayFilter.GetType() == typeof(DisplayFilter))
                {
                    //invalidate UI with display settings recovered from the active document
                    DisplayFilterSettings = ((DisplayFilter)ActiveDocument.DisplayFilter).Settings;
                }
				PopulateColorList();

				UpdateFilteredCountDisplay(ActiveDocument.FilteredSegmentPairsCount, ActiveDocument.TotalSegmentPairsCount);
            }
        }
        private void ApplyDisplayFilter(DisplayFilterSettings displayFilterSettings, CustomFilterSettings customFilterSettings,bool reverse,FilteredCountsCallback result)
        {
            if (ActiveDocument == null)
                return;

            DisplayFilter = new DisplayFilter(displayFilterSettings,customFilterSettings, reverse,ActiveDocument);
			ActiveDocument.ApplyFilterOnSegments(DisplayFilter);

            result.Invoke(ActiveDocument.FilteredSegmentPairsCount, ActiveDocument.TotalSegmentPairsCount);
        }
        private void UpdateFilteredCountDisplay(int filteredSegments, int totalSegments)
        {
            FilteredSegmentPairsCount = filteredSegments;
            TotalSegmentPairsCount = totalSegments;

            label_filterStatusBarMessage.Text =
                string.Format(StringResources.DisplayFilterControl_Filtered_of_rows
                    , filteredSegments, totalSegments);

            UpdateFilterExpression();
            CheckEnabledFilterIcons();
        }
        private void UpdateFilterExpression()
        {
            filterExpressionControl.ClearItems();

            if (DisplayFilterSettings.SourceText != string.Empty
                || DisplayFilterSettings.TargetText != string.Empty)
            {
                if (DisplayFilterSettings.SourceText != string.Empty)
                    filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Source + ":\"" + DisplayFilterSettings.SourceText + "\"");
                if (DisplayFilterSettings.TargetText != string.Empty)
                    filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Target + ":\"" + DisplayFilterSettings.TargetText + "\"");

                if (DisplayFilterSettings.IsRegularExpression)
                    filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Regular_Expression + ":\"" + DisplayFilterSettings.IsRegularExpression + "\"");
                if (DisplayFilterSettings.IsCaseSensitive)
                    filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Case_Sensitive + ":\"" + DisplayFilterSettings.IsCaseSensitive + "\"");
            }

            if (DisplayFilterSettings.ShowAllContent)
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Show_All_Content
                    + ":\"" + DisplayFilterSettings.ShowAllContent + "\"");

            if (DisplayFilterSettings.ConfirmationLevels.Any())
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Status + ":"
                    + "(" + DisplayFilterSettings.ConfirmationLevels.Aggregate(string.Empty, (current, item) => current
                    + (current != string.Empty ? " " + "|" + " " : string.Empty)
                    + Helper.GetTypeName((DisplayFilterSettings.ConfirmationLevel)Enum.Parse(
                        typeof(DisplayFilterSettings.ConfirmationLevel), item, true))) + ")");

            if (DisplayFilterSettings.OriginTypes.Any())
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Origin + ":"
                    + "(" + DisplayFilterSettings.OriginTypes.Aggregate(string.Empty, (current, item) => current
                    + (current != string.Empty ? " " + "|" + " " : string.Empty)
                    + Helper.GetTypeName((OriginType)Enum.Parse(
                        typeof(OriginType), item, true))) + ")");

            if (DisplayFilterSettings.PreviousOriginTypes.Any())
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Previous_Origin + ":"
                    + "(" + DisplayFilterSettings.PreviousOriginTypes.Aggregate(string.Empty, (current, item) => current
                    + (current != string.Empty ? " " + "|" + " " : string.Empty)
                    + Helper.GetTypeName((OriginType)Enum.Parse(
                        typeof(OriginType), item, true))) + ")");

			//if (DisplayFilterSettings.RepetitionTypes.Any())
			//    filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Repetitions + ":"
			//        + "(" + DisplayFilterSettings.RepetitionTypes.Aggregate(string.Empty, (current, item) => current
			//        + (current != string.Empty ? " " + "|" + " " : string.Empty)
			//        + Helper.GetTypeName((DisplayFilterSettings.RepetitionType)Enum.Parse(
			//            typeof(DisplayFilterSettings.RepetitionType), item, true))) + ")");

	        if (_reverseFilter)
	        {
				filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Reverse + ":\"" +
				                                _reverseFilter + "\"");
			}
            if (DisplayFilterSettings.SegmentReviewTypes.Any())
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Segment_Review + ":"
                    + "(" + DisplayFilterSettings.SegmentReviewTypes.Aggregate(string.Empty, (current, item) => current
                    + (current != string.Empty ? " " + "|" + " " : string.Empty)
                    + Helper.GetTypeName((DisplayFilterSettings.SegmentReviewType)Enum.Parse(
                        typeof(DisplayFilterSettings.SegmentReviewType), item, true))) + ")");

            if (DisplayFilterSettings.SegmentLockingTypes.Any())
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Segment_Locking + ":"
                    + "(" + DisplayFilterSettings.SegmentLockingTypes.Aggregate(string.Empty, (current, item) => current
                    + (current != string.Empty ? " " + "|" + " " : string.Empty)
                    + Helper.GetTypeName((DisplayFilterSettings.SegmentLockingType)Enum.Parse(
                        typeof(DisplayFilterSettings.SegmentLockingType), item, true))) + ")");

            if (DisplayFilterSettings.SegmentContentTypes.Any())
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Segment_Content + ":"
                    + "(" + DisplayFilterSettings.SegmentContentTypes.Aggregate(string.Empty, (current, item) => current
                    + (current != string.Empty ? " " + "|" + " " : string.Empty)
                    + Helper.GetTypeName((DisplayFilterSettings.SegmentContentType)Enum.Parse(
                        typeof(DisplayFilterSettings.SegmentContentType), item, true))) + ")");

            if (DisplayFilterSettings.CommentText != string.Empty)
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Comment_text + ":\"" + DisplayFilterSettings.CommentText + "\"");
            if (DisplayFilterSettings.CommentAuthor != string.Empty)
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Comment_author + ":\"" + DisplayFilterSettings.CommentAuthor + "\"");
            if (DisplayFilterSettings.CommentSeverity > 0)
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Comment_severity + ":\"" + (DisplayFilterSettings.CommentSeverityType)DisplayFilterSettings.CommentSeverity + "\"");
			
            if (DisplayFilterSettings.ContextInfoTypes.Any())
                filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Document_structure + ":"
                    + "(" + DisplayFilterSettings.ContextInfoTypes.Aggregate(string.Empty, (current, item) => current
                    + (current != string.Empty ? " " + "|" + " " : string.Empty)
                    + ContextInfoList.FirstOrDefault(a => a.ContextType == item).DisplayCode) + ")");

	        if (CustomFilter != null)
	        {
		        //filter color
		        if (CustomFilter.Colors != null)
		        {
			        if (CustomFilter.Colors.Count > 0)
			        {
				        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Colors + ":"
				                                        + "(" + CustomFilter.Colors.Aggregate(string.Empty,
					                                        (current, item) => current
					                                                           + (current != string.Empty
						                                                           ? " " + "|" + " "
						                                                           : string.Empty)
					                                                           + CustomFilter.Colors.FirstOrDefault(a => a == item)) +
				                                        ")");
			        }
		        }

		        if (CustomFilter.SplitSegments)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_SplitSegments + ":\"" +
			                                        CustomFilter.SplitSegments + "\"");
		        }
		        if (CustomFilter.MergedSegments)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_MergedSegments + ":\"" + CustomFilter.MergedSegments + "\"");
		        }
		        if (CustomFilter.MergedAcross)
		        {
					filterExpressionControl.AddItem(StringResources.DisplayFilterControl_MergedAcross + ":\"" + CustomFilter.MergedAcross + "\"");
				}
				if (CustomFilter.EvenNo)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_EvenSegments + ":\"" +
			                                        CustomFilter.EvenNo + "\"");
		        }

		        if (CustomFilter.OddsNo)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_OddSegments + ":\"" +
			                                        CustomFilter.OddsNo + "\"");
		        }

		        if (CustomFilter.Grouped)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_GroupedList + ":\"" +
			                                        CustomFilter.Grouped + "\"");
		        }

		        if (CustomFilter.UseRegexCommentSearch)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_UseRegexComments + ":\"" +
			                                        CustomFilter.UseRegexCommentSearch + "\"");
		        }

		        if (CustomFilter.SourceEqualsTarget)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_SourceEqualsTarget + ":\"" +
			                                        CustomFilter.SourceEqualsTarget + "\"");
		        }

		        if (CustomFilter.IsEqualsCaseSensitive)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_SourceEqualsTargetCDisplayFilterControl_SourceEqualsTargetCase+ ":\"" +
			                                        CustomFilter.IsEqualsCaseSensitive + "\"");
		        }

				if (CustomFilter.FuzzyMax != string.Empty && CustomFilter.FuzzyMin != string.Empty)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Fuzzy + ":\"" + CustomFilter.FuzzyMin +
			                                        " and " + CustomFilter.FuzzyMax + "\"");
		        }

		        if (CustomFilter.ContainsTags)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_Segments_With_tags + ":\"" +
			                                        CustomFilter.ContainsTags + "\"");

		        }
		        if (CustomFilter.CreatedByChecked)
		        {
					filterExpressionControl.AddItem(StringResources.DisplayFilterControl_CreatedBy+ ":\"" +
					                                CustomFilter.CreatedBy + "\"");
				}
		        if (CustomFilter.ModifiedByChecked)
		        {
			        filterExpressionControl.AddItem(StringResources.DisplayFilterControl_MidifiedBy + ":\"" +
			                                        CustomFilter.ModifiedBy + "\"");
		        }
			}

        }



        #region  |  Helpers  |

        #region  |  Tab icons  |

        private void InitializeTabPageIcons()
        {
            tabPage_content.ImageIndex = -1;
            tabPage_filters.ImageIndex = -1;
            tabPage_comments.ImageIndex = -1;
            tabPage_contextInfo.ImageIndex = -1;
        }
        private void CheckEnabledFilterIcons()
        {
			if (ActiveDocument != null)
            {
                if (ActiveDocument.DisplayFilter != null
                    && ActiveDocument.DisplayFilter.GetType() == typeof(DisplayFilter))
                {
                    var settings = ((DisplayFilter)ActiveDocument.DisplayFilter).Settings;

                    InvalidateIconsFilterApplied_contentTab(settings);
                    InvalidateIconsFilterApplied_filtersTab(settings);
                    InvalidateIconsFilterApplied_commentsTab(settings);
                    InvalidateIconsFilterApplied_contextInfoTab(settings);
	                InvalidateIconsFilterApplied_segmentNumbers(CustomFilter);
	                InvalidateIconsFilterApplied_colorPicker(CustomFilter);
					
					SetStatusBackgroundColorCode(IsFilterApplied(settings));
                }
                else
                {
                    SetStatusBackgroundColorCode(false);

                    InvalidateIconsFilterApplied(tabPage_content);
                    InvalidateIconsFilterApplied(tabPage_filters);
                    InvalidateIconsFilterApplied(tabPage_comments);
                    InvalidateIconsFilterApplied(tabPage_contextInfo);
	                InvalidateIconsFilterApplied(tabPage_segmentNumbers);
	               InvalidateIconsFilterApplied(tabPage_Colors);

				}
            }
        }

	  

	    private void InvalidateIconsFilterApplied_colorPicker(CustomFilterSettings customFilter)
	    {
		    if (customFilter.Colors.Count > 0)
		    {
			    tabPage_Colors.ImageIndex = 0;
		    }
		    else
		    {
				  tabPage_Colors.ImageIndex = -1;
			}
		}

	    private void SetStatusBackgroundColorCode(bool visible)
        {
            panel_filterStatusBarImage.Visible = visible;
            panel_filterStatusBar.BackColor = visible ? SystemColors.GradientInactiveCaption : Color.Transparent;
        }

        private bool IsFilterApplied(DisplayFilterSettings settings)
        {
	        if (!string.IsNullOrEmpty(settings.SourceText)
                || !string.IsNullOrEmpty(settings.TargetText)
                || settings.ContextInfoTypes.Any()
                || settings.SegmentReviewTypes.Any()
                || settings.ConfirmationLevels.Any()
                || settings.OriginTypes.Any()
                || settings.PreviousOriginTypes.Any()
                || settings.RepetitionTypes.Any()
                || settings.SegmentContentTypes.Any()
                || settings.SegmentLockingTypes.Any()
                || settings.SegmentReviewTypes.Any()
                || settings.ShowAllContent
                || !string.IsNullOrEmpty(settings.CommentText)
                || !string.IsNullOrEmpty(settings.CommentAuthor)
                || settings.CommentSeverity > 0)
            {
                return true;
            }
	        return false;
        }

        private void InvalidateIconsFilterApplied(TabPage tabPage)
        {
            tabPage.ImageIndex = -1;
        }
        private void InvalidateIconsFilterApplied_contentTab(DisplayFilterSettings settings)
        {
            if (!string.IsNullOrEmpty(settings.SourceText)
                || !string.IsNullOrEmpty(settings.TargetText))
            {
                tabPage_content.ImageIndex = 0;
            }
            else
            {
                tabPage_content.ImageIndex = -1;
            }
        }
        private void InvalidateIconsFilterApplied_filtersTab(DisplayFilterSettings settings)
        {
            if (settings.SegmentReviewTypes.Any()
                || settings.ConfirmationLevels.Any()
                || settings.OriginTypes.Any()
                || settings.PreviousOriginTypes.Any()
                || settings.RepetitionTypes.Any()
                || settings.SegmentContentTypes.Any()
                || settings.SegmentLockingTypes.Any()
                || settings.SegmentReviewTypes.Any()
                || settings.ShowAllContent)
            {
                tabPage_filters.ImageIndex = 0;
            }
            else
            {
                tabPage_filters.ImageIndex = -1;
            }
        }
        private void InvalidateIconsFilterApplied_commentsTab(DisplayFilterSettings settings)
        {
            if (!string.IsNullOrEmpty(settings.CommentText)
                || !string.IsNullOrEmpty(settings.CommentAuthor)
                || settings.CommentSeverity > 0)
            {
                tabPage_comments.ImageIndex = 0;
            }
            else
            {
                tabPage_comments.ImageIndex = -1;
            }
        }
        private void InvalidateIconsFilterApplied_contextInfoTab(DisplayFilterSettings settings)
        {
            if (settings.ContextInfoTypes.Any())
            {
                tabPage_contextInfo.ImageIndex = 0;
            }
            else
            {
                tabPage_contextInfo.ImageIndex = -1;
            }

        }

	    private void InvalidateIconsFilterApplied_segmentNumbers(CustomFilterSettings customFilterSettings)
	    {
		    if (customFilterSettings.EvenNo || customFilterSettings.Grouped ||
		        customFilterSettings.OddsNo || customFilterSettings.SplitSegments ||
		        customFilterSettings.MergedSegments || customFilterSettings.SourceEqualsTarget ||
		        !string.IsNullOrWhiteSpace(customFilterSettings.FuzzyMin) &&
		        !string.IsNullOrWhiteSpace(customFilterSettings.FuzzyMax)
		        || customFilterSettings.MergedAcross||customFilterSettings.ContainsTags||customFilterSettings.CreatedByChecked
				||customFilterSettings.ModifiedByChecked)
		    {
			    tabPage_segmentNumbers.ImageIndex = 0;
		    }
		    else
		    {
			    tabPage_segmentNumbers.ImageIndex = -1;
		    }
	    }

	    private void InvalidateIconsFilterEdited(TabPage tabPage)
        {
            if (ActiveDocument != null && ActiveDocument.DisplayFilter != null
                && ActiveDocument.DisplayFilter.GetType() == typeof(DisplayFilter))
            {
                var settings = ((DisplayFilter)ActiveDocument.DisplayFilter).Settings;

                if (tabPage == tabPage_content
                    && (!string.IsNullOrEmpty(settings.SourceText)
                    || !string.IsNullOrEmpty(settings.TargetText)))
                {
					
					var item1 = textBox_source.Text + ", " + textBox_target.Text + ", " +
								checkBox_regularExpression.Checked + ", " + checkBox_caseSensitive.Checked;

					var item2 = settings.SourceText + ", " + settings.TargetText + ", " +
								settings.IsRegularExpression + ", " + settings.IsCaseSensitive;

					tabPage.ImageIndex = string.CompareOrdinal(item1, item2) == 0 ? 0 : 1;
                }
                else if (tabPage == tabPage_filters
                    && (settings.SegmentReviewTypes.Any()
                    || settings.ConfirmationLevels.Any()
                    || settings.OriginTypes.Any()
                    || settings.PreviousOriginTypes.Any()
                    || settings.RepetitionTypes.Any()
                    || settings.SegmentContentTypes.Any()
                    || settings.SegmentLockingTypes.Any()
                    || settings.SegmentReviewTypes.Any()
                    || settings.ShowAllContent))
                {
                    var list1 = new List<string> { DisplayFilterSettings.ShowAllContent.ToString() };
                    list1.AddRange(DisplayFilterSettings.OriginTypes);
                    list1.AddRange(DisplayFilterSettings.PreviousOriginTypes);
                    list1.AddRange(DisplayFilterSettings.ConfirmationLevels);
                    list1.AddRange(DisplayFilterSettings.RepetitionTypes);
                    list1.AddRange(DisplayFilterSettings.SegmentReviewTypes);
                    list1.AddRange(DisplayFilterSettings.SegmentLockingTypes);
                    list1.AddRange(DisplayFilterSettings.SegmentContentTypes);

                    var list2 = new List<string> { settings.ShowAllContent.ToString() };
                    list2.AddRange(settings.OriginTypes);
                    list2.AddRange(settings.PreviousOriginTypes);
                    list2.AddRange(settings.ConfirmationLevels);
                    list2.AddRange(settings.RepetitionTypes);
                    list2.AddRange(settings.SegmentReviewTypes);
                    list2.AddRange(settings.SegmentLockingTypes);
                    list2.AddRange(settings.SegmentContentTypes);

                    tabPage.ImageIndex = string.Join(", ", list1) == string.Join(", ", list2)
                        ? 0
                        : 1;
                }
                else if (tabPage == tabPage_comments
                    && (!string.IsNullOrEmpty(settings.CommentText)
                    || !string.IsNullOrEmpty(settings.CommentAuthor)
                    || settings.CommentSeverity > 0))
                {
                    var item1 = textBox_commentText.Text + ", " + textBox_commentAuthor.Text + ", " +
                                comboBox_commentSeverity.SelectedIndex;

                    var item2 = settings.CommentText + ", " + settings.CommentAuthor + ", " +
                                settings.CommentSeverity;

                    tabPage.ImageIndex = string.CompareOrdinal(item1, item2) == 0 ? 0 : 1;
                }
                else if (tabPage == tabPage_contextInfo && settings.ContextInfoTypes.Any())
                {
                    // check if identical selection
                    var list = new List<string>();
                    foreach (var contextInfo in listView_contextInfo.SelectedItems
                        .Cast<ListViewItem>().Select(selectedItem => selectedItem.Tag as IContextInfo)
                        .Where(contextInfo => contextInfo != null
                                              && !list.Contains(contextInfo.ContextType)))
                    {
                        list.Add(contextInfo.ContextType);
                    }
                    tabPage.ImageIndex = string.Join(", ", list) == string.Join(", ", settings.ContextInfoTypes)
                        ? 0
                        : 1;
                }
                else
                    tabPage.ImageIndex = 1;
            }
            else
            {
                tabPage.ImageIndex = 1;
            }
        }
		
        #endregion

        #region  |  Filter Attributes group  |

        private void CheckEnabledActionButtons()
        {
            var add = true;
            var remove = true;
            var removeAll = true;

            if (listView_available.SelectedItems.Count == 0)
                add = false;

            if (listView_selected.SelectedItems.Count == 0)
                remove = false;

            if (listView_selected.Items.Count == 0)
                removeAll = false;

            button_add.Enabled = add;
            button_remove.Enabled = remove;
            button_removeAll.Enabled = removeAll;
        }
        private void MoveSelectedListViewItem(ListView from, ListView to)
        {
            if (from.SelectedItems.Count > 0)
            {
                var itemIndex = 0;
                foreach (ListViewItem itemFrom in from.SelectedItems)
                {
                    itemIndex = itemFrom.Index;

                    var itemTo = to.Items.Add((ListViewItem)itemFrom.Clone());

                    AssignOriginTypeListViewItemGroup(itemTo, itemFrom.Group);


                    itemFrom.Remove();
                }
                SelectDefaultItem(from, itemIndex);

                listView_available.Sort();
                listView_selected.Sort();

                CheckEnabledActionButtons();
            }
        }
        private static void SelectDefaultItem(ListView from, int itemIndex)
        {
            if (from.Items.Count > 0)
            {
                foreach (var item in from.Items.Cast<ListViewItem>())
                    item.Selected = false;

                if (from.Items.Count - 1 < itemIndex)
                    itemIndex--;

                from.Items[itemIndex].Selected = true;
            }
        }
        private void MoveListViewItem(ListView from, ListViewItem itemFrom, ListView to)
        {
            var itemIndex = itemFrom.Index;

            var itemTo = to.Items.Add((ListViewItem)itemFrom.Clone());

            AssignOriginTypeListViewItemGroup(itemTo, itemFrom.Group);

            itemFrom.Remove();

            SelectDefaultItem(from, itemIndex);

            listView_available.Sort();
            listView_selected.Sort();

            CheckEnabledActionButtons();

        }
        private void MoveAllListViewItems(ListView from, ListView to)
        {
            foreach (ListViewItem itemFrom in from.Items)
            {
                var itemTo = to.Items.Add((ListViewItem)itemFrom.Clone());
                AssignOriginTypeListViewItemGroup(itemTo, itemFrom.Group);

                itemFrom.Remove();
            }

            listView_available.Sort();
            listView_selected.Sort();

            CheckEnabledActionButtons();
        }
        private void AssignOriginTypeListViewItemGroup(ListViewItem itemTo, ListViewGroup fromGroup)
        {
            if (itemTo.ListView.Equals(listView_available)
                || itemTo.ListView.Equals(listView_selected))
            {
                if (itemTo.ListView.Equals(listView_available))
                {
                    if (fromGroup == GroupOriginSelected)
                        itemTo.Group = GroupOriginAvailable;
                    else if (fromGroup == GroupPreviousOriginSelected)
                        itemTo.Group = GroupPreviousOriginAvailable;
                    else if (fromGroup == GroupStatusSelected)
                        itemTo.Group = GroupStatusAvailable;
                    else if (fromGroup == GroupGeneralSelected)
                        itemTo.Group = GroupGeneralAvailable;
                    else if (fromGroup == GroupRepetitionTypesSelected)
                        itemTo.Group = GroupRepetitionTypesAvailable;
                    else if (fromGroup == GroupReviewTypesSelected)
                        itemTo.Group = GroupReviewTypesAvailable;
                    else if (fromGroup == GroupLockingTypesSelected)
                        itemTo.Group = GroupLockingTypesAvailable;
                    else if (fromGroup == GroupContentTypesSelected)
                        itemTo.Group = GroupContentTypesAvailable;
                }
                else
                {
                    if (fromGroup == GroupOriginAvailable)
                        itemTo.Group = GroupOriginSelected;
                    else if (fromGroup == GroupPreviousOriginAvailable)
                        itemTo.Group = GroupPreviousOriginSelected;
                    else if (fromGroup == GroupStatusAvailable)
                        itemTo.Group = GroupStatusSelected;
                    else if (fromGroup == GroupGeneralAvailable)
                        itemTo.Group = GroupGeneralSelected;
                    else if (fromGroup == GroupRepetitionTypesAvailable)
                        itemTo.Group = GroupRepetitionTypesSelected;
                    else if (fromGroup == GroupReviewTypesAvailable)
                        itemTo.Group = GroupReviewTypesSelected;
                    else if (fromGroup == GroupLockingTypesAvailable)
                        itemTo.Group = GroupLockingTypesSelected;
                    else if (fromGroup == GroupContentTypesAvailable)
                        itemTo.Group = GroupContentTypesSelected;
                }
            }
        }
        private void AddGroupsToOriginTypeListview()
        {
            listView_available.ShowGroups = true;
            listView_selected.ShowGroups = true;

            GroupGeneralAvailable = new ListViewGroup(StringResources.DisplayFilterControl_General);
            GroupStatusAvailable = new ListViewGroup(StringResources.DisplayFilterControl_Status);
            GroupOriginAvailable = new ListViewGroup(StringResources.DisplayFilterControl_Origin);
            GroupPreviousOriginAvailable = new ListViewGroup(StringResources.DisplayFilterControl_Previous_Origin);
            GroupRepetitionTypesAvailable = new ListViewGroup(StringResources.DisplayFilterControl_Repetitions);
            GroupReviewTypesAvailable = new ListViewGroup(StringResources.DisplayFilterControl_Segment_Review);
            GroupLockingTypesAvailable = new ListViewGroup(StringResources.DisplayFilterControl_Segment_Locking);
            GroupContentTypesAvailable = new ListViewGroup(StringResources.DisplayFilterControl_Segment_Content);

            listView_available.Groups.Add(GroupGeneralAvailable);
            listView_available.Groups.Add(GroupStatusAvailable);
            listView_available.Groups.Add(GroupOriginAvailable);
            listView_available.Groups.Add(GroupPreviousOriginAvailable);
            listView_available.Groups.Add(GroupRepetitionTypesAvailable);
            listView_available.Groups.Add(GroupReviewTypesAvailable);
            listView_available.Groups.Add(GroupLockingTypesAvailable);
            listView_available.Groups.Add(GroupContentTypesAvailable);

            GroupGeneralSelected = new ListViewGroup(StringResources.DisplayFilterControl_General);
            GroupStatusSelected = new ListViewGroup(StringResources.DisplayFilterControl_Status);
            GroupOriginSelected = new ListViewGroup(StringResources.DisplayFilterControl_Origin);
            GroupPreviousOriginSelected = new ListViewGroup(StringResources.DisplayFilterControl_Previous_Origin);
            GroupRepetitionTypesSelected = new ListViewGroup(StringResources.DisplayFilterControl_Repetitions);
            GroupReviewTypesSelected = new ListViewGroup(StringResources.DisplayFilterControl_Segment_Review);
            GroupLockingTypesSelected = new ListViewGroup(StringResources.DisplayFilterControl_Segment_Locking);
            GroupContentTypesSelected = new ListViewGroup(StringResources.DisplayFilterControl_Segment_Content);

            listView_selected.Groups.Add(GroupGeneralSelected);
            listView_selected.Groups.Add(GroupStatusSelected);
            listView_selected.Groups.Add(GroupOriginSelected);
            listView_selected.Groups.Add(GroupPreviousOriginSelected);
            listView_selected.Groups.Add(GroupRepetitionTypesSelected);
            listView_selected.Groups.Add(GroupReviewTypesSelected);
            listView_selected.Groups.Add(GroupLockingTypesSelected);
            listView_selected.Groups.Add(GroupContentTypesSelected);
        }

        #endregion

        #region  |  Context info  |
        private void SetContextInfoList()
        {
            ContextInfoList = new List<IContextInfo>();
            foreach (var segmentPair in ActiveDocument.SegmentPairs)
            {
                var contexts = segmentPair.GetParagraphUnitProperties().Contexts;
                if (contexts == null) continue;
                foreach (var contextInfo in contexts.Contexts
                   .Where(a => a.DisplayCode != null)
                   .Where(contextInfo => ContextInfoList.All(a => a.ContextType != contextInfo.ContextType)))
                {
                    ContextInfoList.Add(contextInfo);
                }
            }
        }
        private void PopulateContextInfoList()
        {
            try
            {
                listView_contextInfo.BeginUpdate();
                listView_contextInfo.Items.Clear();

                if (ContextInfoList == null) return;
                foreach (var contextInfo in ContextInfoList)
                {
                    var item = listView_contextInfo.Items.Add(contextInfo.DisplayCode);
                    item.SubItems.Add(contextInfo.DisplayName);
                    item.SubItems.Add(contextInfo.Description ?? contextInfo.ContextType);
                    item.BackColor = contextInfo.DisplayColor;
                    item.UseItemStyleForSubItems = false;
                    item.Tag = contextInfo;
                }
            }
            finally
            {
                listView_contextInfo.EndUpdate();
            }

            UpdatedContextInfoSelectedStatusCount();
        }

        #endregion

        #endregion

        #region  |  ToolbarStrip events  |
        private void toolStripButton_applyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilter(false);
        }

        private void toolStripButton_clearFilter_Click(object sender, EventArgs e)
        {
            ClearFilter();
        }

        private void toolStripButton_saveFilter_Click(object sender, EventArgs e)
        {
            SaveFilter();
        }

        private void toolStripButton_loadFilter_Click(object sender, EventArgs e)
        {
            LoadFilter();
        }
        #endregion

        #region  |  Content tab events  |
        private void textBox_source_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                ApplyFilter(false);
        }

        private void textBox_target_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                ApplyFilter(false);
        }

        private void textBox_source_TextChanged(object sender, EventArgs e)
        {
            InvalidateIconsFilterEdited(tabPage_content);
        }

        private void textBox_target_TextChanged(object sender, EventArgs e)
        {
            InvalidateIconsFilterEdited(tabPage_content);
        }

        private void checkBox_regularExpression_CheckedChanged(object sender, EventArgs e)
        {
            InvalidateIconsFilterEdited(tabPage_content);
        }

        private void checkBox_caseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            InvalidateIconsFilterEdited(tabPage_content);
        }

        #endregion

        #region  |  Filter Attributes tab events  |

        private void button_add_Click(object sender, EventArgs e)
        {
	        var isSelected =IsUniqueSelected();
	        if (isSelected)
	        {
		        _uniqueSegments = true;
	        }
			MoveSelectedListViewItem(listView_available, listView_selected);
            InvalidateIconsFilterEdited(tabPage_filters);
        }

	    private bool IsUniqueSelected()
	    {
		    foreach (ListViewItem selectedItem in listView_available.SelectedItems)
		    {

			    if (selectedItem.Tag.Equals("Unique"))
			    {
				    return true;
			    }
		    }
		    return false;
	    }
        private void button_remove_Click(object sender, EventArgs e)
        {
	        var isSelected = IsUniqueSelected();
	        if (isSelected)
	        {
		        _uniqueSegments = false;
	        }
			MoveSelectedListViewItem(listView_selected, listView_available);
            InvalidateIconsFilterEdited(tabPage_filters);
        }

        private void button_removeAll_Click(object sender, EventArgs e)
        {
            MoveAllListViewItems(listView_selected, listView_available);
            InvalidateIconsFilterEdited(tabPage_filters);
        }

        private void listView_available_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckEnabledActionButtons();
        }

        private void listView_selected_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckEnabledActionButtons();
        }
        #endregion

        #region  |  Comments tab events  |

        private void textBox_commentText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                ApplyFilter(false);
        }

        private void textBox_commentAuthor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                ApplyFilter(false);
        }
		
        private void textBox_commentText_TextChanged(object sender, EventArgs e)
        {
            InvalidateIconsFilterEdited(tabPage_comments);
        }

        private void textBox_commentAuthor_TextChanged(object sender, EventArgs e)
        {
            InvalidateIconsFilterEdited(tabPage_comments);
        }

        private void comboBox_commentSeverity_SelectedIndexChanged(object sender, EventArgs e)
        {
            InvalidateIconsFilterEdited(tabPage_comments);
        }
        #endregion

        #region  |  Contextinfo tab events  |
        private void linkLabel_contextInfoClearSelection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            listView_contextInfo.BeginUpdate();
            foreach (ListViewItem item in listView_contextInfo.Items)
                item.Selected = false;
            listView_contextInfo.EndUpdate();

            if (ActiveDocument != null && ActiveDocument.DisplayFilter != null
                && ActiveDocument.DisplayFilter.GetType() == typeof(DisplayFilter))
            {
                var settings = ((DisplayFilter)ActiveDocument.DisplayFilter).Settings;

                if (settings.ContextInfoTypes.Any())
                {
                    tabPage_contextInfo.ImageIndex = 1;
                }
                else
                {
                    tabPage_contextInfo.ImageIndex = -1;
                }
            }
            else
            {
                tabPage_contextInfo.ImageIndex = -1;
            }

            UpdatedContextInfoSelectedStatusCount();
        }

        private void listView_contextInfo_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            InvalidateIconsFilterEdited(tabPage_contextInfo);
            UpdatedContextInfoSelectedStatusCount();
        }

        private void UpdatedContextInfoSelectedStatusCount()
        {
            label_contextInfoSelected.Text = string.Format("Selected: {0}", listView_contextInfo.SelectedItems.Count);
        }
        #endregion
		
        private void listView_contextInfo_Resize(object sender, EventArgs e)
        {
            var width = ((ListView)sender).Width - 20 - SystemInformation.VerticalScrollBarWidth;

            columnHeader_code.Width = Convert.ToInt32(width * .2);
            columnHeader_name.Width = Convert.ToInt32(width * .4);
            columnHeader_description.Width = Convert.ToInt32(width * .45);
        }

        private void listView_available_Resize(object sender, EventArgs e)
        {
            var width = ((ListView)sender).Width - 20 - SystemInformation.VerticalScrollBarWidth;
            columnHeader_filtersAvailable_name.Width = width;            
        }

        private void listView_selected_Resize(object sender, EventArgs e)
        {
            var width = ((ListView)sender).Width - 20 - SystemInformation.VerticalScrollBarWidth;
            columnHeader_filtersSelected_name.Width = width;
        }

		private void evenBtn_CheckedChanged(object sender, EventArgs e)
		{
			if (evenBtn.Checked)
			{
				segmentsBox.Enabled = false;
			}
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void oddBtn_CheckedChanged(object sender, EventArgs e)
		{
			if (oddBtn.Checked)
			{
				segmentsBox.Enabled = false;
			}
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}
		
		private void groupedBtn_CheckedChanged(object sender, EventArgs e)
		{
			if (groupedBtn.Checked)
			{
				segmentsBox.Enabled = true;
			}
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}
		
	    private void colorsListView_SelectedIndexChanged(object sender, EventArgs e)
	    {
		    var selectedColors = colorsListView.SelectedItems;
		  
			if (AvailableColorsList != null)
		    {
			    AvailableColorsList.Clear();
				foreach (ListViewItem color in selectedColors)
			    {
				    var colorCode = color.Text;

				    if (!AvailableColorsList.Contains(colorCode))
				    {
					    AvailableColorsList.Add(colorCode);
				    }
			    }
			}
		    InvalidateIconsFilterEdited(tabPage_Colors);
		}

		private void commentRegexBox_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_comments);
		}

		private void splitCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void mergedCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		

		private void sourceSameBox_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void equalsCaseSensitive_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void fuzzyMin_TextChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void fuzzyMax_TextChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void reverseBtn_Click(object sender, EventArgs e)
		{
			_reverseFilter = true;
			ApplyFilter(true);
		}

		private void mergedAcross_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void containsTagsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void modifiedByCheck_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}

		private void createdByCheck_CheckedChanged(object sender, EventArgs e)
		{
			InvalidateIconsFilterEdited(tabPage_segmentNumbers);
		}
	}
}
