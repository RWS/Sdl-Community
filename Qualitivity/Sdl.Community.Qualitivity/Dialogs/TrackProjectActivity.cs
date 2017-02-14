using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Sdl.Community.Comparison;
using Sdl.Community.Qualitivity.Custom;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Report;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.Structures.Rates;

namespace Sdl.Community.Qualitivity.Dialogs
{
    public partial class TrackProjectActivity : Form
    {

        public bool Saved { get; set; }
        public bool IsEdit { get; set; }
        public bool IsMerge { get; set; }
        private bool IsLoading { get; set; }
     
       
        public Activity Activity { get; set; }
        public List<Project> Projects { get; set; }

        private Project Project { get; set; }
        public CompanyProfile CompanyProfile { get; private set; }

        public List<DocumentActivity> DocumentActivities { get; set; }
        
        private ErrorProvider _activityName;
        private ErrorProvider _dateStart;
        private ErrorProvider _dateEnd;

        private ActivityRates ActivityRates { get; set; }

        public TrackProjectActivity()
        {
            InitializeComponent();

            InitializeErrorProviders();

            InitializeDocumentsListView();

        }
        private void TrackProjectActivity_Load(object sender, EventArgs e)
        {


            try
            {
                IsLoading = true;

                if (IsMerge)
                    Text += PluginResources.___Merge_;
                else
                    Text += IsEdit ? PluginResources.___Edit_ : PluginResources.___New_;




                InitializeProjectArea();

                InitializeDocumentsTab();

                InitializeDetailsTab();

                InitializeRatesTab();

                InitializeMetricReportsTab();



                textBox_name.Select();



                CheckActivatedControls();


                IsLoading = false;
                if (!IsEdit)
                    comboBox_pem_rates_SelectedIndexChanged(null, null);


                if (IsMerge)
                {                 
                    comboBox_pem_rates_SelectedIndexChanged(null, null);
                    linkLabel_get_total_hours_elapsed_from_documents_LinkClicked(null, null);
                }

                check_activity_rate_total();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }

        }

        
        private void InitializeProjectArea()
        {
            Project = Helper.GetProjectFromId(Activity.ProjectId);
            CompanyProfile = Helper.GetClientFromId(Project.CompanyProfileId);
        }
        private void InitializeErrorProviders()
        {
            _activityName = new ErrorProvider();
            _activityName.SetIconAlignment(textBox_name, ErrorIconAlignment.MiddleRight);
            _activityName.SetIconPadding(textBox_name, 2);
            _activityName.BlinkRate = 1000;
            _activityName.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;


            _dateStart = new ErrorProvider();
            _dateStart.SetIconAlignment(dateTimePicker_start_hours, ErrorIconAlignment.MiddleRight);
            _dateStart.SetIconPadding(dateTimePicker_start_hours, 2);
            _dateStart.BlinkRate = 1000;
            _dateStart.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;

            _dateEnd = new ErrorProvider();
            _dateEnd.SetIconAlignment(dateTimePicker_end_hours, ErrorIconAlignment.MiddleRight);
            _dateEnd.SetIconPadding(dateTimePicker_end_hours, 2);
            _dateEnd.BlinkRate = 1000;
            _dateEnd.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;


        }
        private void InitializeDocumentsListView()
        {
            olvColumn_name.AspectGetter = delegate(object x)
            {
                var path = ((DocumentActivity)x).TranslatableDocument.DocumentPath;

                return path.Trim() != string.Empty ? path : string.Empty + ((DocumentActivity)x).TranslatableDocument.DocumentName;
            };

            olvColumn_changes.AspectGetter = x => ((DocumentActivity) x).Records.Count.ToString();

            olvColumn_elapsed.AspectGetter = delegate(object x)
            {
                var ts = new TimeSpan(((DocumentActivity)x).TicksActivity);

                return ts.Hours.ToString().PadLeft(2, '0') + ":" + ts.Minutes.ToString().PadLeft(2, '0') + ":" + ts.Seconds.ToString().PadLeft(2, '0');
            };


            olvColumn_source.AspectGetter = x => ((DocumentActivity) x).TranslatableDocument.SourceLanguage;

            olvColumn_target.AspectGetter = x => ((DocumentActivity) x).TranslatableDocument.TargetLanguage;

            olvColumn_source.ImageGetter = delegate(object x)
            {
                var key = ((DocumentActivity)x).TranslatableDocument.SourceLanguage + ".gif";
                return !imageList_flags.Images.ContainsKey(key) ? "empty.gif" : key;
            };
            olvColumn_target.ImageGetter = delegate(object x)
            {
                var key = ((DocumentActivity)x).TranslatableDocument.TargetLanguage + ".gif";
                return !imageList_flags.Images.ContainsKey(key) ? "empty.gif" : key;
            };
        }
       
  


        private void InitializeDetailsTab()
        {

            #region  |  Project Group Area  |
            try
            {
                comboBox_client.BeginUpdate();

                comboBox_client.Items.Clear();

                if (IsEdit || CompanyProfile != null)
                {
                    //if it is an edit of an existing activity record and/or a new activity but the client is already assigned to that project
                    comboBox_client.Items.Add(CompanyProfile != null ? new ComboboxItem(CompanyProfile.Name, CompanyProfile) : new ComboboxItem("[no client]", CompanyProfile));
                    comboBox_client.SelectedIndex = 0;
                    comboBox_client.Enabled = false;                    
                }
                else
                {
                    //only when it is a new project activity where the client has been assigned
                    comboBox_client.Items.Add( new ComboboxItem(@"[no client]", null));
                    foreach(var cpi in Tracked.Settings.CompanyProfiles)
                        comboBox_client.Items.Add(new ComboboxItem(cpi.Name, cpi));
                    
                    comboBox_client.SelectedIndex = 0;
                    comboBox_client.Enabled = true;  
                }             
            }
            finally
            {
                comboBox_client.EndUpdate();
            }



         
            #region  |  set project  |

            comboBox_projects.BeginUpdate();

            foreach (var tp in Projects)
            {
                comboBox_projects.Items.Add(new ComboboxItem(tp.Name, tp));
            }

            comboBox_projects.EndUpdate();

            var selectedProjectIndex = 0;
            for (var i = 0; i < comboBox_projects.Items.Count; i++)
            {
                var cbi = (ComboboxItem)comboBox_projects.Items[i];

                if (cbi.Value == null) continue;
                var tp = (Project)cbi.Value;

                if (tp.Id != Project.Id) continue;
                selectedProjectIndex = i;
                break;
            }

            comboBox_projects.SelectedIndex = selectedProjectIndex;
            #endregion

            #endregion


            textBox_name.Text = Activity.Name;
            textBox_description.Text = Activity.Description;

            comboBox_status.SelectedIndex = Activity.ActivityStatus == Activity.Status.New ? 0 : 1;
            comboBox_billable.SelectedIndex = Activity.Billable ? 0 : 1;


            #region  |  dates/times  |

            if (Activity.Started == null)
                Activity.Started = DateTime.Now;
            if (Activity.Stopped == null)
                Activity.Stopped = DateTime.Now;

            dateTimePicker_start_date.Value = Activity.Started.Value;
            dateTimePicker_start_hours.Value = Activity.Started.Value;

            dateTimePicker_end_date.Value = Activity.Stopped.Value;
            dateTimePicker_end_hours.Value = Activity.Stopped.Value;

            #endregion

            linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.Enabled = objectListView_documents.Items.Count > 0;

        }
        private void InitializeDocumentsTab()
        {
            #region  |  documents  |

            //query the database only if there are no activites loaded; depending on where the dialog was launched from
            if (Activity.Activities.Count > 0
                && (DocumentActivities == null || DocumentActivities.Count == 0))
                DocumentActivities = Helper.GetDocumentActivityObjects(Activity);


            objectListView_documents.BeginUpdate();
            try
            {
                objectListView_documents.SetObjects(DocumentActivities);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                objectListView_documents.EndUpdate();
            }

            label_document_count.Text = string.Format(PluginResources.Document_Activities___0_, objectListView_documents.Items.Count);

            #endregion
        }
        private void InitializeRatesTab()
        {
            var acc = new Accordion
            {
                CheckBoxMargin = new Padding(2),
                ContentMargin = new Padding(15, 5, 15, 5),
                ContentPadding = new Padding(1),
                Insets = new Padding(5),
                ControlBackColor = Color.White,
                ContentBackColor = Color.CadetBlue
            };
            panel_activity_rates_parent.Controls.Add(acc);

            panel_language_rate.Dock = DockStyle.Fill;
            panel_hourly_rate.Dock = DockStyle.Fill;
            panel_custom_rate.Dock = DockStyle.Fill;

            acc.Add(panel_language_rate, "Language Rate", PluginResources.Select_a_language_rate, 0, true, contentBackColor: Color.Transparent);
            acc.Add(panel_hourly_rate, "Hourly Rate", PluginResources.Specify_an_hourly_rate, 1, true, contentBackColor: Color.Transparent);
            acc.Add(panel_custom_rate, "Custom Rate", PluginResources.Specify_a_custom_rate, 0, false, contentBackColor: Color.Transparent);

         
            checkBox_hourly_rate_CheckedChanged(null, null);
            checkBox_pem_rate_CheckedChanged(null, null);
            checkBox_custome_rate_CheckedChanged(null, null);


            checkBox_custom_rate.Checked = Activity.CustomRateChecked;
            textBox_custom_rate_description.Text = Activity.DocumentActivityRates.CustomRateDescription;
            numericUpDown_custom_rate_total.Value = Convert.ToDecimal(Activity.DocumentActivityRates.CustomRateTotal);

            textBox_hourly_rate_description.Text = Activity.DocumentActivityRates.HourlyRateDescription;
            textBox_language_rate_description.Text = Activity.DocumentActivityRates.LanguageRateDescription;

            if (objectListView_documents.Items.Count > 0)
            {
                checkBox_language_rate.Enabled = true;
            }
            else
            {
                checkBox_language_rate.Enabled = false;
                checkBox_language_rate.Checked = false;
            }


            ActivityRates = Activity.DocumentActivityRates.Clone() as ActivityRates;

           
            #region  |  PEM combobox  |

            comboBox_pem_rates.BeginUpdate();

            comboBox_pem_rates.Items.Clear();
            var _cbi = new ComboboxItem(@"<empty>", null);
            comboBox_pem_rates.Items.Add(_cbi);
          
            if (IsEdit)
            {               
                if (Activity.DocumentActivityRates.LanguageRateId > -1 && Activity.DocumentActivityRates.LanguageRateName.Trim() != string.Empty) 
                {                    
                    var cbi = new ComboboxItem(Activity.DocumentActivityRates.LanguageRateName + @" <saved version>", Activity.DocumentActivityRates.Clone() as ActivityRates);
                    comboBox_pem_rates.Items.Add(cbi);
                }
            }
           
            foreach (var prg in Tracked.Settings.LanguageRateGroups)
            {
                var cbi = new ComboboxItem(prg.Name, prg.Clone() as LanguageRateGroup);
                comboBox_pem_rates.Items.Add(cbi);
            }
            comboBox_pem_rates.Sorted = true;
            comboBox_pem_rates.EndUpdate();
           
            #endregion


            if (!IsEdit)
            {
                #region  |  hourly  rate  |

                linkLabel_get_default_hourly_rate_LinkClicked(null, null);
                linkLabel_get_total_hours_elapsed_from_documents_LinkClicked(null, null);
                calculate_hourly_rate();

                if (numericUpDown_hourly_rate_total.Value > 0)
                    checkBox_hourly_rate.Checked = true;

                #endregion
                
                #region  |  PEM Rate  |


                var selectedIndex = 0;
                var foundPriceRateGroup = false;
                LanguageRateGroup prg =null;
                if (CompanyProfile != null && CompanyProfile.Id > -1)
                {
                    foreach (ComboboxItem cbi in comboBox_pem_rates.Items)
                    {
                        if (cbi.Value != null && cbi.Value.GetType() == typeof(LanguageRateGroup))
                        {
                            prg = (LanguageRateGroup)cbi.Value;
                            if (prg.Id == CompanyProfile.ProfileRate.LanguageRateId)
                            {
                                foundPriceRateGroup = true;
                                break;
                            }
                            selectedIndex++;
                        }
                        else
                            selectedIndex++;
                    }
                }
                comboBox_pem_rates.SelectedIndex = foundPriceRateGroup ? selectedIndex : 0;

         

                if (objectListView_documents.Items.Count > 0)
                {
                  
                    checkBox_language_rate.Checked = foundPriceRateGroup;
                  
                    SetActivityRateFromLanguageRate(prg);               
                }

               

                #endregion

             
                checkBox_includeTagsInComparison.Checked = CompanyProfile == null || CompanyProfile.ComparerOptions.IncludeTagsInComparison;
                checkBox_group_changes.Checked = CompanyProfile == null || CompanyProfile.ComparerOptions.ConsolidateChanges;
                comboBox_comparisonType.SelectedIndex = CompanyProfile != null ? CompanyProfile.ComparerOptions.ComparisonType : 0;
            }
            else
            {
                #region  |  hourly rate  |

                numericUpDown_hourly_rate_rate.Value = Convert.ToDecimal(Activity.DocumentActivityRates.HourlyRateRate);
                numericUpDown_hourly_rate_hours.Value = Convert.ToDecimal(Activity.DocumentActivityRates.HourlyRateQuantity);
                numericUpDown_hourly_rate_total.Value = Convert.ToDecimal(Activity.DocumentActivityRates.HourlyRateTotal);
                checkBox_hourly_rate.Checked = Activity.HourlyRateChecked;

                label_hourly_rate_curency.Text = Activity.DocumentActivityRates.HourlyRateCurrency;

                #endregion

                #region  |  PEM Rate  |


                var selectedIndex = 0;
                var foundPriceRateGroup = false;
                if (Activity.DocumentActivityRates.LanguageRateId > -1 && Activity.DocumentActivityRates.LanguageRateName.Trim() != string.Empty)
                {

                    foreach (ComboboxItem cbi in comboBox_pem_rates.Items)
                    {
                        if (cbi.Value != null && cbi.Value.GetType() == typeof(ActivityRates))
                        {
                            var prg = cbi.Value as ActivityRates;
                            if (prg != null && prg.LanguageRateId == Activity.DocumentActivityRates.LanguageRateId)
                            {
                                foundPriceRateGroup = true;
                                break;
                            }
                            selectedIndex++;
                        }
                        else
                            selectedIndex++;
                    }

                }

                if (!foundPriceRateGroup)
                {
                    foreach (ComboboxItem cbi in comboBox_pem_rates.Items)
                    {
                        if (cbi.Value != null && cbi.Value.GetType() == typeof(LanguageRateGroup))
                        {
                            var prg = (LanguageRateGroup)cbi.Value;

                            if (prg.Id == Activity.DocumentActivityRates.LanguageRateId)
                            {
                                foundPriceRateGroup = true;
                                break;
                            }
                            selectedIndex++;
                        }
                        else
                            selectedIndex++;
                    }
                }
            

                comboBox_pem_rates.SelectedIndex = foundPriceRateGroup ? selectedIndex : 0;

                checkBox_language_rate.Checked = Activity.LanguageRateChecked;
                numericUpDown_pem_rate_total.Value = Convert.ToDecimal(Activity.DocumentActivityRates.LanguageRateTotal);

                if (foundPriceRateGroup)
                {
                    label_rate_currency.Text = Activity.DocumentActivityRates.LanguageRateCurrency;
                    label_hourly_rate_curency.Text = Activity.DocumentActivityRates.HourlyRateCurrency;
                }
                else
                    label_rate_currency.Text = Tracked.Settings.GetGeneralProperty("defaultCurrency").Value;


               
                #endregion

                checkBox_includeTagsInComparison.Checked = Activity.ComparisonOptions.IncludeTagsInComparison;
                checkBox_group_changes.Checked = Activity.ComparisonOptions.ConsolidateChanges;
                comboBox_comparisonType.SelectedIndex = Activity.ComparisonOptions.ComparisonType;
            }


            Activity.DocumentActivityRates.CustomRateCurrency = Activity.DocumentActivityRates.HourlyRateCurrency;
            label_custom_rate_currency.Text = Activity.DocumentActivityRates.CustomRateCurrency;

            label_hourly_rate_curency_TextChanged(null, null);

            comboBox_billable_SelectedIndexChanged(null, null);


            if (comboBox_pem_rates.SelectedIndex == 0)
            {
                label_rate_currency.Text = label_hourly_rate_curency.Text;
            }
        }
        private void InitializeMetricReportsTab()
        {
            
            if (!IsEdit)
            {
                if (CompanyProfile != null && CompanyProfile.Id > -1)
                {
                    textBox_quality_metric_name.Text = CompanyProfile.MetricGroup.Name;
                    numericUpDown_company_profile_maximum_value.Value = CompanyProfile.MetricGroup.MaxSeverityValue;
                    numericUpDown_company_profile_maximum_value_in_words.Value = CompanyProfile.MetricGroup.MaxSeverityInValue;
                 
                }
                else
                {
                    textBox_quality_metric_name.Text = Activity.MetricReportSettings.MetricGroupName;
                    numericUpDown_company_profile_maximum_value.Value = Activity.MetricReportSettings.MaxSeverityValue;
                    numericUpDown_company_profile_maximum_value_in_words.Value = Activity.MetricReportSettings.MaxSeverityInValue;
             
                }
            }
            else
            {
                textBox_quality_metric_name.Text = Activity.MetricReportSettings.MetricGroupName;
                numericUpDown_company_profile_maximum_value.Value = Activity.MetricReportSettings.MaxSeverityValue;
                numericUpDown_company_profile_maximum_value_in_words.Value = Activity.MetricReportSettings.MaxSeverityInValue;
                
            }

            numericUpDown_company_profile_maximum_value_in_words.Minimum = numericUpDown_company_profile_maximum_value.Value;
            numericUpDown_company_profile_maximum_value.Maximum = numericUpDown_company_profile_maximum_value_in_words.Value;
        }

        private bool CheckForErrors(DateTime start, DateTime end)
        {
            var enabled = true;
            if (textBox_name.Text.Trim() == string.Empty)
            {
                enabled = false;
                _activityName.SetError(textBox_name, PluginResources.The_activity_name_cannot_be_empty_);
            }
            else
            {
                _activityName.SetError(textBox_name, string.Empty);
            }

            if (end < start)
            {
                enabled = false;
                _dateEnd.SetError(dateTimePicker_end_hours, PluginResources.The_activity_end_date_cannot_precede_the_started_date_);
            }
            else if (end > Project.Due)
            {
                //enabled = false;
                _dateEnd.SetError(dateTimePicker_end_hours, PluginResources.The_activity_end_date_cannot_supersede_the_project_due_date_);
            }
            else
            {
                _dateEnd.SetError(dateTimePicker_end_hours, string.Empty);
            }

            _dateStart.SetError(dateTimePicker_start_hours,
                start > Project.Due
                    ? PluginResources.The_activity_started_date_cannot_supersede_the_project_due_date_
                    : string.Empty);

            return enabled;

        }
        private void EnableSaveButton()
        {
          

            var start = new DateTime(dateTimePicker_start_date.Value.Year, dateTimePicker_start_date.Value.Month, dateTimePicker_start_date.Value.Day
                , dateTimePicker_start_hours.Value.Hour, dateTimePicker_start_hours.Value.Minute, dateTimePicker_start_hours.Value.Second);

            var end = new DateTime(dateTimePicker_end_date.Value.Year, dateTimePicker_end_date.Value.Month, dateTimePicker_end_date.Value.Day
                , dateTimePicker_end_hours.Value.Hour, dateTimePicker_end_hours.Value.Minute, dateTimePicker_end_hours.Value.Second);


            var enabled = CheckForErrors(start, end);

            if (end < start)
            {
                var ts1 = start.Subtract(end);
                label_total_elapsed.Text = "- " + Math.Truncate(ts1.TotalDays) + " days, " + ts1.Hours + " hours, " + ts1.Minutes + " mins, " + ts1.Seconds + " secs";
                label_total_elapsed.ForeColor = Color.Red;                
            }
            else
            {
                var ts1 = end.Subtract(start);

                label_total_elapsed.Text = Math.Truncate(ts1.TotalDays) + " days, " + ts1.Hours + " hours, " + ts1.Minutes + " mins, " + ts1.Seconds + " secs";
                label_total_elapsed.ForeColor = Color.Black;
            }


            button_save.Enabled = enabled;
        }

        private void CheckActivatedControls()
        {
            var cbiProj = (ComboboxItem)comboBox_projects.SelectedItem;
            if (cbiProj.Value != null)
            {
                var tp = (Project)cbiProj.Value;
                Activity.ProjectId = tp.Id;                
            }
            EnableSaveButton();
        }



        private void SetActivityRateFromLanguageRate(LanguageRateGroup languageRateGroup)
        {
            if (languageRateGroup != null)
            {
                
                ActivityRates.LanguageRateId = languageRateGroup.Id;
                ActivityRates.LanguageRateName = languageRateGroup.Name;
                ActivityRates.LanguageRateCurrency = languageRateGroup.Currency;
                ActivityRates.LanguageRateTotal = Convert.ToDouble(numericUpDown_pem_rate_total.Value);
                ActivityRates.LanguageRates.Clear();

                foreach (var languageRate in languageRateGroup.LanguageRates)
                {
                    var newLanguageRate = new Sdl.Community.Structures.Projects.Activities.LanguageRate
                    {
                        ProjectActivityRateId = -1,
                        ProjectActivityId = Activity.Id,
                        SourceLanguage = languageRate.SourceLanguage,
                        TargetLanguage = languageRate.TargetLanguage,
                        RndType = languageRate.RndType,
                        BaseRate = languageRate.BaseRate,
                        RatePm = languageRate.RatePm,
                        RateCm = languageRate.RateCm,
                        RateRep = languageRate.RateRep,
                        Rate100 = languageRate.Rate100,
                        Rate95 = languageRate.Rate95,
                        Rate85 = languageRate.Rate85,
                        Rate75 = languageRate.Rate75,
                        Rate50 = languageRate.Rate50,
                        RateNew = languageRate.RateNew
                    };




                    ActivityRates.LanguageRates.Add(newLanguageRate);
                }

                ActivityRates.ProjectActivityId = Activity.Id;
               
            }
            else
            {
                ActivityRates.LanguageRateId = -1;
                ActivityRates.LanguageRateName = string.Empty;
                ActivityRates.LanguageRateCurrency = ActivityRates.LanguageRateCurrency;
                ActivityRates.LanguageRateTotal = 0;
                ActivityRates.LanguageRates.Clear();
            }
           
        }
        private void button_save_Click(object sender, EventArgs e)
        {

            if (numericUpDown_company_profile_maximum_value_in_words.Value < numericUpDown_company_profile_maximum_value.Value)
            {
                MessageBox.Show(PluginResources.The_maximum_penalty_value_applied_cannot_be_less_than_the_number_);
                return;
            }

            Activity.Name = textBox_name.Text.Trim();
            Activity.Description = textBox_description.Text;

            Activity.ActivityStatus = (Activity.Status)Enum.Parse(
                typeof(Activity.Status),comboBox_status.SelectedItem.ToString(), true);

            var start = new DateTime(dateTimePicker_start_date.Value.Year, dateTimePicker_start_date.Value.Month, dateTimePicker_start_date.Value.Day
                , dateTimePicker_start_hours.Value.Hour, dateTimePicker_start_hours.Value.Minute, dateTimePicker_start_hours.Value.Second);

            var end = new DateTime(dateTimePicker_end_date.Value.Year, dateTimePicker_end_date.Value.Month, dateTimePicker_end_date.Value.Day
                , dateTimePicker_end_hours.Value.Hour, dateTimePicker_end_hours.Value.Minute, dateTimePicker_end_hours.Value.Second);

            Activity.Billable = comboBox_billable.SelectedIndex == 0;

            Activity.Started = start;
            Activity.Stopped = end;

            


            Activity.MetricReportSettings.MaxSeverityValue = Convert.ToInt32(numericUpDown_company_profile_maximum_value.Value);
            Activity.MetricReportSettings.MaxSeverityInValue = Convert.ToInt32(numericUpDown_company_profile_maximum_value_in_words.Value);


          

            Activity.DocumentActivityRates.HourlyRateCurrency = CompanyProfile != null ? CompanyProfile.ProfileRate.HourlyRateCurrency : Tracked.Settings.GetGeneralProperty("defaultCurrency").Value;
            Activity.ComparisonOptions.IncludeTagsInComparison = checkBox_includeTagsInComparison.Checked;
            Activity.ComparisonOptions.ConsolidateChanges = checkBox_group_changes.Checked;
            Activity.ComparisonOptions.ComparisonType = comboBox_comparisonType.SelectedIndex;

            
            

            Activity.CustomRateChecked = checkBox_custom_rate.Checked;
            Activity.LanguageRateChecked = checkBox_language_rate.Checked;
            Activity.HourlyRateChecked = checkBox_hourly_rate.Checked;


            

            var cbi = (ComboboxItem)comboBox_pem_rates.SelectedItem;
            if (cbi.Value != null)
            {
                //check type; if the type of already = to ActivityRates, then leave it as it has already
                //been assigned; otherwise create a new ActivityRates class...
                if (cbi.Value.GetType() == typeof(ActivityRates))
                {
                    var prg = (ActivityRates)cbi.Value;                   
                    Activity.DocumentActivityRates = prg;                    
                }
                else if (cbi.Value.GetType() == typeof(LanguageRateGroup))
                {                   
                    var prg = (LanguageRateGroup)((LanguageRateGroup)cbi.Value).Clone();
                    SetActivityRateFromLanguageRate(prg);
                   
                    Activity.DocumentActivityRates = ActivityRates;
                    Activity.LanguageRateChecked = checkBox_language_rate.Checked;
                    //if the pem rate is specified, then the curreny associated with this rate takes precedence
                    //you cannot have multiple currencies related to one activity
                    Activity.DocumentActivityRates.HourlyRateCurrency = prg.Currency;

                  
                }                
            }
            else
            {
                Activity.LanguageRateChecked = false;
                Activity.DocumentActivityRates.LanguageRateId = -1;
                Activity.DocumentActivityRates.LanguageRateTotal = 0;
                Activity.DocumentActivityRates.LanguageRateName = string.Empty;
                Activity.DocumentActivityRates.LanguageRates.Clear();
            }

            Activity.DocumentActivityRates.CustomRateDescription = textBox_custom_rate_description.Text;
            Activity.DocumentActivityRates.CustomRateTotal = Convert.ToDouble(numericUpDown_custom_rate_total.Value);
            Activity.DocumentActivityRates.CustomRateCurrency = Activity.DocumentActivityRates.HourlyRateCurrency;

            Activity.DocumentActivityRates.HourlyRateDescription = textBox_hourly_rate_description.Text;            
            Activity.DocumentActivityRates.LanguageRateDescription = textBox_language_rate_description.Text;

            
            Activity.DocumentActivityRates.HourlyRateRate = Convert.ToDouble(numericUpDown_hourly_rate_rate.Value);
            Activity.DocumentActivityRates.HourlyRateQuantity = Convert.ToDouble(numericUpDown_hourly_rate_hours.Value);
            Activity.DocumentActivityRates.HourlyRateTotal = Convert.ToDouble(numericUpDown_hourly_rate_total.Value);
            Activity.DocumentActivityRates.LanguageRateTotal = Convert.ToDouble(numericUpDown_pem_rate_total.Value);
            Activity.HourlyRateChecked = checkBox_hourly_rate.Checked;



            var cbiClient = (ComboboxItem)comboBox_client.SelectedItem;
            CompanyProfile = (CompanyProfile) cbiClient.Value;
            
        
            Saved = true;

            Close();
        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }


        private void check_activity_rate_total()
        {
            decimal activityTotal = 0;

            if (checkBox_language_rate.Checked)
                activityTotal += numericUpDown_pem_rate_total.Value;

            if (checkBox_hourly_rate.Checked)
                activityTotal += numericUpDown_hourly_rate_total.Value;

            if (checkBox_custom_rate.Checked)
                activityTotal += numericUpDown_custom_rate_total.Value;

            label_activity_rates_total.Text = Math.Round(activityTotal, 2).ToString(CultureInfo.InvariantCulture);
            label_rate_total_currency.Text = label_hourly_rate_curency.Text;
        }


        #region   |  Details  |

        private void dateTimePicker_start_date_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                EnableSaveButton();
        }

        private void dateTimePicker_start_hours_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                EnableSaveButton();
        }

        private void dateTimePicker_end_date_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                EnableSaveButton();
        }

        private void dateTimePicker_end_hours_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                EnableSaveButton();
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                EnableSaveButton();
        }

        private void textBox_description_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                EnableSaveButton();
        }

        private void comboBox_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                EnableSaveButton();


           
        }

        private void comboBox_billable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                EnableSaveButton();

            if (comboBox_billable.SelectedIndex == 0)
            {
                //yes
                checkBox_hourly_rate.Enabled = true;
                checkBox_custom_rate.Enabled = true;

                checkBox_language_rate.Enabled = objectListView_documents.Items.Count > 0;
            }
            else
            {
                checkBox_hourly_rate.Checked = false;
                checkBox_hourly_rate.Enabled = false;
                

                checkBox_custom_rate.Checked = false;
                checkBox_custom_rate.Enabled = false;

                checkBox_language_rate.Checked = false;
                checkBox_language_rate.Enabled = false;
            }
        }

        private void linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            DateTime? ds = null;
            DateTime? de = null;

            foreach (var documentActivity in DocumentActivities)
            {
                if (!ds.HasValue)
                    ds = documentActivity.Started;

                if (!de.HasValue)
                    de = documentActivity.Stopped;

                if (ds > documentActivity.Started)
                    ds = documentActivity.Started;

                if (de < documentActivity.Stopped)
                    de = documentActivity.Stopped;
            }


            if (ds.HasValue && de.HasValue)
            {
                Activity.Started = ds;
                Activity.Stopped = de;

                dateTimePicker_start_date.Value = Activity.Started.Value;
                dateTimePicker_start_hours.Value = Activity.Started.Value;

                dateTimePicker_end_date.Value = Activity.Stopped.Value;
                dateTimePicker_end_hours.Value = Activity.Stopped.Value;
            }

        }

        #endregion


        #region  |  Activity Rates  |


        private void calculate_hourly_rate()
        {
            numericUpDown_hourly_rate_total.Value = Math.Round(numericUpDown_hourly_rate_rate.Value * numericUpDown_hourly_rate_hours.Value, 2);

            label_hourly_rate_curency.Text = CompanyProfile != null ? CompanyProfile.ProfileRate.HourlyRateCurrency : Tracked.Settings.GetGeneralProperty("defaultCurrency").Value;
            check_activity_rate_total();
        }

        private void calculate_pem_total(ActivityRates activityRates)
        {
            if (objectListView_documents.Items.Count > 0 && activityRates.LanguageRateName.Trim() != string.Empty)
            {
                numericUpDown_pem_rate_total.Value = 0;
                label_rate_currency.Text = activityRates.LanguageRateCurrency;

                var tcc = new TextComparer {Type = TextComparer.ComparisonType.Words};


                var dictTcas = new Dictionary<string, List<DocumentActivity>>();
                foreach (var documentActivity in DocumentActivities)
                    if (!dictTcas.ContainsKey(documentActivity.DocumentId))
                        dictTcas.Add(documentActivity.DocumentId, new List<DocumentActivity> { documentActivity });
                    else
                        dictTcas[documentActivity.DocumentId].Add(documentActivity);

             
                foreach (var kvp in dictTcas)
                {

                    var mergedDocuments = new MergedDocuments(kvp.Value, Activity, tcc, activityRates);


                    if (mergedDocuments.LanguageRate != null)
                    {                    
                        var exactWordsPem = mergedDocuments.LanguageRate.Rate100 * mergedDocuments.ExactWords;
                        var p99WordsPem = mergedDocuments.LanguageRate.Rate95 * mergedDocuments.Fuzzy99Words;
                        var p94WordsPem = mergedDocuments.LanguageRate.Rate85 * mergedDocuments.Fuzzy94Words;
                        var p84WordsPem = mergedDocuments.LanguageRate.Rate75 * mergedDocuments.Fuzzy84Words;
                        var p74WordsPem = mergedDocuments.LanguageRate.Rate50 * mergedDocuments.Fuzzy74Words;
                        var newWordsPem = mergedDocuments.LanguageRate.RateNew * mergedDocuments.NewWords;

                        var totalWordsPem = exactWordsPem + p99WordsPem + p94WordsPem + p84WordsPem + p74WordsPem + newWordsPem;

                        numericUpDown_pem_rate_total.Value += totalWordsPem;
                    }
                }
           
            }
            else
            {
                numericUpDown_pem_rate_total.Value = 0;
                label_rate_currency.Text = Tracked.Settings.GetGeneralProperty("defaultCurrency").Value;
            }

            check_activity_rate_total();

        }

        private void checkBox_hourly_rate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_hourly_rate.Checked)
            {
                numericUpDown_hourly_rate_hours.Enabled = true;
                numericUpDown_hourly_rate_rate.Enabled = true;
               
                linkLabel_get_total_hours_elapsed_from_documents.Enabled = objectListView_documents.Items.Count > 0 ? true : false;
                textBox_hourly_rate_description.Enabled = true;
                linkLabel_get_default_hourly_rate.Enabled = Activity.CompanyProfileId != -1 ? true : false;
                
            }
            else
            {
                numericUpDown_hourly_rate_hours.Enabled = false;
                numericUpDown_hourly_rate_rate.Enabled = false;
               
                linkLabel_get_total_hours_elapsed_from_documents.Enabled = false;
                textBox_hourly_rate_description.Enabled = false;
                linkLabel_get_default_hourly_rate.Enabled = false;
            }
            check_activity_rate_total();

        }
        private void checkBox_pem_rate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_language_rate.Checked)
            {
                comboBox_pem_rates.Enabled = true;
              
                textBox_language_rate_description.Enabled = true;
                linkLabel_pem_rate_view_details.Enabled = true;
            }
            else
            {
                comboBox_pem_rates.Enabled = false;
           
                textBox_language_rate_description.Enabled = false;
                linkLabel_pem_rate_view_details.Enabled = false;
            }
            check_activity_rate_total();
        }
        private void linkLabel_pem_rate_view_details_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //MessageBox.Show("TO DO!");
        }
        private void linkLabel_get_default_hourly_rate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (CompanyProfile != null)
            {
                numericUpDown_hourly_rate_rate.Value = CompanyProfile.ProfileRate.HourlyRateRate;
            }
        }
        private void linkLabel_get_total_hours_elapsed_from_documents_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (objectListView_documents.Items.Count > 0)
            {
              
                var tsTotal = new TimeSpan();

                foreach (var documentActivity in DocumentActivities)                
                    tsTotal = tsTotal.Add(new TimeSpan(documentActivity.TicksActivity));
                
                numericUpDown_hourly_rate_hours.Value = Convert.ToDecimal(tsTotal.TotalHours);
            }
            else
            {
                numericUpDown_hourly_rate_hours.Value = 0;
            }
        }
        private void numericUpDown_hourly_rate_hours_ValueChanged(object sender, EventArgs e)
        {
            calculate_hourly_rate();
        }
        private void numericUpDown_hourly_rate_rate_ValueChanged(object sender, EventArgs e)
        {
            calculate_hourly_rate();
        }
        private void comboBox_pem_rates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            var cbi = (ComboboxItem)comboBox_pem_rates.SelectedItem;
            if (cbi.Value != null)
            {

                if (cbi.Value.GetType() == typeof(ActivityRates))
                {
                    var prg = (ActivityRates)cbi.Value;
                    ActivityRates = prg.Clone() as ActivityRates ; 
                }
                else if (cbi.Value.GetType() == typeof(LanguageRateGroup))
                {
                    var prg = (LanguageRateGroup)cbi.Value;
                    SetActivityRateFromLanguageRate(prg);

                    Activity.LanguageRateChecked = checkBox_language_rate.Checked;
                    //if the pem rate is specified, then the curreny associated with this rate takes precedence
                    //you cannot have multiple currencies related to one activity
                    ActivityRates.HourlyRateCurrency = prg.Currency;
                }
                else
                {
                    SetActivityRateFromLanguageRate(null);
                }

                calculate_pem_total(ActivityRates);

                if (ActivityRates != null) label_hourly_rate_curency.Text = ActivityRates.LanguageRateCurrency;

                linkLabel_pem_rate_view_details.Enabled = true;
            }
            else
            {
                numericUpDown_pem_rate_total.Value = 0;
                label_rate_currency.Text = Tracked.Settings.GetGeneralProperty("defaultCurrency").Value;

                linkLabel_pem_rate_view_details.Enabled = false;
            }

           
        }


     

        private void checkBox_custome_rate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_custom_rate.Checked)
            {
                textBox_custom_rate_description.Enabled = true;
                numericUpDown_custom_rate_total.Enabled = true;
            }
            else
            {
                textBox_custom_rate_description.Enabled = false;
                numericUpDown_custom_rate_total.Enabled = false;
            }
            check_activity_rate_total();
        }

        private void textBox_custom_rate_name_TextChanged(object sender, EventArgs e)
        {
            check_activity_rate_total();
        }

        private void numericUpDown_custom_rate_total_ValueChanged(object sender, EventArgs e)
        {
            check_activity_rate_total();
        }

        private void textBox_language_rate_name_TextChanged(object sender, EventArgs e)
        {
            check_activity_rate_total();
        }

        private void textBox_hourly_rate_name_TextChanged(object sender, EventArgs e)
        {
            check_activity_rate_total();
        }

        #endregion

        #region  |   company  |


        private void numericUpDown_company_profile_maximum_value_in_words_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_company_profile_maximum_value_in_words.Value < numericUpDown_company_profile_maximum_value.Value)
            {
                MessageBox.Show(PluginResources.The_maximum_penalty_value_applied_cannot_be_greater_than_the_number_of_words_);
            }
            else
            {
                numericUpDown_company_profile_maximum_value.Maximum = numericUpDown_company_profile_maximum_value_in_words.Value;
            }
        }

        private void numericUpDown_company_profile_maximum_value_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_company_profile_maximum_value_in_words.Value < numericUpDown_company_profile_maximum_value.Value)
            {
                MessageBox.Show(PluginResources.The_maximum_penalty_value_applied_cannot_be_greater_than_the_number_of_words_);
            }
            else
            {
                numericUpDown_company_profile_maximum_value_in_words.Minimum = numericUpDown_company_profile_maximum_value.Value;
            }
        }
        #endregion

        #region  |  comparer  |

        private void checkBox_group_changes_CheckedChanged(object sender, EventArgs e)
        {
            Activity.ComparisonOptions.ConsolidateChanges = checkBox_group_changes.Checked;
        }

        #endregion

        private void label_hourly_rate_curency_TextChanged(object sender, EventArgs e)
        {
            label_custom_rate_currency.Text = label_hourly_rate_curency.Text;
            label_rate_total_currency.Text = label_hourly_rate_curency.Text;
        }



    }
}
