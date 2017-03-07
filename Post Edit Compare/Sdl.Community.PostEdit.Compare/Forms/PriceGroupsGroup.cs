using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PostEdit.Compare.ListViewSortManager;

namespace PostEdit.Compare.Forms
{
    public partial class PriceGroupsGroup : Form
    {
        private ListViewSortManager.ListViewSortManager _mSortMgr;

        public bool Saved { get; set; }

        public Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup LanguageGroup { get; set; }
        public bool IsEdit { get; set; }

        public PriceGroupsGroup()
        {
            InitializeComponent();

            _mSortMgr = new ListViewSortManager.ListViewSortManager(
            listView_languages,
            new[] {  
                                                   typeof(ListViewTextSort),
							                       typeof(ListViewTextSort),
							                       typeof(ListViewTextSort)
						                       },
            0,
            SortOrder.Ascending
            );



            _mSortMgr = new ListViewSortManager.ListViewSortManager(
                    listView_source,
                    new[] {  
                                                   typeof(ListViewTextSort),
							                       typeof(ListViewTextSort),
							                       typeof(ListViewTextSort)
						                       },
                    0,
                    SortOrder.Ascending
                    );


            _mSortMgr = new ListViewSortManager.ListViewSortManager(
                    listView_target,
                    new[] {  
                                                   typeof(ListViewTextSort),
							                       typeof(ListViewTextSort),
							                       typeof(ListViewTextSort)
						                       },
                    0,
                    SortOrder.Ascending
                    );

        }

    

     

        private void PriceGroupsGroup_Load(object sender, EventArgs e)
        {


            textBox_name.Text = LanguageGroup.Name;
            textBox_description.Text = LanguageGroup.Description;
            comboBox_currency.Text = LanguageGroup.Currency;

            listView_languages.Items.Clear();
            foreach (var ci in CultureInfo.GetCultures( CultureTypes.SpecificCultures))
            {
                var language = new Sdl.Community.PostEdit.Compare.Core.Settings.Language(ci.Name, "", ci.TwoLetterISOLanguageName, ci.EnglishName);

                
                var itmx = listView_languages.Items.Add(language.Id);
                itmx.SubItems.Add(language.Iso2);
                itmx.SubItems.Add(language.Name);
                itmx.UseItemStyleForSubItems = false;
                itmx.BackColor = Color.Beige;
                itmx.Tag = language;
            
            }


            listView_source.Items.Clear();
            foreach (var language in LanguageGroup.SourceLanguages)
            {
                ListViewItem itmx = listView_source.Items.Add(language.Id);
                itmx.SubItems.Add(language.Iso2);
                itmx.SubItems.Add(language.Name);
                itmx.UseItemStyleForSubItems = false;
                itmx.BackColor = Color.Beige;
                itmx.Tag = language;
            }

            listView_target.Items.Clear();
            foreach (var language in LanguageGroup.TargetLanguages)
            {
                ListViewItem itmx = listView_target.Items.Add(language.Id);
                itmx.SubItems.Add(language.Iso2);
                itmx.SubItems.Add(language.Name);
                itmx.UseItemStyleForSubItems = false;
                itmx.BackColor = Color.Beige;
                itmx.Tag = language;
            }



            numericUpDown_percentagePerfect.Value = LanguageGroup.DefaultAnalysisBand.PercentagePerfect;
            numericUpDown_percentageContext.Value = LanguageGroup.DefaultAnalysisBand.PercentageContext;
            numericUpDown_percentageRepetitions.Value = LanguageGroup.DefaultAnalysisBand.PercentageRepetition;
            numericUpDown_percentageExact.Value = LanguageGroup.DefaultAnalysisBand.PercentageExact;
            numericUpDown_percentageFuzzy99.Value = LanguageGroup.DefaultAnalysisBand.Percentage9599;
            numericUpDown_percentageFuzzy94.Value = LanguageGroup.DefaultAnalysisBand.Percentage8594;
            numericUpDown_percentageFuzzy84.Value = LanguageGroup.DefaultAnalysisBand.Percentage7584;
            numericUpDown_percentageFuzzy74.Value = LanguageGroup.DefaultAnalysisBand.Percentage5074;
            numericUpDown_percentageNew.Value = LanguageGroup.DefaultAnalysisBand.PercentageNew;


            
        }

      

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (textBox_name.Text.Trim() == string.Empty) return;
            LanguageGroup.Name = textBox_name.Text;
            LanguageGroup.Description = textBox_description.Text;
            LanguageGroup.Currency = comboBox_currency.Text;

            LanguageGroup.SourceLanguages.Clear();
            foreach (ListViewItem itmx in listView_source.Items)
            {
                var language = (Sdl.Community.PostEdit.Compare.Core.Settings.Language)itmx.Tag;
                LanguageGroup.SourceLanguages.Add(language);
            }
            LanguageGroup.TargetLanguages.Clear();
            foreach (ListViewItem itmx in listView_target.Items)
            {
                var language = (Sdl.Community.PostEdit.Compare.Core.Settings.Language)itmx.Tag;
                LanguageGroup.TargetLanguages.Add(language);
            }

            var newGroupPrices = new List<Sdl.Community.PostEdit.Compare.Core.Settings.Price>();
            foreach (Sdl.Community.PostEdit.Compare.Core.Settings.Price price in LanguageGroup.GroupPrices)
            {
                var foundSourceLanguage = LanguageGroup.SourceLanguages.Any(language => string.Compare(language.Id, price.Source, StringComparison.OrdinalIgnoreCase) == 0);
                bool foundTargetLanguage = LanguageGroup.TargetLanguages.Any(language => string.Compare(language.Id, price.Target, StringComparison.OrdinalIgnoreCase) == 0);

                if (foundSourceLanguage && foundTargetLanguage)
                {
                    newGroupPrices.Add(price);
                }
            }
            LanguageGroup.GroupPrices = newGroupPrices;



            LanguageGroup.DefaultAnalysisBand.PercentagePerfect = Convert.ToInt32(numericUpDown_percentagePerfect.Value);
            LanguageGroup.DefaultAnalysisBand.PercentageContext = Convert.ToInt32(numericUpDown_percentageContext.Value);
            LanguageGroup.DefaultAnalysisBand.PercentageRepetition = Convert.ToInt32(numericUpDown_percentageRepetitions.Value);
            LanguageGroup.DefaultAnalysisBand.PercentageExact = Convert.ToInt32(numericUpDown_percentageExact.Value);
            LanguageGroup.DefaultAnalysisBand.Percentage9599 = Convert.ToInt32(numericUpDown_percentageFuzzy99.Value);
            LanguageGroup.DefaultAnalysisBand.Percentage8594 = Convert.ToInt32(numericUpDown_percentageFuzzy94.Value);
            LanguageGroup.DefaultAnalysisBand.Percentage7584 = Convert.ToInt32(numericUpDown_percentageFuzzy84.Value);
            LanguageGroup.DefaultAnalysisBand.Percentage5074 = Convert.ToInt32(numericUpDown_percentageFuzzy74.Value);
            LanguageGroup.DefaultAnalysisBand.PercentageNew = Convert.ToInt32(numericUpDown_percentageNew.Value);

            Saved = true;
            Close();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void button_languageAdd_source_Click(object sender, EventArgs e)
        {
            var selectedLanguages = (from ListViewItem itmx in listView_languages.Items where itmx.Selected select (Sdl.Community.PostEdit.Compare.Core.Settings.Language) itmx.Tag).ToList();

            var sourceLanguageIndex = (from ListViewItem itmx in listView_source.Items select (Sdl.Community.PostEdit.Compare.Core.Settings.Language) itmx.Tag into language select language.Id).ToList();
            foreach (var language in selectedLanguages)
            {
                if (sourceLanguageIndex.Contains(language.Id)) continue;
                sourceLanguageIndex.Add(language.Id);
                var itmx = listView_source.Items.Add(language.Id);
                itmx.SubItems.Add(language.Iso2);
                itmx.SubItems.Add(language.Name);
                itmx.UseItemStyleForSubItems = false;
                itmx.BackColor = Color.Beige;
                itmx.Tag = language;
            }

        }

        private void button_languageRemove_source_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itmx in listView_source.Items)
            {
                if (!itmx.Selected) continue;
                var language = (Sdl.Community.PostEdit.Compare.Core.Settings.Language)itmx.Tag;
                if (language.Id != "*")
                {
                    itmx.Remove();
                }
            }
        }

        private void button_languageRemoveAll_soruce_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itmx in listView_source.Items)
            {
                var language = (Sdl.Community.PostEdit.Compare.Core.Settings.Language)itmx.Tag;
                if (language.Id != "*")
                {
                    itmx.Remove();
                }
            }
        }

        private void button_languageAdd_target_Click(object sender, EventArgs e)
        {
            var selectedLanguages = (from ListViewItem itmx in listView_languages.Items where itmx.Selected select (Sdl.Community.PostEdit.Compare.Core.Settings.Language) itmx.Tag).ToList();

            var targetLanguageIndex = (from ListViewItem itmx in listView_target.Items select (Sdl.Community.PostEdit.Compare.Core.Settings.Language) itmx.Tag into language select language.Id).ToList();
            foreach (var language in selectedLanguages)
            {
                if (targetLanguageIndex.Contains(language.Id)) continue;
                targetLanguageIndex.Add(language.Id);
                var itmx = listView_target.Items.Add(language.Id);
                itmx.SubItems.Add(language.Iso2);
                itmx.SubItems.Add(language.Name);
                itmx.UseItemStyleForSubItems = false;
                itmx.BackColor = Color.Beige;
                itmx.Tag = language;
            }
        }

        private void button_languageRemove_taret_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itmx in listView_target.Items)
            {
                if (!itmx.Selected) continue;
                var language = (Sdl.Community.PostEdit.Compare.Core.Settings.Language)itmx.Tag;
                if (language.Id != "*")
                {
                    itmx.Remove();
                }
            }
        }

        private void button_languageRemoveAll_target_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itmx in listView_target.Items)
            {
                var language = (Sdl.Community.PostEdit.Compare.Core.Settings.Language)itmx.Tag;
                if (language.Id != "*")
                {
                    itmx.Remove();
                }
            }
        }

       
    }
}
