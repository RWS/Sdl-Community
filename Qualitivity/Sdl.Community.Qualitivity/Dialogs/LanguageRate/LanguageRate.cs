using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Sdl.Community.Structures.Rates;
using Sdl.Community.Structures.Rates.Base;

namespace Sdl.Community.Qualitivity.Dialogs.LanguageRate
{
    public partial class LanguageRate : Form
    {
        public Dictionary<string, List<string>> LanguageCombinations { get; private set; }

        public LanguageRate(Dictionary<string, List<string>> languageCombinations)
        {
            LanguageCombinations = languageCombinations;
            InitializeComponent();
        }


        public bool Saved { get; set; }
        public Sdl.Community.Structures.Rates.LanguageRate Rate { get; set; }
        public Sdl.Community.Structures.Rates.LanguageRateGroup RateGroup { get; set; }


        public bool IsEdit { get; set; }
        public bool IsEditMultiple { get; set; }


        private void ResizeGui()
        {

            groupBox_priceProperties.Top = groupBox_typeProperties.Top;
            groupBox_typeProperties.Visible = false;

            button_cancel.Top = 442;
            button_save.Top = 442;

            Height = 512;

        }

        private void SetBasePercentageBase()
        {
            if (IsEdit && Rate.BaseRate > 0)
            {
                if (Rate.RatePm > 0)
                    numericUpDown_percentagePerfect.Value = Rate.RatePm / Rate.BaseRate * 100;
                else
                    numericUpDown_percentagePerfect.Value = 0;

                if (Rate.RateCm > 0)
                    numericUpDown_percentageContext.Value = Rate.RateCm / Rate.BaseRate * 100;
                else
                    numericUpDown_percentageContext.Value = 0;

                if (Rate.RateRep > 0)
                    numericUpDown_percentageRepetitions.Value = Rate.RateRep / Rate.BaseRate * 100;
                else
                    numericUpDown_percentageRepetitions.Value = 0;

                if (Rate.Rate100 > 0)
                    numericUpDown_percentageExact.Value = Rate.Rate100 / Rate.BaseRate * 100;
                else
                    numericUpDown_percentageExact.Value = 0;

                if (Rate.Rate95 > 0)
                    numericUpDown_percentageFuzzy99.Value = Rate.Rate95 / Rate.BaseRate * 100;
                else
                    numericUpDown_percentageFuzzy99.Value = 0;

                if (Rate.Rate85 > 0)
                    numericUpDown_percentageFuzzy94.Value = Rate.Rate85 / Rate.BaseRate * 100;
                else
                    numericUpDown_percentageFuzzy94.Value = 0;

                if (Rate.Rate75 > 0)
                    numericUpDown_percentageFuzzy84.Value = Rate.Rate75 / Rate.BaseRate * 100;
                else
                    numericUpDown_percentageFuzzy84.Value = 0;

                if (Rate.Rate50 > 0)
                    numericUpDown_percentageFuzzy74.Value = Rate.Rate50 / Rate.BaseRate * 100;
                else
                    numericUpDown_percentageFuzzy74.Value = 0;

                if (Rate.RateNew > 0)
                    numericUpDown_percentageNew.Value = Rate.RateNew / Rate.BaseRate * 100;
                else
                    numericUpDown_percentageNew.Value = 0;
            }
            else
            {
                numericUpDown_percentagePerfect.Value = RateGroup.DefaultAnalysisBand.PercentPm;
                numericUpDown_percentageContext.Value = RateGroup.DefaultAnalysisBand.PercentCm;
                numericUpDown_percentageRepetitions.Value = RateGroup.DefaultAnalysisBand.PercentRep;
                numericUpDown_percentageExact.Value = RateGroup.DefaultAnalysisBand.Percent100;
                numericUpDown_percentageFuzzy99.Value = RateGroup.DefaultAnalysisBand.Percent95;
                numericUpDown_percentageFuzzy94.Value = RateGroup.DefaultAnalysisBand.Percent85;
                numericUpDown_percentageFuzzy84.Value = RateGroup.DefaultAnalysisBand.Percent75;
                numericUpDown_percentageFuzzy74.Value = RateGroup.DefaultAnalysisBand.Percent50;
                numericUpDown_percentageNew.Value = RateGroup.DefaultAnalysisBand.PercentNew;
            }
        }

        
        private bool IsSettingBasePercentage { get; set; }
        private bool IsSettingBasePrice { get; set; }


        private void EnableButtons()
        {

            if (comboBox_sourceLanguages.SelectedItem == null
                || comboBox_targetLanguages.SelectedItem == null)
            {
                if (IsEdit)
                {
                    button_save.Enabled = numericUpDown_priceBase.Value > 0;
                }
                else
                {
                    button_save.Enabled = false;
                }
            }
            else
            {
                button_save.Enabled = numericUpDown_priceBase.Value > 0;
            }
        }

        private void FormLanguageRate_Load(object sender, EventArgs e)
        {

            try
            {
                IsSettingBasePercentage = true;

                if (IsEdit)
                {
                    comboBox_sourceLanguages.Enabled = false;
                    comboBox_targetLanguages.Enabled = false;
                    if (IsEditMultiple)
                        ResizeGui();
                    Text += @" (Edit)";
                }
                else
                {
                    Text += @" (New)";
                }

                SetBasePercentageBase();


                #region  |  comboBox_sourceLanguages  |
                try
                {
                    comboBox_sourceLanguages.BeginUpdate();
                    comboBox_sourceLanguages.Items.Clear();

                    var selectedIndex = 0;
                    foreach (var language in RateGroup.GroupLanguages)
                    {
                        CultureInfo ci = null;
                        if (language.LanguageIdCi != "*")
                            ci = new CultureInfo(language.LanguageIdCi);

                        if (language.Type == LanguageRateGroupLanguage.LanguageType.Source)
                            comboBox_sourceLanguages.Items.Add(language.LanguageIdCi + " - " + (ci != null ? ci.EnglishName : "{all}"));
                    }

                    comboBox_sourceLanguages.Sorted = true;
                    for (var i = 0; i < comboBox_sourceLanguages.Items.Count; i++)
                    {
                        var languageText = comboBox_sourceLanguages.Items[i].ToString();
                        var sourceLanguageId = languageText.Substring(0, languageText.IndexOf(" ", StringComparison.Ordinal));
                        if (string.Compare(sourceLanguageId, Rate.SourceLanguage, StringComparison.OrdinalIgnoreCase) !=
                            0) continue;
                        selectedIndex = i;
                        break;
                    }
                    comboBox_sourceLanguages.SelectedIndex = selectedIndex;
                }
                finally
                {
                    comboBox_sourceLanguages.EndUpdate();
                }
                #endregion
                #region  |  comboBox_targetLanguages  |
                try
                {
                    comboBox_targetLanguages.BeginUpdate();
                    comboBox_targetLanguages.Items.Clear();

                    var selectedIndex = 0;
                    foreach (var language in RateGroup.GroupLanguages)
                    {
                        CultureInfo ci = null;
                        if (language.LanguageIdCi != "*")
                            ci = new CultureInfo(language.LanguageIdCi);

                        if (language.Type == LanguageRateGroupLanguage.LanguageType.Target)
                            comboBox_targetLanguages.Items.Add(language.LanguageIdCi + " - " + (ci != null ? ci.EnglishName : "{all}"));
                    }
                    comboBox_targetLanguages.Sorted = true;
                    for (var i = 0; i < comboBox_targetLanguages.Items.Count; i++)
                    {
                        var languageText = comboBox_targetLanguages.Items[i].ToString();
                        var targetLanguageId = languageText.Substring(0, languageText.IndexOf(" ", StringComparison.Ordinal));
                        if (string.Compare(targetLanguageId, Rate.TargetLanguage, StringComparison.OrdinalIgnoreCase) !=
                            0) continue;
                        selectedIndex = i;
                        break;
                    }
                    comboBox_targetLanguages.SelectedIndex = selectedIndex;
                }
                finally
                {
                    comboBox_targetLanguages.EndUpdate();
                }
                #endregion

                numericUpDown_priceBase.Value = Rate.BaseRate;

                //0=RoundUp, 1=RoundDown, 2=Round
                comboBox_RoundType.SelectedIndex = (int)Rate.RndType;



                numericUpDown_pricePerfect.Value = Rate.RatePm;
                numericUpDown_priceContext.Value = Rate.RateCm;
                numericUpDown_priceRepetitions.Value = Rate.RateRep;
                numericUpDown_priceExact.Value = Rate.Rate100;
                numericUpDown_priceFuzzy99.Value = Rate.Rate95;
                numericUpDown_priceFuzzy94.Value = Rate.Rate85;
                numericUpDown_priceFuzzy84.Value = Rate.Rate75;
                numericUpDown_priceFuzzy74.Value = Rate.Rate50;
                numericUpDown_priceFuzzyNew.Value = Rate.RateNew;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

                EnableButtons();

                IsSettingBasePercentage = false;
            }
        }




        private void GetBasePriceOnNew()
        {
            if (!IsEdit)
            {
                foreach (var price in RateGroup.LanguageRates)
                {
                    var sourceId = comboBox_sourceLanguages.SelectedItem.ToString().Substring(0, comboBox_sourceLanguages.SelectedItem.ToString().IndexOf(" ", StringComparison.Ordinal));
                    var targetId = comboBox_targetLanguages.SelectedItem.ToString().Substring(0, comboBox_targetLanguages.SelectedItem.ToString().IndexOf(" ", StringComparison.Ordinal));


                    if (string.Compare(price.SourceLanguage, sourceId, StringComparison.OrdinalIgnoreCase) != 0 ||
                        string.Compare(price.TargetLanguage, targetId, StringComparison.OrdinalIgnoreCase) != 0)
                        continue;
                    numericUpDown_priceBase.Value = price.BaseRate;
                    break;
                }
            }
        }


        private void AutoCalculateAnalysisBandPrices()
        {
            try
            {
                IsSettingBasePrice = true;
                if (numericUpDown_priceBase.Value > 0)
                {

                    switch (comboBox_RoundType.SelectedIndex)
                    {
                        case 0:

                            #region  |  this.comboBox_RoundType.SelectedIndex == 0  |
                            if (numericUpDown_percentagePerfect.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_pricePerfect.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_pricePerfect.Value = 0;


                            if (numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceContext.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceContext.Value = 0;


                            if (numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceRepetitions.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceRepetitions.Value = 0;


                            if (numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceExact.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceExact.Value = 0;


                            if (numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzy99.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy99.Value = 0;


                            if (numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzy94.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy94.Value = 0;


                            if (numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzy84.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy84.Value = 0;


                            if (numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzy74.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy74.Value = 0;


                            if (numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzyNew.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzyNew.Value = 0;
                            break;

                            #endregion

                        case 1:

                            #region  |  this.comboBox_RoundType.SelectedIndex == 1  |

                            var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                            var decimalLen = Convert.ToInt32(strDecimalLen);

                            if (numericUpDown_percentagePerfect.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_pricePerfect.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_pricePerfect.Value = 0;



                            if (numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_priceContext.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceContext.Value = 0;



                            if (numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_priceRepetitions.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceRepetitions.Value = 0;



                            if (numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_priceExact.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceExact.Value = 0;



                            if (numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzy99.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy99.Value = 0;



                            if (numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzy94.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy94.Value = 0;



                            if (numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzy84.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy84.Value = 0;



                            if (numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzy74.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy74.Value = 0;



                            if (numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzyNew.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzyNew.Value = 0;
                            break;

                            #endregion

                        case 2:

                            #region  |  this.comboBox_RoundType.SelectedIndex == 2  |

                            if (numericUpDown_percentagePerfect.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_pricePerfect.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_pricePerfect.Value = 0;




                            if (numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_priceContext.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceContext.Value = 0;



                            if (numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_priceRepetitions.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceRepetitions.Value = 0;



                            if (numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_priceExact.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceExact.Value = 0;



                            if (numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_priceFuzzy99.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy99.Value = 0;




                            if (numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_priceFuzzy94.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy94.Value = 0;




                            if (numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_priceFuzzy84.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy84.Value = 0;



                            if (numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_priceFuzzy74.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy74.Value = 0;



                            if (numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M), Convert.ToInt32(3));
                                numericUpDown_priceFuzzyNew.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzyNew.Value = 0;
                            break;

                            #endregion
                    }

                    numericUpDown_pricePerfect.DecimalPlaces = Convert.ToInt32(3);
                    numericUpDown_priceContext.DecimalPlaces = Convert.ToInt32(3);
                    numericUpDown_priceRepetitions.DecimalPlaces = Convert.ToInt32(3);
                    numericUpDown_priceExact.DecimalPlaces = Convert.ToInt32(3);
                    numericUpDown_priceFuzzy99.DecimalPlaces = Convert.ToInt32(3);
                    numericUpDown_priceFuzzy94.DecimalPlaces = Convert.ToInt32(3);
                    numericUpDown_priceFuzzy84.DecimalPlaces = Convert.ToInt32(3);
                    numericUpDown_priceFuzzy74.DecimalPlaces = Convert.ToInt32(3);
                    numericUpDown_priceFuzzyNew.DecimalPlaces = Convert.ToInt32(3);

                }

            }
            finally
            {
                IsSettingBasePrice = false;
            }
            EnableButtons();
        }


        private void SaveRecord()
        {
            AutoCalculateAnalysisBandPrices();

            if (!button_save.Enabled)
                return;

            Rate = new Sdl.Community.Structures.Rates.LanguageRate();


            if (!IsEdit)
            {

                var sourceLanguageId = comboBox_sourceLanguages.SelectedItem.ToString().Substring(0, comboBox_sourceLanguages.SelectedItem.ToString().IndexOf(" ", StringComparison.Ordinal));
                var targetLanguageId = comboBox_targetLanguages.SelectedItem.ToString().Substring(0, comboBox_targetLanguages.SelectedItem.ToString().IndexOf(" ", StringComparison.Ordinal));
                Rate.SourceLanguage = sourceLanguageId;
                Rate.TargetLanguage = targetLanguageId;
            }

            Rate.BaseRate = numericUpDown_priceBase.Value;


            Rate.RndType = (RoundType)comboBox_RoundType.SelectedIndex;


            Rate.RatePm = numericUpDown_pricePerfect.Value;
            Rate.RateCm = numericUpDown_priceContext.Value;
            Rate.RateRep = numericUpDown_priceRepetitions.Value;
            Rate.Rate100 = numericUpDown_priceExact.Value;
            Rate.Rate95 = numericUpDown_priceFuzzy99.Value;
            Rate.Rate85 = numericUpDown_priceFuzzy94.Value;
            Rate.Rate75 = numericUpDown_priceFuzzy84.Value;
            Rate.Rate50 = numericUpDown_priceFuzzy74.Value;
            Rate.RateNew = numericUpDown_priceFuzzyNew.Value;


            Saved = true;
            Close();



        }


        private void comboBox_RoundType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_priceBase_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                IsSettingBasePrice = true;
                numericUpDown_pricePerfect.Maximum = numericUpDown_priceBase.Value;
                numericUpDown_priceContext.Maximum = numericUpDown_priceBase.Value;
                numericUpDown_priceRepetitions.Maximum = numericUpDown_priceBase.Value;
                numericUpDown_priceExact.Maximum = numericUpDown_priceBase.Value;
                numericUpDown_priceFuzzy99.Maximum = numericUpDown_priceBase.Value;
                numericUpDown_priceFuzzy94.Maximum = numericUpDown_priceBase.Value;
                numericUpDown_priceFuzzy84.Maximum = numericUpDown_priceBase.Value;
                numericUpDown_priceFuzzy74.Maximum = numericUpDown_priceBase.Value;
                numericUpDown_priceFuzzyNew.Maximum = numericUpDown_priceBase.Value;
            }
            finally
            {
                IsSettingBasePrice = false;
            }

            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentagePerfect_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {
                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:
                        if (numericUpDown_percentagePerfect.Value > 0)
                            numericUpDown_pricePerfect.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);

                        if (numericUpDown_percentagePerfect.Value > 0)
                            numericUpDown_pricePerfect.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:
                        if (numericUpDown_percentagePerfect.Value > 0)
                            numericUpDown_pricePerfect.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M), Convert.ToInt32(3));
                        break;
                }
            }
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentageContext_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {

                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:

                        if (numericUpDown_percentageContext.Value > 0)
                            numericUpDown_priceContext.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);



                        if (numericUpDown_percentageContext.Value > 0)
                            numericUpDown_priceContext.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:

                        if (numericUpDown_percentageContext.Value > 0)
                            numericUpDown_priceContext.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M), Convert.ToInt32(3));
                        break;
                }

            }
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentageRepetitions_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {

                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:

                        if (numericUpDown_percentageRepetitions.Value > 0)
                            numericUpDown_priceRepetitions.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);

                        if (numericUpDown_percentageRepetitions.Value > 0)
                            numericUpDown_priceRepetitions.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:


                        if (numericUpDown_percentageRepetitions.Value > 0)
                            numericUpDown_priceRepetitions.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M), Convert.ToInt32(3));
                        break;
                }
            }
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentageExact_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {

                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:

                        if (numericUpDown_percentageExact.Value > 0)
                            numericUpDown_priceExact.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);


                        if (numericUpDown_percentageExact.Value > 0)
                            numericUpDown_priceExact.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:


                        if (numericUpDown_percentageExact.Value > 0)
                            numericUpDown_priceExact.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M), Convert.ToInt32(3));
                        break;
                }
            }
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentageFuzzy99_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {

                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:

                        if (numericUpDown_percentageFuzzy99.Value > 0)
                            numericUpDown_priceFuzzy99.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);



                        if (numericUpDown_percentageFuzzy99.Value > 0)
                            numericUpDown_priceFuzzy99.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:


                        if (numericUpDown_percentageFuzzy99.Value > 0)
                            numericUpDown_priceFuzzy99.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M), Convert.ToInt32(3));
                        break;
                }
            }
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentageFuzzy94_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {

                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:

                        if (numericUpDown_percentageFuzzy94.Value > 0)
                            numericUpDown_priceFuzzy94.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);


                        if (numericUpDown_percentageFuzzy94.Value > 0)
                            numericUpDown_priceFuzzy94.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:


                        if (numericUpDown_percentageFuzzy94.Value > 0)
                            numericUpDown_priceFuzzy94.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M), Convert.ToInt32(3));
                        break;
                }
            }
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentageFuzzy84_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {

                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:

                        if (numericUpDown_percentageFuzzy84.Value > 0)
                            numericUpDown_priceFuzzy84.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);



                        if (numericUpDown_percentageFuzzy84.Value > 0)
                            numericUpDown_priceFuzzy84.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:


                        if (numericUpDown_percentageFuzzy84.Value > 0)
                            numericUpDown_priceFuzzy84.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M), Convert.ToInt32(3));
                        break;
                }
            }
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentageFuzzy74_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {

                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:

                        if (numericUpDown_percentageFuzzy74.Value > 0)
                            numericUpDown_priceFuzzy74.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);

                        if (numericUpDown_percentageFuzzy74.Value > 0)
                            numericUpDown_priceFuzzy74.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:


                        if (numericUpDown_percentageFuzzy74.Value > 0)
                            numericUpDown_priceFuzzy74.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M), Convert.ToInt32(3));
                        break;
                }
            }
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_percentageNew_ValueChanged(object sender, EventArgs e)
        {
            if (IsSettingBasePercentage) return;
            if (numericUpDown_priceBase.Value > 0)
            {
                switch (comboBox_RoundType.SelectedIndex)
                {
                    case 0:
                        if (numericUpDown_percentageNew.Value > 0)
                            numericUpDown_priceFuzzyNew.Value = Helper.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M), Convert.ToInt32(3));
                        break;
                    case 1:
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);


                        if (numericUpDown_percentageNew.Value > 0)
                            numericUpDown_priceFuzzyNew.Value = Math.Truncate(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M) * decimalLen) / decimalLen;
                        break;
                    case 2:

                        if (numericUpDown_percentageNew.Value > 0)
                            numericUpDown_priceFuzzyNew.Value = Helper.Round(numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M), Convert.ToInt32(3));
                        break;
                }
            }
            AutoCalculateAnalysisBandPrices();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            SaveRecord();
        }

        private void numericUpDown_pricePerfect_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;

                var message = string.Empty;


                if (numericUpDown_pricePerfect.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentagePerfect.Value = numericUpDown_pricePerfect.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_pricePerfect.Value > numericUpDown_priceBase.Value)
                    message += PluginResources.The_price__Perfect_Match__cannot_be_greater_than_the_base_price + "\r\n";

                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

        private void numericUpDown_priceContext_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;
                var message = string.Empty;


                if (numericUpDown_priceContext.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageContext.Value = numericUpDown_priceContext.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_priceContext.Value > numericUpDown_priceBase.Value)
                    message += PluginResources.The_price__Context_Match__cannot_be_greater_than_the_base_price + "\r\n";


                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

        private void numericUpDown_priceRepetitions_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;
                var message = string.Empty;


                if (numericUpDown_priceRepetitions.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageRepetitions.Value = numericUpDown_priceRepetitions.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_priceRepetitions.Value > numericUpDown_priceBase.Value)
                    message += PluginResources.The_price__Repetitions__cannot_be_greater_than_the_base_price + "\r\n";


                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

        private void numericUpDown_priceExact_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;
                var message = string.Empty;

                if (numericUpDown_priceExact.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageExact.Value = numericUpDown_priceExact.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_priceExact.Value > numericUpDown_priceBase.Value)
                    message += PluginResources.The_price__Exact_Match__cannot_be_greater_than_the_base_price + "\r\n";

                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

















        private void numericUpDown_priceFuzzy99_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;
                var message = string.Empty;
                //if (!this.checkBox_autoUpdateAnalysisBandPrices.Checked)
                //{
                if (numericUpDown_priceFuzzy99.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageFuzzy99.Value = numericUpDown_priceFuzzy99.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_priceFuzzy99.Value > numericUpDown_priceBase.Value)
                    message += "The price 'Fuzzy 99%-94%' cannot be greater than the base price\r\n";
                //}

                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

        private void numericUpDown_priceFuzzy94_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;
                var message = string.Empty;

                //if (!this.checkBox_autoUpdateAnalysisBandPrices.Checked)
                //{
                if (numericUpDown_priceFuzzy94.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageFuzzy94.Value = numericUpDown_priceFuzzy94.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_priceFuzzy94.Value > numericUpDown_priceBase.Value)
                    message += "The price 'Fuzzy 94%-85%' cannot be greater than the base price\r\n";
                //}

                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

        private void numericUpDown_priceFuzzy84_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;
                var message = string.Empty;
                //if (!this.checkBox_autoUpdateAnalysisBandPrices.Checked)
                //{
                if (numericUpDown_priceFuzzy84.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageFuzzy84.Value = numericUpDown_priceFuzzy84.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_priceFuzzy84.Value > numericUpDown_priceBase.Value)
                    message += "The price 'Fuzzy 84%-75%' cannot be greater than the base price\r\n";
                //}

                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

        private void numericUpDown_priceFuzzy74_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;
                var message = string.Empty;

                //if (!this.checkBox_autoUpdateAnalysisBandPrices.Checked)
                //{
                if (numericUpDown_priceFuzzy74.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageFuzzy74.Value = numericUpDown_priceFuzzy74.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_priceFuzzy74.Value > numericUpDown_priceBase.Value)
                    message += "The price 'Fuzzy 74%-50%' cannot be greater than the base price\r\n";
                //}

                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

        private void numericUpDown_priceFuzzyNew_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsSettingBasePrice)
                    return;

                IsSettingBasePercentage = true;
                var message = string.Empty;

                //if (!this.checkBox_autoUpdateAnalysisBandPrices.Checked)
                //{
                if (numericUpDown_priceFuzzyNew.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageNew.Value = numericUpDown_priceFuzzyNew.Value / numericUpDown_priceBase.Value * 100;
                else if (numericUpDown_priceFuzzyNew.Value > numericUpDown_priceBase.Value)
                    message += "The price 'New' cannot be greater than the base price\r\n";
                //}

                if (message.Trim() != string.Empty)
                {
                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                IsSettingBasePercentage = false;
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }





    }
}
