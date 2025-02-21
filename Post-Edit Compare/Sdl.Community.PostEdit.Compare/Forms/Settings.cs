using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Globalization;


namespace PostEdit.Compare.Forms
{
    public partial class Settings : Form
    {

        public bool Saved { get; set; }

        private TreeNode SelectedTreeNode { get; set; }

        public List<Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup> RateGroups { get; set; }

        public List<Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject> ComparisonProjects { get; set; }

        public List<Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting> FilterSettings { get; set; }

        public bool ClearFolderComparisonHistory { get; set; }
        public bool ClearComparisonLogHistory { get; set; }


        public Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting StyleNewText { get; set; }
        public Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting StyleRemovedText { get; set; }
        public Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting StyleNewTag  { get; set; }
        public Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting StyleRemovedTag { get; set; }

        public Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonType ComparisonType { get; set; }
        public bool IncludeTagContentInComparison { get; set; }
        public decimal PemDifferentFormattingPenalty { get; set; }

        private ListViewSortManager.ListViewSortManager _mSortMgr;
        public Settings()
        {
            InitializeComponent();


            _mSortMgr = new ListViewSortManager.ListViewSortManager(
                  this.listView_price_groups,
                  new Type[] {  
                        typeof(ListViewSortManager.ListViewTextSort),
						typeof(ListViewSortManager.ListViewTextSort),
                        typeof(ListViewSortManager.ListViewDoubleSort),
						typeof(ListViewSortManager.ListViewTextSort),
                        typeof(ListViewSortManager.ListViewDoubleSort),
						typeof(ListViewSortManager.ListViewDoubleSort),
                        typeof(ListViewSortManager.ListViewDoubleSort),
						typeof(ListViewSortManager.ListViewDoubleSort),
                        typeof(ListViewSortManager.ListViewDoubleSort),
						typeof(ListViewSortManager.ListViewDoubleSort),
                        typeof(ListViewSortManager.ListViewDoubleSort),
						typeof(ListViewSortManager.ListViewDoubleSort),
						typeof(ListViewSortManager.ListViewDoubleSort)
					},
                  0,
                  SortOrder.Ascending
                  );

            Saved = false;

            this.panel_general.Dock = DockStyle.Fill;
            this.panel_filters.Dock = DockStyle.Fill;
            this.panel_events_log.Dock = DockStyle.Fill;
            this.panel_folder_viewer.Dock = DockStyle.Fill;
            this.panel_comparision_projects.Dock = DockStyle.Fill;
            this.panel_report_viewer.Dock = DockStyle.Fill;
            this.panel_reports.Dock = DockStyle.Fill;
            this.panel_fonts.Dock = DockStyle.Fill;
            this.panel_price_groups.Dock = DockStyle.Fill;
            this.panel_comparer.Dock = DockStyle.Fill;
            this.panel_viewers.Dock = DockStyle.Fill;

            this.treeView_main.SelectedNode = this.treeView_main.Nodes[0];
        }


        public void InitializeObject()
        {


            #region  |  rate groups  |
            this.treeView_price_groups.Nodes.Clear();

            foreach (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup in RateGroups)
            {
                TreeNode tn = this.treeView_price_groups.Nodes.Add(priceGroup.Name);
                tn.Tag = priceGroup;
            }

            if (RateGroups.Count > 0)
                this.treeView_price_groups.SelectedNode = this.treeView_price_groups.Nodes[0];
            #endregion


            #region  |  comparison projects  |
            this.listView_comparison_projects.Items.Clear();
            foreach (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject in ComparisonProjects)
            {
                ListViewItem itmx = this.listView_comparison_projects.Items.Add(comparisonProject.Name);
                itmx.SubItems.Add(comparisonProject.Created);
                itmx.SubItems.Add(comparisonProject.PathLeft);
                itmx.SubItems.Add(comparisonProject.PathRight);

                itmx.Tag = comparisonProject;


                bool isPathNotFoundError = false;
                if (!Directory.Exists(comparisonProject.PathLeft))
                {
                    itmx.SubItems[2].BackColor = Color.Pink;
                    isPathNotFoundError = true;
                    itmx.ToolTipText = "The left directory path does not exist!";
                }              
                else if (!Directory.Exists(comparisonProject.PathLeft))
                {
                    itmx.SubItems[3].BackColor = Color.Pink;
                    isPathNotFoundError = true;
                    itmx.ToolTipText = "The right directory path does not exist!";
                }
                
                itmx.ImageIndex = (isPathNotFoundError ? 4 : 3);

            }
            #endregion


            #region  |  filter settings  |

            this.listView_filters.Items.Clear();
            foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting filterSetting in FilterSettings)
            {
                if (filterSetting.Name.Trim() != string.Empty)
                {
                    ListViewItem itmx = this.listView_filters.Items.Add(filterSetting.Name,0);



                    if (filterSetting.FilterNamesInclude.Count > 0)
                    {
                        string namesInclude = string.Empty;
                        foreach (string str in filterSetting.FilterNamesInclude)
                        {
                            if (str.Trim() != string.Empty)
                                namesInclude += (namesInclude.Trim() != string.Empty ? "; " : string.Empty) + str;
                        }
                        itmx.SubItems.Add(namesInclude);
                    }
                    else if (filterSetting.FilterNamesExclude.Count > 0)
                    {
                        string namesExclude = string.Empty;
                        foreach (string str in filterSetting.FilterNamesExclude)
                        {
                            if (str.Trim() != string.Empty)
                                namesExclude += (namesExclude.Trim() != string.Empty ? "; " : string.Empty) + str;
                        }
                        itmx.SubItems.Add("-" + namesExclude);
                    }

                    string filterDateStr = string.Empty;
                    if (filterSetting.FilterDateUsed)
                    {
                        if (filterSetting.FilterDate.Type == Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.LessThan)
                            filterDateStr += "before ";
                        else
                            filterDateStr += "after ";

                        filterDateStr += filterSetting.FilterDate.Date.ToString();
                    }
                    itmx.SubItems.Add(filterDateStr);

                    string attributes = string.Empty;
                    if (filterSetting.FilterAttributeArchiveUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (filterSetting.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                    }
                    if (filterSetting.FilterAttributeSystemUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (filterSetting.FilterAttributeSystemType == "Included" ? "S" : "-S");
                    }
                    if (filterSetting.FilterAttributeHiddenUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (filterSetting.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                    }
                    if (filterSetting.FilterAttributeReadOnlyUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (filterSetting.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                    }
                  

                    itmx.SubItems.Add(attributes);

                    itmx.Tag = filterSetting;
                }
            }
            #endregion


            #region  |  comparer settings  |


            this.comboBox_comparisonType.SelectedIndex = (ComparisonType == Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonType.Words ? 0 : 1);
            this.checkBox_includeTagsInComparison.Checked = IncludeTagContentInComparison;
            
            UpdateVisualStyle();

            #endregion
        }




        #region  |  comparer  |

        private void UpdateVisualStyle()
        {
            UpdateVisualTextFormatting(StyleNewText, this.richTextBox_formatting_Text_New, "This is an example of inserted text formatting.", 31, 4);
            UpdateVisualTextFormatting(StyleRemovedText, this.richTextBox_formatting_Text_Removed, "This is an example of deleted text formatting.", 30, 4);
            UpdateVisualTextFormatting(StyleNewTag, this.richTextBox_formatting_Tag_New, "This is an example of inserted <tag/> formatting.", 31, 6);
            UpdateVisualTextFormatting(StyleRemovedTag, this.richTextBox_formatting_Tag_Removed, "This is an example of deleted <tag/> formatting.", 30, 6);
        }

        private void UpdateVisualTextFormatting(Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting differencesFormatting, RichTextBox richTextBox, string text, int selectionStart, int selectionLength)
        {
            richTextBox.Text = text;

            bool isBold = (string.Compare(differencesFormatting.StyleBold, "Activate", true) == 0 ? true : false);
            bool isItalic = (string.Compare(differencesFormatting.StyleItalic, "Activate", true) == 0 ? true : false);
            bool isStrikethrough = (string.Compare(differencesFormatting.StyleStrikethrough, "Activate", true) == 0 ? true : false);
            bool isUnderline = (string.Compare(differencesFormatting.StyleUnderline, "Activate", true) == 0 ? true : false);
            string strPosition = differencesFormatting.TextPosition;

            System.Drawing.FontStyle FontStyle_new = getFontStyle(isBold, isItalic, isStrikethrough, isUnderline);

            richTextBox.Select(selectionStart, selectionLength);
            richTextBox.SelectionFont = new Font(richTextBox.Font.Name, richTextBox.Font.Size, FontStyle_new);

            if (differencesFormatting.FontSpecifyColor)
                richTextBox.SelectionColor = ColorTranslator.FromHtml(differencesFormatting.FontColor);
            else
                richTextBox.SelectionColor = richTextBox.ForeColor;

            if (differencesFormatting.FontSpecifyBackroundColor)
                richTextBox.SelectionBackColor = ColorTranslator.FromHtml(differencesFormatting.FontBackroundColor);
            else
                richTextBox.SelectionBackColor = richTextBox.BackColor;

            switch (strPosition)
            {
                case "Normal": richTextBox.SelectionCharOffset = 0; break;
                case "Superscript": richTextBox.SelectionCharOffset = 5; break;
                case "Subscript": richTextBox.SelectionCharOffset = -5; break;
            }


        }

        private FontStyle getFontStyle(bool Bold, bool Italic, bool Strikethrough, bool Underline)
        {
            FontStyle FontStyle = FontStyle.Regular;
            if (Bold)
            {
                FontStyle = FontStyle.Bold;
            }
            if (Italic)
            {
                if (FontStyle != FontStyle.Regular)
                    FontStyle = FontStyle | FontStyle.Italic;
                else
                    FontStyle = FontStyle.Italic;
            }
            if (Strikethrough)
            {
                if (FontStyle != FontStyle.Regular)
                    FontStyle = FontStyle | FontStyle.Strikeout;
                else
                    FontStyle = FontStyle.Strikeout;
            }
            if (Underline)
            {
                if (FontStyle != FontStyle.Regular)
                    FontStyle = FontStyle | FontStyle.Underline;
                else
                    FontStyle = FontStyle.Underline;
            }

            return FontStyle;
        }


        #endregion




        private void AddPrice()
        {
            if (this.treeView_price_groups.SelectedNode == null) return;
            var priceGroup = (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup)this.treeView_price_groups.SelectedNode.Tag;
            var f = new PriceAppend
            {
                Rate = new Sdl.Community.PostEdit.Compare.Core.Settings.Price(),
                RateGroup = priceGroup,
                IsEdit = false
            };


            f.ShowDialog();
            if (!f.Saved) return;
            bool foundPrice = priceGroup.GroupPrices.Any(price => string.Compare(price.Source, f.Rate.Source, true) == 0 && string.Compare(price.Target, f.Rate.Target, true) == 0);
            if (!foundPrice)
            {


                priceGroup.GroupPrices.Add(f.Rate);


                ListViewItem itmx = this.listView_price_groups.Items.Add(f.Rate.Source);
                itmx.SubItems.Add(f.Rate.Target);
                      
                itmx.SubItems.Add(f.Rate.PriceBase.ToString());
                       
                       
                itmx.SubItems.Add(f.Rate.Round.ToString());                        
                itmx.SubItems.Add(f.Rate.PricePerfect.ToString());
                itmx.SubItems.Add(f.Rate.PriceContext.ToString());
                itmx.SubItems.Add(f.Rate.PriceRepetition.ToString());
                itmx.SubItems.Add(f.Rate.PriceExact.ToString());
                itmx.SubItems.Add(f.Rate.Price9599.ToString());
                itmx.SubItems.Add(f.Rate.Price8594.ToString());
                itmx.SubItems.Add(f.Rate.Price7584.ToString());
                itmx.SubItems.Add(f.Rate.Price5074.ToString());
                itmx.SubItems.Add(f.Rate.PriceNew.ToString());
                itmx.ImageIndex = 0;
                itmx.Tag = f.Rate;
            }
        }
        private void AddPriceMultiple()
        {
            if (this.treeView_price_groups.SelectedNode != null)
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup = (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup)this.treeView_price_groups.SelectedNode.Tag;

                Forms.AppendMultiplePrices f = new AppendMultiplePrices();

                f.comboBox_sourceLanguages.Items.Clear();
                foreach (Sdl.Community.PostEdit.Compare.Core.Settings.Language language in priceGroup.SourceLanguages)
                {
                    f.comboBox_sourceLanguages.Items.Add(language.Id + " - " + language.Name);
                }
                f.comboBox_sourceLanguages.SelectedIndex = 0;

                f.ShowDialog();
                if (f.Saved)
                {
                    Sdl.Community.PostEdit.Compare.Core.Settings.Language language_source = null;

                    string id = f.comboBox_sourceLanguages.SelectedItem.ToString().Substring(0, f.comboBox_sourceLanguages.SelectedItem.ToString().IndexOf(" "));
                    if (id == "*")
                    {
                        language_source = new Sdl.Community.PostEdit.Compare.Core.Settings.Language("*", "*", "*", "{all}");
                    }
                    else
                    {
                        language_source = Common.GetLanguageFromCultureInfo(id);
                    }


                    List<string> languageComboIndex = new List<string>();
                    foreach (ListViewItem itmx in this.listView_price_groups.Items)
                    {
                        Sdl.Community.PostEdit.Compare.Core.Settings.Price price = (Sdl.Community.PostEdit.Compare.Core.Settings.Price)itmx.Tag;
                        languageComboIndex.Add(price.Source + " - " + price.Target);                        
                    }

                    foreach (Sdl.Community.PostEdit.Compare.Core.Settings.Language language_target in priceGroup.TargetLanguages)
                    {
                        if (!languageComboIndex.Contains(language_source.Id + " - " + language_target.Id))
                        {
                            languageComboIndex.Add(language_source.Id + " - " + language_target.Id);

                            Sdl.Community.PostEdit.Compare.Core.Settings.Price price = new Sdl.Community.PostEdit.Compare.Core.Settings.Price();
                            price.Source = language_source.Id;
                            price.Target = language_target.Id;

                            ListViewItem itmx = this.listView_price_groups.Items.Add(price.Source);
                            itmx.SubItems.Add(price.Target);

                            itmx.SubItems.Add(price.PriceBase.ToString());

                            itmx.SubItems.Add(price.Round.ToString());
                            itmx.SubItems.Add(price.PricePerfect.ToString());
                            itmx.SubItems.Add(price.PriceContext.ToString());
                            itmx.SubItems.Add(price.PriceRepetition.ToString());
                            itmx.SubItems.Add(price.PriceExact.ToString());
                            itmx.SubItems.Add(price.Price9599.ToString());
                            itmx.SubItems.Add(price.Price8594.ToString());
                            itmx.SubItems.Add(price.Price7584.ToString());
                            itmx.SubItems.Add(price.Price5074.ToString());
                            itmx.SubItems.Add(price.PriceNew.ToString());
                            itmx.ImageIndex = 0;
                            itmx.Tag = price;

                            priceGroup.GroupPrices.Add(price);
                        }
                    }

                }
            }

        }
        private void EditPrice()
        {
            if (this.treeView_price_groups.SelectedNode != null)
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup = (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup)this.treeView_price_groups.SelectedNode.Tag;
                Forms.PriceAppend f = new PriceAppend();

                if (this.listView_price_groups.SelectedItems.Count > 0)
                {
                    ListViewItem _itmx = this.listView_price_groups.SelectedItems[0];


                    Sdl.Community.PostEdit.Compare.Core.Settings.Price _price = (Sdl.Community.PostEdit.Compare.Core.Settings.Price)_itmx.Tag;

                    f.Rate = (Sdl.Community.PostEdit.Compare.Core.Settings.Price)_price.Clone();
                    f.RateGroup = priceGroup;

                    f.IsEdit = true;
                    f.IsEditMultiple = (this.listView_price_groups.SelectedItems.Count > 1 ? true : false);
                    f.ShowDialog();
                    if (f.Saved)
                    {

                        foreach (ListViewItem itmx in this.listView_price_groups.Items)
                        {
                            if (itmx.Selected)
                            {

                                Sdl.Community.PostEdit.Compare.Core.Settings.Price price = (Sdl.Community.PostEdit.Compare.Core.Settings.Price)itmx.Tag;

                                price.PriceBase = f.Rate.PriceBase;
                                price.Round = f.Rate.Round;
                                price.PricePerfect = f.Rate.PricePerfect;
                                price.PriceContext = f.Rate.PriceContext;
                                price.PriceRepetition = f.Rate.PriceRepetition;
                                price.PriceExact = f.Rate.PriceExact;
                                price.Price9599 = f.Rate.Price9599;
                                price.Price8594 = f.Rate.Price8594;
                                price.Price7584 = f.Rate.Price7584;
                                price.Price5074 = f.Rate.Price5074;
                                price.PriceNew = f.Rate.PriceNew;


                                itmx.SubItems[2].Text = f.Rate.PriceBase.ToString();
                                itmx.SubItems[3].Text = f.Rate.Round.ToString();
                                itmx.SubItems[4].Text = f.Rate.PricePerfect.ToString();
                                itmx.SubItems[5].Text = f.Rate.PriceContext.ToString();
                                itmx.SubItems[6].Text = f.Rate.PriceRepetition.ToString();
                                itmx.SubItems[7].Text = f.Rate.PriceExact.ToString();
                                itmx.SubItems[8].Text = f.Rate.Price9599.ToString();
                                itmx.SubItems[9].Text = f.Rate.Price8594.ToString();
                                itmx.SubItems[10].Text = f.Rate.Price7584.ToString();
                                itmx.SubItems[11].Text = f.Rate.Price5074.ToString();
                                itmx.SubItems[12].Text = f.Rate.PriceNew.ToString();

                                itmx.Tag = price;
                            }

                        }


                    }
                }
            }
        }
        private void DeletePrice()
        {
            if (this.treeView_price_groups.SelectedNode != null)
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup = (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup)this.treeView_price_groups.SelectedNode.Tag;

                foreach (ListViewItem itmx in this.listView_price_groups.Items)
                {
                    if (itmx.Selected)
                    {
                        Sdl.Community.PostEdit.Compare.Core.Settings.Price papd = (Sdl.Community.PostEdit.Compare.Core.Settings.Price)itmx.Tag;

                        for (int i = 0; i <= priceGroup.GroupPrices.Count; i++)
                        {
                            if (papd.Source == priceGroup.GroupPrices[i].Source
                                && papd.Target == priceGroup.GroupPrices[i].Target)
                            {
                                priceGroup.GroupPrices.RemoveAt(i);
                                break;
                            }
                        }
                        itmx.Remove();
                    }

                }
            }
        }

        private void EditAnalysisBandPercentages()
        {

            if (this.treeView_price_groups.SelectedNode != null)
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup = (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup)this.treeView_price_groups.SelectedNode.Tag;
                if (this.listView_price_groups.SelectedItems.Count > 0)
                {



                    Sdl.Community.PostEdit.Compare.Core.Settings.Price _papd = (Sdl.Community.PostEdit.Compare.Core.Settings.Price)this.listView_price_groups.SelectedItems[0].Tag;

                    Forms.AppendAnlaysisBand f = new AppendAnlaysisBand();

                    #region  |  rate analysis band percentages  |
                    if (_papd.PricePerfect > 0)
                        f.numericUpDown_percentagePerfect.Value = (_papd.PricePerfect / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentagePerfect.Value = 0;

                    if (_papd.PriceContext > 0)
                        f.numericUpDown_percentageContext.Value = (_papd.PriceContext / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentageContext.Value = 0;

                    if (_papd.PriceRepetition > 0)
                        f.numericUpDown_percentageRepetitions.Value = (_papd.PriceRepetition / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentageRepetitions.Value = 0;

                    if (_papd.PriceExact > 0)
                        f.numericUpDown_percentageExact.Value = (_papd.PriceExact / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentageExact.Value = 0;

                    if (_papd.Price9599 > 0)
                        f.numericUpDown_percentageFuzzy99.Value = (_papd.Price9599 / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentageFuzzy99.Value = 0;

                    if (_papd.Price8594 > 0)
                        f.numericUpDown_percentageFuzzy94.Value = (_papd.Price8594 / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentageFuzzy94.Value = 0;

                    if (_papd.Price7584 > 0)
                        f.numericUpDown_percentageFuzzy84.Value = (_papd.Price7584 / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentageFuzzy84.Value = 0;

                    if (_papd.Price5074 > 0)
                        f.numericUpDown_percentageFuzzy74.Value = (_papd.Price5074 / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentageFuzzy74.Value = 0;

                    if (_papd.PriceNew > 0)
                        f.numericUpDown_percentageNew.Value = (_papd.PriceNew / _papd.PriceBase) * 100;
                    else
                        f.numericUpDown_percentageNew.Value = 0;
                    #endregion


                    if (f.numericUpDown_percentagePerfect.Value == 0
                        && f.numericUpDown_percentageContext.Value == 0
                        && f.numericUpDown_percentageRepetitions.Value == 0
                        && f.numericUpDown_percentageExact.Value == 0
                        && f.numericUpDown_percentageFuzzy99.Value == 0
                        && f.numericUpDown_percentageFuzzy94.Value == 0
                        && f.numericUpDown_percentageFuzzy84.Value == 0
                        && f.numericUpDown_percentageFuzzy74.Value == 0
                        && f.numericUpDown_percentageNew.Value == 0)
                    {
                        f.Text += " (default percentages)";
                        f.numericUpDown_percentagePerfect.Value = priceGroup.DefaultAnalysisBand.PercentagePerfect;
                        f.numericUpDown_percentageContext.Value = priceGroup.DefaultAnalysisBand.PercentageContext;
                        f.numericUpDown_percentageRepetitions.Value = priceGroup.DefaultAnalysisBand.PercentageRepetition;
                        f.numericUpDown_percentageExact.Value = priceGroup.DefaultAnalysisBand.PercentageExact;
                        f.numericUpDown_percentageFuzzy99.Value = priceGroup.DefaultAnalysisBand.Percentage9599;
                        f.numericUpDown_percentageFuzzy94.Value = priceGroup.DefaultAnalysisBand.Percentage8594;
                        f.numericUpDown_percentageFuzzy84.Value = priceGroup.DefaultAnalysisBand.Percentage7584;
                        f.numericUpDown_percentageFuzzy74.Value = priceGroup.DefaultAnalysisBand.Percentage5074;
                        f.numericUpDown_percentageNew.Value = priceGroup.DefaultAnalysisBand.PercentageNew;
                    }


                    f.ShowDialog();

                    if (f.Saved)
                    {
                        //if (this.listView_price_groups.SelectedItems.Count != this.listView_price_groups.Items.Count)
                        //{
                        //    DialogResult dr = MessageBox.Show(this, "Do you want to apply changes to the analysis band percentages for all prices?"
                        //        + "\r\n\r\n\tSelect 'Yes' to update all prices"
                        //        + "\r\n\tSelect 'No' to update (only) the selected prices"
                        //        , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //    if (dr == System.Windows.Forms.DialogResult.Yes)
                        //    {
                        //        foreach (ListViewItem item in this.listView_price_groups.Items)
                        //        {
                        //            item.Selected = true;
                        //        }
                        //    }
                        //}

                        foreach (ListViewItem itmx in this.listView_price_groups.SelectedItems)
                        {
                            Sdl.Community.PostEdit.Compare.Core.Settings.Price papd = (Sdl.Community.PostEdit.Compare.Core.Settings.Price)itmx.Tag;
                            papd.PricePerfect = 0;
                            papd.PriceContext = 0;
                            papd.PriceRepetition = 0;
                            papd.PriceExact = 0;
                            papd.Price9599 = 0;
                            papd.Price8594 = 0;
                            papd.Price7584 = 0;
                            papd.Price5074 = 0;
                            papd.PriceNew = 0;


                            if (papd.PriceBase > 0)
                            {
                                if (papd.Round == Sdl.Community.PostEdit.Compare.Core.Settings.Price.RoundType.Roundup)
                                {
                                    #region  |  round up  |

                                    if (f.numericUpDown_percentagePerfect.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentagePerfect.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.PricePerfect = value;
                                        else
                                            papd.PricePerfect = papd.PriceBase;
                                    }


                                    if (f.numericUpDown_percentageContext.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentageContext.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.PriceContext = value;
                                        else
                                            papd.PriceContext = papd.PriceBase;
                                    }


                                    if (f.numericUpDown_percentageRepetitions.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentageRepetitions.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.PriceRepetition = value;
                                        else
                                            papd.PriceRepetition = papd.PriceBase;
                                    }


                                    if (f.numericUpDown_percentageExact.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentageExact.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.PriceExact = value;
                                        else
                                            papd.PriceExact = papd.PriceBase;
                                    }


                                    if (f.numericUpDown_percentageFuzzy99.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentageFuzzy99.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.Price9599 = value;
                                        else
                                            papd.Price9599 = papd.PriceBase;
                                    }


                                    if (f.numericUpDown_percentageFuzzy94.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentageFuzzy94.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.Price8594 = value;
                                        else
                                            papd.Price8594 = papd.PriceBase;
                                    }


                                    if (f.numericUpDown_percentageFuzzy84.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentageFuzzy84.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.Price7584 = value;
                                        else
                                            papd.Price7584 = papd.PriceBase;
                                    }


                                    if (f.numericUpDown_percentageFuzzy74.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentageFuzzy74.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.Price5074 = value;
                                        else
                                            papd.Price5074 = papd.PriceBase;
                                    }


                                    if (f.numericUpDown_percentageNew.Value > 0)
                                    {
                                        decimal value = Common.RoundUp(papd.PriceBase, Convert.ToDecimal(f.numericUpDown_percentageNew.Value * .01M), 3);

                                        if (value <= papd.PriceBase)
                                            papd.PriceNew = value;
                                        else
                                            papd.PriceNew = papd.PriceBase;
                                    }
                                    #endregion
                                }
                                else if (papd.Round == Sdl.Community.PostEdit.Compare.Core.Settings.Price.RoundType.Rounddown)
                                {
                                    #region  |  round down  |
                                    string str_decimalLen = "10".PadRight(3 + 1, '0');
                                    Int32 decimalLen = Convert.ToInt32(str_decimalLen);

                                    if (f.numericUpDown_percentagePerfect.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentagePerfect.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.PricePerfect = value;
                                        else
                                            papd.PricePerfect = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageContext.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageContext.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.PriceContext = value;
                                        else
                                            papd.PriceContext = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageRepetitions.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageRepetitions.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.PriceRepetition = value;
                                        else
                                            papd.PriceRepetition = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageExact.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageExact.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.PriceExact = value;
                                        else
                                            papd.PriceExact = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageFuzzy99.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageFuzzy99.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.Price9599 = value;
                                        else
                                            papd.Price9599 = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageFuzzy94.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageFuzzy94.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.Price8594 = value;
                                        else
                                            papd.Price8594 = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageFuzzy84.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageFuzzy84.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.Price7584 = value;
                                        else
                                            papd.Price7584 = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageFuzzy74.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageFuzzy74.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.Price5074 = value;
                                        else
                                            papd.Price5074 = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageNew.Value > 0)
                                    {
                                        decimal value = Math.Truncate((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageNew.Value * .01M)) * decimalLen) / decimalLen;

                                        if (value <= papd.PriceBase)
                                            papd.PriceNew = value;
                                        else
                                            papd.PriceNew = papd.PriceBase;
                                    }
                                    #endregion
                                }
                                else if (papd.Round == Sdl.Community.PostEdit.Compare.Core.Settings.Price.RoundType.Round)
                                {
                                    #region  |  round  |

                                    if (f.numericUpDown_percentagePerfect.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentagePerfect.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.PricePerfect = value;
                                        else
                                            papd.PricePerfect = papd.PriceBase;
                                    }




                                    if (f.numericUpDown_percentageContext.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageContext.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.PriceContext = value;
                                        else
                                            papd.PriceContext = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageRepetitions.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageRepetitions.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.PriceRepetition = value;
                                        else
                                            papd.PriceRepetition = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageExact.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageExact.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.PriceExact = value;
                                        else
                                            papd.PriceExact = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageFuzzy99.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageFuzzy99.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.Price9599 = value;
                                        else
                                            papd.Price9599 = papd.PriceBase;
                                    }




                                    if (f.numericUpDown_percentageFuzzy94.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageFuzzy94.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.Price8594 = value;
                                        else
                                            papd.Price8594 = papd.PriceBase;
                                    }




                                    if (f.numericUpDown_percentageFuzzy84.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageFuzzy84.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.Price7584 = value;
                                        else
                                            papd.Price7584 = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageFuzzy74.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageFuzzy74.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.Price5074 = value;
                                        else
                                            papd.Price5074 = papd.PriceBase;
                                    }



                                    if (f.numericUpDown_percentageNew.Value > 0)
                                    {
                                        decimal value = Common.Round((papd.PriceBase * Convert.ToDecimal(f.numericUpDown_percentageNew.Value * .01M)), 3);
                                        if (value <= papd.PriceBase)
                                            papd.PriceNew = value;
                                        else
                                            papd.PriceNew = papd.PriceBase;
                                    }
                                    #endregion
                                }
                            }


                            itmx.SubItems[2].Text = papd.PriceBase.ToString();
                            itmx.SubItems[3].Text = papd.Round.ToString();
                            itmx.SubItems[4].Text = papd.PricePerfect.ToString();
                            itmx.SubItems[5].Text = papd.PriceContext.ToString();
                            itmx.SubItems[6].Text = papd.PriceRepetition.ToString();
                            itmx.SubItems[7].Text = papd.PriceExact.ToString();
                            itmx.SubItems[8].Text = papd.Price9599.ToString();
                            itmx.SubItems[9].Text = papd.Price8594.ToString();
                            itmx.SubItems[10].Text = papd.Price7584.ToString();
                            itmx.SubItems[11].Text = papd.Price5074.ToString();
                            itmx.SubItems[12].Text = papd.PriceNew.ToString();

                            itmx.Tag = papd;


                        }
                    }
                }
            }
        }






        private void EditProjectFileAlignment()
        {
            ListViewItem itmx = this.listView_comparison_projects.SelectedItems[0];
            Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject = ((Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject)itmx.Tag);



            Forms.ComparisonProjectFileAlignment f = new Forms.ComparisonProjectFileAlignment();
            f.ComparisonProject = (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject)comparisonProject.Clone();



            f.ShowDialog();

            if (f.Saved)
            {
                comparisonProject.FileAlignment = f.ComparisonProject.FileAlignment;


                //foreach (Compare.Core.Settings.ComparisonProject cp in Cache.Application.settings.comparisonProjects)
                //{
                //    if (string.Compare(cp.id, f.comparisonProject.id, true) == 0
                //        && string.Compare(cp.pathRight, f.comparisonProject.pathRight, true) == 0)
                //    {
                //        cp.fileAlignment = comparisonProject.fileAlignment;
                //        break;
                //    }
                //}
                //saveApplicationSettings();
            }
        }
        private void AddFolderPairItem(string pathLeft, string pathRight)
        {
            Forms.ComparisonProject f = new ComparisonProject();

            f.IsEdit = false;
            f.comparisonProject = new Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject();
            f.comparisonProject.Name = "New comparison project";
            f.comparisonProject.Created = DateTime.Now.ToString();
            f.comparisonProject.PathLeft = pathLeft;
            f.comparisonProject.PathRight = pathRight;

            f.ShowDialog();
            {
                if (f.Saved)
                {
                    bool foundNameUsed = false;
                    foreach (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject in ComparisonProjects)
                    {
                        if (string.Compare(comparisonProject.Name, f.comparisonProject.Name, true) == 0)
                        {
                            foundNameUsed = true;
                            break;
                        }
                    }
                    #region  |  foundNameUsed  |
                    if (foundNameUsed)
                    {
                        int i = 0;
                        while (true)
                        {
                            i++;
                            string newName = f.comparisonProject.Name + " " + i.ToString().PadLeft(2, '0');

                            foundNameUsed = false;
                            foreach (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject in ComparisonProjects)
                            {
                                if (string.Compare(comparisonProject.Name, newName, true) == 0)
                                {
                                    foundNameUsed = true;
                                    break;
                                }
                            }
                            if (!foundNameUsed)
                            {
                                f.comparisonProject.Name = newName;
                                break;
                            }                            
                        }
                    }
                    #endregion

                    ListViewItem itmx = this.listView_comparison_projects.Items.Add(f.comparisonProject.Name);
                    itmx.SubItems.Add(f.comparisonProject.Created);
                    itmx.SubItems.Add(f.comparisonProject.PathLeft);
                    itmx.SubItems.Add(f.comparisonProject.PathRight);

                    itmx.Tag = f.comparisonProject;


                    bool isPathNotFoundError = false;
                    if (!Directory.Exists(f.comparisonProject.PathLeft))
                    {
                        itmx.SubItems[2].BackColor = Color.Pink;
                        isPathNotFoundError = true;
                        itmx.ToolTipText = "The left directory path does not exist!";
                    }
                    else if (!Directory.Exists(f.comparisonProject.PathLeft))
                    {
                        itmx.SubItems[3].BackColor = Color.Pink;
                        isPathNotFoundError = true;
                        itmx.ToolTipText = "The right directory path does not exist!";
                    }

                    itmx.ImageIndex = (isPathNotFoundError ? 4 : 3);

                    ComparisonProjects.Add(f.comparisonProject);

                }
            }
        }
        private void EditFolderPairItem()
        {

            if (this.listView_comparison_projects.SelectedItems.Count > 0)
            {

                ListViewItem itmx = this.listView_comparison_projects.SelectedItems[0];


                Forms.ComparisonProject f = new ComparisonProject();

                f.IsEdit = true;
                f.comparisonProject = (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject)itmx.Tag;


                f.ShowDialog();
                {
                    if (f.Saved)
                    {
                        bool foundNameUsed = false;
                        foreach (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject in ComparisonProjects)
                        {
                            if (string.Compare(comparisonProject.Name, f.comparisonProject.Name, true) == 0)
                            {
                                if (string.Compare(comparisonProject.Name, f.comparisonProject.Name, true) == 0
                                    && string.Compare(comparisonProject.Created, f.comparisonProject.Created, true) == 0
                                    && string.Compare(comparisonProject.PathLeft, f.comparisonProject.PathLeft, true) == 0
                                    && string.Compare(comparisonProject.PathRight, f.comparisonProject.PathRight, true) == 0)
                                {
                                    //ignore
                                }
                                else
                                {
                                    foundNameUsed = true;
                                    break;
                                }
                            }
                        }
                        #region  |  foundNameUsed  |
                        if (foundNameUsed)
                        {
                            int i = 0;
                            while (true)
                            {
                                i++;
                                string newName = f.comparisonProject.Name + " " + i.ToString().PadLeft(2, '0');

                                foundNameUsed = false;
                                foreach (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject in ComparisonProjects)
                                {
                                    if (string.Compare(comparisonProject.Name, newName, true) == 0)
                                    {
                                        foundNameUsed = true;
                                        break;
                                    }
                                }
                                if (!foundNameUsed)
                                {
                                    f.comparisonProject.Name = newName;
                                    break;
                                }
                            }
                        }
                        #endregion


                        itmx.SubItems[0].Text = f.comparisonProject.Name;
                        itmx.SubItems[1].Text = f.comparisonProject.Created;
                        itmx.SubItems[2].Text = f.comparisonProject.PathLeft;
                        itmx.SubItems[3].Text = f.comparisonProject.PathRight;



                        itmx.Tag = f.comparisonProject;


                        bool isPathNotFoundError = false;
                        if (!Directory.Exists(f.comparisonProject.PathLeft))
                        {
                            itmx.SubItems[2].BackColor = Color.Pink;
                            isPathNotFoundError = true;
                            itmx.ToolTipText = "The left directory path does not exist!";
                        }
                        else if (!Directory.Exists(f.comparisonProject.PathLeft))
                        {
                            itmx.SubItems[3].BackColor = Color.Pink;
                            isPathNotFoundError = true;
                            itmx.ToolTipText = "The right directory path does not exist!";
                        }

                        itmx.ImageIndex = (isPathNotFoundError ? 4 : 3);                   

                    }
                }
            }
        }
        private void RemoveFolderPairItem()
        {
            if (this.listView_comparison_projects.SelectedItems.Count > 0)
            {

                DialogResult dr = MessageBox.Show(this, "Do you want to delete the selected projects?\r\n\r\n"
                    + "Note: you will not be able to recover this information\r\n\r\n"
                    + "Click 'Yes' to delete the selected projects\r\n"
                    + "Click 'No' to cancel", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (ListViewItem itmx in this.listView_comparison_projects.SelectedItems)
                    {



                        Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject = (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject)itmx.Tag;

                        foreach (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject _comparisonProject in ComparisonProjects)
                        {
                            if (string.Compare(comparisonProject.Name, _comparisonProject.Name, true) == 0
                                && string.Compare(comparisonProject.Created, _comparisonProject.Created, true) == 0
                                && string.Compare(comparisonProject.PathLeft, _comparisonProject.PathLeft, true) == 0
                                && string.Compare(comparisonProject.PathRight, _comparisonProject.PathRight, true) == 0)
                            {
                                ComparisonProjects.Remove(_comparisonProject);
                                break;
                            }
                        }

                        itmx.Remove();


                    }
                }
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            this.treeView_main.ExpandAll();

            this.ClearFolderComparisonHistory = false;
            this.pictureBox_clear_folder_comparison_histroy.Visible = false;

            this.ClearComparisonLogHistory = false;
            this.pictureBox_clear_comparison_log.Visible = false;

            InitializeObject();

            listView_comparison_projects_SelectedIndexChanged(null, null);
            treeView_price_groups_AfterSelect(null, null);
            listView_price_groups_SelectedIndexChanged(null, null);
            listView_filters_SelectedIndexChanged(null, null);

            checkBox_reportsAutoSave_CheckedChanged(null, null);

            textBox_javaExecutablePath_TextChanged(null, null);
        }

        private void linkLabel_folderViewer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {          
            
            TreeNode tn = this.treeView_main.Nodes[2];
            TreeNode tn2 = tn.Nodes[0];
            this.treeView_main.SelectedNode = tn2;            
        }

       

        private void linkLabel_reportViewer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TreeNode tn = this.treeView_main.Nodes[2];
            TreeNode tn2 = tn.Nodes[1];
            this.treeView_main.SelectedNode = tn2;            
        }

    

        private void treeView_main_AfterSelect(object sender, TreeViewEventArgs e)
        {

            bool updateSelectedNode = true;
            switch (e.Node.Name)
            {
                case "Node_general": this.panel_general.BringToFront(); break;
                case "Node_filters": this.panel_filters.BringToFront(); break;
                case "Node_viewers": this.panel_viewers.BringToFront(); break;
                case "Node_folder_viewer": this.panel_folder_viewer.BringToFront(); break;      
                case "Node_report_viewer": this.panel_report_viewer.BringToFront(); break;
                case "Node_reports": this.panel_reports.BringToFront(); break;
                case "Node_comparision_projects": this.panel_comparision_projects.BringToFront(); break;      
                case "Node_price_groups": this.panel_price_groups.BringToFront(); break;
                case "Node_comparer": this.panel_comparer.BringToFront(); break;
                case "Node_events_log": this.panel_events_log.BringToFront(); break;
            }
            if (updateSelectedNode)
            {
                SelectedTreeNode = e.Node;

                this.pictureBox_header.Image = this.imageList1.Images[e.Node.ImageIndex];
                this.textBox_header.Text = e.Node.Text;
            }
        }

        private void toolStripButton_priceGroup_new_Click(object sender, EventArgs e)
        {
            Forms.PriceGroupsGroup f = new PriceGroupsGroup();
            f.LanguageGroup = new Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup();
            f.LanguageGroup.Name = "new rate group";
            f.LanguageGroup.Currency = "EUR";
            f.LanguageGroup.SourceLanguages.Add(new Sdl.Community.PostEdit.Compare.Core.Settings.Language("*", "*", "*", "{all}"));

            CultureInfo ci = new CultureInfo("en-US");
            f.LanguageGroup.SourceLanguages.Add( new Sdl.Community.PostEdit.Compare.Core.Settings.Language(ci.Name, "", ci.TwoLetterISOLanguageName, ci.EnglishName));
            

            f.LanguageGroup.TargetLanguages.Add(new Sdl.Community.PostEdit.Compare.Core.Settings.Language("*", "*", "*", "{all}"));
            ci = new CultureInfo("it-IT");
            f.LanguageGroup.TargetLanguages.Add(new Sdl.Community.PostEdit.Compare.Core.Settings.Language(ci.Name, "", ci.TwoLetterISOLanguageName, ci.EnglishName));
            ci = new CultureInfo("de-DE");
            f.LanguageGroup.TargetLanguages.Add(new Sdl.Community.PostEdit.Compare.Core.Settings.Language(ci.Name, "", ci.TwoLetterISOLanguageName, ci.EnglishName));
            ci = new CultureInfo("es-ES");
            f.LanguageGroup.TargetLanguages.Add(new Sdl.Community.PostEdit.Compare.Core.Settings.Language(ci.Name, "", ci.TwoLetterISOLanguageName, ci.EnglishName));
            ci = new CultureInfo("fr-FR");
            f.LanguageGroup.TargetLanguages.Add(new Sdl.Community.PostEdit.Compare.Core.Settings.Language(ci.Name, "", ci.TwoLetterISOLanguageName, ci.EnglishName));

            f.IsEdit = false;

            f.ShowDialog();
            if (f.Saved)
            {
                if (f.LanguageGroup.Name.Trim() != string.Empty)
                {
                    #region  |  check nameAlreadyUsed  |
                    bool nameAlreadyUsed = false;
                    foreach (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup in this.RateGroups)
                    {
                        if (string.Compare(priceGroup.Name.Trim(), f.LanguageGroup.Name.Trim(), true) == 0)
                        {
                            nameAlreadyUsed = true;
                        }
                    }
                    if (nameAlreadyUsed)
                    {
                        int index = 0;
                        string newName = string.Empty;

                        while (true)
                        {
                            nameAlreadyUsed = false;
                            index++;
                            newName = f.LanguageGroup.Name + "_" + index.ToString().PadLeft(3, '0');
                            foreach (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup in this.RateGroups)
                            {
                                if (string.Compare(priceGroup.Name.Trim(), newName, true) == 0)
                                {
                                    nameAlreadyUsed = true;
                                    break;
                                }
                            }
                            if (!nameAlreadyUsed)
                                break;

                        }
                        f.LanguageGroup.Name = newName;
                    }
                    #endregion

                    this.RateGroups.Add(f.LanguageGroup);

                    TreeNode tn = this.treeView_price_groups.Nodes.Add(f.LanguageGroup.Name);
                    tn.Tag = f.LanguageGroup;

                    this.treeView_price_groups.SelectedNode = tn;
                }
            }

        }




        private void AddFilter()
        {
            Forms.FilterAppend f = new FilterAppend();

            f.FilterSetting = new Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting();
            f.Name = "New Filter";
            f.IsEdit = false;
            f.ShowDialog();
            if (f.Saved)
            {
                bool foundName = false;
                foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting filterSetting in FilterSettings)
                {
                    if (string.Compare(filterSetting.Name, f.FilterSetting.Name, true) == 0)
                    {
                        foundName = true;
                        break;
                    }
                }
                if (foundName)
                {
                    MessageBox.Show(this, "Filter Name already exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    FilterSettings.Add(f.FilterSetting);


                    ListViewItem itmx = this.listView_filters.Items.Add(f.FilterSetting.Name,0);


                    string namesInclude = string.Empty;
                    if (f.FilterSetting.FilterNamesInclude.Count > 0)
                    {
                        foreach (string str in f.FilterSetting.FilterNamesInclude)
                        {
                            if (str.Trim() != string.Empty)
                                namesInclude += (namesInclude.Trim() != string.Empty ? "; " : string.Empty) + str;
                        }
                        itmx.SubItems.Add(namesInclude);
                    }
                    else if (f.FilterSetting.FilterNamesInclude.Count > 0)
                    {
                        string namesExclude = string.Empty;
                        foreach (string str in f.FilterSetting.FilterNamesExclude)
                        {
                            if (str.Trim() != string.Empty)
                                namesExclude += (namesExclude.Trim() != string.Empty ? "; " : string.Empty) + str;
                        }
                        itmx.SubItems.Add("-" + namesExclude);
                    }
                    else
                    {
                        itmx.SubItems.Add(string.Empty);
                    }

                    

                    string filterDateStr = string.Empty;
                    if (f.FilterSetting.FilterDateUsed)
                    {
                        if (f.FilterSetting.FilterDate.Type == Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.LessThan)
                            filterDateStr += "before ";
                        else
                            filterDateStr += "after ";

                        filterDateStr += f.FilterSetting.FilterDate.Date.ToString();
                    }
                    itmx.SubItems.Add(filterDateStr);
                   

                    string attributes = string.Empty;
                    if (f.FilterSetting.FilterAttributeArchiveUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (f.FilterSetting.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                    }
                    if (f.FilterSetting.FilterAttributeSystemUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (f.FilterSetting.FilterAttributeSystemType == "Included" ? "S" : "-S");
                    }
                    if (f.FilterSetting.FilterAttributeHiddenUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (f.FilterSetting.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                    }
                    if (f.FilterSetting.FilterAttributeReadOnlyUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (f.FilterSetting.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                    }
                    

                    itmx.SubItems.Add(attributes);

                    itmx.Tag = f.FilterSetting;

                }
            }
        }
        private void EditFilter()
        {
            if (this.listView_filters.SelectedItems.Count > 0)
            {
                ListViewItem itmx = this.listView_filters.SelectedItems[0];

                Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting filterSetting = (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting)itmx.Tag;

                

                Forms.FilterAppend f = new FilterAppend();
                f.FilterSetting = (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting)filterSetting.Clone();

                f.IsEdit = true;
                f.ShowDialog();
                if (f.Saved)
                {
                    filterSetting.Name = f.FilterSetting.Name;
                    filterSetting.FilterNamesInclude = f.FilterSetting.FilterNamesInclude;
                    filterSetting.FilterNamesExclude = f.FilterSetting.FilterNamesExclude;
                    filterSetting.UseRegularExpressionMatching = f.FilterSetting.UseRegularExpressionMatching;

                    filterSetting.FilterDateUsed = f.FilterSetting.FilterDateUsed;
                    filterSetting.FilterDate.Date = f.FilterSetting.FilterDate.Date;
                    filterSetting.FilterDate.Type = f.FilterSetting.FilterDate.Type;
                    filterSetting.IsDefault = f.FilterSetting.IsDefault;

                    filterSetting.FilterAttributeAchiveType = f.FilterSetting.FilterAttributeAchiveType;
                    filterSetting.FilterAttributeArchiveUsed = f.FilterSetting.FilterAttributeArchiveUsed;

                    filterSetting.FilterAttributeHiddenType = f.FilterSetting.FilterAttributeHiddenType;
                    filterSetting.FilterAttributeHiddenUsed = f.FilterSetting.FilterAttributeHiddenUsed;

                    filterSetting.FilterAttributeReadOnlyType = f.FilterSetting.FilterAttributeReadOnlyType;
                    filterSetting.FilterAttributeReadOnlyUsed = f.FilterSetting.FilterAttributeReadOnlyUsed;

                    filterSetting.FilterAttributeSystemType = f.FilterSetting.FilterAttributeSystemType;
                    filterSetting.FilterAttributeSystemUsed = f.FilterSetting.FilterAttributeSystemUsed;


                    itmx.SubItems[0].Text = filterSetting.Name;

                    if (f.FilterSetting.FilterNamesInclude.Count > 0)
                    {
                        string namesInclude = string.Empty;
                        foreach (string str in f.FilterSetting.FilterNamesInclude)
                        {
                            if (str.Trim() != string.Empty)
                                namesInclude += (namesInclude.Trim() != string.Empty ? "; " : string.Empty) + str;
                        }
                        itmx.SubItems[1].Text = namesInclude;
                    }
                    else if (f.FilterSetting.FilterNamesExclude.Count > 0)
                    {
                        string namesExclude = string.Empty;
                        foreach (string str in f.FilterSetting.FilterNamesExclude)
                        {
                            if (str.Trim() != string.Empty)
                                namesExclude += (namesExclude.Trim() != string.Empty ? "; " : string.Empty) + str;
                        }
                        itmx.SubItems[1].Text = "-" + namesExclude;
                    }
                    else
                    {
                        itmx.SubItems[1].Text = string.Empty;
                    }


             

                    string filterDateStr = string.Empty;
                    if (f.FilterSetting.FilterDateUsed)
                    {
                        if (f.FilterSetting.FilterDate.Type == Sdl.Community.PostEdit.Compare.Core.Settings.FilterDate.FilterType.LessThan)
                            filterDateStr += "before ";
                        else
                            filterDateStr += "after ";

                        filterDateStr += f.FilterSetting.FilterDate.Date.ToString();
                    }
                    itmx.SubItems[2].Text = filterDateStr;






                    string attributes = string.Empty;
                    if (filterSetting.FilterAttributeArchiveUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (filterSetting.FilterAttributeAchiveType == "Included" ? "A" : "-A");
                    }
                    if (filterSetting.FilterAttributeSystemUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (filterSetting.FilterAttributeSystemType == "Included" ? "S" : "-S");
                    }
                    if (filterSetting.FilterAttributeHiddenUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (filterSetting.FilterAttributeHiddenType == "Included" ? "H" : "-H");
                    }
                    if (filterSetting.FilterAttributeReadOnlyUsed)
                    {
                        attributes += (attributes.Trim() != string.Empty ? "; " : string.Empty)
                            + (filterSetting.FilterAttributeReadOnlyType == "Included" ? "R" : "-R");
                    }
                  
                    itmx.SubItems[3].Text = attributes;


                    itmx.Tag = filterSetting;
                }
            }
        }
        private void RemoveFilter()
        {
            if (this.listView_filters.SelectedItems.Count > 0)
            {
                DialogResult dr = MessageBox.Show(this, "Do you want to delete the selected filters?\r\n\r\n"
                    + "Note: you will not be able to recover this information\r\n\r\n"
                    + "Click 'Yes' to delete the selected filters\r\n"
                    + "Click 'No' to cancel", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (ListViewItem itmx in this.listView_filters.SelectedItems)
                    {

                        Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting filterSetting = (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting)itmx.Tag;

                        foreach (Sdl.Community.PostEdit.Compare.Core.Settings.FilterSetting fs in FilterSettings)
                        {
                            if (string.Compare(fs.Name, filterSetting.Name, true) == 0)
                            {
                                FilterSettings.Remove(fs);
                                break;
                            }
                        }
                        itmx.Remove();
                    }
                }
            }
        }
        private static void ExportFilters()
        {
        }
        private static void ImportFilters()
        {
        }
        private void FilterDown()
        {
            MoveListViewItem(this.listView_filters, false);
        }
        private void FilterUp()
        {
            MoveListViewItem(this.listView_filters, true);
        }



        private void button_Save_Click(object sender, EventArgs e)
        {


            List<Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject> comparisonProjects_new = new List<Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject>();
            foreach (ListViewItem itmx in this.listView_comparison_projects.Items)
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject = (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject)itmx.Tag;
                comparisonProjects_new.Add(comparisonProject);
            }
            ComparisonProjects = comparisonProjects_new;


            Saved = true;
            this.Close();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Saved = false;
            this.Close();
        }


        private void DeletePriceGroup()
        {

            if (this.treeView_price_groups.SelectedNode != null)
            {

                DialogResult dr = MessageBox.Show(this, "Do you want to delete the selected rate group?\r\n\r\n"
                    + "Note: you will not be able to recover this information\r\n\r\n"
                    + "Click 'Yes' to delete the selected rate group\r\n"
                    + "Click 'No' to cancel", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {




                    this.listView_price_groups.Items.Clear();

                    Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup = (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup)this.treeView_price_groups.SelectedNode.Tag;
                    foreach (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup _priceGroup in RateGroups)
                    {
                        if (string.Compare(priceGroup.Name, _priceGroup.Name, true) == 0)
                        {
                            RateGroups.Remove(_priceGroup);

                            break;
                        }
                    }
                    this.treeView_price_groups.SelectedNode.Remove();
                }
            }

        }

        private void treeView_price_groups_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.listView_price_groups.Items.Clear();
            if (this.treeView_price_groups.SelectedNode != null)
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup = (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup)this.treeView_price_groups.SelectedNode.Tag;
                foreach (Sdl.Community.PostEdit.Compare.Core.Settings.Price price in priceGroup.GroupPrices)
                {
                    ListViewItem itmx = this.listView_price_groups.Items.Add(price.Source);
                    itmx.SubItems.Add(price.Target);
                    itmx.SubItems.Add(price.PriceBase.ToString());
                    itmx.SubItems.Add(price.Round.ToString());
                    itmx.SubItems.Add(price.PricePerfect.ToString());
                    itmx.SubItems.Add(price.PriceContext.ToString());
                    itmx.SubItems.Add(price.PriceRepetition.ToString());
                    itmx.SubItems.Add(price.PriceExact.ToString());
                    itmx.SubItems.Add(price.Price9599.ToString());
                    itmx.SubItems.Add(price.Price8594.ToString());
                    itmx.SubItems.Add(price.Price7584.ToString());
                    itmx.SubItems.Add(price.Price5074.ToString());
                    itmx.SubItems.Add(price.PriceNew.ToString());
                    itmx.ImageIndex = 0;
                    itmx.Tag = price;
                }

                if (this.listView_price_groups.Items.Count > 0)
                    this.listView_price_groups.Items[0].Selected = true;



                this.toolStripButton_priceGroup_new.Enabled = true;
                this.toolStripButton_priceGroup_edit.Enabled = true;
                this.toolStripButton_priceGroup_remove.Enabled = true;


                this.newPriceGroupToolStripMenuItem.Enabled = true;
                this.editPriceGroupToolStripMenuItem.Enabled = true;
                this.removePriceGroupToolStripMenuItem.Enabled = true;
                this.setAsDefaultPriceGroupToolStripMenuItem.Enabled = true;

            }
            else
            {
                this.toolStripButton_priceGroup_new.Enabled = true;
                this.toolStripButton_priceGroup_edit.Enabled = false;
                this.toolStripButton_priceGroup_remove.Enabled = false;


                this.newPriceGroupToolStripMenuItem.Enabled = true;
                this.editPriceGroupToolStripMenuItem.Enabled = false;
                this.removePriceGroupToolStripMenuItem.Enabled = false;
                this.setAsDefaultPriceGroupToolStripMenuItem.Enabled = false;
            }





        }

        private void toolStripButton_priceGroup_remove_Click(object sender, EventArgs e)
        {
            DeletePriceGroup();

        }

        private void toolStripButton_priceGroup_edit_Click(object sender, EventArgs e)
        {
            if (this.treeView_price_groups.SelectedNode != null)
            {
                TreeNode tn = this.treeView_price_groups.SelectedNode;
                Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup_current = (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup)tn.Tag;

                
                Forms.PriceGroupsGroup f = new PriceGroupsGroup();
                f.LanguageGroup = priceGroup_current;
               
               
                f.IsEdit = true;

                f.ShowDialog();
                if (f.Saved)
                {
                    if (f.LanguageGroup.Name.Trim() != string.Empty)
                    {
                        
                        int i = 0;
                        foreach (Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup priceGroup in this.RateGroups)
                        {
                            if (string.Compare(priceGroup.Name.Trim(), f.LanguageGroup.Name.Trim(), true) == 0)
                            {
                                i++;
                            }
                        }
                        if (i > 1)
                        {
                            MessageBox.Show(this, "Unable to save group settings; the updated name is already used!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            tn.Text = f.LanguageGroup.Name;
                            tn.Tag = f.LanguageGroup;

                            treeView_price_groups_AfterSelect(null, null);
                           
                        }
                    }
                }
            }
        }

        private void treeView_price_groups_DoubleClick(object sender, EventArgs e)
        {
            toolStripButton_priceGroup_edit_Click(null, null);
        }

        private void toolStripButton_groupPrice_add_Click(object sender, EventArgs e)
        {
            AddPrice();
        }

        private void toolStripButton_groupPrice_edit_Click(object sender, EventArgs e)
        {
            EditPrice();
        }

        private void listView_price_groups_DoubleClick(object sender, EventArgs e)
        {
            EditPrice();
        }

        private void toolStripButton_analysisBandPercentage_Click(object sender, EventArgs e)
        {
            EditAnalysisBandPercentages();
        }

        private void toolStripButton_groupPrice_remove_Click(object sender, EventArgs e)
        {
            DeletePrice();
        }

        private void toolStripButton_groupPrice_addMultiple_Click(object sender, EventArgs e)
        {
            AddPriceMultiple();
        }

        private void addPriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPrice();
        }

        private void editPriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditPrice();
        }

        private void removePriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeletePrice();
        }

        private void analysisBandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditAnalysisBandPercentages();
        }

        private void newPriceGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_priceGroup_new_Click(null, null);
        }

        private void editPriceGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_priceGroup_edit_Click(null, null);
        }

        private void removePriceGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_priceGroup_remove_Click(null, null);
        }

        private void setAsDefaultPriceGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton_comparison_project_item_new_Click(object sender, EventArgs e)
        {
            AddFolderPairItem(string.Empty, string.Empty);
        }

        private void toolStripButton_comparison_project_item_edit_Click(object sender, EventArgs e)
        {
            EditFolderPairItem();
        }

        private void toolStripButton_comparison_project_item_remove_Click(object sender, EventArgs e)
        {
            RemoveFolderPairItem();
        }

        private void listView_comparison_project_DoubleClick(object sender, EventArgs e)
        {
            EditFolderPairItem();
        }

        private void listView_comparison_project_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy; // Okay
            else
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
        }

        private void listView_comparison_project_DragDrop(object sender, DragEventArgs e)
        {
            try
            {

                string[] DirectoryList = (string[])e.Data.GetData(DataFormats.FileDrop, false);


                Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject = new Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject();

                if (DirectoryList.Count() == 2
                    && Directory.Exists(DirectoryList[0]) && Directory.Exists(DirectoryList[1]))
                {
                    comparisonProject.PathLeft = DirectoryList[0];
                    comparisonProject.PathRight = DirectoryList[1];
                }
                else if (Directory.Exists(DirectoryList[0]))
                {
                    comparisonProject.PathLeft = DirectoryList[0];
                }
                AddFolderPairItem(comparisonProject.PathLeft, comparisonProject.PathRight);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
               
            }
        }

        private void newCompareListItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFolderPairItem(string.Empty, string.Empty);
        }

        private void editCompareListItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditFolderPairItem();
        }

        private void removeCompareListItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveFolderPairItem();
        }

        private void fileAlignmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditProjectFileAlignment();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveListViewItem(this.listView_comparison_projects, false);
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveListViewItem(this.listView_comparison_projects, true);
        }



        private void toolStripButton_comparison_project_item_moveDown_Click(object sender, EventArgs e)
        {
            MoveListViewItem(this.listView_comparison_projects, false);
        }

        private void toolStripButton_comparison_project_item_moveUp_Click(object sender, EventArgs e)
        {
            MoveListViewItem(this.listView_comparison_projects, true);
        }

        private void MoveListViewItem(ListView lv, bool moveUp)
        {
            string cache;
            int selIdx;

            if (lv.Items.Count == 0)
                return;
            if (lv.SelectedItems.Count == 0)
                return;

            selIdx = lv.SelectedItems[0].Index;
            object tag_Current = lv.SelectedItems[0].Tag;

            if (moveUp)
            {
                // ignore moveup of row(0)
                if (selIdx == 0)
                    return;

                object tag_previous = lv.Items[selIdx - 1].Tag;

                // move the subitems for the previous row
                // to cache to make room for the selected row
                for (int i = 0; i < lv.Items[selIdx].SubItems.Count; i++)
                {
                    cache = lv.Items[selIdx - 1].SubItems[i].Text;
                    lv.Items[selIdx - 1].SubItems[i].Text = lv.Items[selIdx].SubItems[i].Text;
                    lv.Items[selIdx].SubItems[i].Text = cache;
                }
                lv.Items[selIdx].Selected = false;
                lv.Items[selIdx].Tag = tag_previous;
                lv.Items[selIdx - 1].Tag = tag_Current;

                lv.Items[selIdx - 1].Selected = true;
                lv.Refresh();
                lv.Focus();
            }
            else
            {
                // ignore movedown of last item
                if (selIdx == lv.Items.Count - 1)
                    return;

                object tag_previous = lv.Items[selIdx + 1].Tag;

                // move the subitems for the next row
                // to cache so we can move the selected row down
                for (int i = 0; i < lv.Items[selIdx].SubItems.Count; i++)
                {
                    cache = lv.Items[selIdx + 1].SubItems[i].Text;
                    lv.Items[selIdx + 1].SubItems[i].Text =
                      lv.Items[selIdx].SubItems[i].Text;
                    lv.Items[selIdx].SubItems[i].Text = cache;
                }
                lv.Items[selIdx].Selected = false;
                lv.Items[selIdx].Tag = tag_previous;
                lv.Items[selIdx + 1].Tag = tag_Current;

                lv.Items[selIdx + 1].Selected = true;
                lv.Refresh();
                lv.Focus();
            }
        }

        private void linkLabel_clear_folder_comparison_histroy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearFolderComparisonHistory = true;
            pictureBox_clear_folder_comparison_histroy.Visible = true;

            

        }

        private void toolStripButton_filters_add_Click(object sender, EventArgs e)
        {
            AddFilter();
        }

        private void toolStripButton_filters_edit_Click(object sender, EventArgs e)
        {
            EditFilter();
        }

        private void toolStripButton_filers_remove_Click(object sender, EventArgs e)
        {
            RemoveFilter();
        }

     

        private void toolStripButton_filters_move_down_Click(object sender, EventArgs e)
        {
            FilterDown();
        }

        private void toolStripButton_filters_moveUp_Click(object sender, EventArgs e)
        {
            FilterUp();
        }

        private void listView_filters_DoubleClick(object sender, EventArgs e)
        {
            EditFilter();
        }

        private void toolStripButton_filers_export_Click(object sender, EventArgs e)
        {
            ExportFilters();
        }

        private void toolStripButton_filters_import_Click(object sender, EventArgs e)
        {
            ImportFilters();
        }

        private void listView_price_groups_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeletePrice();
            }
        }

        private void addFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFilter();
        }

        private void editFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditFilter();
        }

        private void removeFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveFilter();
        }

        private void moveDownFilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterDown();
        }

        private void moveUpFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterUp();
        }

        private void linkLabel_clear_comparison_log_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearComparisonLogHistory = true;
            pictureBox_clear_comparison_log.Visible = true;
        }

        private void listView_comparison_projects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView_comparison_projects.SelectedItems.Count > 0)
            {
                Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject comparisonProject = (Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonProject)this.listView_comparison_projects.SelectedItems[0].Tag;


                this.toolStripButton_comparison_project_item_new.Enabled = true;
                
                this.toolStripButton_comparison_project_item_edit.Enabled = true;
                this.toolStripButton_comparison_project_item_remove.Enabled = true;
                this.toolStripButton_comparison_project_file_alignment.Enabled = true;

                this.toolStripButton_comparison_project_item_moveDown.Enabled = true;
                this.toolStripButton_comparison_project_item_moveUp.Enabled = true;


                this.moveDownToolStripMenuItem.Enabled = true;
                this.moveUpToolStripMenuItem.Enabled = true;

                this.newCompareListItemToolStripMenuItem.Enabled = true;
                this.editCompareListItemToolStripMenuItem.Enabled = true;
                this.removeCompareListItemToolStripMenuItem.Enabled = true;
                this.fileAlignmentToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.toolStripButton_comparison_project_item_new.Enabled = true;
                
                this.toolStripButton_comparison_project_item_edit.Enabled = false;
                this.toolStripButton_comparison_project_item_remove.Enabled = false;
                this.toolStripButton_comparison_project_file_alignment.Enabled = false;

                this.toolStripButton_comparison_project_item_moveDown.Enabled = false;
                this.toolStripButton_comparison_project_item_moveUp.Enabled = false;


                this.moveDownToolStripMenuItem.Enabled = false;
                this.moveUpToolStripMenuItem.Enabled = false;

                this.newCompareListItemToolStripMenuItem.Enabled = true;
                this.editCompareListItemToolStripMenuItem.Enabled = false;
                this.removeCompareListItemToolStripMenuItem.Enabled = false;
                this.fileAlignmentToolStripMenuItem.Enabled = false;
            }
        }

        private void listView_price_groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView_price_groups.SelectedItems.Count > 0)
            {
                this.toolStripButton_groupPrice_add.Enabled = true;
                this.toolStripButton_groupPrice_edit.Enabled = true;
                this.toolStripButton_groupPrice_remove.Enabled = true;
                this.toolStripButton_analysisBandPercentage.Enabled = true;

                this.addPriceToolStripMenuItem.Enabled = true;
                this.editPriceToolStripMenuItem.Enabled = true;
                this.removePriceToolStripMenuItem.Enabled = true;
                this.analysisBandToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.toolStripButton_groupPrice_add.Enabled = true;
                this.toolStripButton_groupPrice_edit.Enabled = false;
                this.toolStripButton_groupPrice_remove.Enabled = false;
                this.toolStripButton_analysisBandPercentage.Enabled = false;

                this.addPriceToolStripMenuItem.Enabled = true;
                this.editPriceToolStripMenuItem.Enabled = false;
                this.removePriceToolStripMenuItem.Enabled = false;
                this.analysisBandToolStripMenuItem.Enabled = false;
            }
        }

        private void listView_filters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView_filters.SelectedItems.Count > 0)
            {
                this.toolStripButton_filters_add.Enabled = true;
                this.toolStripButton_filters_edit.Enabled = true;
                this.toolStripButton_filers_remove.Enabled = true;

                this.toolStripButton_filters_import.Enabled = false;
                this.toolStripButton_filers_export.Enabled = false;

                this.toolStripButton_filters_move_down.Enabled = true;
                this.toolStripButton_filters_moveUp.Enabled = true;



                this.addFilterToolStripMenuItem.Enabled = true;
                this.editFilterToolStripMenuItem.Enabled = true;
                this.removeFilterToolStripMenuItem.Enabled = true;

                this.moveDownFilerToolStripMenuItem.Enabled = true;
                this.moveUpFilterToolStripMenuItem.Enabled = true;
            }
            else
            {
                this.toolStripButton_filters_add.Enabled = true;
                this.toolStripButton_filters_edit.Enabled = false;
                this.toolStripButton_filers_remove.Enabled = false;

                this.toolStripButton_filters_import.Enabled = false;
                this.toolStripButton_filers_export.Enabled = false;

                this.toolStripButton_filters_move_down.Enabled = false;
                this.toolStripButton_filters_moveUp.Enabled = false;



                this.addFilterToolStripMenuItem.Enabled = true;
                this.editFilterToolStripMenuItem.Enabled = false;
                this.removeFilterToolStripMenuItem.Enabled = false;

                this.moveDownFilerToolStripMenuItem.Enabled = false;
                this.moveUpFilterToolStripMenuItem.Enabled = false;
            }
        }

        private void toolStripButton_comparison_project_file_alignment_Click(object sender, EventArgs e)
        {
            EditProjectFileAlignment();
        }

        private void treeView_price_groups_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeletePriceGroup();  
            }
        }



        private void button_changeFormattingTextNew_Click(object sender, EventArgs e)
        {
            FormSettingsFormatting f = new FormSettingsFormatting();

            f.checkBox_fontColor.Checked = StyleNewText.FontSpecifyColor;
            f.checkBox_backroundColor.Checked = StyleNewText.FontSpecifyBackroundColor;

            if (f.checkBox_fontColor.Checked)
                f.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(StyleNewText.FontColor);
            if (f.checkBox_backroundColor.Checked)
                f.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(StyleNewText.FontBackroundColor);


            f.comboBox_styleBold.SelectedItem = StyleNewText.StyleBold;
            f.comboBox_styleItalic.SelectedItem = StyleNewText.StyleItalic;
            f.comboBox_styleStrikethrough.SelectedItem = StyleNewText.StyleStrikethrough;
            f.comboBox_styleUnderline.SelectedItem = StyleNewText.StyleUnderline;
            f.comboBox_stylePosition.SelectedItem = StyleNewText.TextPosition;


            f.ShowDialog();

            if (f.SaveSettings)
            {
                StyleNewText.FontSpecifyColor = f.checkBox_fontColor.Checked;
                StyleNewText.FontSpecifyBackroundColor = f.checkBox_backroundColor.Checked;

                if (StyleNewText.FontSpecifyColor)
                    StyleNewText.FontColor = ColorTranslator.ToHtml(f.label_fontColorDisplay.BackColor);
                if (StyleNewText.FontSpecifyBackroundColor)
                    StyleNewText.FontBackroundColor = ColorTranslator.ToHtml(f.label_backroundColorDisplay.BackColor);

                StyleNewText.StyleBold = f.comboBox_styleBold.SelectedItem.ToString();
                StyleNewText.StyleItalic = f.comboBox_styleItalic.SelectedItem.ToString();
                StyleNewText.StyleStrikethrough = f.comboBox_styleStrikethrough.SelectedItem.ToString();
                StyleNewText.StyleUnderline = f.comboBox_styleUnderline.SelectedItem.ToString();
                StyleNewText.TextPosition = f.comboBox_stylePosition.SelectedItem.ToString();
                UpdateVisualStyle();
            }
        }

        private void button_changeFormattingTextRemoved_Click(object sender, EventArgs e)
        {
            FormSettingsFormatting f = new FormSettingsFormatting();

            f.checkBox_fontColor.Checked = StyleRemovedText.FontSpecifyColor;
            f.checkBox_backroundColor.Checked = StyleRemovedText.FontSpecifyBackroundColor;

            if (f.checkBox_fontColor.Checked)
                f.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(StyleRemovedText.FontColor);
            if (f.checkBox_backroundColor.Checked)
                f.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(StyleRemovedText.FontBackroundColor);


            f.comboBox_styleBold.SelectedItem = StyleRemovedText.StyleBold;
            f.comboBox_styleItalic.SelectedItem = StyleRemovedText.StyleItalic;
            f.comboBox_styleStrikethrough.SelectedItem = StyleRemovedText.StyleStrikethrough;
            f.comboBox_styleUnderline.SelectedItem = StyleRemovedText.StyleUnderline;
            f.comboBox_stylePosition.SelectedItem = StyleRemovedText.TextPosition;
            f.ShowDialog();

            if (f.SaveSettings)
            {
                StyleRemovedText.FontSpecifyColor = f.checkBox_fontColor.Checked;
                StyleRemovedText.FontSpecifyBackroundColor = f.checkBox_backroundColor.Checked;

                if (StyleRemovedText.FontSpecifyColor)
                    StyleRemovedText.FontColor = ColorTranslator.ToHtml(f.label_fontColorDisplay.BackColor);
                if (StyleRemovedText.FontSpecifyBackroundColor)
                    StyleRemovedText.FontBackroundColor = ColorTranslator.ToHtml(f.label_backroundColorDisplay.BackColor);

                StyleRemovedText.StyleBold = f.comboBox_styleBold.SelectedItem.ToString();
                StyleRemovedText.StyleItalic = f.comboBox_styleItalic.SelectedItem.ToString();
                StyleRemovedText.StyleStrikethrough = f.comboBox_styleStrikethrough.SelectedItem.ToString();
                StyleRemovedText.StyleUnderline = f.comboBox_styleUnderline.SelectedItem.ToString();
                StyleRemovedText.TextPosition = f.comboBox_stylePosition.SelectedItem.ToString();

                UpdateVisualStyle();
            }
        }

        private void button_changeFormattingTagNew_Click(object sender, EventArgs e)
        {
            FormSettingsFormatting f = new FormSettingsFormatting();

            f.checkBox_fontColor.Checked = StyleNewTag.FontSpecifyColor;
            f.checkBox_backroundColor.Checked = StyleNewTag.FontSpecifyBackroundColor;

            if (f.checkBox_fontColor.Checked)
                f.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(StyleNewTag.FontColor);
            if (f.checkBox_backroundColor.Checked)
                f.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(StyleNewTag.FontBackroundColor);


            f.comboBox_styleBold.SelectedItem = StyleNewTag.StyleBold;
            f.comboBox_styleItalic.SelectedItem = StyleNewTag.StyleItalic;
            f.comboBox_styleStrikethrough.SelectedItem = StyleNewTag.StyleStrikethrough;
            f.comboBox_styleUnderline.SelectedItem = StyleNewTag.StyleUnderline;
            f.comboBox_stylePosition.SelectedItem = StyleNewTag.TextPosition;

            f.ShowDialog();

            if (f.SaveSettings)
            {
                StyleNewTag.FontSpecifyColor = f.checkBox_fontColor.Checked;
                StyleNewTag.FontSpecifyBackroundColor = f.checkBox_backroundColor.Checked;

                if (StyleNewTag.FontSpecifyColor)
                    StyleNewTag.FontColor = ColorTranslator.ToHtml(f.label_fontColorDisplay.BackColor);
                if (StyleNewTag.FontSpecifyBackroundColor)
                    StyleNewTag.FontBackroundColor = ColorTranslator.ToHtml(f.label_backroundColorDisplay.BackColor);

                StyleNewTag.StyleBold = f.comboBox_styleBold.SelectedItem.ToString();
                StyleNewTag.StyleItalic = f.comboBox_styleItalic.SelectedItem.ToString();
                StyleNewTag.StyleStrikethrough = f.comboBox_styleStrikethrough.SelectedItem.ToString();
                StyleNewTag.StyleUnderline = f.comboBox_styleUnderline.SelectedItem.ToString();
                StyleNewTag.TextPosition = f.comboBox_stylePosition.SelectedItem.ToString();
                UpdateVisualStyle();
            }
        }

        private void button_changeFormattingTagRemoved_Click(object sender, EventArgs e)
        {
            FormSettingsFormatting f = new FormSettingsFormatting();

            f.checkBox_fontColor.Checked = StyleRemovedTag.FontSpecifyColor;
            f.checkBox_backroundColor.Checked = StyleRemovedTag.FontSpecifyBackroundColor;

            if (f.checkBox_fontColor.Checked)
                f.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(StyleRemovedTag.FontColor);
            if (f.checkBox_backroundColor.Checked)
                f.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(StyleRemovedTag.FontBackroundColor);


            f.comboBox_styleBold.SelectedItem = StyleRemovedTag.StyleBold;
            f.comboBox_styleItalic.SelectedItem = StyleRemovedTag.StyleItalic;
            f.comboBox_styleStrikethrough.SelectedItem = StyleRemovedTag.StyleStrikethrough;
            f.comboBox_styleUnderline.SelectedItem = StyleRemovedTag.StyleUnderline;
            f.comboBox_stylePosition.SelectedItem = StyleRemovedTag.TextPosition;
            f.ShowDialog();

            if (f.SaveSettings)
            {
                StyleRemovedTag.FontSpecifyColor = f.checkBox_fontColor.Checked;
                StyleRemovedTag.FontSpecifyBackroundColor = f.checkBox_backroundColor.Checked;

                if (StyleRemovedTag.FontSpecifyColor)
                    StyleRemovedTag.FontColor = ColorTranslator.ToHtml(f.label_fontColorDisplay.BackColor);
                if (StyleRemovedTag.FontSpecifyBackroundColor)
                    StyleRemovedTag.FontBackroundColor = ColorTranslator.ToHtml(f.label_backroundColorDisplay.BackColor);

                StyleRemovedTag.StyleBold = f.comboBox_styleBold.SelectedItem.ToString();
                StyleRemovedTag.StyleItalic = f.comboBox_styleItalic.SelectedItem.ToString();
                StyleRemovedTag.StyleStrikethrough = f.comboBox_styleStrikethrough.SelectedItem.ToString();
                StyleRemovedTag.StyleUnderline = f.comboBox_styleUnderline.SelectedItem.ToString();
                StyleRemovedTag.TextPosition = f.comboBox_stylePosition.SelectedItem.ToString();
                UpdateVisualStyle();
            }
        }

        private void comboBox_comparisonType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ComparisonType = this.comboBox_comparisonType.SelectedIndex == 0 ? Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonType.Words : Sdl.Community.PostEdit.Compare.Core.Settings.ComparisonType.Characters;
        }

        private void checkBox_includeTagsInComparison_CheckedChanged(object sender, EventArgs e)
        {
            this.IncludeTagContentInComparison = checkBox_includeTagsInComparison.Checked;


        
            
        }

        private void linkLabel_resetToDefaults_comparer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            #region  |  general settings  |

            this.comboBox_comparisonType.SelectedItem = "words";
            this.checkBox_includeTagsInComparison.Checked = true;

            #endregion

            #region  |  new text style  |
            StyleNewText = new Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting();
            StyleNewText.StyleBold = "Deactivate";
            StyleNewText.StyleItalic = "Deactivate";
            StyleNewText.StyleStrikethrough = "Deactivate";
            StyleNewText.StyleUnderline = "Activate";
            StyleNewText.TextPosition = "Normal";
            StyleNewText.FontSpecifyColor = true;
            StyleNewText.FontColor = "#0000FF";
            StyleNewText.FontSpecifyBackroundColor = true;
            StyleNewText.FontBackroundColor = "#FFFF66";
            #endregion

            #region  |  removed text style  |
            StyleRemovedText = new Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting();
            StyleRemovedText.StyleBold = "Deactivate";
            StyleRemovedText.StyleItalic = "Deactivate";
            StyleRemovedText.StyleStrikethrough = "Activate";
            StyleRemovedText.StyleUnderline = "Deactivate";
            StyleRemovedText.TextPosition = "Normal";
            StyleRemovedText.FontSpecifyColor = true;
            StyleRemovedText.FontColor = "#FF0000";
            StyleRemovedText.FontSpecifyBackroundColor = false;
            StyleRemovedText.FontBackroundColor = "#FFFFFF";
            #endregion

            #region  |  new tag style  |
            StyleNewTag = new Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting();
            StyleNewTag.StyleBold = "Deactivate";
            StyleNewTag.StyleItalic = "Deactivate";
            StyleNewTag.StyleStrikethrough = "Deactivate";
            StyleNewTag.StyleUnderline = "Deactivate";
            StyleNewTag.TextPosition = "Normal";
            StyleNewTag.FontSpecifyColor = false;
            StyleNewTag.FontColor = "#000000";
            StyleNewTag.FontSpecifyBackroundColor = true;
            StyleNewTag.FontBackroundColor = "#DDEEFF";
            #endregion

            #region  |  removed tag style  |

            StyleRemovedTag = new Sdl.Community.PostEdit.Compare.Core.Settings.DifferencesFormatting();
            StyleRemovedTag.StyleBold = "Deactivate";
            StyleRemovedTag.StyleItalic = "Deactivate";
            StyleRemovedTag.StyleStrikethrough = "Deactivate";
            StyleRemovedTag.StyleUnderline = "Deactivate";
            StyleRemovedTag.TextPosition = "Normal";
            StyleRemovedTag.FontSpecifyColor = false;
            StyleRemovedTag.FontColor = "#000000";
            StyleRemovedTag.FontSpecifyBackroundColor = true;
            StyleRemovedTag.FontBackroundColor = "#FFE8E8";

            #endregion

            UpdateVisualStyle();

        }
        
        private void button_reportsAutoSaveFullPath_Click(object sender, EventArgs e)
        {
            try
            {
                string sPath = this.textBox_reportsAutoSaveFullPath.Text;

                if (!Directory.Exists(sPath))
                {
                    while (sPath.Contains("\\"))
                    {
                        sPath = sPath.Substring(0, sPath.LastIndexOf("\\"));
                        if (Directory.Exists(sPath))
                        {
                            break;
                        }
                    }
                }

                var fsd = new FolderSelectDialog();
                fsd.Title = "Select reports folder";
                fsd.InitialDirectory = sPath;
                if (fsd.ShowDialog(IntPtr.Zero))
                {
                    if (fsd.FileName.Trim() != string.Empty)
                    {
                        sPath = fsd.FileName;


                        this.textBox_reportsAutoSaveFullPath.Text = sPath;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {

            }
        }

        private void linkLabel_reports_viewFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            try
            {
                string sPath = this.textBox_reportsAutoSaveFullPath.Text;

                if (!Directory.Exists(sPath))
                {
                    while (sPath.Contains("\\"))
                    {
                        sPath = sPath.Substring(0, sPath.LastIndexOf("\\"));
                        if (Directory.Exists(sPath))
                        {
                            break;
                        }
                    }
                }

                System.Diagnostics.Process.Start(sPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {

            }
        
        }

        private void checkBox_reportsAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_reportsCreateMonthlySubFolders.Enabled = checkBox_reportsAutoSave.Checked;
        }
















        private void textBox_javaExecutablePath_TextChanged(object sender, EventArgs e)
        {
            if (textBox_javaExecutablePath.Text.Trim() != string.Empty
                && File.Exists(textBox_javaExecutablePath.Text))
            {
                checkBox_showSegmentTERPAnalysis.Enabled = true;
            }
            else
            {
                checkBox_showSegmentTERPAnalysis.Enabled = false;
                checkBox_showSegmentTERPAnalysis.Checked = false;
            }
        }

        private void button_javaExecutablePath_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog
            {
                Title = "Select the Java executable file",
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "Java executable file (java.exe)|java.exe",
                FilterIndex = 0
            };


            if (textBox_javaExecutablePath.Text.Trim() != string.Empty)
            {
                var dir = Path.GetDirectoryName(textBox_javaExecutablePath.Text);
                if (Directory.Exists(dir))                
                    f.InitialDirectory = dir;                                   
            }

            var dr = f.ShowDialog();
            if (dr != DialogResult.OK) 
                return;
            try
            {
                if (f.FileName != null && f.FileName.Trim() != string.Empty)                
                    textBox_javaExecutablePath.Text = f.FileName;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void javaWebsiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.oracle.com/technetwork/java/javase/downloads/index.html");
        }

       
       

      

       

       
       

     

     



    }
}
