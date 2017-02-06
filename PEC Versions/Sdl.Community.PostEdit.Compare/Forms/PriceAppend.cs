using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.PostEdit.Compare.Properties;


namespace PostEdit.Compare.Forms
{
    public partial class PriceAppend : Form
    {
        public PriceAppend()
        {           
            InitializeComponent();
        }




        public bool Saved { get; set; }
        public Sdl.Community.PostEdit.Compare.Core.Settings.Price Rate { get; set; }
        public Sdl.Community.PostEdit.Compare.Core.Settings.PriceGroup RateGroup { get; set; }


        public bool IsEdit { get; set; }
        public bool IsEditMultiple { get; set; }

        private Dictionary<Sdl.Community.PostEdit.Compare.Core.Settings.Language, List<Sdl.Community.PostEdit.Compare.Core.Settings.Language>> LanguageCombinations { get; set; }


        private void ResizeGui()
        {

            groupBox_priceProperties.Top = groupBox_typeProperties.Top;
            groupBox_typeProperties.Visible = false;

            button_cancel.Top = 442;
            button_save.Top = 442;

            Height = 515;

        }

        private void SetBasePercentageBase()
        {
            if (IsEdit && Rate.PriceBase > 0)
            {
                if (Rate.PricePerfect > 0)
                    numericUpDown_percentagePerfect.Value = (Rate.PricePerfect / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentagePerfect.Value = 0;

                if (Rate.PriceContext > 0)
                    numericUpDown_percentageContext.Value = (Rate.PriceContext / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentageContext.Value = 0;

                if (Rate.PriceRepetition > 0)
                    numericUpDown_percentageRepetitions.Value = (Rate.PriceRepetition / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentageRepetitions.Value = 0;

                if (Rate.PriceExact > 0)
                    numericUpDown_percentageExact.Value = (Rate.PriceExact / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentageExact.Value = 0;

                if (Rate.Price9599 > 0)
                    numericUpDown_percentageFuzzy99.Value = (Rate.Price9599 / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentageFuzzy99.Value = 0;

                if (Rate.Price8594 > 0)
                    numericUpDown_percentageFuzzy94.Value = (Rate.Price8594 / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentageFuzzy94.Value = 0;

                if (Rate.Price7584 > 0)
                    numericUpDown_percentageFuzzy84.Value = (Rate.Price7584 / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentageFuzzy84.Value = 0;

                if (Rate.Price5074 > 0)
                    numericUpDown_percentageFuzzy74.Value = (Rate.Price5074 / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentageFuzzy74.Value = 0;

                if (Rate.PriceNew > 0)
                    numericUpDown_percentageNew.Value = (Rate.PriceNew / Rate.PriceBase) * 100;
                else
                    numericUpDown_percentageNew.Value = 0;
            }
            else
            {
                numericUpDown_percentagePerfect.Value = RateGroup.DefaultAnalysisBand.PercentagePerfect;
                numericUpDown_percentageContext.Value = RateGroup.DefaultAnalysisBand.PercentageContext;
                numericUpDown_percentageRepetitions.Value = RateGroup.DefaultAnalysisBand.PercentageRepetition;
                numericUpDown_percentageExact.Value = RateGroup.DefaultAnalysisBand.PercentageExact;
                numericUpDown_percentageFuzzy99.Value = RateGroup.DefaultAnalysisBand.Percentage9599;
                numericUpDown_percentageFuzzy94.Value = RateGroup.DefaultAnalysisBand.Percentage8594;
                numericUpDown_percentageFuzzy84.Value = RateGroup.DefaultAnalysisBand.Percentage7584;
                numericUpDown_percentageFuzzy74.Value = RateGroup.DefaultAnalysisBand.Percentage5074;
                numericUpDown_percentageNew.Value = RateGroup.DefaultAnalysisBand.PercentageNew;
            }
        }

        //herehererere
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

        private void FormAPGPriceAppend_Load(object sender, EventArgs e)
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
                    Text += Resources.PriceAppend_FormAPGPriceAppend_Load_Edit;
                }
                else
                {
                    Text += Resources.PriceAppend_FormAPGPriceAppend_Load_New;
                }

                SetBasePercentageBase();


                #region  |  comboBox_sourceLanguages  |
                try
                {
                    comboBox_sourceLanguages.BeginUpdate();
                    comboBox_sourceLanguages.Items.Clear();

                    var selectedIndex = 0;
                    foreach (var language in RateGroup.SourceLanguages)
                    {
                        comboBox_sourceLanguages.Items.Add(language.Id + " - " + language.Name);
                    }
                    comboBox_sourceLanguages.Sorted = true;
                    for (var i = 0; i < comboBox_sourceLanguages.Items.Count; i++)
                    {
                        var languageText = comboBox_sourceLanguages.Items[i].ToString();
                        var sourceLanguageId = languageText.Substring(0, languageText.IndexOf(" ", StringComparison.Ordinal));
                        if (string.Compare(sourceLanguageId, Rate.Source, StringComparison.OrdinalIgnoreCase) != 0) continue;
                        selectedIndex = i;
                        break;
                    }
                    comboBox_sourceLanguages.SelectedIndex = selectedIndex;
                }
                catch (Exception ex)
                {
                    throw ex;
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
                    foreach (var language in RateGroup.TargetLanguages)
                    {
                        comboBox_targetLanguages.Items.Add(language.Id + " - " + language.Name);
                    }
                    comboBox_targetLanguages.Sorted = true;
                    for (var i = 0; i < comboBox_targetLanguages.Items.Count; i++)
                    {
                        var languageText = comboBox_targetLanguages.Items[i].ToString();
                        var targetLanguageId = languageText.Substring(0, languageText.IndexOf(" ", StringComparison.Ordinal));
                        if (string.Compare(targetLanguageId, Rate.Target, StringComparison.OrdinalIgnoreCase) != 0) continue;
                        selectedIndex = i;
                        break;
                    }
                    comboBox_targetLanguages.SelectedIndex = selectedIndex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    comboBox_targetLanguages.EndUpdate();
                }
                #endregion

                numericUpDown_priceBase.Value = Rate.PriceBase;

                //0=RoundUp, 1=RoundDown, 2=Round
                comboBox_RoundType.SelectedIndex = (int)Rate.Round;



                numericUpDown_pricePerfect.Value = Rate.PricePerfect;
                numericUpDown_priceContext.Value = Rate.PriceContext;
                numericUpDown_priceRepetitions.Value = Rate.PriceRepetition;
                numericUpDown_priceExact.Value = Rate.PriceExact;
                numericUpDown_priceFuzzy99.Value = Rate.Price9599;
                numericUpDown_priceFuzzy94.Value = Rate.Price8594;
                numericUpDown_priceFuzzy84.Value = Rate.Price7584;
                numericUpDown_priceFuzzy74.Value = Rate.Price5074;
                numericUpDown_priceFuzzyNew.Value = Rate.PriceNew;
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
            if (IsEdit) return;
            foreach (Sdl.Community.PostEdit.Compare.Core.Settings.Price price in RateGroup.GroupPrices)
            {
                string sourceId = comboBox_sourceLanguages.SelectedItem.ToString().Substring(0, comboBox_sourceLanguages.SelectedItem.ToString().IndexOf(" ", StringComparison.Ordinal));
                string targetId = comboBox_targetLanguages.SelectedItem.ToString().Substring(0, comboBox_targetLanguages.SelectedItem.ToString().IndexOf(" ", StringComparison.Ordinal));


                if (string.Compare(price.Source, sourceId, StringComparison.OrdinalIgnoreCase) != 0 ||
                    string.Compare(price.Target, targetId, StringComparison.OrdinalIgnoreCase) != 0) continue;
                numericUpDown_priceBase.Value = price.PriceBase;
                break;
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
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_pricePerfect.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_pricePerfect.Value = 0;


                            if (numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceContext.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceContext.Value = 0;


                            if (numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceRepetitions.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceRepetitions.Value = 0;


                            if (numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceExact.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceExact.Value = 0;


                            if (numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzy99.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy99.Value = 0;


                            if (numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzy94.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy94.Value = 0;


                            if (numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzy84.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy84.Value = 0;


                            if (numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M), Convert.ToInt32(3));

                                numericUpDown_priceFuzzy74.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy74.Value = 0;


                            if (numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M), Convert.ToInt32(3));

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
                                decimal value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M)) * decimalLen) / decimalLen;

                                numericUpDown_pricePerfect.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_pricePerfect.Value = 0;



                            if (numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M)) * decimalLen) / decimalLen;

                                numericUpDown_priceContext.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceContext.Value = 0;



                            if (numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M)) * decimalLen) / decimalLen;

                                numericUpDown_priceRepetitions.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceRepetitions.Value = 0;



                            if (numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M)) * decimalLen) / decimalLen;

                                numericUpDown_priceExact.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceExact.Value = 0;



                            if (numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M)) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzy99.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy99.Value = 0;



                            if (numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M)) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzy94.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy94.Value = 0;



                            if (numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M)) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzy84.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy84.Value = 0;



                            if (numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M)) * decimalLen) / decimalLen;

                                numericUpDown_priceFuzzy74.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy74.Value = 0;



                            if (numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M)) * decimalLen) / decimalLen;

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
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M)), Convert.ToInt32(3));
                                numericUpDown_pricePerfect.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_pricePerfect.Value = 0;




                            if (numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M)), Convert.ToInt32(3));
                                numericUpDown_priceContext.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceContext.Value = 0;



                            if (numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M)), Convert.ToInt32(3));
                                numericUpDown_priceRepetitions.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceRepetitions.Value = 0;



                            if (numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M)), Convert.ToInt32(3));
                                numericUpDown_priceExact.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceExact.Value = 0;



                            if (numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M)), Convert.ToInt32(3));
                                numericUpDown_priceFuzzy99.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy99.Value = 0;




                            if (numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M)), Convert.ToInt32(3));
                                numericUpDown_priceFuzzy94.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy94.Value = 0;




                            if (numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M)), Convert.ToInt32(3));
                                numericUpDown_priceFuzzy84.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy84.Value = 0;



                            if (numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M)), Convert.ToInt32(3));
                                numericUpDown_priceFuzzy74.Value = value <= numericUpDown_priceBase.Value ? value : numericUpDown_priceBase.Value;
                            }
                            else
                                numericUpDown_priceFuzzy74.Value = 0;



                            if (numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M)), Convert.ToInt32(3));
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

            Rate = new Sdl.Community.PostEdit.Compare.Core.Settings.Price();


            if (!IsEdit)
            {

                var sourceLanguageId = comboBox_sourceLanguages.SelectedItem.ToString().Substring(0, comboBox_sourceLanguages.SelectedItem.ToString().IndexOf(" "));
                var targetLanguageId = comboBox_targetLanguages.SelectedItem.ToString().Substring(0, comboBox_targetLanguages.SelectedItem.ToString().IndexOf(" "));
                Rate.Source = sourceLanguageId;
                Rate.Target = targetLanguageId;
            }

            Rate.PriceBase = numericUpDown_priceBase.Value;


            Rate.Round = (Sdl.Community.PostEdit.Compare.Core.Settings.Price.RoundType)comboBox_RoundType.SelectedIndex;


            Rate.PricePerfect = numericUpDown_pricePerfect.Value;
            Rate.PriceContext = numericUpDown_priceContext.Value;
            Rate.PriceRepetition = numericUpDown_priceRepetitions.Value;
            Rate.PriceExact = numericUpDown_priceExact.Value;
            Rate.Price9599 = numericUpDown_priceFuzzy99.Value;
            Rate.Price8594 = numericUpDown_priceFuzzy94.Value;
            Rate.Price7584 = numericUpDown_priceFuzzy84.Value;
            Rate.Price5074 = numericUpDown_priceFuzzy74.Value;
            Rate.PriceNew = numericUpDown_priceFuzzyNew.Value;


            Saved = true;
            Close();



        }



        private void comboBox_categories_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetBasePriceOnNew();
            EnableButtons();

        }

        private void checkBox_autoUpdateAnalysisBandPrices_CheckedChanged(object sender, EventArgs e)
        {
            AutoCalculateAnalysisBandPrices();
        }

        private void numericUpDown_Decimal_Point_Length_ValueChanged(object sender, EventArgs e)
        {
            AutoCalculateAnalysisBandPrices();
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
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {
                    switch (comboBox_RoundType.SelectedIndex)
                    {
                        case 0:
                            if (numericUpDown_percentagePerfect.Value > 0)
                                numericUpDown_pricePerfect.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M), Convert.ToInt32(3));
                            break;
                        case 1:
                            var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                            var decimalLen = Convert.ToInt32(strDecimalLen);

                            if (numericUpDown_percentagePerfect.Value > 0)
                                numericUpDown_pricePerfect.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M)) * decimalLen) / decimalLen;
                            break;
                        case 2:
                            if (numericUpDown_percentagePerfect.Value > 0)
                                numericUpDown_pricePerfect.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentagePerfect.Value * .01M)), Convert.ToInt32(3));
                            break;
                    }
                }
                AutoCalculateAnalysisBandPrices();
            }
        }

        private void numericUpDown_percentageContext_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {

                    if (comboBox_RoundType.SelectedIndex == 0)
                    {

                        if (numericUpDown_percentageContext.Value > 0)
                            numericUpDown_priceContext.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M), Convert.ToInt32(3));

                    }
                    else if (comboBox_RoundType.SelectedIndex == 1)
                    {
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);



                        if (numericUpDown_percentageContext.Value > 0)
                            numericUpDown_priceContext.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M)) * decimalLen) / decimalLen;


                    }
                    else if (comboBox_RoundType.SelectedIndex == 2)
                    {

                        if (numericUpDown_percentageContext.Value > 0)
                            numericUpDown_priceContext.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageContext.Value * .01M)), Convert.ToInt32(3));


                    }

                }
                AutoCalculateAnalysisBandPrices();
            }
        }

        private void numericUpDown_percentageRepetitions_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {

                    if (comboBox_RoundType.SelectedIndex == 0)
                    {

                        if (numericUpDown_percentageRepetitions.Value > 0)
                            numericUpDown_priceRepetitions.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M), Convert.ToInt32(3));
                    }
                    else if (comboBox_RoundType.SelectedIndex == 1)
                    {
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);

                        if (numericUpDown_percentageRepetitions.Value > 0)
                            numericUpDown_priceRepetitions.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M)) * decimalLen) / decimalLen;


                    }
                    else if (comboBox_RoundType.SelectedIndex == 2)
                    {


                        if (numericUpDown_percentageRepetitions.Value > 0)
                            numericUpDown_priceRepetitions.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageRepetitions.Value * .01M)), Convert.ToInt32(3));



                    }
                }
                AutoCalculateAnalysisBandPrices();
            }
        }

        private void numericUpDown_percentageExact_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {

                    if (comboBox_RoundType.SelectedIndex == 0)
                    {

                        if (numericUpDown_percentageExact.Value > 0)
                            numericUpDown_priceExact.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M), Convert.ToInt32(3));

                    }
                    else if (comboBox_RoundType.SelectedIndex == 1)
                    {
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);


                        if (numericUpDown_percentageExact.Value > 0)
                            numericUpDown_priceExact.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M)) * decimalLen) / decimalLen;


                    }
                    else if (comboBox_RoundType.SelectedIndex == 2)
                    {


                        if (numericUpDown_percentageExact.Value > 0)
                            numericUpDown_priceExact.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageExact.Value * .01M)), Convert.ToInt32(3));


                    }
                }
                AutoCalculateAnalysisBandPrices();
            }
        }

        private void numericUpDown_percentageFuzzy99_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {

                    if (comboBox_RoundType.SelectedIndex == 0)
                    {

                        if (numericUpDown_percentageFuzzy99.Value > 0)
                            numericUpDown_priceFuzzy99.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M), Convert.ToInt32(3));

                    }
                    else if (comboBox_RoundType.SelectedIndex == 1)
                    {
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);



                        if (numericUpDown_percentageFuzzy99.Value > 0)
                            numericUpDown_priceFuzzy99.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M)) * decimalLen) / decimalLen;


                    }
                    else if (comboBox_RoundType.SelectedIndex == 2)
                    {


                        if (numericUpDown_percentageFuzzy99.Value > 0)
                            numericUpDown_priceFuzzy99.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy99.Value * .01M)), Convert.ToInt32(3));

                    }
                }
                AutoCalculateAnalysisBandPrices();
            }
        }

        private void numericUpDown_percentageFuzzy94_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {

                    if (comboBox_RoundType.SelectedIndex == 0)
                    {

                        if (numericUpDown_percentageFuzzy94.Value > 0)
                            numericUpDown_priceFuzzy94.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M), Convert.ToInt32(3));

                    }
                    else if (comboBox_RoundType.SelectedIndex == 1)
                    {
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);


                        if (numericUpDown_percentageFuzzy94.Value > 0)
                            numericUpDown_priceFuzzy94.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M)) * decimalLen) / decimalLen;

                    }
                    else if (comboBox_RoundType.SelectedIndex == 2)
                    {


                        if (numericUpDown_percentageFuzzy94.Value > 0)
                            numericUpDown_priceFuzzy94.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy94.Value * .01M)), Convert.ToInt32(3));



                    }
                }
                AutoCalculateAnalysisBandPrices();
            }
        }

        private void numericUpDown_percentageFuzzy84_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {

                    if (comboBox_RoundType.SelectedIndex == 0)
                    {

                        if (numericUpDown_percentageFuzzy84.Value > 0)
                            numericUpDown_priceFuzzy84.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M), Convert.ToInt32(3));

                    }
                    else if (comboBox_RoundType.SelectedIndex == 1)
                    {
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);



                        if (numericUpDown_percentageFuzzy84.Value > 0)
                            numericUpDown_priceFuzzy84.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M)) * decimalLen) / decimalLen;


                    }
                    else if (comboBox_RoundType.SelectedIndex == 2)
                    {


                        if (numericUpDown_percentageFuzzy84.Value > 0)
                            numericUpDown_priceFuzzy84.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy84.Value * .01M)), Convert.ToInt32(3));



                    }
                }
                AutoCalculateAnalysisBandPrices();
            }
        }

        private void numericUpDown_percentageFuzzy74_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {

                    if (comboBox_RoundType.SelectedIndex == 0)
                    {

                        if (numericUpDown_percentageFuzzy74.Value > 0)
                            numericUpDown_priceFuzzy74.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M), Convert.ToInt32(3));

                    }
                    else if (comboBox_RoundType.SelectedIndex == 1)
                    {
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);

                        if (numericUpDown_percentageFuzzy74.Value > 0)
                            numericUpDown_priceFuzzy74.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M)) * decimalLen) / decimalLen;
                    }
                    else if (comboBox_RoundType.SelectedIndex == 2)
                    {


                        if (numericUpDown_percentageFuzzy74.Value > 0)
                            numericUpDown_priceFuzzy74.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageFuzzy74.Value * .01M)), Convert.ToInt32(3));



                    }
                }
                AutoCalculateAnalysisBandPrices();
            }
        }

        private void numericUpDown_percentageNew_ValueChanged(object sender, EventArgs e)
        {
            if (!IsSettingBasePercentage)
            {
                if (numericUpDown_priceBase.Value > 0)
                {
                    if (comboBox_RoundType.SelectedIndex == 0)
                    {
                        if (numericUpDown_percentageNew.Value > 0)
                            numericUpDown_priceFuzzyNew.Value = Common.RoundUp(numericUpDown_priceBase.Value, Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M), Convert.ToInt32(3));
                    }
                    else if (comboBox_RoundType.SelectedIndex == 1)
                    {
                        var strDecimalLen = "10".PadRight(Convert.ToInt32(3) + 1, '0');
                        var decimalLen = Convert.ToInt32(strDecimalLen);


                        if (numericUpDown_percentageNew.Value > 0)
                            numericUpDown_priceFuzzyNew.Value = Math.Truncate((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M)) * decimalLen) / decimalLen;
                    }
                    else if (comboBox_RoundType.SelectedIndex == 2)
                    {

                        if (numericUpDown_percentageNew.Value > 0)
                            numericUpDown_priceFuzzyNew.Value = Common.Round((numericUpDown_priceBase.Value * Convert.ToDecimal(numericUpDown_percentageNew.Value * .01M)), Convert.ToInt32(3));

                    }
                }
                AutoCalculateAnalysisBandPrices();
            }
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


                if (numericUpDown_pricePerfect.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentagePerfect.Value = (numericUpDown_pricePerfect.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_pricePerfect.Value > numericUpDown_priceBase.Value)
                    ;

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
                    numericUpDown_percentageContext.Value = (numericUpDown_priceContext.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_priceContext.Value > numericUpDown_priceBase.Value)
                    message += Resources.PriceAppend_numericUpDown_priceContext_ValueChanged_The_rate_Context_Match_cannot_be_greater_than_the_base_rate;


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
                    numericUpDown_percentageRepetitions.Value = (numericUpDown_priceRepetitions.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_priceRepetitions.Value > numericUpDown_priceBase.Value)
                    message += Resources.PriceAppend_numericUpDown_priceRepetitions_ValueChanged_The_rate_Repetitions_cannot_be_greater_than_the_base_rate;
                
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
                    numericUpDown_percentageExact.Value = (numericUpDown_priceExact.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_priceExact.Value > numericUpDown_priceBase.Value)
            


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
             
                if (numericUpDown_priceFuzzy99.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageFuzzy99.Value = (numericUpDown_priceFuzzy99.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_priceFuzzy99.Value > numericUpDown_priceBase.Value)
                    message += Resources.PriceAppend_numericUpDown_priceFuzzy99_ValueChanged_The_rate_Fuzzy_99_94_cannot_be_greater_than_the_base_rate;
               

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

              
                if (numericUpDown_priceFuzzy94.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageFuzzy94.Value = (numericUpDown_priceFuzzy94.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_priceFuzzy94.Value > numericUpDown_priceBase.Value)
                    message += Resources.PriceAppend_numericUpDown_priceFuzzy94_ValueChanged_The_rate_Fuzzy_94_85_cannot_be_greater_than_the_base_rate;
             

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
               
                if (numericUpDown_priceFuzzy84.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageFuzzy84.Value = (numericUpDown_priceFuzzy84.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_priceFuzzy84.Value > numericUpDown_priceBase.Value)
                    message += Resources.PriceAppend_numericUpDown_priceFuzzy84_ValueChanged_The_rate__Fuzzy_84_75_cannot_be_greater_than_the_base_rate;
           
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

              
                if (numericUpDown_priceFuzzy74.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageFuzzy74.Value = (numericUpDown_priceFuzzy74.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_priceFuzzy74.Value > numericUpDown_priceBase.Value)
                    message += Resources.PriceAppend_numericUpDown_priceFuzzy74_ValueChanged_The_rate_Fuzzy_74__50_cannot_be_greater_than_the_base_rate;
               
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

                
                if (numericUpDown_priceFuzzyNew.Value <= numericUpDown_priceBase.Value)
                    numericUpDown_percentageNew.Value = (numericUpDown_priceFuzzyNew.Value / numericUpDown_priceBase.Value) * 100;
                else if (numericUpDown_priceFuzzyNew.Value > numericUpDown_priceBase.Value)
                    message += Resources.PriceAppend_numericUpDown_priceFuzzyNew_ValueChanged_The_rate_New_cannot_be_greater_than_the_base_rate;
              

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
