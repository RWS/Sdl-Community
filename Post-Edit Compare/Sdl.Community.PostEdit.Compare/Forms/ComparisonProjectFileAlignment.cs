using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sdl.Community.PostEdit.Compare.Core;
using Sdl.Community.PostEdit.Compare.Core.SDLXLIFF;
using Sdl.Community.PostEdit.Compare.Properties;
using Comparer = Sdl.Community.PostEdit.Compare.Core.Comparison.Comparer;
using TextComparer = Sdl.Community.PostEdit.Compare.Core.Comparison.Text.TextComparer;

namespace PostEdit.Compare.Forms
{
    public partial class ComparisonProjectFileAlignment : Form
    {

        public bool Saved { get; set; }
        public Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject ComparisonProject { get; set; }
        private readonly TextComparer _textComparer = new TextComparer();
        public Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting FilterSettingSelected { get; set; }
        private bool IsLinkFileToActive { get; set; }
        private bool IsLinkFiletoActiveLeft { get; set; }
        private FileAlignmentCell FacIsLinkFileToActive { get; set; }
        private int FacIsLinkFileToActiveRowIndex { get; set; }

        private int WordServerCharsExclude { get; set; }

        private decimal FuzzyMatchFileNameValue { get; set; }
        private bool FuzzyMatchFileNameValueChanged { get; set; }

        public ComparisonProjectFileAlignment()
        {
            InitializeComponent();
            Saved = false;


        }


        private void CheckShowExactlyMatchedFilesTooltip()
        {
            toolStripButton_showExactFileMatches.ToolTipText = toolStripButton_showExactFileMatches.Checked ? "Hide fully linked files" : "Show fully linked files";
        }
        private void CheckShowManuallyMatchedFilesTooltip()
        {
            toolStripButton_showManualFileMatches.ToolTipText = toolStripButton_showManualFileMatches.Checked ? "Hide manually linked files" : "Show manually linked files";
        }
        private void CheckActivateFiltersTooltip()
        {
            toolStripButton_Activate_Filters.ToolTipText = toolStripButton_Activate_Filters.Checked ? "Suppress file filters" : "Activate file filters";
        }


        private static string ConvertToRtFstr(string str)
        {
            str = str.Replace(@"\", @"\\");
            str = str.Replace(@"{", @"\{");
            str = str.Replace(@"}", @"\}");
            return str;
        }
        private static string ConvertToWordUni(string inputStr)
        {

            inputStr = inputStr.Replace("\n", @"{\line }");

            var strOut = string.Empty;
            foreach (var c in inputStr)
            {
                if (c <= 0x7f)
                {
                    strOut += c;
                }
                else
                {
                    strOut += "\\u" + Convert.ToUInt32(c) + "?";// "\\'83";// "\\ ";

                }
            }
            return strOut;
        }



        private void toolStripButton_save_Click(object sender, EventArgs e)
        {

            Saved = true;
            Save();
            Close();
        }
        private void Save()
        {

            for (var i = 0; i < dataGridView_fileAlignment.Rows.Count; i++)
            {
                var fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[i].DataBoundItem;

                if (fac.FileAlignment.IsFullMatch) continue;
                var found = false;
                foreach (var fa in ComparisonProject.FileAlignment)
                {
                    if (string.Compare(fa.PathLeft, fac.FileAlignment.PathLeft, StringComparison.OrdinalIgnoreCase) != 0 ||
                        string.Compare(fa.PathRight, fac.FileAlignment.PathRight, StringComparison.OrdinalIgnoreCase) != 0) continue;
                    if (fa.PathLeft.Trim() != string.Empty && fac.FileAlignment.PathLeft.Trim() != string.Empty
                        && fa.PathRight.Trim() != string.Empty && fac.FileAlignment.PathRight.Trim() != string.Empty)
                    {
                        found = true;
                    }
                    break;
                }

                if (!found)
                {

                    ComparisonProject.FileAlignment.Add(fac.FileAlignment);
                }
            }
        }


        private bool IsLoading { get; set; }

        private void ComparisonProjectFileAlignment_Load(object sender, EventArgs e)
        {
            FuzzyMatchFileNameValueChanged = true;
            try
            {
                IsLoading = true;

                Cursor = Cursors.WaitCursor;

                foreach (var fa in ComparisonProject.FileAlignment)
                {
                    if (fa.IsWorldServerFile)
                        checkBox_worldServerFiles.Checked = true;
                }


                checkBox_worldServerFiles_CheckedChanged(null, null);

                numericUpDown_fuzzyMatchFileNames_ValueChanged(null, null);


                IsLinkFileToActive = false;
                IsLinkFiletoActiveLeft = false;

                FuzzyMatchFileNameValue = numericUpDown_fuzzyMatchFileNames.Value;
                FuzzyMatchFileNameValueChanged = false;

                FileAlignments = new List<Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment>();
                resize();
                LoadFilterSettings();

                Visible = true;
                Application.DoEvents();

                CheckShowExactlyMatchedFilesTooltip();
                CheckShowManuallyMatchedFilesTooltip();
                CheckActivateFiltersTooltip();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoading = false;
                Cursor = Cursors.Default;
            }

            LoadFileLists();
        }


        private void LoadFilterSettings()
        {


            Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting fsSelected = null;

            if (toolStripComboBox_fileFilters.SelectedItem != null)
            {
                if (toolStripComboBox_fileFilters.Items.Count > 0)
                {
                    fsSelected = (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting)((ComboboxItem)toolStripComboBox_fileFilters.SelectedItem).Value;
                }
            }
            else if (FilterSettingSelected != null)
            {
                fsSelected = FilterSettingSelected;
            }

            toolStripComboBox_fileFilters.Items.Clear();

            if (Cache.Application.Settings.FilterSettings.Count <= 0) return;
            var index = -1;
            var selectedIndex = 0;
            foreach (var fs_selected in Cache.Application.Settings.FilterSettings)
            {
                if (fs_selected.Name.Trim() == string.Empty) continue;
                var cbi = new ComboboxItem
                {
                    Text = string.Empty,
                    Value = fs_selected
                };

                #region  |  get updated filter text  |

                var filterTextUpdated = string.Empty;

                if (fs_selected.FilterNamesInclude.Count > 0)
                {
                    filterTextUpdated = fs_selected.FilterNamesInclude.Aggregate(filterTextUpdated, (current, str) => current + ((current != string.Empty ? ";" : string.Empty) + str));
                }
                else if (fs_selected.FilterNamesExclude.Count > 0)
                {
                    filterTextUpdated = fs_selected.FilterNamesExclude.Aggregate(filterTextUpdated, (current, str) => current + ((current != string.Empty ? ";" : string.Empty) + str));
                    filterTextUpdated = "-" + filterTextUpdated;
                }



                if (fs_selected.FilterDateUsed)
                {
                    if (fs_selected.FilterDate.Type == Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.GreaterThan)
                    {
                        filterTextUpdated += (filterTextUpdated.Trim() != string.Empty ? "; " : string.Empty)
                                             + ">" + fs_selected.FilterDate.Date.Date.ToShortDateString() + " " + fs_selected.FilterDate.Date.ToShortTimeString();
                    }
                    else
                    {
                        filterTextUpdated += (filterTextUpdated.Trim() != string.Empty ? "; " : string.Empty)
                                             + "<" + fs_selected.FilterDate.Date.Date.ToShortDateString() + " " + fs_selected.FilterDate.Date.ToShortTimeString();
                    }
                }

                var attributes = string.Empty;
                if (fs_selected.FilterAttributeArchiveUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fs_selected.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                }
                if (fs_selected.FilterAttributeSystemUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fs_selected.FilterAttributeSystemType == "Included" ? "S" : "-S");
                }
                if (fs_selected.FilterAttributeHiddenUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fs_selected.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                }
                if (fs_selected.FilterAttributeReadOnlyUsed)
                {
                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                  + (fs_selected.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                }

                #endregion

                cbi.Text += "" + filterTextUpdated + (filterTextUpdated.Trim() != string.Empty ? "; " : string.Empty) + attributes;


                if (cbi.Text.Trim() == string.Empty) continue;
                index++;
                toolStripComboBox_fileFilters.Items.Add(cbi);

                if (fsSelected != null
                    && string.Compare(fs_selected.Name, fsSelected.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    selectedIndex = index;
                }
            }
            toolStripComboBox_fileFilters.SelectedIndex = selectedIndex;
        }


        private List<Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment> FileAlignments { get; set; }


        private IEnumerable<TextComparer.ComparisonTextUnit> GetComparisonUnits(string left, string right)
        {
            var leftSections = new List<SegmentSection>
            {
                new SegmentSection(SegmentSection.ContentType.Text, string.Empty, left)
            };
            var rightSections = new List<SegmentSection>
            {
                new SegmentSection(SegmentSection.ContentType.Text, string.Empty, right)
            };

            Processor.Settings.comparisonType = Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonType.Characters;

            var units = _textComparer.GetComparisonTextUnits(leftSections, rightSections);

            return units;
        }


        private class FileAlignmentCell
        {
            public Image CellImage { get; set; }
            public string CellImageToolTip { get; set; }
            public string left { get; set; }
            public string Match { get; set; }
            public string right { get; set; }


            public Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment FileAlignment { get; set; }
            public FileAlignmentCell()
            {
                CellImage = null;
                CellImageToolTip = string.Empty;
                left = string.Empty;
                Match = string.Empty;
                right = string.Empty;
                FileAlignment = new Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment();
            }
        }




        private void LoadFileLists()
        {

            var rtbLeft = new RichTextBox();
            var rtbRight = new RichTextBox();



            try
            {
                panel_listViewMessage.Visible = true;


                progressBar1.Value = 0;
                progressBar1.Maximum = 100;
                label_progress_percentage.Text = "0%";


                numericUpDown_fuzzyMatchFileNames_ValueChanged(null, null);

                IsLinkFileToActive = false;

                FacIsLinkFileToActive = null;


                Cursor = Cursors.WaitCursor;

                SetFilterList(toolStripButton_Activate_Filters.Checked);

                var facs = new List<FileAlignmentCell>();


                if (FileAlignments.Count == 0 || FuzzyMatchFileNameValueChanged)
                {
                    progressBar1.Value = 75;
                    progressBar1.Maximum = 100;
                    label_progress_percentage.Text = "75%";

                    FileAlignments = new List<Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment>();

                    GetFiles(ComparisonProject.PathLeft, true);
                    if (string.Compare(ComparisonProject.PathLeft, ComparisonProject.PathRight, StringComparison.OrdinalIgnoreCase) != 0)
                        GetFiles(ComparisonProject.PathRight, false);
                }

                FuzzyMatchFileNameValueChanged = false;

                dataGridView_fileAlignment.SuspendLayout();

                dataGridView_fileAlignment.DataSource = null;

                dataGridView_fileAlignment.Rows.Clear();
















                label_action_message.Text = Resources.ComparisonProjectFileAlignment_LoadFileLists_Loading_files;
                progressBar1.Value = 0;
                progressBar1.Maximum = FileAlignments.Count;
                label_progress_percentage.Text = "0%";
                Application.DoEvents();




                const string rtfStart = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil Microsoft Sans Serif;}{\\f1\\fnil\\fcharset0 Microsoft Sans Serif;}}\r\n{\\colortbl ;\\red0\\green0\\blue0;\\red128\\green128\\blue128;\\red0\\green0\\blue255;\\red255\\green0\\blue0;}\r\n\\viewkind4\\uc1\\pard\\cf1\\f0\\fs17 ";
                const string rtfEnd = "\\f1\\par\r\n}\r\n";

                const string rtfBlack = "\\cf0 ";
                const string rtfGray = "\\cf2 ";
                const string rtfBlueStart = "\\cf3\\ul ";
                const string rtfBlueClose = "\\ulnone";

                var rtf_red_start = "\\cf4\\strike ";
                var rtf_red_close = "\\cf0\\strike0";





                rtbLeft.SuspendLayout();
                rtbRight.SuspendLayout();


                decimal counterCurrent = 0;
                progressBar1.Maximum = FileAlignments.Count;
                decimal maximum = progressBar1.Maximum;

                foreach (var fa in FileAlignments)
                {
                    if (fa.PathLeft.Trim() == string.Empty || fa.PathRight.Trim() == string.Empty)
                        fa.MatchPercentage = 0;
                }

                foreach (var fa in FileAlignments)
                {
                    var addToFilter = true;

                    counterCurrent++;

                    if (counterCurrent % 10 == 0)
                    {
                        label_action_message.Text = Resources.ComparisonProjectFileAlignment_LoadFileLists_Loading_files;
                        progressBar1.Value = Convert.ToInt32(counterCurrent);


                        var percent = Math.Truncate((counterCurrent / maximum) * 100);

                        label_progress_percentage.Text = percent + "%";
                        Application.DoEvents();
                    }



                    if (toolStripButton_Activate_Filters.Checked && !AddToFilteredList(fa.PathLeft, fa.PathRight))
                        addToFilter = false;
                    else
                    {
                        var fac = new FileAlignmentCell();


                        var faFromProject = ComparisonProject.FileAlignment.FirstOrDefault(_fa => string.Compare(fa.PathLeft, _fa.PathLeft, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(fa.PathRight, _fa.PathRight, StringComparison.OrdinalIgnoreCase) == 0);

                        Image img;
                        string toolTipText;
                        if (faFromProject != null)
                        {
                            #region  |  get image  |
                            if (faFromProject.MatchPercentage == 100)
                            {
                                if (fa.IsFullMatch)
                                {
                                    img = imageList_main.Images["Link-Green"];
                                    img.Tag = "Link-Green";
                                    toolTipText = "fully linked file";
                                }
                                else
                                {
                                    img = imageList_main.Images["Link-Yellow"];
                                    img.Tag = "Link-Yellow";
                                    toolTipText = "manually linked file";
                                }
                            }
                            else
                            {

                                img = imageList_main.Images["Link-Yellow"];
                                img.Tag = "Link-Yellow";
                                toolTipText = "manually linked file";



                            }
                            fac.CellImage = img;
                            fac.CellImageToolTip = toolTipText;
                            #endregion
                        }
                        else
                        {
                            #region  |  get image  |


                            if (fa.MatchPercentage == 100)
                            {
                                if (fa.IsFullMatch)
                                {
                                    img = imageList_main.Images["Link-Green"];
                                    img.Tag = "Link-Green";
                                    toolTipText = "fully linked file";
                                }
                                else
                                {
                                    img = imageList_main.Images["Link-Yellow"];
                                    img.Tag = "Link-Yellow";
                                    toolTipText = "fuzzy matched file";
                                }
                            }
                            else if (fa.MatchPercentage > 75 && fa.MatchPercentage < 100)
                            {
                                img = imageList_main.Images["yellow"];
                                img.Tag = "yellow";
                                toolTipText = "fuzzy matched file";
                            }
                            else if (fa.MatchPercentage > 0 && fa.MatchPercentage < 100 && fa.MatchPercentage >= numericUpDown_fuzzyMatchFileNames.Value)
                            {
                                img = imageList_main.Images["red"];
                                img.Tag = "red";
                                toolTipText = "low fuzzy matched file";
                            }
                            else
                            {
                                img = imageList_main.Images["blue"];
                                img.Tag = "blue";
                                toolTipText = "no matching file";
                            }

                            fac.CellImage = img;
                            fac.CellImageToolTip = toolTipText;


                            #endregion
                        }

                        #region  |  set left  |

                        var pathLeft = string.Empty;
                        var fileNameLeft = string.Empty;
                        var fileExtensionLeft = string.Empty;
                        var fileNameNoExtensionLeft = string.Empty;

                        rtbLeft.Clear();
                        var leftRtf = rtfStart;


                        string fileNameLeftCompareA;
                        string fileNameLeftCompareB;

                        if (fa.PathLeft.Trim() != string.Empty && fa.PathLeft.Length > ComparisonProject.PathLeft.Length)
                        {
                            var fullPathLeft = fa.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1);
                            pathLeft = Path.GetDirectoryName(fullPathLeft);
                            fileNameLeft = Path.GetFileName(fullPathLeft);
                            fileExtensionLeft = Path.GetExtension(fullPathLeft);
                            fileNameNoExtensionLeft = fileNameLeft.Substring(0, fileNameLeft.Length - fileExtensionLeft.Length);


                            fileNameLeftCompareA = fileNameLeft;
                            fileNameLeftCompareB = string.Empty;
                            if (WordServerCharsExclude > 0)
                            {
                                if (fileNameNoExtensionLeft.Length > 0 && fileNameNoExtensionLeft.Length >= WordServerCharsExclude)
                                {
                                    fileNameLeftCompareA = fileNameNoExtensionLeft.Substring(0, fileNameNoExtensionLeft.Length - WordServerCharsExclude);
                                    fileNameLeftCompareB = fileNameNoExtensionLeft.Substring(fileNameNoExtensionLeft.Length - WordServerCharsExclude);
                                }
                                else
                                {
                                    fileNameLeftCompareA = string.Empty;
                                    fileNameLeftCompareB = fileNameNoExtensionLeft;
                                }
                            }


                            if (pathLeft != null && pathLeft.Length > 20)
                                fileNameLeftCompareA = "..." + pathLeft.Substring(pathLeft.Length - 20);
                            leftRtf += rtfGray;
                            leftRtf += ConvertToWordUni(ConvertToRtFstr(" " + pathLeft + (pathLeft.Trim() != string.Empty ? "\\" : string.Empty)));

                            leftRtf += rtfBlack;
                            leftRtf += ConvertToWordUni(ConvertToRtFstr(fileNameLeftCompareA));


                            if (fileNameLeftCompareB.Trim() != string.Empty)
                            {
                                leftRtf += rtfGray;
                                leftRtf += ConvertToWordUni(ConvertToRtFstr(fileNameLeftCompareB));
                            }
                            if (WordServerCharsExclude > 0 && fileExtensionLeft.Trim() != string.Empty)
                            {
                                leftRtf += rtfBlack;
                                leftRtf += ConvertToWordUni(ConvertToRtFstr(fileExtensionLeft));
                            }

                            leftRtf += rtfEnd;

                        }
                        else
                        {
                            fileNameLeftCompareA = fileNameLeft;
                            fileNameLeftCompareB = string.Empty;
                            if (WordServerCharsExclude > 0)
                            {
                                if (fileNameNoExtensionLeft.Length > 0 && fileNameNoExtensionLeft.Length >= WordServerCharsExclude)
                                {
                                    fileNameLeftCompareA = fileNameNoExtensionLeft.Substring(0, fileNameNoExtensionLeft.Length - WordServerCharsExclude);
                                    fileNameLeftCompareB = fileNameNoExtensionLeft.Substring(fileNameNoExtensionLeft.Length - WordServerCharsExclude);
                                }
                                else
                                {
                                    fileNameLeftCompareA = string.Empty;
                                    fileNameLeftCompareB = fileNameNoExtensionLeft;
                                }
                            }

                            leftRtf += rtfGray;
                            leftRtf += ConvertToWordUni(ConvertToRtFstr(" " + pathLeft + (pathLeft.Trim() != string.Empty ? "\\" : string.Empty)));


                            leftRtf += rtfBlack;
                            leftRtf += ConvertToWordUni(ConvertToRtFstr(fileNameLeftCompareA));

                            if (fileNameLeftCompareB.Trim() != string.Empty)
                            {
                                leftRtf += rtfGray;
                                leftRtf += ConvertToWordUni(ConvertToRtFstr(fileNameLeftCompareB));
                            }
                            if (WordServerCharsExclude > 0 && fileExtensionLeft.Trim() != string.Empty)
                            {
                                leftRtf += rtfBlack;
                                leftRtf += ConvertToWordUni(ConvertToRtFstr(fileExtensionLeft));
                            }
                            leftRtf += rtfEnd;

                        }



                        fac.left = leftRtf;

                        #endregion



                        fac.Match = Math.Round(fa.MatchPercentage, 2) + "%";


                        #region  |  set right  |

                        var pathRight = string.Empty;
                        var fileNameRight = string.Empty;

                        rtbRight.Clear();
                        var rightRtf = rtfStart;

                        if (fa.PathRight.Trim() != string.Empty && fa.PathRight.Length > ComparisonProject.PathRight.Length)
                        {
                            var fullPathRight = fa.PathRight.Substring(ComparisonProject.PathRight.Length + 1);
                            pathRight = Path.GetDirectoryName(fullPathRight);
                            fileNameRight = Path.GetFileName(fullPathRight);
                            var fileExtensionRight = Path.GetExtension(fullPathRight);
                            var fileNameNoExtensionRight = fileNameRight.Substring(0, fileNameRight.Length - fileExtensionRight.Length);

                            if (pathRight.Length > 20)
                                pathRight = "..." + pathRight.Substring(pathRight.Length - 20);

                            if (fa.PathLeft.Trim() != string.Empty)
                            {

                                rightRtf += rtfGray;
                                rightRtf += ConvertToWordUni(ConvertToRtFstr(" " + pathRight + (pathRight.Trim() != string.Empty ? "\\" : string.Empty)));




                                var fileNameRightCompareA = fileNameRight;
                                var fileNameRightCompareB = string.Empty;

                                if (WordServerCharsExclude > 0)
                                {

                                    if (fileNameNoExtensionRight.Length > 0 && fileNameNoExtensionRight.Length >= WordServerCharsExclude)
                                    {
                                        fileNameRightCompareA = fileNameNoExtensionRight.Substring(0, fileNameNoExtensionRight.Length - WordServerCharsExclude);
                                        fileNameRightCompareB = fileNameNoExtensionRight.Substring(fileNameNoExtensionRight.Length - WordServerCharsExclude);
                                    }
                                    else
                                    {
                                        fileNameRightCompareA = string.Empty;
                                        fileNameRightCompareB = fileNameNoExtensionRight;
                                    }
                                }

                                var comparisonUnits = GetComparisonUnits(fileNameLeftCompareA, fileNameRightCompareA);

                                #region  |  write path  compare  |
                                foreach (var comparisonTextUnit in comparisonUnits)
                                {
                                    if (comparisonTextUnit.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.Identical)
                                    {
                                        foreach (var xSegmentSection in comparisonTextUnit.TextSections)
                                        {
                                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                                            {

                                                rightRtf += rtfBlack;
                                                rightRtf += ConvertToWordUni(ConvertToRtFstr(xSegmentSection.Content));
                                            }
                                            else
                                            {
                                                rightRtf += rtfGray;
                                                rightRtf += ConvertToWordUni(ConvertToRtFstr(xSegmentSection.Content));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (SegmentSection xSegmentSection in comparisonTextUnit.TextSections)
                                        {
                                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                                            {
                                                if (comparisonTextUnit.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.New)
                                                {

                                                    rightRtf += rtfBlueStart;
                                                    rightRtf += ConvertToWordUni(ConvertToRtFstr(xSegmentSection.Content));
                                                    rightRtf += rtfBlueClose;
                                                }
                                                else
                                                {

                                                    rightRtf += rtf_red_start;
                                                    rightRtf += ConvertToWordUni(ConvertToRtFstr(xSegmentSection.Content));
                                                    rightRtf += rtf_red_close;
                                                }
                                            }
                                            else
                                            {
                                                if (comparisonTextUnit.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.New)
                                                {

                                                    rightRtf += rtfBlueStart;
                                                    rightRtf += ConvertToWordUni(ConvertToRtFstr(xSegmentSection.Content));
                                                    rightRtf += rtfBlueClose;
                                                }
                                                else
                                                {

                                                    rightRtf += rtf_red_start;
                                                    rightRtf += ConvertToWordUni(ConvertToRtFstr(xSegmentSection.Content));
                                                    rightRtf += rtf_red_close;
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                if (fileNameRightCompareB.Trim() != string.Empty)
                                {
                                    rightRtf += rtfGray;
                                    rightRtf += ConvertToWordUni(ConvertToRtFstr(fileNameRightCompareB));
                                }

                                if (WordServerCharsExclude > 0 && fileExtensionRight.Trim() != string.Empty)
                                {
                                    rightRtf += rtfBlack;
                                    rightRtf += ConvertToWordUni(ConvertToRtFstr(fileExtensionRight));
                                }

                                rightRtf += rtfEnd;
                            }
                            else
                            {


                                rightRtf += rtfGray;
                                rightRtf += ConvertToWordUni(ConvertToRtFstr(" " + pathRight + (pathRight.Trim() != string.Empty ? "\\" : string.Empty)));



                                var fileNameRightCompareA = fileNameRight;
                                var fileNameRightCompareB = string.Empty;

                                if (WordServerCharsExclude > 0)
                                {

                                    if (fileNameNoExtensionRight.Length > 0 && fileNameNoExtensionRight.Length >= WordServerCharsExclude)
                                    {
                                        fileNameRightCompareA = fileNameNoExtensionRight.Substring(0, fileNameNoExtensionRight.Length - WordServerCharsExclude);
                                        fileNameRightCompareB = fileNameNoExtensionRight.Substring(fileNameNoExtensionRight.Length - WordServerCharsExclude);
                                    }
                                    else
                                    {
                                        fileNameRightCompareA = string.Empty;
                                        fileNameRightCompareB = fileNameNoExtensionRight;
                                    }
                                }

                                rightRtf += rtfBlack;
                                rightRtf += ConvertToWordUni(ConvertToRtFstr(fileNameRightCompareA));

                                if (fileNameRightCompareB.Trim() != string.Empty)
                                {
                                    rightRtf += rtfGray;
                                    rightRtf += ConvertToWordUni(ConvertToRtFstr(fileNameRightCompareB));
                                }

                                if (WordServerCharsExclude > 0 && fileExtensionRight.Trim() != string.Empty)
                                {
                                    rightRtf += rtfBlack;
                                    rightRtf += ConvertToWordUni(ConvertToRtFstr(fileExtensionRight));
                                }

                                rightRtf += rtfEnd;
                            }
                        }
                        else
                        {

                            rightRtf += rtfGray;
                            rightRtf += ConvertToWordUni(ConvertToRtFstr(" " + pathRight + (pathRight.Trim() != string.Empty ? "\\" : string.Empty)));

                            rightRtf += rtfBlack;
                            rightRtf += ConvertToWordUni(ConvertToRtFstr(fileNameRight));


                            rightRtf += rtfEnd;

                        }
                        fac.right = rightRtf;
                        #endregion



                        if (fa.MatchPercentage != 100)
                        {
                            if (fileNameLeft.Trim() != string.Empty && fileNameRight.Trim() != string.Empty)
                            {
                                string nameLeftCompareA;

                                if (fileNameLeft.Length > 0 && fileNameLeft.Length >= WordServerCharsExclude)
                                {
                                    nameLeftCompareA = fileNameLeft.Substring(0, fileNameLeft.Length - WordServerCharsExclude);
                                }
                                else
                                {
                                    nameLeftCompareA = string.Empty;
                                }



                                string fileNameRightCompareA;

                                if (fileNameRight.Length > 0 && fileNameRight.Length >= WordServerCharsExclude)
                                {
                                    fileNameRightCompareA = fileNameRight.Substring(0, fileNameRight.Length - WordServerCharsExclude);
                                }
                                else
                                {
                                    fileNameRightCompareA = string.Empty;
                                }

                                decimal charsDistPer = Comparer.DamerauLevenshteinDistance(fileNameRightCompareA, nameLeftCompareA);
                                decimal charsTotal = fileNameRightCompareA.Length > nameLeftCompareA.Length ? fileNameRightCompareA.Length : nameLeftCompareA.Length;

                                var charsDistPerTmp = charsDistPer / charsTotal;
                                var totalDistPerRem = 1.0m - charsDistPerTmp;


                                fa.MatchPercentage = (totalDistPerRem * 100);
                                fac.Match = Math.Round(fa.MatchPercentage, 2).ToString(CultureInfo.InvariantCulture) + "%";

                            }
                        }



                        fac.FileAlignment = fa;


                        if (!toolStripButton_showExactFileMatches.Checked && fac.FileAlignment.MatchPercentage == 100)
                            addToFilter = false;
                        else if (!toolStripButton_showManualFileMatches.Checked && fac.CellImage.Tag.ToString() == "Link-Yellow")
                            addToFilter = false;

                        if (addToFilter)
                        {

                            facs.Add(fac);
                        }

                    }
                }


                rtbLeft.ResumeLayout();
                rtbRight.ResumeLayout();

                dataGridView_fileAlignment.ResumeLayout();
                dataGridView_fileAlignment.AutoGenerateColumns = false;
                dataGridView_fileAlignment.DataSource = facs;

                dataGridView_fileAlignment.ClearSelection();
            }
            catch (Exception ex)
            {
                panel_listViewMessage.Visible = false;
                Cursor = Cursors.Default;
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Cursor = Cursors.Default;

                panel_listViewMessage.Visible = false;


                rtbLeft.ResumeLayout();
                rtbRight.ResumeLayout();

                dataGridView_fileAlignment.ResumeLayout();
                SelectionChanged();

                Visible = true;
                Application.DoEvents();
            }


        }

        private void UdpateStatusCounter()
        {

            var fullLinks = 0;
            var fuzzyMatches = 0;
            var fuzzyLinks = 0;
            var noMatches = 0;
            var totalFilesLeft = 0;
            var totalFilesRight = 0;

            var fuzzyMatchesNotLinked = 0;

            foreach (DataGridViewRow dgvr in dataGridView_fileAlignment.Rows)
            {
                var fac = (FileAlignmentCell)dgvr.DataBoundItem;

                if (fac.FileAlignment.PathLeft.Trim() != string.Empty)
                {
                    totalFilesLeft++;
                }

                if (fac.FileAlignment.PathRight.Trim() != string.Empty)
                {
                    totalFilesRight++;
                }

                if (fac.CellImage.Tag.ToString() == "Link-Green")
                {
                    fullLinks++;
                }
                else if (fac.CellImage.Tag.ToString() == "Link-Yellow")
                {
                    fuzzyLinks++;
                }
                else if (fac.FileAlignment.MatchPercentage > 0
                    && fac.FileAlignment.PathLeft.Trim() != string.Empty
                    && fac.FileAlignment.PathRight.Trim() != string.Empty)
                {
                    fuzzyMatchesNotLinked++;
                }

                if (fac.FileAlignment.MatchPercentage != 100
                    && fac.FileAlignment.MatchPercentage > 0
                    && fac.FileAlignment.PathLeft.Trim() != string.Empty
                    && fac.FileAlignment.PathRight.Trim() != string.Empty)
                {
                    fuzzyMatches++;
                }
                else
                {
                    noMatches++;
                }
            }

            toolStripStatusLabel_totalFiles.Text = "left (" + totalFilesLeft + "); right (" + totalFilesRight + ")";
            toolStripStatusLabel_linkedFiles.Text = "full (" + fullLinks + "); manual (" + fuzzyLinks + ")";
            toolStripStatusLabel_unLinkedFiles.Text = "fuzzy matches (" + fuzzyMatches + "); no matches (" + noMatches + ")";

            label_full_match.Text = fullLinks.ToString();
            label_fuzzy_match.Text = fuzzyMatches.ToString();
            label_no_match.Text = noMatches.ToString();


            label_fully_linked.Text = fullLinks.ToString();
            label_manually_linked.Text = fuzzyLinks.ToString();

            toolStripButton_linkAllFuzzyMatches.Enabled = (fuzzyMatchesNotLinked > 0 ? true : false);
            toolStripButton_unLinkAllFuzzyMatches.Enabled = (fuzzyLinks > 0 ? true : false);

        }



        private void LinkAllFuzzyMatchedFiles()
        {
            foreach (DataGridViewRow dgvr in dataGridView_fileAlignment.Rows)
            {
                var fac = (FileAlignmentCell)dgvr.DataBoundItem;
                if (fac.CellImage.Tag.ToString() == "Link-Green" || fac.CellImage.Tag.ToString() == "Link-Yellow" ||
                    fac.FileAlignment.MatchPercentage <= 0 || fac.FileAlignment.PathLeft.Trim() == string.Empty ||
                    fac.FileAlignment.PathRight.Trim() == string.Empty) continue;
                var img = imageList_main.Images["green"];
                var toolTipText = string.Empty;


                img = imageList_main.Images["Link-Yellow"];
                img.Tag = "Link-Yellow";
                toolTipText = "manually linked file";

                fac.CellImage = img;
                fac.CellImageToolTip = toolTipText;


                bool found = ComparisonProject.FileAlignment.Any(fa => string.Compare(fa.PathLeft, fac.FileAlignment.PathLeft, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(fa.PathRight, fac.FileAlignment.PathRight, StringComparison.OrdinalIgnoreCase) == 0);
                if (!found)
                {
                    ComparisonProject.FileAlignment.Add(fac.FileAlignment);
                }
                dataGridView_fileAlignment.InvalidateRow(dgvr.Index);
            }

            SelectionChanged();
        }
        private void UnLinkAllFuzzyMatchedFiles()
        {
            foreach (DataGridViewRow dgvr in dataGridView_fileAlignment.Rows)
            {
                var fac = (FileAlignmentCell)dgvr.DataBoundItem;
                if (fac.CellImage.Tag.ToString() != "Link-Yellow") continue;
                foreach (var fa in ComparisonProject.FileAlignment)
                {
                    if (string.Compare(fa.PathLeft, fac.FileAlignment.PathLeft, StringComparison.OrdinalIgnoreCase) != 0 ||
                        string.Compare(fa.PathRight, fac.FileAlignment.PathRight, StringComparison.OrdinalIgnoreCase) != 0) continue;
                    ComparisonProject.FileAlignment.Remove(fa);
                    break;
                }



                var img = imageList_main.Images["green"];
                string toolTipText;


                img = imageList_main.Images["yellow"];
                img.Tag = "yellow";
                toolTipText = "fuzzy matched file";

                fac.CellImage = img;
                fac.CellImageToolTip = toolTipText;


                FuzzyMatchFileNameValueChanged = true;


                LoadFileLists();
            }

            SelectionChanged();
        }

        private bool AddToFilteredList(string nameLeft, string nameRight)
        {
            var success = true;


            var b1 = false;
            var b2 = false;
            var b3 = true;

            try
            {
                FileInfo fil = null;
                FileInfo fir = null;

                if (filters_useDateTime || (filters_filterAttributeArchiveUsed || filters_filterAttributeHiddenUsed
                     || filters_filterAttributeReadOnlyUsed || filters_filterAttributeSystemUsed))
                {
                    if (nameLeft.Trim() != string.Empty)
                        fil = new FileInfo(nameLeft);

                    if (nameRight.Trim() != string.Empty)
                        fir = new FileInfo(nameRight);
                }

                var NameLeft = Path.GetFileName(nameLeft);
                var NameRight = Path.GetFileName(nameRight);


                #region  |  filters_useDateTime  |

                if (filters_useDateTime)
                {
                    if (filters_dateTime_before)
                    {
                        if ((fil != null && fil.LastWriteTime <= filters_dateTime)
                            || (fir != null && fir.LastWriteTime <= filters_dateTime))
                        {
                            b1 = true;
                        }
                    }
                    else
                    {
                        if ((fil != null && fil.LastWriteTime >= filters_dateTime)
                            || (fir != null && fir.LastWriteTime >= filters_dateTime))
                        {
                            b1 = true;
                        }
                    }
                }
                else
                {
                    b1 = true;
                }
                #endregion


                if (b1)
                {
                    #region  |  check file extensions  |

                    if (_filtersExtensionsRegex.Count > 0)
                    {
                        foreach (var t in _filtersExtensionsRegex)
                        {

                            if (_filtersExtensionsInclude)
                            {
                                #region  |  include extensions  |
                                if (NameLeft.Trim() != string.Empty)
                                {
                                    //if (dn.NameLeft.ToLower().IndexOf(filter_clean) > -1)
                                    //{
                                    b2 = t.Match(NameLeft).Success;
                                    //}
                                }
                                if (!b2 && string.Compare(NameLeft, NameRight, StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    if (NameRight.Trim() != string.Empty)
                                    {
                                        //if (dn.NameRight.ToLower().IndexOf(filter_clean) > -1)
                                        //{
                                        b2 = t.Match(NameRight).Success;
                                        //}
                                    }
                                }
                                if (b2)
                                {
                                    break;
                                }
                                #endregion
                            }
                            else
                            {
                                #region  |  exclude extensions  |

                                if (NameLeft.Trim() != string.Empty)
                                {
                                    b2 = !t.Match(NameLeft).Success;

                                    //if (dn.NameLeft.ToLower().IndexOf(filter_clean) == -1)
                                    //{
                                    //    b2 = true;
                                    //}
                                }
                                if (!b2 && string.Compare(NameLeft, NameRight, true) != 0)
                                {
                                    if (NameRight.Trim() != string.Empty)
                                    {
                                        b2 = !t.Match(NameRight).Success;
                                        //if (dn.NameRight.ToLower().IndexOf(filter_clean) == -1)
                                        //{
                                        //    b2 = true;
                                        //}
                                    }
                                }
                                if (b2)
                                {
                                    break;
                                }
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        b2 = true;
                    }
                    #endregion


                }


                #region  |  check file properties  |

                if (b1 && b2
                    && (filters_filterAttributeArchiveUsed || filters_filterAttributeHiddenUsed
                     || filters_filterAttributeReadOnlyUsed || filters_filterAttributeSystemUsed))
                {

                    var propertiesArrayLeft = new List<string>();
                    var propertiesArrayRight = new List<string>();

                    if (fil != null && (fil.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                    {
                        propertiesArrayLeft.Add("A");
                    }
                    if (fil != null && (fil.Attributes & FileAttributes.System) == FileAttributes.System)
                    {
                        propertiesArrayLeft.Add("S");
                    }
                    if (fil != null && (fil.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        propertiesArrayLeft.Add("H");
                    }
                    if (fil != null && (fil.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        propertiesArrayLeft.Add("R");
                    }
                    if (fir != null && (fir.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                    {
                        propertiesArrayRight.Add("A");
                    }
                    if (fir != null && (fir.Attributes & FileAttributes.System) == FileAttributes.System)
                    {
                        propertiesArrayRight.Add("S");
                    }
                    if (fir != null && (fir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        propertiesArrayRight.Add("H");
                    }
                    if (fir != null && (fir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        propertiesArrayRight.Add("R");
                    }

                    if (b3 && filters_filterAttributeArchiveUsed)
                    {
                        if (filters_filterAttributeAchiveType == "Included")
                        {
                            if (!propertiesArrayLeft.Contains("A") && !propertiesArrayLeft.Contains("A"))
                            {
                                b3 = false;
                            }
                        }
                        else if (propertiesArrayLeft.Contains("A") && propertiesArrayLeft.Contains("A"))
                        {
                            b3 = false;
                        }
                    }
                    if (b3 && filters_filterAttributeHiddenUsed)
                    {
                        if (filters_filterAttributeHiddenType == "Included")
                        {
                            if (!propertiesArrayLeft.Contains("H") && !propertiesArrayLeft.Contains("H"))
                            {
                                b3 = false;
                            }
                        }
                        else if (propertiesArrayLeft.Contains("H") && propertiesArrayLeft.Contains("H"))
                        {
                            b3 = false;
                        }
                    }
                    if (b3 && filters_filterAttributeReadOnlyUsed)
                    {
                        if (filters_filterAttributeReadOnlyType == "Included")
                        {
                            if (!propertiesArrayLeft.Contains("R") && !propertiesArrayLeft.Contains("R"))
                            {
                                b3 = false;
                            }
                        }
                        else if (propertiesArrayLeft.Contains("R") && propertiesArrayLeft.Contains("R"))
                        {
                            b3 = false;
                        }
                    }
                    if (b3 && filters_filterAttributeSystemUsed)
                    {
                        if (filters_filterAttributeSystemType == "Included")
                        {
                            if (!propertiesArrayLeft.Contains("S") && !propertiesArrayLeft.Contains("S"))
                            {
                                b3 = false;
                            }
                        }
                        else if (propertiesArrayLeft.Contains("S") && propertiesArrayLeft.Contains("S"))
                        {
                            b3 = false;
                        }
                    }
                }
                #endregion

                if (b1 && b2 && b3)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            return success;
        }

        private void GetFiles(string directoryFullPath, bool isLeft)
        {

            var directories = Directory.GetDirectories(directoryFullPath);
            foreach (var directory in directories)
            {
                GetFiles(directory, isLeft);
            }

            var files = Directory.GetFiles(directoryFullPath);
            if (isLeft)
            {
                foreach (string file in files)
                {
                    Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa = new Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment();
                    fa.MatchPercentage = 0;
                    fa.PathLeft = file;

                    if (String.Compare(ComparisonProject.PathLeft, ComparisonProject.PathRight, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        fa.PathRight = file;
                        fa.MatchPercentage = 100;
                        fa.IsFullMatch = true;
                        fa.IsWorldServerFile = (WordServerCharsExclude > 0 ? true : false);
                    }
                    FileAlignments.Add(fa);
                }
            }
            else
            {
                if (string.Compare(ComparisonProject.PathLeft, ComparisonProject.PathRight, StringComparison.OrdinalIgnoreCase) == 0)
                    return;


                #region  |  foreach (string file in files)  |

                foreach (string file in files)
                {
                    string fullPath_right = file.Substring(ComparisonProject.PathRight.Length + 1);
                    string path_right = Path.GetDirectoryName(fullPath_right);
                    string fileName_right = Path.GetFileName(fullPath_right);
                    string fileExtension_right = Path.GetExtension(fullPath_right);
                    string fileNameNoExtension_right = fileName_right.Substring(0, fileName_right.Length - fileExtension_right.Length);

                    if (WordServerCharsExclude > 0)
                    {
                        if (fileNameNoExtension_right.Length > 0 && fileNameNoExtension_right.Length >= WordServerCharsExclude)
                            fileNameNoExtension_right = fileNameNoExtension_right.Substring(0, fileNameNoExtension_right.Length - WordServerCharsExclude);
                        else
                            fileNameNoExtension_right = string.Empty;
                    }
                    Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa = null;
                    Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa_100 = null;
                    Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa_fuzzy = null;
                    Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa_manual = null;
                    #region  |  foreach (Core.Settings.FileAlignment _fa in fileAlignments)  |

                    foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment _fa in FileAlignments)
                    {


                        if (_fa.PathLeft.Trim() != string.Empty)
                        {

                            string fullPath_left = _fa.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1);
                            string path_left = Path.GetDirectoryName(fullPath_left);
                            string fileName_left = Path.GetFileName(fullPath_left);
                            string fileExtension_left = Path.GetExtension(fullPath_left);
                            string fileNameNoExtension_left = fileName_left.Substring(0, fileName_left.Length - fileExtension_left.Length);


                            //here include the check for removing chars from the right
                            if (WordServerCharsExclude > 0)
                            {


                                if (fileNameNoExtension_left.Length > 0 && fileNameNoExtension_left.Length >= WordServerCharsExclude)
                                    fileNameNoExtension_left = fileNameNoExtension_left.Substring(0, fileNameNoExtension_left.Length - WordServerCharsExclude);
                                else
                                    fileNameNoExtension_left = string.Empty;
                            }


                            if (string.Compare(fileName_right, fileName_left, true) == 0)
                            {
                                fa_100 = (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment)_fa.Clone();
                                fa_100.MatchPercentage = 100;
                                fa_100.IsWorldServerFile = (WordServerCharsExclude > 0 ? true : false);
                                fa_100.IsFullMatch = true;

                                break;
                            }
                            if (_fa != null
                                && (_fa.MatchPercentage < 100 || !_fa.IsFullMatch && _fa.MatchPercentage == 100)
                                && string.Compare(path_right, path_left, true) == 0
                                && string.Compare(fileExtension_right, fileExtension_left, true) == 0)
                            {
                                decimal chars_dist_per = Comparer.DamerauLevenshteinDistance((fileNameNoExtension_right + fileExtension_right), (fileNameNoExtension_left + fileExtension_left));
                                decimal chars_total = (fileNameNoExtension_right + fileExtension_right).Length > (fileNameNoExtension_left + fileExtension_left).Length ? (fileNameNoExtension_right + fileExtension_right).Length : (fileNameNoExtension_left + fileExtension_left).Length;

                                decimal chars_dist_per_tmp = (chars_dist_per > 0 ? chars_dist_per / chars_total : 0);
                                decimal total_dist_per_rem = 1.0m - chars_dist_per_tmp;



                                if (fa_fuzzy != null)
                                {
                                    if ((total_dist_per_rem * 100) > fa_fuzzy.MatchPercentage)
                                    {
                                        fa_fuzzy.MatchPercentage = 0;
                                        fa_fuzzy = (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment)_fa.Clone();
                                        fa_fuzzy.MatchPercentage = (total_dist_per_rem * 100);
                                    }
                                }
                                else
                                {
                                    fa_fuzzy = (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment)_fa.Clone();
                                    fa_fuzzy.MatchPercentage = (total_dist_per_rem * 100);

                                }
                                fa_fuzzy.IsFullMatch = false;
                                fa_fuzzy.IsWorldServerFile = (WordServerCharsExclude > 0 ? true : false);



                                foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa_proj in ComparisonProject.FileAlignment)
                                {
                                    // if (string.Compare(fa_proj.pathLeft.Substring(comparisonProject.pathLeft.Length + 1), fullPath_left, true) == 0)
                                    //{
                                    if (fa_proj.PathLeft.Trim() != string.Empty && fa_proj.PathRight.Trim() != string.Empty && string.Compare(fa_proj.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1), fullPath_left, true) == 0)
                                    {
                                        if (string.Compare(fa_proj.PathRight.Substring(ComparisonProject.PathRight.Length + 1), fullPath_right, true) != 0)
                                            fa_fuzzy = null;
                                        break;
                                    }
                                }


                                foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa_proj in ComparisonProject.FileAlignment)
                                {
                                    //if (string.Compare(fa_proj.pathRight.Substring(comparisonProject.pathRight.Length + 1), fullPath_right, true) == 0)
                                    //{
                                    if (fa_proj.PathLeft.Trim() != string.Empty && fa_proj.PathRight.Trim() != string.Empty && string.Compare(fa_proj.PathRight.Substring(ComparisonProject.PathRight.Length + 1), fullPath_right, true) == 0)
                                    {
                                        fa_manual = fa_proj;
                                        _fa.MatchPercentage = fa_proj.MatchPercentage;
                                        fa_manual.MatchPercentage = fa_proj.MatchPercentage;
                                        fa_manual.IsFullMatch = fa_proj.IsFullMatch;
                                        fa_manual.IsWorldServerFile = fa_proj.IsWorldServerFile;
                                        break;
                                    }
                                }
                                //}
                            }
                        }
                    }
                    #endregion
                    if (fa_100 != null)
                        fa = fa_100;
                    else if (fa_manual != null)
                        fa = fa_manual;
                    else if (fa_fuzzy != null && fa_fuzzy.MatchPercentage >= FuzzyMatchFileNameValue)
                        fa = fa_fuzzy;


                    if (fa == null) continue;

                    foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment _fa in FileAlignments)
                    {
                        if (_fa.PathLeft.Trim() == string.Empty) continue;
                        if (string.Compare(_fa.PathLeft, fa.PathLeft, StringComparison.OrdinalIgnoreCase) != 0) continue;
                        _fa.PathRight = file;
                        _fa.MatchPercentage = fa.MatchPercentage;
                        _fa.IsFullMatch = fa.IsFullMatch;
                        _fa.IsWorldServerFile = fa.IsWorldServerFile;
                        break;
                    }

                }
                #endregion


                foreach (string file in files)
                {

                    var fullPathRight = Path.GetDirectoryName(file).Substring(ComparisonProject.PathRight.Length);


                    var found = false;
                    foreach (var fa in FileAlignments)
                    {
                        if (string.Compare(fa.PathRight.Trim(), file, true) == 0)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) continue;

                    int iAlignmentIndex = 0;
                    int iFileAlignmentsIndexInsert = -1;
                    foreach (var _fa in FileAlignments)
                    {

                        var fullPathLeft = string.Empty;

                        if (_fa.PathLeft.Trim() != string.Empty)
                        {
                            fullPathLeft = Path.GetDirectoryName(_fa.PathLeft).Substring(ComparisonProject.PathLeft.Length);

                        }

                        var _fullPath_right = string.Empty;

                        if (_fa.PathRight.Trim() != string.Empty)
                        {
                            _fullPath_right = Path.GetDirectoryName(_fa.PathRight).Substring(ComparisonProject.PathRight.Length);

                        }


                        if (String.Compare(fullPathLeft, fullPathRight, StringComparison.OrdinalIgnoreCase) == 0
                            || String.Compare(_fullPath_right, fullPathRight, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (fullPathRight.Trim() == string.Empty)
                            {
                                if (fullPathLeft.Trim() == string.Empty && _fullPath_right.Trim() == string.Empty)
                                {
                                    iFileAlignmentsIndexInsert = iAlignmentIndex;
                                    break;
                                }
                            }
                            else
                            {
                                iFileAlignmentsIndexInsert = iAlignmentIndex;
                                break;
                            }

                        }

                        iAlignmentIndex++;
                    }
                    Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa1 = new Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment
                    {
                        MatchPercentage = 0,
                        PathRight = file
                    };


                    if (iFileAlignmentsIndexInsert > -1)
                        FileAlignments.Insert(iFileAlignmentsIndexInsert, fa1);
                    else
                        FileAlignments.Add(fa1);

                }
            }

        }



        private void resize()
        {
            var totalWidth = (dataGridView_fileAlignment.Width - 30) - 65;

            var d1 = (totalWidth * 1.00M);

            var dataGridViewColumn = dataGridView_fileAlignment.Columns["left"];
            if (dataGridViewColumn != null)
                dataGridViewColumn.Width = Convert.ToInt32((int) (d1 / 2));
            var gridViewColumn = dataGridView_fileAlignment.Columns["right"];
            if (gridViewColumn != null)
                gridViewColumn.Width = Convert.ToInt32((int) (d1 / 2));
        }

        private void SelectionChanged()
        {
            if (dataGridView_fileAlignment.SelectedCells.Count > 0)
            {
                Dictionary<int, List<int>> rowIndexes = new Dictionary<int, List<int>>();
                foreach (DataGridViewCell dgvc in dataGridView_fileAlignment.SelectedCells)
                {
                    if (!rowIndexes.ContainsKey(dgvc.RowIndex))
                    {
                        List<Int32> columnIndexes = new List<Int32>();
                        columnIndexes.Add(dgvc.ColumnIndex);
                        rowIndexes.Add(dgvc.RowIndex, columnIndexes);
                    }
                    else
                    {
                        List<Int32> columnIndexes = rowIndexes[dgvc.RowIndex];
                        columnIndexes.Add(dgvc.ColumnIndex);
                        rowIndexes[dgvc.RowIndex] = columnIndexes;
                    }
                }
                if (rowIndexes.Count == 1)
                {
                    var rowIndex = -1;
                    var columns = new List<int>();
                    foreach (KeyValuePair<int, List<int>> kvp in rowIndexes)
                    {
                        rowIndex = kvp.Key;
                        columns = kvp.Value;
                        break;
                    }

                    FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[rowIndex].DataBoundItem;

                    switch (fac.CellImage.Tag.ToString())
                    {
                        case "Link-Green":
                            if (fac.FileAlignment.IsFullMatch)
                            {
                                #region  |  Link-Green  |

                                toolStripButton_link.Enabled = false;
                                toolStripButton_unlink.Enabled = false;

                                linkFileToToolStripMenuItem.Enabled = false;
                                unlinkFilesToolStripMenuItem.Enabled = false;

                                #endregion
                            }
                            else
                            {
                                #region  |  Link-Yellow  |

                                toolStripButton_link.Enabled = false;
                                toolStripButton_unlink.Enabled = true;

                                linkFileToToolStripMenuItem.Enabled = false;
                                unlinkFilesToolStripMenuItem.Enabled = true;

                                #endregion
                            }
                            break;
                        case "Link-Yellow":

                            #region  |  Link-Yellow  |

                            toolStripButton_link.Enabled = false;
                            toolStripButton_unlink.Enabled = true;

                            linkFileToToolStripMenuItem.Enabled = false;
                            unlinkFilesToolStripMenuItem.Enabled = true;
                            break;

                            #endregion

                        default:

                            #region  |  other  |
                            var column1 = false;
                            var column3 = false;
                            foreach (Int32 iColumn in columns)
                            {
                                switch (iColumn)
                                {
                                    case 1:
                                        column1 = true;
                                        break;
                                    case 3:
                                        column3 = true;
                                        break;
                                }
                            }

                            if (column1 && column3)
                            {
                                if (fac.FileAlignment.MatchPercentage > 0)
                                {
                                    toolStripButton_link.Enabled = true;
                                    toolStripButton_unlink.Enabled = false;

                                    linkFileToToolStripMenuItem.Enabled = true;
                                    unlinkFilesToolStripMenuItem.Enabled = false;
                                }
                                else
                                {
                                    toolStripButton_link.Enabled = false;
                                    toolStripButton_unlink.Enabled = false;

                                    linkFileToToolStripMenuItem.Enabled = false;
                                    unlinkFilesToolStripMenuItem.Enabled = false;
                                }
                            }
                            else if (column1 && !column3 && fac.FileAlignment.PathLeft.Trim() != string.Empty)
                            {
                                toolStripButton_link.Enabled = true;
                                toolStripButton_unlink.Enabled = false;

                                linkFileToToolStripMenuItem.Enabled = true;
                                unlinkFilesToolStripMenuItem.Enabled = false;
                            }
                            else if (!column1 && column3 && fac.FileAlignment.PathRight.Trim() != string.Empty)
                            {
                                toolStripButton_link.Enabled = true;
                                toolStripButton_unlink.Enabled = false;

                                linkFileToToolStripMenuItem.Enabled = true;
                                unlinkFilesToolStripMenuItem.Enabled = false;
                            }
                            else
                            {
                                toolStripButton_link.Enabled = false;
                                toolStripButton_unlink.Enabled = false;

                                linkFileToToolStripMenuItem.Enabled = false;
                                unlinkFilesToolStripMenuItem.Enabled = false;
                            }
                            break;

                            #endregion
                    }

                    label_fileInfo.Text = string.Empty;

                    FileInfo fiLeft = null;
                    FileInfo fiRight = null;

                    if (fac.FileAlignment.PathLeft.Trim() != string.Empty)
                    {
                        fiLeft = new FileInfo(fac.FileAlignment.PathLeft);
                        if (!fiLeft.Exists)
                            fiLeft = null;
                    }
                    if (fac.FileAlignment.PathRight.Trim() != string.Empty)
                    {
                        fiRight = new FileInfo(fac.FileAlignment.PathRight);
                        if (!fiRight.Exists)
                            fiRight = null;
                    }



                    string fileInfoText = string.Empty;
                    if (fiLeft != null)
                    {
                        fileInfoText =
                                                      "Left Path: " + fiLeft.FullName
                                                      + "\r\nModified: " + fiLeft.LastWriteTime
                                                      + "\r\nSize: " + CalculateFileSize(fiLeft.Length);
                    }
                    if (fiRight != null)
                    {
                        if (fileInfoText.Trim() != string.Empty)
                            fileInfoText += "\r\n\r\n";
                        fileInfoText +=
                                                      "Right Path: " + fiRight.FullName
                                                      + "\r\nModified: " + fiRight.LastWriteTime
                                                      + "\r\nSize: " + CalculateFileSize(fiRight.Length);
                    }

                    label_fileInfo.Text = fileInfoText;


                }
                else
                {
                    label_fileInfo.Text = string.Empty;

                    toolStripButton_link.Enabled = false;
                    toolStripButton_unlink.Enabled = false;

                    linkFileToToolStripMenuItem.Enabled = false;
                    unlinkFilesToolStripMenuItem.Enabled = false;
                }
            }

            UdpateStatusCounter();
        }

        private void LinkFiles()
        {


            if (dataGridView_fileAlignment.SelectedCells.Count > 0)
            {
                Dictionary<Int32, List<Int32>> rowIndexes = new Dictionary<int, List<int>>();
                foreach (DataGridViewCell dgvc in dataGridView_fileAlignment.SelectedCells)
                {
                    if (!rowIndexes.ContainsKey(dgvc.RowIndex))
                    {
                        List<Int32> columnIndexes = new List<Int32>();
                        columnIndexes.Add(dgvc.ColumnIndex);
                        rowIndexes.Add(dgvc.RowIndex, columnIndexes);
                    }
                    else
                    {
                        List<Int32> columnIndexes = rowIndexes[dgvc.RowIndex];
                        columnIndexes.Add(dgvc.ColumnIndex);
                        rowIndexes[dgvc.RowIndex] = columnIndexes;
                    }
                }
                if (rowIndexes.Count == 1)
                {

                    Int32 rowIndex = -1;
                    List<Int32> columns = new List<int>();
                    foreach (KeyValuePair<Int32, List<Int32>> kvp in rowIndexes)
                    {
                        rowIndex = kvp.Key;
                        columns = kvp.Value;
                        break;
                    }

                    FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[rowIndex].DataBoundItem;

                    if (fac.CellImage.Tag.ToString() == "Link-Green")
                    {
                        #region  |  Link-Green  |

                        //ignore

                        #endregion
                    }
                    else if (fac.CellImage.Tag.ToString() == "Link-Yellow")
                    {
                        #region  |  Link-Yellow  |

                        //ignore

                        #endregion
                    }
                    else
                    {
                        #region  |  other  |
                        var column1 = false;
                        var column3 = false;
                        foreach (Int32 iColumn in columns)
                        {
                            switch (iColumn)
                            {
                                case 1:
                                    column1 = true;
                                    break;
                                case 3:
                                    column3 = true;
                                    break;
                            }
                        }

                        if (column1 && column3)
                        {
                            if (fac.FileAlignment.MatchPercentage > 0)
                            {
                                Image img = imageList_main.Images["green"];
                                string toolTipText = string.Empty;


                                img = imageList_main.Images["Link-Yellow"];
                                img.Tag = "Link-Yellow";
                                toolTipText = "manually linked file";

                                fac.CellImage = img;
                                fac.CellImageToolTip = toolTipText;


                                var found = ComparisonProject.FileAlignment.Any(fa => String.Compare(fa.PathLeft, fac.FileAlignment.PathLeft, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(fa.PathRight, fac.FileAlignment.PathRight, StringComparison.OrdinalIgnoreCase) == 0);
                                if (!found)
                                {
                                    ComparisonProject.FileAlignment.Add(fac.FileAlignment);
                                }
                                dataGridView_fileAlignment.InvalidateRow(rowIndex);


                            }
                        }
                        else if (column1 && fac.FileAlignment.PathLeft.Trim() != string.Empty)
                        {
                           
                            FacIsLinkFileToActive = fac;
                            FacIsLinkFileToActiveRowIndex = rowIndex;



                            IsLinkFileToActive = true;
                            IsLinkFiletoActiveLeft = false;

                            Cursor = Cursors.Help;

              
                        }
                        else if (!column1 && column3 && fac.FileAlignment.PathRight.Trim() != string.Empty)
                        {
                      

                            FacIsLinkFileToActive = fac;
                            FacIsLinkFileToActiveRowIndex = rowIndex;

                            IsLinkFileToActive = true;
                            IsLinkFiletoActiveLeft = true;


                            Cursor = Cursors.Help;
                   
                        }

                        #endregion
                    }
                }
                SelectionChanged();
            }



        }


        private void UnLinkFiles()
        {
            if (dataGridView_fileAlignment.SelectedCells.Count > 0)
            {
                Dictionary<Int32, List<Int32>> rowIndexes = new Dictionary<int, List<int>>();
                foreach (DataGridViewCell dgvc in dataGridView_fileAlignment.SelectedCells)
                {
                    if (!rowIndexes.ContainsKey(dgvc.RowIndex))
                    {
                        List<Int32> columnIndexes = new List<Int32>();
                        columnIndexes.Add(dgvc.ColumnIndex);
                        rowIndexes.Add(dgvc.RowIndex, columnIndexes);
                    }
                    else
                    {
                        List<Int32> columnIndexes = rowIndexes[dgvc.RowIndex];
                        columnIndexes.Add(dgvc.ColumnIndex);
                        rowIndexes[dgvc.RowIndex] = columnIndexes;
                    }
                }
                if (rowIndexes.Count == 1)
                {

                    Int32 rowIndex = -1;
                    List<Int32> columns = new List<int>();
                    foreach (KeyValuePair<Int32, List<Int32>> kvp in rowIndexes)
                    {
                        rowIndex = kvp.Key;
                        columns = kvp.Value;
                        break;
                    }

                    FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[rowIndex].DataBoundItem;

                    if (fac.CellImage.Tag.ToString() == "Link-Green")
                    {
                        #region  |  Link-Green  |

                        //ignore

                        #endregion
                    }
                    else if (fac.CellImage.Tag.ToString() == "Link-Yellow")
                    {
                        #region  |  Link-Yellow  |



                        foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fa in ComparisonProject.FileAlignment)
                        {
                            if (string.Compare(fa.PathLeft, fac.FileAlignment.PathLeft, true) == 0
                                && string.Compare(fa.PathRight, fac.FileAlignment.PathRight, true) == 0)
                            {
                                ComparisonProject.FileAlignment.Remove(fa);
                                break;
                            }
                        }



                        Image img = imageList_main.Images["green"];
                        string toolTipText = string.Empty;


                        img = imageList_main.Images["yellow"];
                        img.Tag = "yellow";
                        toolTipText = "fuzzy matched file";

                        fac.CellImage = img;
                        fac.CellImageToolTip = toolTipText;


                        FuzzyMatchFileNameValueChanged = true;

                        
                        SelectionChanged();

                        #endregion
                    }
                    LoadFileLists();
                }
                SelectionChanged();
            }
        }



        private void ComparisonProjectFileAlignment_Resize(object sender, EventArgs e)
        {
            resize();
        }


        public static string CalculateFileSize(long numBytes)
        {
            string fileSize = "";

            if (numBytes > 1073741824)
                fileSize = String.Format("{0:0.00} Gb", (double)numBytes / 1073741824);
            else if (numBytes > 1048576)
                fileSize = String.Format("{0:0.00} Mb", (double)numBytes / 1048576);
            else
                fileSize = String.Format("{0:0} Kb", (double)numBytes / 1024);


            if (fileSize == "0 Kb")
                if (numBytes > 0)
                    fileSize = "1 Kb";	// min.							
                else
                    fileSize = "0 kb";
            return fileSize;
        }

        private void dataGridView_fileAlignment_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                if (e.RowIndex > -1)
                {
                    FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[e.RowIndex].DataBoundItem;
                    if (fac.CellImage.Tag.ToString() == "Link-Green")
                    {
                        if (fac.FileAlignment.IsFullMatch)
                            e.CellStyle.BackColor = Color.LightGreen;
                        else
                            e.CellStyle.BackColor = Color.LightGoldenrodYellow;
                    }
                    else if (fac.CellImage.Tag.ToString() == "Link-Yellow")
                    {
                        if (fac.FileAlignment.MatchPercentage > 0 && fac.FileAlignment.MatchPercentage >= numericUpDown_fuzzyMatchFileNames.Value)
                        {
                            e.CellStyle.BackColor = Color.LightGoldenrodYellow;
                        }
                        else
                        {
                            e.CellStyle.BackColor = Color.Pink;
                        }
                    }
                    else if (fac.FileAlignment.MatchPercentage > 0 &&
                             fac.FileAlignment.MatchPercentage >= numericUpDown_fuzzyMatchFileNames.Value)
                    {
                        e.CellStyle.BackColor = Color.Yellow;
                    }
                    else
                    {
                        dataGridView_fileAlignment[2, e.RowIndex].Value = "-";
                        e.CellStyle.BackColor = Color.LightBlue;
                    }
                    dataGridView_fileAlignment[2, e.RowIndex].ToolTipText = fac.CellImageToolTip;
                }
            }
        }




        private readonly List<Regex> _filtersExtensionsRegex = new List<Regex>();
        private bool _filtersExtensionsInclude = true;


        private bool filters_useDateTime;
        private DateTime filters_dateTime = Common.DateNull;
        private bool filters_dateTime_before = true;

        private string filters_filterAttributeAchiveType = string.Empty;
        private bool filters_filterAttributeArchiveUsed;
        private string filters_filterAttributeHiddenType = string.Empty;
        private bool filters_filterAttributeHiddenUsed;
        private string filters_filterAttributeReadOnlyType = string.Empty;
        private bool filters_filterAttributeReadOnlyUsed;
        private string filters_filterAttributeSystemType = string.Empty;
        private bool filters_filterAttributeSystemUsed;

        private void SetFilterList(bool enabled)
        {

            _filtersExtensionsRegex.Clear();


            filters_useDateTime = false;
            filters_dateTime = Common.DateNull;
            filters_dateTime_before = true;

            filters_filterAttributeAchiveType = string.Empty;
            filters_filterAttributeArchiveUsed = false;
            filters_filterAttributeHiddenType = string.Empty;
            filters_filterAttributeHiddenUsed = false;
            filters_filterAttributeReadOnlyType = string.Empty;
            filters_filterAttributeReadOnlyUsed = false;
            filters_filterAttributeSystemType = string.Empty;
            filters_filterAttributeSystemUsed = false;

            if (enabled)
            {
                if (toolStripComboBox_fileFilters.Text.Trim() != string.Empty)
                {
                    Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting fs_selected = new Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting();
                    if (toolStripComboBox_fileFilters.Items.Count > 0)
                    {
                        fs_selected = (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting)((ComboboxItem)toolStripComboBox_fileFilters.SelectedItem).Value;

                        #region  |  fs_selected.filterNamesInclude  |

                        _filtersExtensionsInclude = (fs_selected.FilterNamesInclude.Count > 0 ? true : false);

                        List<string> filters_extensions_ = (_filtersExtensionsInclude ? fs_selected.FilterNamesInclude : fs_selected.FilterNamesExclude);


                        foreach (string fs_name in filters_extensions_)
                        {
                            if (fs_name.Trim() != string.Empty)
                            {
                                bool foundFilterInList = false;

                                string filter_str = fs_name;

                                if (!foundFilterInList)
                                {

                                    if (filter_str.StartsWith("*"))
                                        filter_str = "^" + filter_str;
                                    _filtersExtensionsRegex.Add(new Regex(filter_str, RegexOptions.IgnoreCase | RegexOptions.Compiled));
                                }
                            }
                        }
                        #endregion



                        #region  |  fs_selected.filterDate  |

                        if (fs_selected.FilterDateUsed)
                        {
                            filters_useDateTime = true;
                            filters_dateTime = fs_selected.FilterDate.Date;
                            filters_dateTime_before = fs_selected.FilterDate.Type == Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.GreaterThan ? true : false;
                        }
                        else
                        {
                            filters_useDateTime = false;
                            filters_dateTime = Common.DateNull;
                            filters_dateTime_before = true;
                        }
                        #endregion



                        #region  |  fs_selected.properties  |

                        filters_filterAttributeAchiveType = fs_selected.FilterAttributeAchiveType;
                        filters_filterAttributeArchiveUsed = fs_selected.FilterAttributeArchiveUsed;

                        filters_filterAttributeHiddenType = fs_selected.FilterAttributeHiddenType;
                        filters_filterAttributeHiddenUsed = fs_selected.FilterAttributeHiddenUsed;

                        filters_filterAttributeReadOnlyType = fs_selected.FilterAttributeReadOnlyType;
                        filters_filterAttributeReadOnlyUsed = fs_selected.FilterAttributeReadOnlyUsed;

                        filters_filterAttributeSystemType = fs_selected.FilterAttributeSystemType;
                        filters_filterAttributeSystemUsed = fs_selected.FilterAttributeSystemUsed;

                        #endregion
                    }
                }
            }
        }




        private void toolStripButton_Activate_Filters_Click(object sender, EventArgs e)
        {

            try
            {


                if (toolStripButton_Activate_Filters.Checked)
                    toolStripButton_Activate_Filters.Image = imageList1.Images["Filter_Feather_Enabled"];
                else
                    toolStripButton_Activate_Filters.Image = imageList1.Images["Filter_Feather_Disabled"];



                CheckActivateFiltersTooltip();
                LoadFileLists();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }


        }
        private void toolStripButton_filter_settings_edit_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox_fileFilters.SelectedItem != null)
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting fs_selected = new Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting();
                if (toolStripComboBox_fileFilters.Items.Count > 0)
                {
                    ComboboxItem cbi = (ComboboxItem)toolStripComboBox_fileFilters.SelectedItem;

                    fs_selected = (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting)cbi.Value;


                    FilterAppend f = new FilterAppend();
                    f.FilterSetting = (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting)fs_selected.Clone();

                    f.IsEdit = true;
                    f.ShowDialog();
                    if (f.Saved)
                    {

                        fs_selected.Name = f.FilterSetting.Name;
                        fs_selected.FilterNamesInclude = f.FilterSetting.FilterNamesInclude;
                        fs_selected.FilterNamesExclude = f.FilterSetting.FilterNamesExclude;
                        fs_selected.UseRegularExpressionMatching = f.FilterSetting.UseRegularExpressionMatching;
                        fs_selected.FilterDateUsed = f.FilterSetting.FilterDateUsed;
                        fs_selected.FilterDate.Date = f.FilterSetting.FilterDate.Date;
                        fs_selected.FilterDate.Type = f.FilterSetting.FilterDate.Type;
                        fs_selected.IsDefault = f.FilterSetting.IsDefault;
                        fs_selected.FilterAttributeAchiveType = f.FilterSetting.FilterAttributeAchiveType;
                        fs_selected.FilterAttributeArchiveUsed = f.FilterSetting.FilterAttributeArchiveUsed;
                        fs_selected.FilterAttributeHiddenType = f.FilterSetting.FilterAttributeHiddenType;
                        fs_selected.FilterAttributeHiddenUsed = f.FilterSetting.FilterAttributeHiddenUsed;
                        fs_selected.FilterAttributeReadOnlyType = f.FilterSetting.FilterAttributeReadOnlyType;
                        fs_selected.FilterAttributeReadOnlyUsed = f.FilterSetting.FilterAttributeReadOnlyUsed;
                        fs_selected.FilterAttributeSystemType = f.FilterSetting.FilterAttributeSystemType;
                        fs_selected.FilterAttributeSystemUsed = f.FilterSetting.FilterAttributeSystemUsed;

                        cbi.Value = fs_selected;


                        if (Cache.Application.Settings != null)
                        {
                            SettingsSerializer.SaveSettings(Cache.Application.Settings);
                        }


                        #region  |  filter setttings  |


                        toolStripComboBox_fileFilters.Items.Clear();
                        Int32 index = -1;
                        Int32 selectedIndex = 0;
                        foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting fs in Cache.Application.Settings.FilterSettings)
                        {
                            if (fs.Name.Trim() != string.Empty)
                            {

                                cbi = new ComboboxItem();
                                cbi.Text = string.Empty;
                                cbi.Value = fs;

                                string filterText = string.Empty;
                                if (fs.FilterNamesInclude.Count > 0)
                                {
                                    foreach (string str in fs.FilterNamesInclude)
                                    {
                                        filterText += (filterText != string.Empty ? ";" : string.Empty) + str;
                                    }
                                }
                                if (fs.FilterNamesExclude.Count > 0)
                                {
                                    foreach (string str in fs.FilterNamesExclude)
                                    {
                                        filterText += (filterText != string.Empty ? ";" : string.Empty) + "-" + str;
                                    }
                                }
                                if (fs.FilterDateUsed)
                                {
                                    if (fs.FilterDate.Type == Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.GreaterThan)
                                    {
                                        filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                            + ">" + fs.FilterDate.Date.Date.ToShortDateString() + " " + fs.FilterDate.Date.ToShortTimeString();
                                    }
                                    else
                                    {
                                        filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                            + "<" + fs.FilterDate.Date.Date.ToShortDateString() + " " + fs.FilterDate.Date.ToShortTimeString();
                                    }
                                }

                                string attributes = string.Empty;
                                if (fs.FilterAttributeArchiveUsed)
                                {
                                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                        + (fs.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                                }
                                if (fs.FilterAttributeSystemUsed)
                                {
                                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                        + (fs.FilterAttributeSystemType == "Included" ? "S" : "-S");
                                }
                                if (fs.FilterAttributeHiddenUsed)
                                {
                                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                        + (fs.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                                }
                                if (fs.FilterAttributeReadOnlyUsed)
                                {
                                    attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                                        + (fs.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                                }


                                cbi.Text += "" + filterText + (filterText.Trim() != string.Empty ? "; " : string.Empty) + attributes;

                                if (cbi.Text.Trim() != string.Empty)
                                {
                                    index++;
                                    toolStripComboBox_fileFilters.Items.Add(cbi);

                                    if (string.Compare(fs.Id, fs_selected.Id, true) == 0)
                                    {
                                        selectedIndex = index;
                                    }
                                }
                            }
                        }

                        if (toolStripComboBox_fileFilters.Items.Count > 0)
                            toolStripComboBox_fileFilters.SelectedIndex = selectedIndex;


                        #endregion

                        //loadFileLists();

                    }
                }
            }
            else
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting fs_selected = new Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting();

                ComboboxItem cbi = new ComboboxItem();

                cbi.Value = fs_selected;


                FilterAppend f = new FilterAppend();
                f.FilterSetting = (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting)fs_selected.Clone();

                f.IsEdit = false;
                f.ShowDialog();
                if (f.Saved)
                {
                    fs_selected.Name = f.FilterSetting.Name;
                    fs_selected.FilterNamesInclude = f.FilterSetting.FilterNamesInclude;
                    fs_selected.FilterNamesExclude = f.FilterSetting.FilterNamesExclude;
                    fs_selected.UseRegularExpressionMatching = f.FilterSetting.UseRegularExpressionMatching;
                    fs_selected.FilterDateUsed = f.FilterSetting.FilterDateUsed;
                    fs_selected.FilterDate.Date = f.FilterSetting.FilterDate.Date;
                    fs_selected.FilterDate.Type = f.FilterSetting.FilterDate.Type;
                    fs_selected.IsDefault = f.FilterSetting.IsDefault;
                    fs_selected.FilterAttributeAchiveType = f.FilterSetting.FilterAttributeAchiveType;
                    fs_selected.FilterAttributeArchiveUsed = f.FilterSetting.FilterAttributeArchiveUsed;
                    fs_selected.FilterAttributeHiddenType = f.FilterSetting.FilterAttributeHiddenType;
                    fs_selected.FilterAttributeHiddenUsed = f.FilterSetting.FilterAttributeHiddenUsed;
                    fs_selected.FilterAttributeReadOnlyType = f.FilterSetting.FilterAttributeReadOnlyType;
                    fs_selected.FilterAttributeReadOnlyUsed = f.FilterSetting.FilterAttributeReadOnlyUsed;
                    fs_selected.FilterAttributeSystemType = f.FilterSetting.FilterAttributeSystemType;
                    fs_selected.FilterAttributeSystemUsed = f.FilterSetting.FilterAttributeSystemUsed;

                    cbi.Text = string.Empty;
                    cbi.Value = fs_selected;

                    string filterText = string.Empty;
                    if (fs_selected.FilterNamesInclude.Count > 0)
                    {
                        foreach (string str in fs_selected.FilterNamesInclude)
                        {
                            filterText += (filterText != string.Empty ? ";" : string.Empty) + str;
                        }
                    }
                    else if (fs_selected.FilterNamesExclude.Count > 0)
                    {
                        foreach (string str in fs_selected.FilterNamesExclude)
                        {
                            filterText += (filterText != string.Empty ? ";" : string.Empty) + "-" + str;
                        }
                        filterText = "-" + filterText;
                    }



                    if (fs_selected.FilterDateUsed)
                    {
                        if (fs_selected.FilterDate.Type == Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.GreaterThan)
                        {
                            filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                + ">" + fs_selected.FilterDate.Date.Date.ToShortDateString() + " " + fs_selected.FilterDate.Date.ToShortTimeString();
                        }
                        else
                        {
                            filterText += (filterText.Trim() != string.Empty ? "; " : string.Empty)
                                + "<" + fs_selected.FilterDate.Date.Date.ToShortDateString() + " " + fs_selected.FilterDate.Date.ToShortTimeString();
                        }
                    }

                    string attributes = string.Empty;
                    if (fs_selected.FilterAttributeArchiveUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (fs_selected.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                    }
                    if (fs_selected.FilterAttributeSystemUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (fs_selected.FilterAttributeSystemType == "Included" ? "S" : "-S");
                    }
                    if (fs_selected.FilterAttributeHiddenUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (fs_selected.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                    }
                    if (fs_selected.FilterAttributeReadOnlyUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (fs_selected.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                    }




                    cbi.Text += "" + filterText + (filterText.Trim() != string.Empty ? "; " : string.Empty) + attributes;

                    toolStripComboBox_fileFilters.Items.Add(cbi);

                    Cache.Application.Settings.FilterSettings.Add(fs_selected);

                    toolStripComboBox_fileFilters.SelectedItem = cbi;

                    toolStripComboBox_fileFilters.Text = cbi.Text;


                    if (Cache.Application.Settings != null)
                    {
                        SettingsSerializer.SaveSettings(Cache.Application.Settings);
                    }

                    LoadFileLists();
                }
            }

        }


        private void toolStripComboBox_fileFilters_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Return)
            //{

            //}
        }

        private void toolStripButton_showExactFileMatches_Click(object sender, EventArgs e)
        {
            CheckShowExactlyMatchedFilesTooltip();
            LoadFileLists();
        }

        private void toolStripButton_refresh_Click(object sender, EventArgs e)
        {
            LoadFileLists();
        }

        private void numericUpDown_fuzzyMatchFileNames_ValueChanged(object sender, EventArgs e)
        {
            if (FileAlignments != null && FileAlignments.Count > 0)
            {
                if (FuzzyMatchFileNameValue != numericUpDown_fuzzyMatchFileNames.Value)
                {
                    FuzzyMatchFileNameValue = numericUpDown_fuzzyMatchFileNames.Value;
                    FuzzyMatchFileNameValueChanged = true;
                }
            }

        }
        private void numericUpDown_ignore_chars_right_ValueChanged(object sender, EventArgs e)
        {
            if (FileAlignments != null && FileAlignments.Count > 0)
            {

                FuzzyMatchFileNameValueChanged = true;

            }
        }

        private void dataGridView_fileAlignment_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged();
        }

        private void toolStripButton_link_Click(object sender, EventArgs e)
        {
            LinkFiles();
        }

        private void toolStripButton_unlink_Click(object sender, EventArgs e)
        {
            UnLinkFiles();
        }

        private void dataGridView_fileAlignment_MouseHover(object sender, EventArgs e)
        {


            //mouseHover(sender, e);
        }

        private void dataGridView_fileAlignment_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Cursor = Cursors.Default;



            if (IsLinkFileToActive)
            {



                if (e.RowIndex > -1)
                {


                    //this.dataGridView_fileAlignment.Columns["left"].DefaultCellStyle.BackColor = Color.White;
                    //this.dataGridView_fileAlignment.Columns["right"].DefaultCellStyle.BackColor = Color.White;
                    if (IsLinkFiletoActiveLeft)
                    {
                        //RichTextBox rtb_01 = new RichTextBox();
                        //rtb_01.Rtf = fac_isLinkFileToActive.right;
                        //rtb_01.Font = new Font(this.dataGridView_fileAlignment.Font.FontFamily.Name, this.dataGridView_fileAlignment.Font.Size, FontStyle.Regular);
                        //fac_isLinkFileToActive.right = rtb_01.Rtf;
                        //this.dataGridView_fileAlignment.InvalidateCell(3, fac_isLinkFileToActiveRowIndex);


                        FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[e.RowIndex].DataBoundItem;

                        Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fac_update_01 = null;
                        foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment _fa in FileAlignments)
                        {
                            if (string.Compare(FacIsLinkFileToActive.FileAlignment.PathLeft, _fa.PathLeft, true) == 0
                                && string.Compare(FacIsLinkFileToActive.FileAlignment.PathRight, _fa.PathRight, true) == 0)
                            {
                                fac_update_01 = _fa;
                                break;
                            }
                        }

                        Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment fac_update_02 = null;
                        foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment _fa in FileAlignments)
                        {
                            if (string.Compare(fac.FileAlignment.PathLeft, _fa.PathLeft, true) == 0
                                && string.Compare(fac.FileAlignment.PathRight, _fa.PathRight, true) == 0)
                            {
                                fac_update_02 = _fa;
                                break;
                            }
                        }


                        if (fac.FileAlignment.PathLeft.Trim() != string.Empty)
                        {
                            if (fac.CellImage.Tag.ToString() != "Link-Green" && fac.CellImage.Tag.ToString() != "Link-Yellow")
                            {
                                string fullPath_right_01 = FacIsLinkFileToActive.FileAlignment.PathRight.Substring(ComparisonProject.PathRight.Length + 1);
                                string path_right_01 = Path.GetDirectoryName(fullPath_right_01);
                                string fileExtension_right_01 = Path.GetExtension(fullPath_right_01);

                                string fullPath_left_02 = fac.FileAlignment.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1);
                                string path_left_02 = Path.GetDirectoryName(fullPath_left_02);
                                string fileExtension_left_02 = Path.GetExtension(fullPath_left_02);

                                if (string.Compare(path_right_01, path_left_02, true) == 0
                                    && string.Compare(fileExtension_right_01, fileExtension_left_02, true) == 0)
                                {
                                    RichTextBox rtb = new RichTextBox();
                                    rtb.Rtf = fac.left;
                                    rtb.Font = new Font(dataGridView_fileAlignment.Font.FontFamily.Name, dataGridView_fileAlignment.Font.Size, FontStyle.Regular);
                                    fac.left = rtb.Rtf;

                                    var pathToCopy = FacIsLinkFileToActive.FileAlignment.PathLeft;
                                    var pathToCopyRtf = FacIsLinkFileToActive.left;

                                    if (FacIsLinkFileToActive != null)
                                    {
                                        FacIsLinkFileToActive.left = fac.left;
                                        FacIsLinkFileToActive.FileAlignment.PathLeft = fac.FileAlignment.PathLeft;




                                        var img = imageList_main.Images["green"];
                                        var toolTipText = string.Empty;


                                        img = imageList_main.Images["Link-Yellow"];
                                        img.Tag = "Link-Yellow";
                                        toolTipText = "manually linked file";

                                        FacIsLinkFileToActive.CellImage = img;
                                        FacIsLinkFileToActive.CellImageToolTip = toolTipText;


                                        var found = ComparisonProject.FileAlignment.Any(fa => string.Compare(fa.PathLeft, FacIsLinkFileToActive.FileAlignment.PathLeft, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(fa.PathRight, FacIsLinkFileToActive.FileAlignment.PathRight, StringComparison.OrdinalIgnoreCase) == 0);
                                        if (!found)
                                        {
                                            ComparisonProject.FileAlignment.Add(FacIsLinkFileToActive.FileAlignment);
                                        }
                                        //this.dataGridView_fileAlignment.InvalidateRow(fac_isLinkFileToActiveRowIndex);


                                        if (fac_update_01 != null)
                                        {
                                            fac_update_01.PathLeft = FacIsLinkFileToActive.FileAlignment.PathLeft;
                                            fac_update_01.PathRight = FacIsLinkFileToActive.FileAlignment.PathRight;
                                        }


                                        dataGridView_fileAlignment.InvalidateRow(FacIsLinkFileToActiveRowIndex);

                                    }

                                    //if (pathToCopy.Trim() != string.Empty)
                                    //{
                                    fac.FileAlignment.PathLeft = pathToCopy;
                                    fac.left = pathToCopyRtf;
                                    if (fac.FileAlignment.PathLeft.Trim() == string.Empty
                                        || fac.FileAlignment.PathRight.Trim() == string.Empty)
                                    {
                                        fac.FileAlignment.MatchPercentage = 0;
                                        fac.Match = Math.Round(fac.FileAlignment.MatchPercentage, 2).ToString(CultureInfo.InvariantCulture) + "%";

                                        var img_01 = imageList_main.Images["blue"];
                                        var toolTipText_01 = string.Empty;

                                        img_01 = imageList_main.Images["blue"];
                                        img_01.Tag = "blue";
                                        toolTipText_01 = "no matching file";

                                        fac.CellImage = img_01;
                                        fac.CellImageToolTip = toolTipText_01;


                                        fac_update_02.PathLeft = fac.FileAlignment.PathLeft;
                                        fac_update_02.PathRight = fac.FileAlignment.PathRight;

                                        dataGridView_fileAlignment.InvalidateRow(e.RowIndex);
                                    }
                                    //
                                    //}
                                    //else
                                    //{

                                    if (fac.FileAlignment.PathLeft.Trim() == string.Empty && fac.FileAlignment.PathRight.Trim() == string.Empty)
                                    {
                                        foreach (var fa in FileAlignments)
                                        {
                                            if (string.Compare(fa.PathLeft, fac.FileAlignment.PathLeft, StringComparison.OrdinalIgnoreCase) != 0 ||
                                                string.Compare(fa.PathRight, fac.FileAlignment.PathRight, StringComparison.OrdinalIgnoreCase) != 0)
                                                continue;
                                            FileAlignments.Remove(fa);
                                            break;
                                        }
                                    }

                                    //}
                                    LoadFileLists();
                                    //this.dataGridView_fileAlignment.Rows[fac_isLinkFileToActiveRowIndex].Cells[1].Selected = true;
                                    //this.dataGridView_fileAlignment.Rows[fac_isLinkFileToActiveRowIndex].Cells[3].Selected = true;

                                }
                            }
                        }
                    }
                    else if (!IsLinkFiletoActiveLeft)
                    {
                        //RichTextBox rtb_01 = new RichTextBox();
                        //rtb_01.Rtf = fac_isLinkFileToActive.left;
                        //rtb_01.Font = new Font(this.dataGridView_fileAlignment.Font.FontFamily.Name, this.dataGridView_fileAlignment.Font.Size, FontStyle.Regular);
                        //fac_isLinkFileToActive.left = rtb_01.Rtf;
                        //this.dataGridView_fileAlignment.InvalidateCell(1, fac_isLinkFileToActiveRowIndex);


                        FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[e.RowIndex].DataBoundItem;


                        Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment facUpdate01 = FileAlignments.FirstOrDefault(_fa => string.Compare(FacIsLinkFileToActive.FileAlignment.PathLeft, _fa.PathLeft, true) == 0 && string.Compare(FacIsLinkFileToActive.FileAlignment.PathRight, _fa.PathRight, true) == 0);

                        Sdl.Community.PostEdit.Compare.Core.Settings.FileAlignment facUpdate02 = FileAlignments.FirstOrDefault(_fa => string.Compare(fac.FileAlignment.PathLeft, _fa.PathLeft, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(fac.FileAlignment.PathRight, _fa.PathRight, StringComparison.OrdinalIgnoreCase) == 0);


                        if (fac.FileAlignment.PathRight.Trim() != string.Empty)
                        {
                            if (fac.CellImage.Tag.ToString() != "Link-Green" && fac.CellImage.Tag.ToString() != "Link-Yellow")
                            {
                                var fullPathLeft01 = FacIsLinkFileToActive.FileAlignment.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1);
                                var pathLeft01 = Path.GetDirectoryName(fullPathLeft01);
                                var fileExtensionLeft01 = Path.GetExtension(fullPathLeft01);

                                var fullPathRight02 = fac.FileAlignment.PathRight.Substring(ComparisonProject.PathRight.Length + 1);
                                var pathRight02 = Path.GetDirectoryName(fullPathRight02);
                                var fileExtensionRight02 = Path.GetExtension(fullPathRight02);

                                if (string.Compare(pathLeft01, pathRight02, StringComparison.OrdinalIgnoreCase) == 0
                                    && string.Compare(fileExtensionLeft01, fileExtensionRight02, StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    RichTextBox rtb = new RichTextBox();
                                    rtb.Rtf = fac.right;
                                    rtb.Font = new Font(dataGridView_fileAlignment.Font.FontFamily.Name, dataGridView_fileAlignment.Font.Size, FontStyle.Regular);
                                    fac.right = rtb.Rtf;

                                    var pathToCopy = FacIsLinkFileToActive.FileAlignment.PathRight;
                                    var pathToCopyRtf = FacIsLinkFileToActive.right;

                                    if (FacIsLinkFileToActive != null)
                                    {
                                        FacIsLinkFileToActive.right = fac.right;
                                        FacIsLinkFileToActive.FileAlignment.PathRight = fac.FileAlignment.PathRight;



                                        var img = imageList_main.Images["green"];
                                        var toolTipText = string.Empty;


                                        img = imageList_main.Images["Link-Yellow"];
                                        img.Tag = "Link-Yellow";
                                        toolTipText = "manually linked file";

                                        FacIsLinkFileToActive.CellImage = img;
                                        FacIsLinkFileToActive.CellImageToolTip = toolTipText;


                                        bool found = ComparisonProject.FileAlignment.Any(fa => String.Compare(fa.PathLeft, FacIsLinkFileToActive.FileAlignment.PathLeft, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(fa.PathRight, FacIsLinkFileToActive.FileAlignment.PathRight, StringComparison.OrdinalIgnoreCase) == 0);
                                        if (!found)
                                        {
                                            ComparisonProject.FileAlignment.Add(FacIsLinkFileToActive.FileAlignment);
                                        }

                                        facUpdate01.PathLeft = FacIsLinkFileToActive.FileAlignment.PathLeft;
                                        facUpdate01.PathRight = FacIsLinkFileToActive.FileAlignment.PathRight;

                                        dataGridView_fileAlignment.InvalidateRow(FacIsLinkFileToActiveRowIndex);

                                    }


                                    //if (pathToCopy.Trim() != string.Empty)
                                    //{
                                    fac.FileAlignment.PathRight = pathToCopy;
                                    fac.right = pathToCopyRtf;


                                    if (fac.FileAlignment.PathLeft.Trim() == string.Empty
                                        || fac.FileAlignment.PathRight.Trim() == string.Empty)
                                    {
                                        fac.FileAlignment.MatchPercentage = 0;
                                        fac.Match = Math.Round(fac.FileAlignment.MatchPercentage, 2).ToString(CultureInfo.InvariantCulture) + "%";

                                        var img_01 = imageList_main.Images["blue"];
                                        var toolTipText_01 = string.Empty;

                                        img_01 = imageList_main.Images["blue"];
                                        img_01.Tag = "blue";
                                        toolTipText_01 = "no matching file";

                                        fac.CellImage = img_01;
                                        fac.CellImageToolTip = toolTipText_01;


                                        facUpdate02.PathLeft = fac.FileAlignment.PathLeft;
                                        facUpdate02.PathRight = fac.FileAlignment.PathRight;

                                        dataGridView_fileAlignment.InvalidateRow(e.RowIndex);
                                    }

                                    //this.dataGridView_fileAlignment.InvalidateRow(e.RowIndex);
                                    //}
                                    //else
                                    //{
                                    if (fac.FileAlignment.PathLeft.Trim() == string.Empty && fac.FileAlignment.PathRight.Trim() == string.Empty)
                                    {
                                        foreach (var fa in FileAlignments)
                                        {
                                            if (
                                                String.Compare(fa.PathLeft, fac.FileAlignment.PathLeft,
                                                    StringComparison.OrdinalIgnoreCase) != 0 ||
                                                String.Compare(fa.PathRight, fac.FileAlignment.PathRight,
                                                    StringComparison.OrdinalIgnoreCase) != 0) continue;
                                            FileAlignments.Remove(fa);
                                            break;
                                        }
                                    }

                                    //}
                                    LoadFileLists();
                                    //if (this.dataGridView_fileAlignment.Rows.Count <= fac_isLinkFileToActiveRowIndex)
                                    //    fac_isLinkFileToActiveRowIndex--;
                                    //this.dataGridView_fileAlignment.Rows[fac_isLinkFileToActiveRowIndex].Cells[1].Selected = true;
                                    //this.dataGridView_fileAlignment.Rows[fac_isLinkFileToActiveRowIndex].Cells[3].Selected = true;


                                }

                            }
                        }
                    }
                }
                SelectionChanged();

            }
            IsLinkFileToActive = false;
            //isLinkFiletoActiveLeft = false;

            FacIsLinkFileToActive = null;
            FacIsLinkFileToActiveRowIndex = -1;


        }



        private void dataGridView_fileAlignment_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (IsLinkFileToActive)
            {
                if (e.RowIndex > -1)
                {
                    if (IsLinkFiletoActiveLeft && e.ColumnIndex == 1)
                    {
                        FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[e.RowIndex].DataBoundItem;

                        if (fac.FileAlignment.PathLeft.Trim() != string.Empty)
                        {

                            if (fac.CellImage.Tag.ToString() != "Link-Green" && fac.CellImage.Tag.ToString() != "Link-Yellow")
                            {
                                var fullPathRight01 = FacIsLinkFileToActive.FileAlignment.PathRight.Substring(ComparisonProject.PathRight.Length + 1);
                                var pathRight01 = Path.GetDirectoryName(fullPathRight01);
                                var fileExtensionRight01 = Path.GetExtension(fullPathRight01);

                                var fullPathLeft02 = fac.FileAlignment.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1);
                                var pathLeft02 = Path.GetDirectoryName(fullPathLeft02);
                                var fileExtensionLeft02 = Path.GetExtension(fullPathLeft02);

                                if (string.Compare(pathRight01, pathLeft02, StringComparison.OrdinalIgnoreCase) != 0 ||
                                    string.Compare(fileExtensionRight01, fileExtensionLeft02, StringComparison.OrdinalIgnoreCase) != 0) return;
                                Cursor = Cursors.Hand;
                                var rtb = new RichTextBox
                                {
                                    Rtf = fac.left,
                                    Font =
                                        new Font(dataGridView_fileAlignment.Font.FontFamily.Name,
                                            dataGridView_fileAlignment.Font.Size, FontStyle.Bold)
                                };
                                fac.left = rtb.Rtf;
                                dataGridView_fileAlignment.InvalidateCell(e.ColumnIndex, e.RowIndex);
                            }
                        }
                    }
                    else if (!IsLinkFiletoActiveLeft && e.ColumnIndex == 3)
                    {
                        var fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[e.RowIndex].DataBoundItem;

                        if (fac.FileAlignment.PathRight.Trim() != string.Empty)
                        {
                            if (fac.CellImage.Tag.ToString() != "Link-Green" && fac.CellImage.Tag.ToString() != "Link-Yellow")
                            {
                                string fullPath_left_01 = FacIsLinkFileToActive.FileAlignment.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1);
                                string path_left_01 = Path.GetDirectoryName(fullPath_left_01);
                                string fileExtension_left_01 = Path.GetExtension(fullPath_left_01);

                                string fullPath_right_02 = fac.FileAlignment.PathRight.Substring(ComparisonProject.PathRight.Length + 1);
                                string path_right_02 = Path.GetDirectoryName(fullPath_right_02);
                                string fileExtension_right_02 = Path.GetExtension(fullPath_right_02);

                                if (string.Compare(path_left_01, path_right_02, true) == 0
                                    && string.Compare(fileExtension_left_01, fileExtension_right_02, true) == 0)
                                {

                                    Cursor = Cursors.Hand;
                                    RichTextBox rtb = new RichTextBox();
                                    rtb.Rtf = fac.right;
                                    rtb.Font = new Font(dataGridView_fileAlignment.Font.FontFamily.Name, dataGridView_fileAlignment.Font.Size, FontStyle.Bold);
                                    fac.right = rtb.Rtf;
                                    dataGridView_fileAlignment.InvalidateCell(e.ColumnIndex, e.RowIndex);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void dataGridView_fileAlignment_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (IsLinkFileToActive)
            {
                if (e.RowIndex > -1)
                {
                    if (IsLinkFiletoActiveLeft && e.ColumnIndex == 1)
                    {
                        FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[e.RowIndex].DataBoundItem;

                        if (fac.FileAlignment.PathLeft.Trim() != string.Empty)
                        {
                            if (fac.CellImage.Tag.ToString() != "Link-Green" && fac.CellImage.Tag.ToString() != "Link-Yellow")
                            {
                                string fullPath_right_01 = FacIsLinkFileToActive.FileAlignment.PathRight.Substring(ComparisonProject.PathRight.Length + 1);
                                string path_right_01 = Path.GetDirectoryName(fullPath_right_01);
                                string fileExtension_right_01 = Path.GetExtension(fullPath_right_01);

                                string fullPath_left_02 = fac.FileAlignment.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1);
                                string path_left_02 = Path.GetDirectoryName(fullPath_left_02);
                                string fileExtension_left_02 = Path.GetExtension(fullPath_left_02);

                                if (string.Compare(path_right_01, path_left_02, true) == 0
                                    && string.Compare(fileExtension_right_01, fileExtension_left_02, true) == 0)
                                {
                                    Cursor = Cursors.Help;
                                    RichTextBox rtb = new RichTextBox();
                                    rtb.Rtf = fac.left;
                                    rtb.Font = new Font(dataGridView_fileAlignment.Font.FontFamily.Name, dataGridView_fileAlignment.Font.Size, FontStyle.Regular);
                                    fac.left = rtb.Rtf;
                                    dataGridView_fileAlignment.InvalidateCell(e.ColumnIndex, e.RowIndex);
                                }
                            }
                        }
                    }
                    else if (!IsLinkFiletoActiveLeft && e.ColumnIndex == 3)
                    {
                        FileAlignmentCell fac = (FileAlignmentCell)dataGridView_fileAlignment.Rows[e.RowIndex].DataBoundItem;
                        if (fac.FileAlignment.PathRight.Trim() != string.Empty)
                        {
                            if (fac.CellImage.Tag.ToString() != "Link-Green" && fac.CellImage.Tag.ToString() != "Link-Yellow")
                            {
                                string fullPath_left_01 = FacIsLinkFileToActive.FileAlignment.PathLeft.Substring(ComparisonProject.PathLeft.Length + 1);
                                string path_left_01 = Path.GetDirectoryName(fullPath_left_01);
                                string fileExtension_left_01 = Path.GetExtension(fullPath_left_01);

                                string fullPath_right_02 = fac.FileAlignment.PathRight.Substring(ComparisonProject.PathRight.Length + 1);
                                string path_right_02 = Path.GetDirectoryName(fullPath_right_02);
                                string fileExtension_right_02 = Path.GetExtension(fullPath_right_02);

                                if (string.Compare(path_left_01, path_right_02, true) == 0
                                    && string.Compare(fileExtension_left_01, fileExtension_right_02, true) == 0)
                                {

                                    Cursor = Cursors.Help;
                                    RichTextBox rtb = new RichTextBox();
                                    rtb.Rtf = fac.right;
                                    rtb.Font = new Font(dataGridView_fileAlignment.Font.FontFamily.Name, dataGridView_fileAlignment.Font.Size, FontStyle.Regular);
                                    fac.right = rtb.Rtf;
                                    dataGridView_fileAlignment.InvalidateCell(e.ColumnIndex, e.RowIndex);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void linkFileToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFiles();
        }

        private void unlinkFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnLinkFiles();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            SelectionChanged();

            //DataGridView grid = (DataGridView)sender;
            //Point clientPos = grid.PointToClient(Control.MousePosition);

            //DataGridView.HitTestInfo hitTest = grid.HitTest(clientPos.X, clientPos.Y);
            //if (hitTest.Type == DataGridViewHitTestType.Cell)
            //{
            //    DataGridViewCell cell = (DataGridViewCell)grid[hitTest.ColumnIndex, hitTest.RowIndex];


            //    int cellRow = cell.RowIndex;
            //    int cellColumn = cell.ColumnIndex;

            //    if (isLinkFiletoActiveLeft && cellColumn == 1)
            //    {
            //        int i = 0;
            //    }
            //    else if (!isLinkFiletoActiveLeft && cellColumn == 3)
            //    {
            //        int i = 0;
            //    }
            //}
        }

        private void toolStripButton_showManualFileMatches_Click(object sender, EventArgs e)
        {
            CheckShowManuallyMatchedFilesTooltip();
            LoadFileLists();
        }

        private void toolStripButton_linkAllFuzzyMatches_Click(object sender, EventArgs e)
        {
            LinkAllFuzzyMatchedFiles();
        }

        private void toolStripButton_unLinkAllFuzzyMatches_Click(object sender, EventArgs e)
        {
            UnLinkAllFuzzyMatchedFiles();
        }

        private void dataGridView_fileAlignment_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsLinkFileToActive)
            {

                Cursor = Cursors.Default;
                IsLinkFileToActive = false;

                FacIsLinkFileToActive = null;
                FacIsLinkFileToActiveRowIndex = -1;

            }
        }

        private void dataGridView_fileAlignment_Resize(object sender, EventArgs e)
        {
            panel_listViewMessage.Left = (dataGridView_fileAlignment.Width / 2) - 150;
        }

        private void toolStripComboBox_fileFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                LoadFileLists();
        }

        private void toolStripComboBox_fileFilters_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void checkBox_worldServerFiles_CheckedChanged(object sender, EventArgs e)
        {
            FuzzyMatchFileNameValueChanged = true;

            if (checkBox_worldServerFiles.Checked)
                WordServerCharsExclude = 19;
            else
                WordServerCharsExclude = 0;

            if (!IsLoading)
                LoadFileLists();
        }








    }
}
