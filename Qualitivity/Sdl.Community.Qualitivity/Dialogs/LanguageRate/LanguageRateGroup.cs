using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Custom;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Configuration;
using Sdl.Community.Structures.Rates;

namespace Sdl.Community.Qualitivity.Dialogs.LanguageRate
{
    public partial class LanguageRateGroup : Form
    {
        private ListViewSortManager _mSortMgr;

        public bool Saved { get; set; }

        public Sdl.Community.Structures.Rates.LanguageRateGroup PriceGroup { get; set; }
        public bool IsEdit { get; set; }

        public LanguageRateGroup()
        {
            InitializeComponent();

            _mSortMgr = new ListViewSortManager(
            listView_languages,
            new[] {  
                                                   typeof(ListViewTextSort),
							                       typeof(ListViewTextSort),
							                       typeof(ListViewTextSort)
						                       },
            0,
            SortOrder.Ascending
            );



            _mSortMgr = new ListViewSortManager(
                    listView_source,
                    new[] {  
                                                   typeof(ListViewTextSort),
							                       typeof(ListViewTextSort),
							                       typeof(ListViewTextSort)
						                       },
                    0,
                    SortOrder.Ascending
                    );


            _mSortMgr = new ListViewSortManager(
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


            textBox_name.Text = PriceGroup.Name;
            textBox_description.Text = PriceGroup.Description;


            if (!IsEdit)
                PriceGroup.Currency = Tracked.Settings.GetGeneralProperty("defaultCurrency").Value;

           
            comboBox_currency.BeginUpdate();
            foreach (var c in Tracked.Currencies)            
                comboBox_currency.Items.Add(new ComboboxItem(c.Name + "  (" + c.Country + ")", c));
            
            comboBox_currency.EndUpdate();


            var iSelectedIndex = -1;
            var iDefaultIndex = 0;
            for (var i = 0; i < comboBox_currency.Items.Count; i++)
            {
                var cbi = (ComboboxItem)comboBox_currency.Items[i];

                var c = (Currency)cbi.Value;
                if (string.Compare(c.Name, PriceGroup.Currency, StringComparison.OrdinalIgnoreCase) != 0) continue;
                iSelectedIndex = i;
                break;
            }
            comboBox_currency.SelectedIndex = iSelectedIndex > -1 ? iSelectedIndex : iDefaultIndex;

        

          
            listView_languages.Items.Clear();
            foreach (var ci in CultureInfo.GetCultures( CultureTypes.SpecificCultures))
            {
                var language = new LanguageRateGroupLanguage();
                language.Type = LanguageRateGroupLanguage.LanguageType.None;
                language.LanguageIdCi = ci.Name;

                var itmx = listView_languages.Items.Add(ci.Name);

                itmx.SubItems.Add(ci.TwoLetterISOLanguageName);
                itmx.SubItems.Add(ci.EnglishName);
                itmx.UseItemStyleForSubItems = false;
                itmx.BackColor = Color.Beige;
                itmx.Tag = language;
                
            }


            listView_source.Items.Clear();
            foreach (var language in PriceGroup.GroupLanguages)
            {
                if (language.Type != LanguageRateGroupLanguage.LanguageType.Source) continue;
                CultureInfo ci = null;
                if (language.LanguageIdCi != "*")
                    ci = new CultureInfo(language.LanguageIdCi);

                var itmx = listView_source.Items.Add(language.LanguageIdCi);
                itmx.SubItems.Add(ci != null ? ci.TwoLetterISOLanguageName : "*");
                itmx.SubItems.Add(ci != null ? ci.TwoLetterISOLanguageName : "*");
                itmx.UseItemStyleForSubItems = false;
                itmx.BackColor = Color.Beige;
                itmx.Tag = language;
            }

            listView_target.Items.Clear();
            foreach (var language in PriceGroup.GroupLanguages)
            {
                if (language.Type != LanguageRateGroupLanguage.LanguageType.Target) continue;
                CultureInfo ci = null;
                if (language.LanguageIdCi != "*")
                    ci = new CultureInfo(language.LanguageIdCi);
                var itmx = listView_target.Items.Add(language.LanguageIdCi);
                itmx.SubItems.Add(ci != null ? ci.TwoLetterISOLanguageName : "*");
                itmx.SubItems.Add(ci != null ? ci.TwoLetterISOLanguageName : "*");
                itmx.UseItemStyleForSubItems = false;
                itmx.BackColor = Color.Beige;
                itmx.Tag = language;
            }



            numericUpDown_percentagePerfect.Value = PriceGroup.DefaultAnalysisBand.PercentPm;
            numericUpDown_percentageContext.Value = PriceGroup.DefaultAnalysisBand.PercentCm;
            numericUpDown_percentageRepetitions.Value = PriceGroup.DefaultAnalysisBand.PercentRep;
            numericUpDown_percentageExact.Value = PriceGroup.DefaultAnalysisBand.Percent100;
            numericUpDown_percentageFuzzy99.Value = PriceGroup.DefaultAnalysisBand.Percent95;
            numericUpDown_percentageFuzzy94.Value = PriceGroup.DefaultAnalysisBand.Percent85;
            numericUpDown_percentageFuzzy84.Value = PriceGroup.DefaultAnalysisBand.Percent75;
            numericUpDown_percentageFuzzy74.Value = PriceGroup.DefaultAnalysisBand.Percent50;
            numericUpDown_percentageNew.Value = PriceGroup.DefaultAnalysisBand.PercentNew;


        }

      

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (textBox_name.Text.Trim() == string.Empty) return;
            PriceGroup.Name = textBox_name.Text;
            PriceGroup.Description = textBox_description.Text;


            var cbi = (ComboboxItem)comboBox_currency.Items[comboBox_currency.SelectedIndex];
            var c = (Currency)cbi.Value;
            PriceGroup.Currency = c.Name;
              
            PriceGroup.GroupLanguages.Clear();

                
            foreach (ListViewItem itmx in listView_source.Items)
            {
                var language = (LanguageRateGroupLanguage)itmx.Tag;
                language.Type = LanguageRateGroupLanguage.LanguageType.Source;
                PriceGroup.GroupLanguages.Add(language);
            }
                

            foreach (ListViewItem itmx in listView_target.Items)
            {
                var language = (LanguageRateGroupLanguage)itmx.Tag;
                language.Type = LanguageRateGroupLanguage.LanguageType.Target;
                PriceGroup.GroupLanguages.Add(language);
            }

            var allLanguageRates = new List<Sdl.Community.Structures.Rates.LanguageRate>();
            #region  |  add any new rates  to the list  |
            foreach (var languageRate in PriceGroup.LanguageRates)
            {
                var foundSourceLanguage = false;
                var foundTargetLanguage = false;
                foreach (var language in PriceGroup.GroupLanguages)
                {
                    if (string.Compare(language.LanguageIdCi, languageRate.SourceLanguage, StringComparison.OrdinalIgnoreCase) == 0)
                        foundSourceLanguage = true;

                    if (string.Compare(language.LanguageIdCi, languageRate.TargetLanguage, StringComparison.OrdinalIgnoreCase) == 0)
                        foundTargetLanguage = true;
                }

                if (foundSourceLanguage && foundTargetLanguage)
                    allLanguageRates.Add(languageRate);
            }
            #endregion
            PriceGroup.LanguageRates = allLanguageRates;



            PriceGroup.DefaultAnalysisBand.PercentPm = Convert.ToInt32(numericUpDown_percentagePerfect.Value);
            PriceGroup.DefaultAnalysisBand.PercentCm= Convert.ToInt32(numericUpDown_percentageContext.Value);
            PriceGroup.DefaultAnalysisBand.PercentRep = Convert.ToInt32(numericUpDown_percentageRepetitions.Value);
            PriceGroup.DefaultAnalysisBand.Percent100 = Convert.ToInt32(numericUpDown_percentageExact.Value);
            PriceGroup.DefaultAnalysisBand.Percent95 = Convert.ToInt32(numericUpDown_percentageFuzzy99.Value);
            PriceGroup.DefaultAnalysisBand.Percent85 = Convert.ToInt32(numericUpDown_percentageFuzzy94.Value);
            PriceGroup.DefaultAnalysisBand.Percent75 = Convert.ToInt32(numericUpDown_percentageFuzzy84.Value);
            PriceGroup.DefaultAnalysisBand.Percent50 = Convert.ToInt32(numericUpDown_percentageFuzzy74.Value);
            PriceGroup.DefaultAnalysisBand.PercentNew = Convert.ToInt32(numericUpDown_percentageNew.Value);

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
            var selectedLanguages = (from ListViewItem itmx in listView_languages.Items where itmx.Selected select (LanguageRateGroupLanguage) itmx.Tag).ToList();

            var sourceLanguageIndex = (from ListViewItem itmx in listView_source.Items select (LanguageRateGroupLanguage) itmx.Tag into language select language.LanguageIdCi).ToList();
            foreach (var language in selectedLanguages)
            {
                CultureInfo ci = null;
                if (language.LanguageIdCi != "*")
                    ci = new CultureInfo(language.LanguageIdCi);

                if (sourceLanguageIndex.Contains(language.LanguageIdCi)) continue;
                sourceLanguageIndex.Add(language.LanguageIdCi);
                var itmx = listView_source.Items.Add(language.LanguageIdCi);

                itmx.SubItems.Add(ci != null ? ci.TwoLetterISOLanguageName : "*");
                itmx.SubItems.Add(ci != null ? ci.TwoLetterISOLanguageName : "*");
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
                var language = (LanguageRateGroupLanguage)itmx.Tag;
                if (language.LanguageIdCi != "*")
                    itmx.Remove();
            }
        }

        private void button_languageRemoveAll_soruce_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itmx in listView_source.Items)
            {

                var language = (LanguageRateGroupLanguage)itmx.Tag;
                if (language.LanguageIdCi != "*")
                    itmx.Remove();

            }
        }

        private void button_languageAdd_target_Click(object sender, EventArgs e)
        {
            var selectedLanguages = (
                from ListViewItem itmx in listView_languages.Items 
                where itmx.Selected 
                select (LanguageRateGroupLanguage) itmx.Tag).ToList();

            var targetLanguageIndex = (
                from ListViewItem itmx 
                    in listView_target.Items 
                    select (LanguageRateGroupLanguage) itmx.Tag 
                        into language 
                        select language.LanguageIdCi).ToList();
            foreach (var language in selectedLanguages)
            {
                CultureInfo ci = null;
                if (language.LanguageIdCi != "*")
                    ci = new CultureInfo(language.LanguageIdCi);

                if (targetLanguageIndex.Contains(language.LanguageIdCi)) continue;
                targetLanguageIndex.Add(language.LanguageIdCi);
                var itmx = listView_target.Items.Add(language.LanguageIdCi);

                itmx.SubItems.Add(ci != null ? ci.TwoLetterISOLanguageName : "*");
                itmx.SubItems.Add(ci != null ? ci.TwoLetterISOLanguageName : "*");
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
                var language = (LanguageRateGroupLanguage)itmx.Tag;
                if (language.LanguageIdCi != "*")                    
                    itmx.Remove();
            }
        }

        private void button_languageRemoveAll_target_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itmx in listView_target.Items)
            {
                var language = (LanguageRateGroupLanguage)itmx.Tag;
                if (language.LanguageIdCi != "*")                
                    itmx.Remove();
                
            }
        }

       
    }
}
