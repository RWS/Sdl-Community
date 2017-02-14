using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Sdl.Community.DQF;
using Sdl.Community.DQF.Core;
using Sdl.Community.Qualitivity.Custom;
using Sdl.Community.Qualitivity.Dialogs.LanguageRate;
using Sdl.Community.Qualitivity.Dialogs.QualityMetrics;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.Community.Structures.Comparer;
using Sdl.Community.Structures.Configuration;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Profile.Base;
using Sdl.Community.Structures.Rates;
using Sdl.Community.Structures.Rates.Base;
using Sdl.Community.TM.Database;
using LanguageRateGroup = Sdl.Community.Structures.Rates.LanguageRateGroup;
using Process = System.Diagnostics.Process;
using QualityMetric = Sdl.Community.Structures.QualityMetrics.QualityMetric;
using QualityMetricGroup = Sdl.Community.Structures.QualityMetrics.QualityMetricGroup;

namespace Sdl.Community.Qualitivity.Dialogs
{
    public partial class Settings : Form
    {

        public bool Saved { get; set; }
        public Sdl.Community.Structures.Configuration.Settings settings { get; set; }
        private bool IsLoading { get; set; }

        private ListViewSortManager _mSortMgr;

        //keep track of what was added, updated & deleted
        public List<ChangedItem> ChangedItems { get; set; }
        public class ChangedItem
        {
            public enum ItemType
            {
                CompanyProfile,
                UserProfile,
                languageRateGroup,
                TrackingSettings,
                GeneralSettings,
                QualityMetricsGroup,
                BackUpSettings,
                DqfSettings
            }
            public enum ItemAction
            {
                Added,
                Updated,
                Deleted
            }
            public int Id { get; set; }
            public ItemType Type { get; set; }
            public ItemAction Action { get; set; }

            public ChangedItem(int id, ItemType itemType, ItemAction itemAction)
            {
                Id = id;
                Type = itemType;
                Action = itemAction;
            }
        }

        public Settings()
        {

            InitializeComponent();

            IsLoading = true;

            _mSortMgr = new ListViewSortManager(
            listView_quality_metrics,
                  new[] {  
                                                           typeof(ListViewTextSort),
							                               typeof(ListViewTextSort),
							                               typeof(ListViewTextSort)
						                               },
                  0,
                  SortOrder.Ascending
                  );


            _mSortMgr = new ListViewSortManager(
                  listView_price_groups,
                        new[] {  
                                                                   typeof(ListViewTextSort),
							                                       typeof(ListViewTextSort),
                                                                    typeof(ListViewDoubleSort),
							                                       typeof(ListViewTextSort),
                                                                    typeof(ListViewDoubleSort),
							                                       typeof(ListViewDoubleSort),
                                                                    typeof(ListViewDoubleSort),
							                                       typeof(ListViewDoubleSort),
                                                                    typeof(ListViewDoubleSort),
							                                       typeof(ListViewDoubleSort),
                                                                    typeof(ListViewDoubleSort),
							                                       typeof(ListViewDoubleSort),
							                                       typeof(ListViewDoubleSort)
						                                       },
                        0,
                        SortOrder.Ascending
                        );


            treeView_clients.MouseDown += (sender, args) =>
                treeView_clients.SelectedNode = treeView_clients.GetNodeAt(args.X, args.Y);


            panel_general.Dock = DockStyle.Fill;
            panel_clients.Dock = DockStyle.Fill;
            panel_language_rates.Dock = DockStyle.Fill;
            panel_my_info.Dock = DockStyle.Fill;
            panel_activity_tracking.Dock = DockStyle.Fill;
            panel_quality_metrics.Dock = DockStyle.Fill;
            panel_backup.Dock = DockStyle.Fill;
            panel_dqf_panel.Dock = DockStyle.Fill;

            treeView_main.SelectedNode = treeView_main.Nodes[0];


        }


        private void Settings_Load(object sender, EventArgs e)
        {



            ChangedItems = new List<ChangedItem>();


            initialize_general();


            initialize_rate_groups();

            initialize_company();

            initialize_user();

            initialize_activity_tracking();

            initialize_qualityMetrics();

            initialize_backup();

            initialize_DQFSettings();

            treeView_price_groups_AfterSelect(null, null);

            listView_price_groups_SelectedIndexChanged(null, null);

            IsLoading = false;
        }


        private void button_Save_Click(object sender, EventArgs e)
        {

            var errorWithCompany = false;

            #region  |  company  |


            var clientNames = new List<string>();
            foreach (TreeNode tn in treeView_clients.Nodes)
            {
                var cpi = (CompanyProfile)tn.Tag;
                if (cpi.Name.Trim() != string.Empty)
                {
                    if (!clientNames.Contains(cpi.Name.Trim()))
                    {
                        clientNames.Add(cpi.Name.Trim());
                    }
                    else
                    {
                        errorWithCompany = true;
                        MessageBox.Show(this, string.Format(PluginResources.Unable_to_save_the_company_profile___0____, cpi.Name), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        break;
                    }
                }
                else
                {
                    errorWithCompany = true;
                    MessageBox.Show(this, PluginResources.Unable_to_save_the_company_profile__the_company_name_cannot_be_null_, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                }
            }


            #endregion

            if (errorWithCompany) return;
            {

                if (textBox_backup_folder.Text.Trim() != string.Empty && Directory.Exists(textBox_backup_folder.Text.Trim()))
                {

                    #region  |  general  |

                    settings.GetGeneralProperty("defaultFilterProjectStatus").Value = comboBox_default_project_status.SelectedItem.ToString().Trim();
                    settings.GetGeneralProperty("defaultFilterActivityStatus").Value = comboBox_default_activity_status.SelectedItem.ToString().Trim();
                    settings.GetGeneralProperty("defaultFilterGroupBy").Value = comboBox_default_project_group_by.SelectedItem.ToString().Trim();
                    settings.GetGeneralProperty("defaultActivityViewGroupsIsOn").Value = comboBox_defaultActivityViewGroupsIsOn.SelectedIndex == 0 ? "true" : "false";
                    settings.GetGeneralProperty("defaultIncludeUnlistedProjects").Value = checkBox_include_unlisted_projects.Checked.ToString().ToLower();




                    var cbi = (ComboboxItem)comboBox_default_currency.Items[comboBox_default_currency.SelectedIndex];
                    var c = (Currency)cbi.Value;
                    settings.GetGeneralProperty("defaultCurrency").Value = c.Name;




                    #endregion

                    #region  |  activity tracking  |


                    settings.GetTrackingProperty("confirmActivitiesOnComplete").Value = checkBox_trackerConfirmActivities.Checked.ToString();
                    settings.GetTrackingProperty("idleTimeOut").Value = checkBox_idleTimeOut.Checked.ToString();
                    settings.GetTrackingProperty("idleTimeOutMinutes").Value = Convert.ToInt32(numericUpDown_idleTimeOutMinutes.Value).ToString();
                    settings.GetTrackingProperty("idleTimeOutShow").Value = checkBox_idleTimeOutShow.Checked.ToString();
                    settings.GetTrackingProperty("startOnLoad").Value = checkBox_startOnLoad.Checked.ToString();

                    settings.GetTrackingProperty("recordNonUpdatedSegments").Value = checkBox_recordNonUpdatedSegments.Checked.ToString();
                    settings.GetTrackingProperty("recordKeyStokes").Value = checkBox_recordKeyStokes.Checked.ToString();

                    settings.GetTrackingProperty("autoStartTrackingOnDocumentOpenEvent").Value = checkBox_autoStartTrackingOnDocumentOpenEvent.Checked.ToString();

                    settings.GetTrackingProperty("warningMessageActivityTrackingNotRunning").Value = checkBox_warningMessageActivityTrackingNotRunning.Checked.ToString();

                    #endregion

                    #region  |  quality metrics  |



                    settings.QualityMetricGroupSettings = new QualityMetricGroupSettings();
                    foreach (TreeNode tn in treeView_metric_group.Nodes)
                    {
                        var qmg = (QualityMetricGroup)tn.Tag;
                        settings.QualityMetricGroupSettings.QualityMetricGroups.Add(qmg);

                        if (qmg.IsDefault)
                            settings.QualityMetricGroup = qmg;

                    }

                    #endregion

                    #region  |  userProfile  |



                    settings.UserProfile.Name = textBox_userCompanyName.Text.Trim();
                    settings.UserProfile.UserName = textBox_userName.Text.Trim();
                    settings.UserProfile.TaxCode = textBox_userTaxCode.Text.Trim();
                    settings.UserProfile.VatCode = textBox_userVatCode.Text.Trim();
                    settings.UserProfile.Email = textBox_userInternetEmail.Text.Trim();
                    settings.UserProfile.Web = textBox_userInternetWebPageAddress.Text.Trim();
                    settings.UserProfile.Phone = textBox_userPhoneNumber.Text.Trim();
                    settings.UserProfile.Fax = textBox_userFaxNumber.Text.Trim();

                    #endregion

                    #region  |  backup  |

                    settings.GetBackupProperty(@"backupFolder").Value = textBox_backup_folder.Text.Trim();
                    settings.GetBackupProperty(@"backupEvery").Value = Convert.ToInt32(numericUpDown_backup_every.Value).ToString();
                    settings.GetBackupProperty(@"backupEveryType").Value = comboBox_backup_every_type.SelectedIndex.ToString();

                    #endregion

                    #region  |  DQF settings  |



                    settings.DqfSettings.UserKey = textBox_profile_user_dqf_key.Text.Trim();
                    settings.DqfSettings.UserName = textBox_userName.Text.Trim();
                    settings.DqfSettings.UserEmail = textBox_userInternetEmail.Text.Trim();
                    settings.DqfSettings.TranslatorKey = textBox_profile_user_translator_dqf_key.Text.Trim();
                    settings.DqfSettings.TranslatorName = textBox_userName.Text.Trim();
                    settings.DqfSettings.TranslatorEmail = textBox_userInternetEmail.Text.Trim();

                    #endregion

                    Saved = true;
                    Close();
                }
                else
                {
                    MessageBox.Show(this, PluginResources.Unable_to_save_settings__unable_to_locate_the_backup_folder_specified_, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void treeView_main_AfterSelect(object sender, TreeViewEventArgs e)
        {
            pictureBox_header.Image = imageList_settings_navigation.Images[e.Node.ImageIndex];
            textBox_header.Text = e.Node.Text;

            switch (e.Node.Name)
            {
                case @"Node_general": panel_general.BringToFront(); break;
                case @"Node_clients": panel_clients.BringToFront(); break;
                case @"Node_language_rates": panel_language_rates.BringToFront(); break;
                case @"Node_my_info": panel_my_info.BringToFront(); break;
                case @"Node_taus_dqf": panel_dqf_panel.BringToFront(); break;
                case @"Node_activity_tracking": panel_activity_tracking.BringToFront(); break;
                case @"Node_quality_metrics":
                    {
                        if (!panel_quality_metrics.Enabled)
                            panel_dqf_panel.BringToFront();

                        panel_quality_metrics.BringToFront(); break;
                    }
                case @"Node_backup": panel_backup.BringToFront(); break;
            }
        }


        #region  |  general  settings  |


        private void initialize_general()
        {

            #region  |  default project status  |


            var defaultFilterProjectStatus = settings.GetGeneralProperty("defaultFilterProjectStatus").Value;

            var defaultIncludeUnlistedProjects = Convert.ToBoolean(settings.GetGeneralProperty("defaultIncludeUnlistedProjects").Value);

            checkBox_include_unlisted_projects.Checked = defaultIncludeUnlistedProjects;

            if (defaultFilterProjectStatus.Trim() != string.Empty)
            {
                if (defaultFilterProjectStatus.Trim().IndexOf(@"Show all projects", StringComparison.Ordinal) > -1)
                    comboBox_default_project_status.SelectedIndex = 0;
                else if (defaultFilterProjectStatus.Trim().IndexOf(@"In progress", StringComparison.Ordinal) > -1)
                    comboBox_default_project_status.SelectedIndex = 1;
                else if (defaultFilterProjectStatus.Trim().IndexOf(@"Completed", StringComparison.Ordinal) > -1)
                    comboBox_default_project_status.SelectedIndex = 2;
                else
                    comboBox_default_project_status.SelectedIndex = 0;
            }
            else
            {
                comboBox_default_project_status.SelectedIndex = 0;
            }


            #endregion

            #region  |  default activity status  |

            var defaultFilterActivityStatus = settings.GetGeneralProperty("defaultFilterActivityStatus").Value;

            if (defaultFilterActivityStatus.Trim() != string.Empty)
            {
                if (defaultFilterActivityStatus.Trim().IndexOf(@"Show all activities", StringComparison.Ordinal) > -1)
                    comboBox_default_activity_status.SelectedIndex = 0;
                else if (defaultFilterActivityStatus.Trim().IndexOf(@"New", StringComparison.Ordinal) > -1)
                    comboBox_default_activity_status.SelectedIndex = 1;
                else if (defaultFilterActivityStatus.Trim().IndexOf(@"Confirmed", StringComparison.Ordinal) > -1)
                    comboBox_default_activity_status.SelectedIndex = 2;
                else
                    comboBox_default_activity_status.SelectedIndex = 0;
            }
            else
            {
                comboBox_default_activity_status.SelectedIndex = 0;
            }


            #endregion

            #region  | default project grouped by  |

            var defaultFilterGroupBy = settings.GetGeneralProperty("defaultFilterGroupBy").Value;

            if (defaultFilterGroupBy.Trim() != string.Empty)
            {
                if (defaultFilterGroupBy.Trim().IndexOf(@"Client name", StringComparison.Ordinal) > -1)
                    comboBox_default_project_group_by.SelectedIndex = 0;
                else if (defaultFilterGroupBy.Trim().IndexOf(@"Project name", StringComparison.Ordinal) > -1)
                    comboBox_default_project_group_by.SelectedIndex = 1;
                else if (defaultFilterGroupBy.Trim().IndexOf(@"Date (year/month)", StringComparison.Ordinal) > -1)
                    comboBox_default_project_group_by.SelectedIndex = 2;
                else
                    comboBox_default_project_group_by.SelectedIndex = 0;
            }
            else
            {
                comboBox_default_project_group_by.SelectedIndex = 0;
            }

            #endregion


            #region  |  currency  |

            var defaultCurrency = settings.GetGeneralProperty("defaultCurrency").Value;

            comboBox_default_currency.BeginUpdate();
            foreach (var c in Tracked.Currencies)
            {
                comboBox_default_currency.Items.Add(new ComboboxItem(c.Name + "  (" + c.Country + ")", c));
            }
            comboBox_default_currency.EndUpdate();


            var iSelectedIndex = -1;
            var iDefaultIndex = 0;
            for (var i = 0; i < comboBox_default_currency.Items.Count; i++)
            {
                var cbi = (ComboboxItem)comboBox_default_currency.Items[i];

                var c = (Currency)cbi.Value;
                if (string.Compare(c.Name, defaultCurrency, StringComparison.OrdinalIgnoreCase) != 0) continue;
                iSelectedIndex = i;
                break;
            }
            comboBox_default_currency.SelectedIndex = iSelectedIndex > -1 ? iSelectedIndex : iDefaultIndex;

            #endregion

            var defaultActivityViewGroupsIsOn = Convert.ToBoolean(settings.GetGeneralProperty("defaultActivityViewGroupsIsOn").Value);


            comboBox_defaultActivityViewGroupsIsOn.SelectedIndex = defaultActivityViewGroupsIsOn ? 0 : 1;


        }

        private void comboBox_default_currency_SelectedIndexChanged(object sender, EventArgs e)
        {

            var cbi = (ComboboxItem)comboBox_default_currency.Items[comboBox_default_currency.SelectedIndex];
            var c = (Currency)cbi.Value;
            settings.GetGeneralProperty("defaultCurrency").Value = c.Name;

            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.GeneralSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.GeneralSettings, ChangedItem.ItemAction.Updated));


        }

        private void comboBox_default_project_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.GeneralSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.GeneralSettings, ChangedItem.ItemAction.Updated));
        }

        private void comboBox_default_activity_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.GeneralSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.GeneralSettings, ChangedItem.ItemAction.Updated));
        }

        private void comboBox_default_project_group_by_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.GeneralSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.GeneralSettings, ChangedItem.ItemAction.Updated));
        }

        private void comboBox_defaultActivityViewGroupsIsOn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.GeneralSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.GeneralSettings, ChangedItem.ItemAction.Updated));
        }

        #endregion


        #region  |  activity tracking  |


        private void initialize_activity_tracking()
        {
            var idleTimeOutMinutes = Convert.ToInt32(settings.GetTrackingProperty("idleTimeOutMinutes").Value);

            checkBox_trackerConfirmActivities.Checked = Convert.ToBoolean(settings.GetTrackingProperty("confirmActivitiesOnComplete").Value);
            checkBox_idleTimeOut.Checked = Convert.ToBoolean(settings.GetTrackingProperty("idleTimeOut").Value);

            if (idleTimeOutMinutes < 5)
                idleTimeOutMinutes = 5;
            if (idleTimeOutMinutes > 60)
                idleTimeOutMinutes = 60;

            numericUpDown_idleTimeOutMinutes.Value = idleTimeOutMinutes;
            checkBox_idleTimeOutShow.Checked = Convert.ToBoolean(settings.GetTrackingProperty("idleTimeOutShow").Value);
            checkBox_startOnLoad.Checked = Convert.ToBoolean(settings.GetTrackingProperty("startOnLoad").Value);
            checkBox_recordNonUpdatedSegments.Checked = Convert.ToBoolean(settings.GetTrackingProperty("recordNonUpdatedSegments").Value);


            checkBox_recordKeyStokes.Checked = Convert.ToBoolean(settings.GetTrackingProperty("recordKeyStokes").Value);
            checkBox_recordKeyStokes.Enabled = true;


            checkBox_autoStartTrackingOnDocumentOpenEvent.Checked = Convert.ToBoolean(settings.GetTrackingProperty("autoStartTrackingOnDocumentOpenEvent").Value);
            checkBox_warningMessageActivityTrackingNotRunning.Checked = Convert.ToBoolean(settings.GetTrackingProperty("warningMessageActivityTrackingNotRunning").Value);

            checkBox_idleTimeOut_CheckedChanged(null, null);
            checkBox_autoStartTrackingOnDocumentOpenEvent_CheckedChanged(null, null);
        }


        private void checkBox_autoStartTrackingOnDocumentOpenEvent_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_autoStartTrackingOnDocumentOpenEvent.Checked)
            {
                checkBox_warningMessageActivityTrackingNotRunning.Enabled = true;
            }
            else
            {
                checkBox_warningMessageActivityTrackingNotRunning.Enabled = false;
                checkBox_warningMessageActivityTrackingNotRunning.Checked = false;
            }

            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void checkBox_idleTimeOut_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_idleTimeOut.Checked)
            {
                checkBox_idleTimeOutShow.Enabled = true;
            }
            else
            {
                checkBox_idleTimeOutShow.Checked = false;
                checkBox_idleTimeOutShow.Enabled = false;
            }

            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void checkBox_trackerConfirmActivities_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void checkBox_startOnLoad_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void checkBox_idleTimeOutShow_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void numericUpDown_idleTimeOutMinutes_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void checkBox_warningMessageActivityTrackingNotRunning_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void checkBox_record_segments_that_are_updated_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void checkBox_recordNonUpdatedSegments_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        private void checkBox_recordKeyStokes_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.TrackingSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.TrackingSettings, ChangedItem.ItemAction.Updated));
        }

        #endregion


        #region  |  company profile info  |

        private void CheckCompanyNameValid(TreeNode tn)
        {
            if (tn == null) return;
            var cpi = (CompanyProfile)tn.Tag;

            if (tn.Text.Trim() == string.Empty)
            {
                tn.ImageKey = @"user_red";
                tn.SelectedImageKey = @"user_red";
                tn.ToolTipText = PluginResources.The_company_name_cannot_be_null_;
            }
            else
            {
                tn.ImageKey = @"user_blue";
                tn.SelectedImageKey = @"user_blue";
                tn.ToolTipText = "";

                #region  |  company  |


                new List<string>();
                foreach (TreeNode _tn in treeView_clients.Nodes)
                {
                    var _cpi = (CompanyProfile)_tn.Tag;

                    if (cpi.Id == _cpi.Id) continue;
                    if (string.Compare(cpi.Name.Trim(), _cpi.Name.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                        continue;
                    tn.ImageKey = @"user_yellow";
                    tn.SelectedImageKey = @"user_yellow";
                    tn.ToolTipText = PluginResources.The_company_name_already_exists_;
                }


                #endregion
            }
        }


        private void initialize_company()
        {
            treeView_clients.Nodes.Clear();

            foreach (var cpi in settings.CompanyProfiles)
            {
                AddCompanyToList(cpi);
            }

            if (treeView_clients.Nodes.Count > 0)
                treeView_clients.SelectedNode = treeView_clients.Nodes[0];
        }

        private void AddNewCompany()
        {
            var cpiNew = new CompanyProfile
            {
                Id = -1,
                Name = PluginResources.New_Company,
                ProfileRate = new CompanyProfileRate(),
                ComparerOptions = new ComparerSettings(),
                MetricGroup = new QualityMetricGroup()
            };

            cpiNew.ProfileRate.HourlyRateCurrency = settings.GetGeneralProperty("defaultCurrency").Value;

            AddCompanyToList(cpiNew);

            settings.CompanyProfiles.Add(cpiNew);

            textBox_companyName.Select();

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpiNew.Id)))
                ChangedItems.Add(new ChangedItem(cpiNew.Id, ChangedItem.ItemType.CompanyProfile
                    , cpiNew.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void AddCompanyToList(Profile cpi)
        {
            var tn = treeView_clients.Nodes.Add(cpi.Name);
            tn.Tag = cpi;
            treeView_clients.SelectedNode = tn;

            CheckCompanyNameValid(tn);
        }

        private void RemoveCompany()
        {
            if (treeView_clients.SelectedNode == null) return;
            var dr = MessageBox.Show(PluginResources.Are_you_sure_that_you_want_to_remove_the_selected_client_profile_ + "\r\n\r\n"
                                     + PluginResources.Note__you_will_not_be_able_to_recover_this_data_if_you_continue_ + "\r\n\r\n"
                                     + PluginResources.Click__Yes__to_continue_and_remove_the_client_profile + "\r\n"
                                     + PluginResources.Click__No__to_cancel
                , Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != DialogResult.Yes) return;
            var tn = treeView_clients.SelectedNode;

            var cpiEdit = (CompanyProfile)tn.Tag;

            var continueDelete = true;
            var message = string.Empty;

            foreach (var tp in Tracked.TrackingProjects.TrackerProjects)
            {
                if (tp.CompanyProfileId != cpiEdit.Id) continue;
                continueDelete = false;
                message = string.Format(PluginResources.Unable_to_delete_the_client___0__, cpiEdit.Name) + "\r\n";
                message += string.Format(PluginResources.Note__it_is_made_reference_to_from_the_project___0__, tp.Name);
                break;
            }


            if (continueDelete)
            {

                if (!IsLoading && cpiEdit.Id > -1)
                {

                    var existingItem = ChangedItems.Find(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpiEdit.Id));
                    if (existingItem != null)
                        existingItem.Action = ChangedItem.ItemAction.Deleted;
                    else
                        ChangedItems.Add(new ChangedItem(cpiEdit.Id, ChangedItem.ItemType.CompanyProfile, ChangedItem.ItemAction.Deleted));
                }


                for (var i = 0; i < settings.CompanyProfiles.Count; i++)
                {

                    var cpi = settings.CompanyProfiles[i];


                    if (cpi.Id != cpiEdit.Id) continue;
                    settings.CompanyProfiles.RemoveAt(i);

                    break;
                }

                tn.Remove();

                treeView_clients_AfterSelect(null, null);
            }
            else
            {
                MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void addClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewCompany();
        }
        private void removeClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveCompany();
        }
        private void toolStripButton_addClient_Click(object sender, EventArgs e)
        {
            AddNewCompany();

        }
        private void toolStripButton_deleteClient_Click(object sender, EventArgs e)
        {
            RemoveCompany();

        }


        private void EditClientAddress()
        {
            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            var f = new AddressDetails
            {
                textBox_addressStreet = { Text = cpi.Street.Trim() },
                textBox_addressCity = { Text = cpi.City.Trim() },
                textBox_addressState = { Text = cpi.State.Trim() },
                textBox_addressZip = { Text = cpi.Zip.Trim() },
                textBox_addressCountry = { Text = cpi.Country.Trim() }
            };


            f.ShowDialog();
            if (!f.Saved) return;
            cpi.Street = f.textBox_addressStreet.Text.Trim();
            cpi.City = f.textBox_addressCity.Text.Trim();
            cpi.State = f.textBox_addressState.Text.Trim();
            cpi.Zip = f.textBox_addressZip.Text.Trim();
            cpi.Country = f.textBox_addressCountry.Text.Trim();


            textBox_companyAddress.Text = cpi.Street + "\r\n"
                                          + cpi.Zip + @" "
                                          + cpi.City
                                          + (cpi.State != string.Empty ? " (" + cpi.State + ")" : string.Empty) + "\r\n"
                                          + cpi.Country;

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void SetClientProfileView(CompanyProfile cpi)
        {
            textBox_companyName.Text = cpi.Name;

            textBox_companyAddress.Text = cpi.Street + "\r\n"
                       + cpi.Zip + @" "
                       + cpi.City
                       + (cpi.State != string.Empty ? " (" + cpi.State + ")" : string.Empty) + "\r\n"
                       + cpi.Country;


            textBox_company_contactName.Text = cpi.ContactName;

            textBox_companyTaxCode.Text = cpi.TaxCode;
            textBox_companyVatCode.Text = cpi.VatCode;

            textBox_companyEmail.Text = cpi.Email;
            textBox_companyWebPageAddress.Text = cpi.Web;

            textBox_companyPhoneNumber.Text = cpi.Phone;
            textBox_companyFaxNumber.Text = cpi.Fax;



            if (cpi.ProfileRate.HourlyRateCurrency == string.Empty)
                cpi.ProfileRate.HourlyRateCurrency = settings.GetGeneralProperty("defaultCurrency").Value;

            try
            {
                #region  |  PEM Rate  |

                comboBox_company_pem_rate.BeginUpdate();

                comboBox_company_pem_rate.Items.Clear();
                var _cbi = new ComboboxItem(@"<empty>", null);
                comboBox_company_pem_rate.Items.Add(_cbi);


                foreach (var _prg in settings.LanguageRateGroups)
                {
                    var cbi = new ComboboxItem(_prg.Name, _prg);
                    comboBox_company_pem_rate.Items.Add(cbi);
                }
                comboBox_company_pem_rate.Sorted = true;


                var selectedIndex = 0;
                var foundPriceRateGroup = false;
                LanguageRateGroup prg = null;
                foreach (ComboboxItem cbi in comboBox_company_pem_rate.Items)
                {
                    if (cbi.Value != null)
                    {
                        prg = (LanguageRateGroup)cbi.Value;
                        if (prg.Id == cpi.ProfileRate.LanguageRateId)
                        {
                            foundPriceRateGroup = true;
                            break;
                        }
                        selectedIndex++;
                    }
                    else
                        selectedIndex++;
                }
                comboBox_company_pem_rate.SelectedIndex = foundPriceRateGroup ? selectedIndex : 0;

                if (prg != null && foundPriceRateGroup && prg.Name.Trim() != string.Empty)
                    label_company_pem_currency.Text = prg.Currency;
                else
                    label_company_pem_currency.Text = @"n/a";

                #endregion

                #region  |  hourly Rate  |

                checkBox_auto_add_hourly_rate.Checked = cpi.ProfileRate.HourlyRateAutoAdd;
                checkBox_auto_add_language_rate.Checked = cpi.ProfileRate.LanguageRateAutoAdd;

                if (foundPriceRateGroup && prg != null)
                {
                    comboBox_company_hourly_rate_currency.Enabled = false;


                    if (prg.Name.Trim() != string.Empty)
                        cpi.ProfileRate.HourlyRateCurrency = prg.Currency;
                }
                else
                {
                    comboBox_company_hourly_rate_currency.Enabled = true;
                }


                comboBox_company_hourly_rate_currency.BeginUpdate();
                comboBox_company_hourly_rate_currency.Items.Clear();
                foreach (var c in Tracked.Currencies)
                {
                    comboBox_company_hourly_rate_currency.Items.Add(new ComboboxItem(c.Name + "  (" + c.Country + ")", c));
                }
                comboBox_company_hourly_rate_currency.EndUpdate();


                var iSelectedIndex = -1;
                var iDefaultIndex = 0;
                for (var i = 0; i < comboBox_company_hourly_rate_currency.Items.Count; i++)
                {
                    var cbi = (ComboboxItem)comboBox_company_hourly_rate_currency.Items[i];

                    var c = (Currency)cbi.Value;
                    if (string.Compare(c.Name, cpi.ProfileRate.HourlyRateCurrency, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        iSelectedIndex = i;
                        break;
                    }
                    if (string.Compare(c.Name, settings.GetGeneralProperty("defaultCurrency").Value, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        iDefaultIndex = i;
                    }
                }
                comboBox_company_hourly_rate_currency.SelectedIndex = iSelectedIndex > -1 ? iSelectedIndex : iDefaultIndex;


                #endregion

                #region  |  comparer settings  |

                comboBox_comparisonType.SelectedIndex = cpi.ComparerOptions.ComparisonType;
                checkBox_includeTagsInComparison.Checked = cpi.ComparerOptions.IncludeTagsInComparison;
                checkBox_group_changes.Checked = cpi.ComparerOptions.ConsolidateChanges;


                UpdateVisualStyle(cpi);

                #endregion

                #region  |  quality metric settings  |

                comboBox_quality_metric_groups.BeginUpdate();

                comboBox_quality_metric_groups.Items.Clear();
                _cbi = new ComboboxItem(@"<empty>", new QualityMetricGroup());
                comboBox_quality_metric_groups.Items.Add(_cbi);


                foreach (var qmg in settings.QualityMetricGroupSettings.QualityMetricGroups)
                {
                    var cbi = new ComboboxItem(qmg.Name, qmg);
                    comboBox_quality_metric_groups.Items.Add(cbi);
                }
                comboBox_quality_metric_groups.Sorted = true;


                selectedIndex = 0;
                var foundMetricGroup = false;
                foreach (ComboboxItem cbi in comboBox_quality_metric_groups.Items)
                {
                    if (cbi.Value != null)
                    {
                        var qmgFound = (QualityMetricGroup)cbi.Value;
                        if (qmgFound.Id == cpi.MetricGroup.Id)
                        {
                            foundMetricGroup = true;
                            break;
                        }
                        selectedIndex++;
                    }
                    else
                        selectedIndex++;
                }
                comboBox_quality_metric_groups.SelectedIndex = foundMetricGroup ? selectedIndex : 0;




                #endregion
            }
            finally
            {
                comboBox_company_pem_rate.EndUpdate();
                comboBox_quality_metric_groups.EndUpdate();
            }

            numericUpDown_company_hourly_rate.Value = cpi.ProfileRate.HourlyRateRate;
        }
        private void UpdateVisualStyle(CompanyProfile cpi)
        {
            UpdateVisualTextFormatting(cpi.ComparerOptions.StyleNewText, richTextBox_formatting_Text_New, PluginResources.This_is_an_example_of_inserted_text_formatting_, 31, 4);
            UpdateVisualTextFormatting(cpi.ComparerOptions.StyleRemovedText, richTextBox_formatting_Text_Removed, PluginResources.This_is_an_example_of_deleted_text_formatting_, 30, 4);
            UpdateVisualTextFormatting(cpi.ComparerOptions.StyleNewTag, richTextBox_formatting_Tag_New, PluginResources.This_is_an_example_of_inserted__tag___formatting_, 31, 6);
            UpdateVisualTextFormatting(cpi.ComparerOptions.StyleRemovedTag, richTextBox_formatting_Tag_Removed, PluginResources.This_is_an_example_of_deleted__tag___formatting_, 30, 6);
        }
        private void UpdateVisualTextFormatting(DifferencesFormatting differencesFormatting, RichTextBox richTextBox, string text, int selectionStart, int selectionLength)
        {
            richTextBox.Text = text;

            var isBold = string.Compare(differencesFormatting.StyleBold, @"Activate", StringComparison.OrdinalIgnoreCase) == 0;
            var isItalic = string.Compare(differencesFormatting.StyleItalic, @"Activate", StringComparison.OrdinalIgnoreCase) == 0;
            var isStrikethrough = string.Compare(differencesFormatting.StyleStrikethrough, @"Activate", StringComparison.OrdinalIgnoreCase) == 0;
            var isUnderline = string.Compare(differencesFormatting.StyleUnderline, @"Activate", StringComparison.OrdinalIgnoreCase) == 0;
            var strPosition = differencesFormatting.TextPosition;

            var fontStyleNew = GetFontStyle(isBold, isItalic, isStrikethrough, isUnderline);

            richTextBox.Select(selectionStart, selectionLength);
            richTextBox.SelectionFont = new Font(richTextBox.Font.Name, richTextBox.Font.Size, fontStyleNew);

            richTextBox.SelectionColor = differencesFormatting.FontSpecifyColor ? ColorTranslator.FromHtml(differencesFormatting.FontColor) : richTextBox.ForeColor;

            richTextBox.SelectionBackColor = differencesFormatting.FontSpecifyBackroundColor ? ColorTranslator.FromHtml(differencesFormatting.FontBackroundColor) : richTextBox.BackColor;

            switch (strPosition)
            {
                case @"Normal": richTextBox.SelectionCharOffset = 0; break;
                case @"Superscript": richTextBox.SelectionCharOffset = 5; break;
                case @"Subscript": richTextBox.SelectionCharOffset = -5; break;
            }


        }
        private static FontStyle GetFontStyle(bool bold, bool italic, bool strikethrough, bool underline)
        {
            var fontStyle = FontStyle.Regular;
            if (bold)
            {
                fontStyle = FontStyle.Bold;
            }
            if (italic)
            {
                if (fontStyle != FontStyle.Regular)
                    fontStyle = fontStyle | FontStyle.Italic;
                else
                    fontStyle = FontStyle.Italic;
            }
            if (strikethrough)
            {
                if (fontStyle != FontStyle.Regular)
                    fontStyle = fontStyle | FontStyle.Strikeout;
                else
                    fontStyle = FontStyle.Strikeout;
            }
            if (!underline) return fontStyle;
            if (fontStyle != FontStyle.Regular)
                fontStyle = fontStyle | FontStyle.Underline;
            else
                fontStyle = FontStyle.Underline;

            return fontStyle;
        }

        private void treeView_clients_AfterSelect(object sender, TreeViewEventArgs e)
        {

            try
            {
                IsLoading = true;

                if (treeView_clients.SelectedNode == null) return;
                var tn = treeView_clients.SelectedNode;
                var cpi = (CompanyProfile)tn.Tag;

                SetClientProfileView(cpi);
                CheckCompanyNameValid(tn);
            }
            finally
            {
                IsLoading = false;
            }


        }


        private void button_companyAddress_Click(object sender, EventArgs e)
        {
            EditClientAddress();
        }

        private void textBox_companyName_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.Name = textBox_companyName.Text;
            tn.Text = cpi.Name;
            tn.Tag = cpi;

            CheckCompanyNameValid(tn);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void comboBox_quality_metric_groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            try
            {

                if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                    ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                        , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));

                if (comboBox_quality_metric_groups.SelectedIndex <= -1) return;
                {
                    var cbi = (ComboboxItem)comboBox_quality_metric_groups.SelectedItem;
                    if (cbi.Value == null) return;
                    var prg = (QualityMetricGroup)cbi.Value;

                    cpi.MetricGroup = prg;
                    tn.Tag = cpi;

                    if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                        ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                            , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void textBox_company_contactName_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.ContactName = textBox_company_contactName.Text;
            tn.Text = cpi.Name;
            tn.Tag = cpi;

            CheckCompanyNameValid(tn);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_companyTaxCode_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.TaxCode = textBox_companyTaxCode.Text;
            tn.Text = cpi.Name;
            tn.Tag = cpi;

            CheckCompanyNameValid(tn);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_companyVatCode_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.VatCode = textBox_companyVatCode.Text;
            tn.Text = cpi.Name;
            tn.Tag = cpi;

            CheckCompanyNameValid(tn);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_companyEmail_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.Email = textBox_companyEmail.Text;
            tn.Text = cpi.Name;
            tn.Tag = cpi;

            CheckCompanyNameValid(tn);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_companyWebPageAddress_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.Web = textBox_companyWebPageAddress.Text;
            tn.Text = cpi.Name;
            tn.Tag = cpi;

            CheckCompanyNameValid(tn);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_companyPhoneNumber_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.Phone = textBox_companyPhoneNumber.Text;
            tn.Text = cpi.Name;
            tn.Tag = cpi;

            CheckCompanyNameValid(tn);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_companyFaxNumber_TextChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.Fax = textBox_companyFaxNumber.Text;
            tn.Text = cpi.Name;
            tn.Tag = cpi;

            CheckCompanyNameValid(tn);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void comboBox_company_price_rate_group_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            try
            {

                if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                    ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                        , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));

                if (comboBox_company_pem_rate.SelectedIndex > -1)
                {
                    var cbi = (ComboboxItem)comboBox_company_pem_rate.SelectedItem;
                    if (cbi.Value != null)
                    {
                        var prg = (LanguageRateGroup)cbi.Value;

                        cpi.ProfileRate.LanguageRateId = prg.Id;
                        tn.Tag = cpi;

                        cpi.ProfileRate.HourlyRateCurrency = prg.Currency;
                        label_company_pem_currency.Text = prg.Currency;

                        comboBox_company_hourly_rate_currency.Enabled = false;

                        var iSelectedIndex = -1;
                        const int iDefaultIndex = 0;
                        for (var i = 0; i < comboBox_company_hourly_rate_currency.Items.Count; i++)
                        {
                            var _cbi = (ComboboxItem)comboBox_company_hourly_rate_currency.Items[i];

                            var c = (Currency)_cbi.Value;
                            if (
                                string.Compare(c.Name, cpi.ProfileRate.HourlyRateCurrency,
                                    StringComparison.OrdinalIgnoreCase) != 0) continue;
                            iSelectedIndex = i;
                            break;
                        }
                        comboBox_company_hourly_rate_currency.SelectedIndex = iSelectedIndex > -1 ? iSelectedIndex : iDefaultIndex;


                        if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                            ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                                , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
                    }
                    else
                    {
                        cpi.ProfileRate.LanguageRateId = -1;
                        comboBox_company_hourly_rate_currency.Enabled = true;
                        label_company_pem_currency.Text = @"n/a";
                    }
                }
                else
                {
                    cpi.ProfileRate.LanguageRateId = -1;
                    comboBox_company_hourly_rate_currency.Enabled = true;
                    label_company_pem_currency.Text = @"n/a";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void numericUpDown_company_hourly_rate_ValueChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.ProfileRate.HourlyRateRate = numericUpDown_company_hourly_rate.Value;
            tn.Tag = cpi;

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void comboBox_company_hourly_rate_currency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            var cbi = (ComboboxItem)comboBox_company_hourly_rate_currency.SelectedItem;
            var c = (Currency)cbi.Value;

            cpi.ProfileRate.HourlyRateCurrency = c.Name;
            tn.Tag = cpi;

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void checkBox_includeTagsInComparison_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.ComparerOptions.IncludeTagsInComparison = checkBox_includeTagsInComparison.Checked;
            tn.Tag = cpi;

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void button_changeFormattingTextNew_Click(object sender, EventArgs e)
        {
            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            var f = new DifferenceFormatting
            {
                checkBox_fontColor = { Checked = cpi.ComparerOptions.StyleNewText.FontSpecifyColor },
                checkBox_backroundColor =
                {
                    Checked = cpi.ComparerOptions.StyleNewText.FontSpecifyBackroundColor
                }
            };


            if (f.checkBox_fontColor.Checked)
                f.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(cpi.ComparerOptions.StyleNewText.FontColor);
            if (f.checkBox_backroundColor.Checked)
                f.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(cpi.ComparerOptions.StyleNewText.FontBackroundColor);


            f.comboBox_styleBold.SelectedItem = cpi.ComparerOptions.StyleNewText.StyleBold;
            f.comboBox_styleItalic.SelectedItem = cpi.ComparerOptions.StyleNewText.StyleItalic;
            f.comboBox_styleStrikethrough.SelectedItem = cpi.ComparerOptions.StyleNewText.StyleStrikethrough;
            f.comboBox_styleUnderline.SelectedItem = cpi.ComparerOptions.StyleNewText.StyleUnderline;
            f.comboBox_stylePosition.SelectedItem = cpi.ComparerOptions.StyleNewText.TextPosition;


            f.ShowDialog();

            if (!f.SaveSettings) return;
            cpi.ComparerOptions.StyleNewText.Name = DifferencesFormatting.StyleName.NewText;

            cpi.ComparerOptions.StyleNewText.FontSpecifyColor = f.checkBox_fontColor.Checked;
            cpi.ComparerOptions.StyleNewText.FontSpecifyBackroundColor = f.checkBox_backroundColor.Checked;

            if (cpi.ComparerOptions.StyleNewText.FontSpecifyColor)
                cpi.ComparerOptions.StyleNewText.FontColor = ColorTranslator.ToHtml(f.label_fontColorDisplay.BackColor);
            if (cpi.ComparerOptions.StyleNewText.FontSpecifyBackroundColor)
                cpi.ComparerOptions.StyleNewText.FontBackroundColor = ColorTranslator.ToHtml(f.label_backroundColorDisplay.BackColor);

            cpi.ComparerOptions.StyleNewText.StyleBold = f.comboBox_styleBold.SelectedItem.ToString();
            cpi.ComparerOptions.StyleNewText.StyleItalic = f.comboBox_styleItalic.SelectedItem.ToString();
            cpi.ComparerOptions.StyleNewText.StyleStrikethrough = f.comboBox_styleStrikethrough.SelectedItem.ToString();
            cpi.ComparerOptions.StyleNewText.StyleUnderline = f.comboBox_styleUnderline.SelectedItem.ToString();
            cpi.ComparerOptions.StyleNewText.TextPosition = f.comboBox_stylePosition.SelectedItem.ToString();

            UpdateVisualStyle(cpi);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void button_changeFormattingTextRemoved_Click(object sender, EventArgs e)
        {
            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;
            var f = new DifferenceFormatting
            {
                checkBox_fontColor = { Checked = cpi.ComparerOptions.StyleRemovedText.FontSpecifyColor },
                checkBox_backroundColor =
                {
                    Checked = cpi.ComparerOptions.StyleRemovedText.FontSpecifyBackroundColor
                }
            };


            if (f.checkBox_fontColor.Checked)
                f.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(cpi.ComparerOptions.StyleRemovedText.FontColor);
            if (f.checkBox_backroundColor.Checked)
                f.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(cpi.ComparerOptions.StyleRemovedText.FontBackroundColor);


            f.comboBox_styleBold.SelectedItem = cpi.ComparerOptions.StyleRemovedText.StyleBold;
            f.comboBox_styleItalic.SelectedItem = cpi.ComparerOptions.StyleRemovedText.StyleItalic;
            f.comboBox_styleStrikethrough.SelectedItem = cpi.ComparerOptions.StyleRemovedText.StyleStrikethrough;
            f.comboBox_styleUnderline.SelectedItem = cpi.ComparerOptions.StyleRemovedText.StyleUnderline;
            f.comboBox_stylePosition.SelectedItem = cpi.ComparerOptions.StyleRemovedText.TextPosition;
            f.ShowDialog();

            if (!f.SaveSettings) return;
            cpi.ComparerOptions.StyleRemovedText.Name = DifferencesFormatting.StyleName.RemovedText;
            cpi.ComparerOptions.StyleRemovedText.FontSpecifyColor = f.checkBox_fontColor.Checked;
            cpi.ComparerOptions.StyleRemovedText.FontSpecifyBackroundColor = f.checkBox_backroundColor.Checked;

            if (cpi.ComparerOptions.StyleRemovedText.FontSpecifyColor)
                cpi.ComparerOptions.StyleRemovedText.FontColor = ColorTranslator.ToHtml(f.label_fontColorDisplay.BackColor);
            if (cpi.ComparerOptions.StyleRemovedText.FontSpecifyBackroundColor)
                cpi.ComparerOptions.StyleRemovedText.FontBackroundColor = ColorTranslator.ToHtml(f.label_backroundColorDisplay.BackColor);

            cpi.ComparerOptions.StyleRemovedText.StyleBold = f.comboBox_styleBold.SelectedItem.ToString();
            cpi.ComparerOptions.StyleRemovedText.StyleItalic = f.comboBox_styleItalic.SelectedItem.ToString();
            cpi.ComparerOptions.StyleRemovedText.StyleStrikethrough = f.comboBox_styleStrikethrough.SelectedItem.ToString();
            cpi.ComparerOptions.StyleRemovedText.StyleUnderline = f.comboBox_styleUnderline.SelectedItem.ToString();
            cpi.ComparerOptions.StyleRemovedText.TextPosition = f.comboBox_stylePosition.SelectedItem.ToString();

            UpdateVisualStyle(cpi);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void button_changeFormattingTagNew_Click(object sender, EventArgs e)
        {
            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;
            var f = new DifferenceFormatting
            {
                checkBox_fontColor = { Checked = cpi.ComparerOptions.StyleNewTag.FontSpecifyColor },
                checkBox_backroundColor = { Checked = cpi.ComparerOptions.StyleNewTag.FontSpecifyBackroundColor }
            };


            if (f.checkBox_fontColor.Checked)
                f.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(cpi.ComparerOptions.StyleNewTag.FontColor);
            if (f.checkBox_backroundColor.Checked)
                f.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(cpi.ComparerOptions.StyleNewTag.FontBackroundColor);


            f.comboBox_styleBold.SelectedItem = cpi.ComparerOptions.StyleNewTag.StyleBold;
            f.comboBox_styleItalic.SelectedItem = cpi.ComparerOptions.StyleNewTag.StyleItalic;
            f.comboBox_styleStrikethrough.SelectedItem = cpi.ComparerOptions.StyleNewTag.StyleStrikethrough;
            f.comboBox_styleUnderline.SelectedItem = cpi.ComparerOptions.StyleNewTag.StyleUnderline;
            f.comboBox_stylePosition.SelectedItem = cpi.ComparerOptions.StyleNewTag.TextPosition;

            f.ShowDialog();

            if (!f.SaveSettings) return;
            cpi.ComparerOptions.StyleNewTag.Name = DifferencesFormatting.StyleName.NewTag;
            cpi.ComparerOptions.StyleNewTag.FontSpecifyColor = f.checkBox_fontColor.Checked;
            cpi.ComparerOptions.StyleNewTag.FontSpecifyBackroundColor = f.checkBox_backroundColor.Checked;

            if (cpi.ComparerOptions.StyleNewTag.FontSpecifyColor)
                cpi.ComparerOptions.StyleNewTag.FontColor = ColorTranslator.ToHtml(f.label_fontColorDisplay.BackColor);
            if (cpi.ComparerOptions.StyleNewTag.FontSpecifyBackroundColor)
                cpi.ComparerOptions.StyleNewTag.FontBackroundColor = ColorTranslator.ToHtml(f.label_backroundColorDisplay.BackColor);

            cpi.ComparerOptions.StyleNewTag.StyleBold = f.comboBox_styleBold.SelectedItem.ToString();
            cpi.ComparerOptions.StyleNewTag.StyleItalic = f.comboBox_styleItalic.SelectedItem.ToString();
            cpi.ComparerOptions.StyleNewTag.StyleStrikethrough = f.comboBox_styleStrikethrough.SelectedItem.ToString();
            cpi.ComparerOptions.StyleNewTag.StyleUnderline = f.comboBox_styleUnderline.SelectedItem.ToString();
            cpi.ComparerOptions.StyleNewTag.TextPosition = f.comboBox_stylePosition.SelectedItem.ToString();
            UpdateVisualStyle(cpi);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void button_changeFormattingTagRemoved_Click(object sender, EventArgs e)
        {
            if (treeView_clients.SelectedNode != null)
            {
                var tn = treeView_clients.SelectedNode;
                var cpi = (CompanyProfile)tn.Tag;
                var f = new DifferenceFormatting
                {
                    checkBox_fontColor = { Checked = cpi.ComparerOptions.StyleRemovedTag.FontSpecifyColor },
                    checkBox_backroundColor =
                    {
                        Checked = cpi.ComparerOptions.StyleRemovedTag.FontSpecifyBackroundColor
                    }
                };


                if (f.checkBox_fontColor.Checked)
                    f.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(cpi.ComparerOptions.StyleRemovedTag.FontColor);
                if (f.checkBox_backroundColor.Checked)
                    f.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(cpi.ComparerOptions.StyleRemovedTag.FontBackroundColor);


                f.comboBox_styleBold.SelectedItem = cpi.ComparerOptions.StyleRemovedTag.StyleBold;
                f.comboBox_styleItalic.SelectedItem = cpi.ComparerOptions.StyleRemovedTag.StyleItalic;
                f.comboBox_styleStrikethrough.SelectedItem = cpi.ComparerOptions.StyleRemovedTag.StyleStrikethrough;
                f.comboBox_styleUnderline.SelectedItem = cpi.ComparerOptions.StyleRemovedTag.StyleUnderline;
                f.comboBox_stylePosition.SelectedItem = cpi.ComparerOptions.StyleRemovedTag.TextPosition;
                f.ShowDialog();

                if (!f.SaveSettings) return;
                cpi.ComparerOptions.StyleRemovedTag.Name = DifferencesFormatting.StyleName.RemovedTag;
                cpi.ComparerOptions.StyleRemovedTag.FontSpecifyColor = f.checkBox_fontColor.Checked;
                cpi.ComparerOptions.StyleRemovedTag.FontSpecifyBackroundColor = f.checkBox_backroundColor.Checked;

                if (cpi.ComparerOptions.StyleRemovedTag.FontSpecifyColor)
                    cpi.ComparerOptions.StyleRemovedTag.FontColor = ColorTranslator.ToHtml(f.label_fontColorDisplay.BackColor);
                if (cpi.ComparerOptions.StyleRemovedTag.FontSpecifyBackroundColor)
                    cpi.ComparerOptions.StyleRemovedTag.FontBackroundColor = ColorTranslator.ToHtml(f.label_backroundColorDisplay.BackColor);

                cpi.ComparerOptions.StyleRemovedTag.StyleBold = f.comboBox_styleBold.SelectedItem.ToString();
                cpi.ComparerOptions.StyleRemovedTag.StyleItalic = f.comboBox_styleItalic.SelectedItem.ToString();
                cpi.ComparerOptions.StyleRemovedTag.StyleStrikethrough = f.comboBox_styleStrikethrough.SelectedItem.ToString();
                cpi.ComparerOptions.StyleRemovedTag.StyleUnderline = f.comboBox_styleUnderline.SelectedItem.ToString();
                cpi.ComparerOptions.StyleRemovedTag.TextPosition = f.comboBox_stylePosition.SelectedItem.ToString();
                UpdateVisualStyle(cpi);

                if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                    ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                        , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
            }
        }

        private void linkLabel_reset_differences_formatting_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            var comparerSettings = new ComparerSettings
            {
                StyleNewTag = { CompanyProfileId = cpi.Id },
                StyleNewText = { CompanyProfileId = cpi.Id },
                StyleRemovedTag = { CompanyProfileId = cpi.Id },
                StyleRemovedText = { CompanyProfileId = cpi.Id }
            };


            comparerSettings.StyleNewTag.Id = cpi.ComparerOptions.StyleNewTag.Id;
            comparerSettings.StyleNewText.Id = cpi.ComparerOptions.StyleNewText.Id;
            comparerSettings.StyleRemovedTag.Id = cpi.ComparerOptions.StyleRemovedTag.Id;
            comparerSettings.StyleRemovedText.Id = cpi.ComparerOptions.StyleRemovedText.Id;

            cpi.ComparerOptions.StyleNewTag = comparerSettings.StyleNewTag;
            cpi.ComparerOptions.StyleNewText = comparerSettings.StyleNewText;
            cpi.ComparerOptions.StyleRemovedTag = comparerSettings.StyleRemovedTag;
            cpi.ComparerOptions.StyleRemovedText = comparerSettings.StyleRemovedText;

            UpdateVisualStyle(cpi);

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void checkBox_group_changes_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.ComparerOptions.ConsolidateChanges = checkBox_group_changes.Checked;
            tn.Tag = cpi;

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void comboBox_comparisonType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.ComparerOptions.ComparisonType = comboBox_comparisonType.SelectedIndex;
            tn.Tag = cpi;

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void checkBox_auto_add_hourly_rate_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.ProfileRate.HourlyRateAutoAdd = checkBox_auto_add_hourly_rate.Checked;
            tn.Tag = cpi;

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void checkBox_auto_add_langauge_rate_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLoading)
                return;

            if (treeView_clients.SelectedNode == null) return;
            var tn = treeView_clients.SelectedNode;
            var cpi = (CompanyProfile)tn.Tag;

            cpi.ProfileRate.LanguageRateAutoAdd = checkBox_auto_add_language_rate.Checked;
            tn.Tag = cpi;

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.CompanyProfile) && (a.Id == cpi.Id)))
                ChangedItems.Add(new ChangedItem(cpi.Id, ChangedItem.ItemType.CompanyProfile
                    , cpi.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }


        #endregion


        #region  |  my info  |

        private void button_userAddress_Click(object sender, EventArgs e)
        {
            var f = new AddressDetails
            {
                textBox_addressStreet = { Text = settings.UserProfile.Street.Trim() },
                textBox_addressCity = { Text = settings.UserProfile.City.Trim() },
                textBox_addressState = { Text = settings.UserProfile.State.Trim() },
                textBox_addressZip = { Text = settings.UserProfile.Zip.Trim() },
                textBox_addressCountry = { Text = settings.UserProfile.Country.Trim() }
            };


            f.ShowDialog();
            if (!f.Saved) return;
            settings.UserProfile.Street = f.textBox_addressStreet.Text.Trim();
            settings.UserProfile.City = f.textBox_addressCity.Text.Trim();
            settings.UserProfile.State = f.textBox_addressState.Text.Trim();
            settings.UserProfile.Zip = f.textBox_addressZip.Text.Trim();
            settings.UserProfile.Country = f.textBox_addressCountry.Text.Trim();


            textBox_userAddress.Text = settings.UserProfile.Street + "\r\n"
                                       + settings.UserProfile.Zip + @" "
                                       + settings.UserProfile.City
                                       + (settings.UserProfile.State != string.Empty ? " (" + settings.UserProfile.State + ")" : string.Empty) + "\r\n"
                                       + settings.UserProfile.Country;

            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }


        private void initialize_user()
        {
            #region  |  userProfile  |

            textBox_userCompanyName.Text = settings.UserProfile.Name.Trim();
            textBox_userName.Text = settings.UserProfile.UserName.Trim();
            textBox_userTaxCode.Text = settings.UserProfile.TaxCode.Trim();
            textBox_userVatCode.Text = settings.UserProfile.VatCode.Trim();
            textBox_userInternetEmail.Text = settings.UserProfile.Email.Trim();
            textBox_userInternetWebPageAddress.Text = settings.UserProfile.Web.Trim();
            textBox_userPhoneNumber.Text = settings.UserProfile.Phone.Trim();
            textBox_userFaxNumber.Text = settings.UserProfile.Fax.Trim();


            textBox_userAddress.Text = settings.UserProfile.Street + "\r\n"
                     + settings.UserProfile.Zip + " "
                     + settings.UserProfile.City
                     + (settings.UserProfile.State != string.Empty ? @" (" + settings.UserProfile.State + @")" : string.Empty) + "\r\n"
                     + settings.UserProfile.Country;

            #endregion
        }


        private void textBox_userCompanyName_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_userAddress_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_userTaxCode_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_userVatCode_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_userInternetEmail_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_userInternetWebPageAddress_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_userPhoneNumber_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_userFaxNumber_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_userName_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.UserProfile))
                ChangedItems.Add(new ChangedItem(settings.UserProfile.Id, ChangedItem.ItemType.UserProfile
                    , settings.UserProfile.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }




        #endregion


        #region  |  quality metrics  |


        private void initialize_qualityMetrics()
        {

            try
            {
                treeView_metric_group.BeginUpdate();

                treeView_metric_group.Nodes.Clear();

                if (settings.QualityMetricGroupSettings.QualityMetricGroups.Count > 0)
                {


                    var foundDefault = false;
                    foreach (var qmg in settings.QualityMetricGroupSettings.QualityMetricGroups)
                    {
                        var tn = treeView_metric_group.Nodes.Add(qmg.Name);
                        tn.Tag = qmg;
                        if (qmg.IsDefault && !foundDefault)
                        {
                            foundDefault = true;
                            tn.ImageKey = @"Default";
                            tn.SelectedImageKey = @"Default";
                        }
                        else
                        {
                            qmg.IsDefault = false;
                            tn.ImageKey = @"Normal";
                            tn.SelectedImageKey = @"Normal";
                        }
                    }
                    CheckSetDefault();

                    treeView_metric_group.SelectedNode = treeView_metric_group.Nodes[0];
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                treeView_metric_group.EndUpdate();
            }
            CheckSelectionMetricControls();

        }

        private void CheckSetDefault()
        {
            if (treeView_metric_group.Nodes.Count <= 0) return;
            var foundDefault = settings.QualityMetricGroupSettings.QualityMetricGroups.Any(qmg => qmg.IsDefault);
            if (foundDefault) return;
            {
                var tn = treeView_metric_group.Nodes[0];
                var qmg = (QualityMetricGroup)tn.Tag;
                qmg.IsDefault = true;
                tn.ImageKey = @"Default";
                tn.SelectedImageKey = @"Default";

                if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.QualityMetricsGroup))
                    ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.QualityMetricsGroup, ChangedItem.ItemAction.Updated));
            }
        }


        private void CheckSelectionMetricControls()
        {
            try
            {

                if (treeView_metric_group.SelectedNode != null)
                {
                    toolStripSplitButton_new_metric_group.Enabled = true;
                    toolStripButton_metric_group_import.Enabled = true;
                    toolStripButton_metric_group_export.Enabled = true;
                    exportMetricGroupToolStripMenuItem.Enabled = true;

                    toolStripButton_metric_group_edit.Enabled = true;
                    toolStripButton_metric_group_delete.Enabled = true;

                    editMetricGroupToolStripMenuItem.Enabled = true;
                    removeMetricGroupToolStripMenuItem.Enabled = true;
                    setAsDefaultToolStripMenuItem.Enabled = true;

                    linkLabel_set_as_default_metric_group.Enabled = true;

                    if (listView_quality_metrics.SelectedItems.Count > 0)
                    {


                        toolStripButton_qualityMetrics_add.Enabled = true;
                        toolStripButton_qualityMetrics_edit.Enabled = true;
                        toolStripButton_qualityMetrics_remove.Enabled = true;

                        addQualityMetricToolStripMenuItem.Enabled = true;
                        editQualityMetricToolStripMenuItem.Enabled = true;
                        removeQualityMetricToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        toolStripButton_qualityMetrics_add.Enabled = true;
                        toolStripButton_qualityMetrics_edit.Enabled = false;
                        toolStripButton_qualityMetrics_remove.Enabled = false;

                        addQualityMetricToolStripMenuItem.Enabled = true;
                        editQualityMetricToolStripMenuItem.Enabled = false;
                        removeQualityMetricToolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    toolStripSplitButton_new_metric_group.Enabled = true;
                    toolStripButton_metric_group_import.Enabled = true;
                    toolStripButton_metric_group_export.Enabled = false;
                    exportMetricGroupToolStripMenuItem.Enabled = false;

                    toolStripButton_metric_group_edit.Enabled = false;
                    toolStripButton_metric_group_delete.Enabled = false;

                    editMetricGroupToolStripMenuItem.Enabled = false;
                    removeMetricGroupToolStripMenuItem.Enabled = false;
                    setAsDefaultToolStripMenuItem.Enabled = false;


                    toolStripButton_qualityMetrics_add.Enabled = false;
                    toolStripButton_qualityMetrics_edit.Enabled = false;
                    toolStripButton_qualityMetrics_remove.Enabled = false;



                    addQualityMetricToolStripMenuItem.Enabled = false;
                    editQualityMetricToolStripMenuItem.Enabled = false;
                    removeQualityMetricToolStripMenuItem.Enabled = false;

                    linkLabel_set_as_default_metric_group.Enabled = false;
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                label_quality_metrics_count.Text = string.Format(PluginResources.Items___0__, listView_quality_metrics.Items.Count);
            }
        }

        private void treeView_metric_group_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView_metric_group.SelectedNode != null)
            {
                var qmg = (QualityMetricGroup)treeView_metric_group.SelectedNode.Tag;
                listView_quality_metrics.Items.Clear();

                if (qmg.Metrics.Count > 0)
                {
                    foreach (var qc in qmg.Metrics)
                    {
                        var lvi = listView_quality_metrics.Items.Add(qc.Name);
                        lvi.SubItems.Add(qc.MetricSeverity.Name + " {" + qc.MetricSeverity.Value + "}");
                        lvi.SubItems.Add(qc.Description);
                        lvi.Tag = qc;
                    }
                }
            }
            CheckSelectionMetricControls();
        }


        private void AddNewMetricGroup(QualityMetricGroup qmg)
        {
            var f = new QualityMetrics.QualityMetricGroup
            {
                MetricGroup = qmg,
                IsEdit = false
            };
            f.ShowDialog();
            if (f.Saved)
            {
                var foundExisting = treeView_metric_group.Nodes.Cast<TreeNode>().Any(tn => string.Compare(tn.Text, f.MetricGroup.Name.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
                if (foundExisting)
                {
                    MessageBox.Show(PluginResources.Item_name_already_exists_in_the_list_);
                }
                else
                {
                    var tn = treeView_metric_group.Nodes.Add(f.MetricGroup.Name);
                    tn.Tag = f.MetricGroup;
                    tn.ImageKey = @"Normal";
                    tn.SelectedImageKey = @"Normal";
                    treeView_metric_group.SelectedNode = tn;

                    settings.QualityMetricGroupSettings.QualityMetricGroups.Add(qmg);
                    comboBox_quality_metric_groups.Items.Add(new ComboboxItem(qmg.Name, qmg));

                    CheckSetDefault();

                    if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.QualityMetricsGroup))
                        ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.QualityMetricsGroup, ChangedItem.ItemAction.Updated));
                }
            }
            CheckSelectionMetricControls();
        }

        private void EdiMetricGroup()
        {
            if (treeView_metric_group.SelectedNode != null)
            {
                var qmg = (QualityMetricGroup)treeView_metric_group.SelectedNode.Tag;

                var f = new QualityMetrics.QualityMetricGroup
                {
                    MetricGroup = (QualityMetricGroup)qmg.Clone(),
                    IsEdit = true
                };


                f.ShowDialog();
                if (f.Saved)
                {
                    qmg = f.MetricGroup;
                    treeView_metric_group.SelectedNode.Text = qmg.Name;
                    treeView_metric_group.SelectedNode.Tag = qmg;

                    listView_quality_metrics.Items.Clear();

                    if (qmg.Metrics.Count > 0)
                    {
                        foreach (var qc in qmg.Metrics)
                        {
                            var lvi = listView_quality_metrics.Items.Add(qc.Name);
                            lvi.SubItems.Add(qc.MetricSeverity.Name + " {" + qc.MetricSeverity.Value + "}");
                            lvi.SubItems.Add(qc.Description);
                            lvi.Tag = qc;
                        }
                    }


                    if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.QualityMetricsGroup))
                        ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.QualityMetricsGroup, ChangedItem.ItemAction.Updated));
                }
            }
            CheckSelectionMetricControls();
        }

        private void RemoveMetricGroup()
        {
            if (treeView_metric_group.SelectedNode != null)
            {
                var dr = MessageBox.Show(this, PluginResources.Do_you_want_to_delete_the_selected_Quality_Metric_Group_ + "\r\n\r\n"
                  + PluginResources.Note__you_will_not_be_able_to_recover_this_information + "\r\n\r\n"
                  + PluginResources.Click__Yes__to_delete_the_selected_item + "\r\n"
                  + PluginResources.Click__No__to_cancel, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    var selectedIndex = 0;
                    selectedIndex = treeView_metric_group.SelectedNode.Index;
                    var qmgSelected = treeView_metric_group.SelectedNode.Tag as QualityMetricGroup;

                    var messageToUser = string.Empty;
                    foreach (var cpi in settings.CompanyProfiles)
                    {
                        if (qmgSelected == null || cpi.MetricGroup.Id <= -1 || cpi.MetricGroup.Id != qmgSelected.Id)
                            continue;
                        messageToUser = string.Format(PluginResources.Unable_to_remove_the_selected_Quality_Metric_Group, cpi.Name);
                        break;
                    }
                    if (messageToUser.Trim() != string.Empty)
                    {
                        MessageBox.Show(messageToUser);
                    }
                    else
                    {
                        try
                        {
                            foreach (var qmg in settings.QualityMetricGroupSettings.QualityMetricGroups)
                            {
                                if (qmgSelected == null || qmg.Id != qmgSelected.Id) continue;
                                settings.QualityMetricGroupSettings.QualityMetricGroups.Remove(qmg);
                                break;
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                        try
                        {
                            foreach (ComboboxItem cbi in comboBox_quality_metric_groups.Items)
                            {
                                var qmg = (QualityMetricGroup)cbi.Value;
                                if (qmgSelected != null && qmg.Id == qmgSelected.Id)
                                {
                                    comboBox_quality_metric_groups.Items.Remove(cbi);
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }

                        listView_quality_metrics.Items.Clear();
                        treeView_metric_group.SelectedNode.Remove();


                        try
                        {
                            if (treeView_metric_group.Nodes.Count > 0)
                            {
                                if (selectedIndex > 0)
                                    selectedIndex--;


                                treeView_metric_group.SelectedNode = treeView_metric_group.Nodes[selectedIndex];
                            }
                        }
                        catch
                        {
                            // ignored
                        }


                        CheckSetDefault();


                        if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.QualityMetricsGroup))
                            ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.QualityMetricsGroup, ChangedItem.ItemAction.Updated));
                    }
                }
            }
            CheckSelectionMetricControls();
        }


        private void SetAsDefaultMetricGroup()
        {
            if (treeView_metric_group.SelectedNode != null)
            {

                var qmg = (QualityMetricGroup)treeView_metric_group.SelectedNode.Tag;


                foreach (TreeNode tn in treeView_metric_group.Nodes)
                {
                    if (tn.Index == treeView_metric_group.SelectedNode.Index) continue;
                    var _qmg = (QualityMetricGroup)tn.Tag;
                    _qmg.IsDefault = false;
                    tn.ImageKey = @"Normal";
                    tn.SelectedImageKey = @"Normal";
                }

                qmg.IsDefault = true;
                treeView_metric_group.SelectedNode.ImageKey = @"Default";
                treeView_metric_group.SelectedNode.SelectedImageKey = @"Default";

                if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.QualityMetricsGroup))
                    ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.QualityMetricsGroup, ChangedItem.ItemAction.Updated));
            }
            CheckSelectionMetricControls();
        }


        private void toolStripButton_metric_group_import_Click(object sender, EventArgs e)
        {
            var f = new OpenFileDialog
            {
                Filter = @"XML file (*.xml)|*.xml",
                Title = PluginResources.Open_the_Qualitity_Metrics_Group_file,
                RestoreDirectory = true
            };
            f.ShowDialog();
            if (f.FileName.Trim() == string.Empty) return;
            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(QualityMetricGroup));
                stream = new FileStream(f.FileName, FileMode.Open);
                var qmg = (QualityMetricGroup)serializer.Deserialize(stream);

                if (!settings.QualityMetricGroupSettings.QualityMetricGroups.Exists(a => a.Name == qmg.Name))
                {
                    //reset the ids
                    qmg.Id = -1;
                    foreach (var qm in qmg.Metrics)
                        qm.Id = -1;

                    AddNewMetricGroup(qmg);
                }
                else
                {
                    MessageBox.Show(PluginResources.Import_Failed_ + "\r\n\r\n"
                        + PluginResources.The_Quality_Metric_Group_name_already_exists_in_the_list_ + "\r\n\r\n", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(PluginResources.Error_while_reading_the_xml_file_ + "\r\n\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        private void toolStripButton_metric_group_export_Click(object sender, EventArgs e)
        {
            if (treeView_metric_group.SelectedNode != null)
            {
                var qmg = (QualityMetricGroup)treeView_metric_group.SelectedNode.Tag;

                var f = new SaveFileDialog
                {
                    Filter = @"XML file (*.xml)|*.xml",
                    Title = PluginResources.Save_the_Qualitity_Metrics_Group_to_file,
                    AddExtension = true,
                    DefaultExt = ".xml",
                    FileName =
                        qmg.Name + "_" + Helper.GetStringFromDateTime(DateTime.Now).Replace(":", ".").Replace("T", " ") +
                        ".xml"
                };


                f.ShowDialog();
                if (f.FileName == string.Empty) return;
                FileStream stream = null;
                try
                {
                    var serializer = new XmlSerializer(typeof(QualityMetricGroup));
                    stream = new FileStream(f.FileName, FileMode.Create, FileAccess.Write);
                    serializer.Serialize(stream, qmg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(PluginResources.Error_while_writing_the_xml_file_ + "\r\n\r\n" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
        }


        private void exportMetricGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_metric_group_export_Click(null, null);
        }

        private void treeView_metric_group_DoubleClick(object sender, EventArgs e)
        {
            EdiMetricGroup();

        }

        private void toolStripSplitButton_new_metric_group_ButtonClick(object sender, EventArgs e)
        {
            var group = new QualityMetricGroup();
            AddNewMetricGroup(group);
        }

        private void sAEJ2450ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var group = SettingsInitialization.get_QualityMetrics("SAE J2450");
            AddNewMetricGroup(group);
        }

        private void tAUSDQFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var group = SettingsInitialization.get_QualityMetrics("TAUS DQF");
            AddNewMetricGroup(group);
        }

        private void lISAQAMetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var group = SettingsInitialization.get_QualityMetrics("LISA QA Metric");
            AddNewMetricGroup(group);
        }


        private void mQMCoreStandardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var group = SettingsInitialization.get_QualityMetrics("MQM Core");
            AddNewMetricGroup(group);
        }

        private void toolStripButton_metric_group_edit_Click(object sender, EventArgs e)
        {


            EdiMetricGroup();

        }

        private void toolStripButton_metric_group_delete_Click(object sender, EventArgs e)
        {
            RemoveMetricGroup();
        }


        private void toolStripButton_qualityMetrics_add_Click(object sender, EventArgs e)
        {
            if (treeView_metric_group.SelectedNode != null)
            {
                var qmg = (QualityMetricGroup)treeView_metric_group.SelectedNode.Tag;

                var f = new QualityMetricItem
                {
                    MetricGroup = qmg,
                    Metric = new QualityMetric(),
                    IsEdit = false
                };


                f.ShowDialog();
                if (f.Saved)
                {
                    var lvi = listView_quality_metrics.Items.Add(f.Metric.Name);
                    lvi.SubItems.Add(f.Metric.MetricSeverity.Name + " {" + f.Metric.MetricSeverity.Value + "}");
                    lvi.SubItems.Add(f.Metric.Description);

                    lvi.Tag = f.Metric;
                    qmg.Metrics.Add(f.Metric);
                    treeView_metric_group.SelectedNode.Tag = qmg;

                    if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.QualityMetricsGroup))
                        ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.QualityMetricsGroup, ChangedItem.ItemAction.Updated));
                }

            }
            CheckSelectionMetricControls();
        }

        private void toolStripButton_qualityMetrics_edit_Click(object sender, EventArgs e)
        {
            if (listView_quality_metrics.SelectedItems.Count > 0)
            {
                var qmg = (QualityMetricGroup)treeView_metric_group.SelectedNode.Tag;

                var itemSelected = listView_quality_metrics.SelectedItems[0];
                var metric = itemSelected.Tag as QualityMetric;


                var f = new QualityMetricItem { MetricGroup = qmg };

                //update here
                if (metric != null)
                {
                    f.Metric = (QualityMetric)metric.Clone();
                    f.IsEdit = true;
                    f.ShowDialog();
                    if (f.Saved)
                    {
                        var found = listView_quality_metrics.Items.Cast<ListViewItem>().Where(item => item.Index != itemSelected.Index).Any(item => string.Compare(item.Text, f.textBox_name.Text.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
                        if (found)
                        {
                            MessageBox.Show(PluginResources.Item_name_already_exists_in_the_list_);
                        }
                        else
                        {
                            metric.Name = f.Metric.Name;
                            metric.Description = f.Metric.Description;
                            metric.MetricSeverity = f.Metric.MetricSeverity;
                            metric.Modifed = DateTime.Now;

                            itemSelected.Text = metric.Name;
                            itemSelected.SubItems[1].Text = metric.MetricSeverity.Name + @" {" + metric.MetricSeverity.Value + @"}";
                            itemSelected.SubItems[2].Text = metric.Description;

                            itemSelected.Tag = metric;


                            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.QualityMetricsGroup))
                                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.QualityMetricsGroup, ChangedItem.ItemAction.Updated));
                        }
                    }
                }
            }
            CheckSelectionMetricControls();
        }

        private void toolStripButton_qualityMetrics_remove_Click(object sender, EventArgs e)
        {
            if (listView_quality_metrics.SelectedItems.Count > 0)
            {
                var qmg = (QualityMetricGroup)treeView_metric_group.SelectedNode.Tag;

                foreach (ListViewItem item in listView_quality_metrics.Items)
                {
                    if (!item.Selected) continue;
                    var metric = item.Tag as QualityMetric;

                    if (
                        !qmg.Metrics.Exists(
                            a =>
                                metric != null && a.Id == metric.Id && a.GroupId == metric.GroupId && a.Name == metric.Name))
                        continue;
                    {
                        qmg.Metrics.RemoveAll(a => metric != null && a.Id == metric.Id && a.GroupId == metric.GroupId && a.Name == metric.Name);
                        item.Remove();
                    }
                }
                treeView_metric_group.SelectedNode.Tag = qmg;

                if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.QualityMetricsGroup))
                    ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.QualityMetricsGroup, ChangedItem.ItemAction.Updated));
            }
            CheckSelectionMetricControls();

        }

        private void listView_quality_metrics_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSelectionMetricControls();
        }



        private void listView_quality_metrics_DoubleClick(object sender, EventArgs e)
        {
            toolStripButton_qualityMetrics_edit_Click(null, null);
        }

        private void newMetricGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewMetricGroup(new QualityMetricGroup());
        }

        private void editMetricGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdiMetricGroup();
        }

        private void removeMetricGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveMetricGroup();
        }

        private void setAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetAsDefaultMetricGroup();
        }

        private void linkLabel_set_as_default_metric_group_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetAsDefaultMetricGroup();
        }

        private void addQualityMetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_qualityMetrics_add_Click(null, null);
        }

        private void editQualityMetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_qualityMetrics_edit_Click(null, null);
        }

        private void removeQualityMetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_qualityMetrics_remove_Click(null, null);
        }

        #endregion


        #region  |  backup  |

        private void initialize_backup()
        {
            textBox_backup_folder.Text = settings.GetBackupProperty("backupFolder").Value;
            numericUpDown_backup_every.Value = Convert.ToInt32(settings.GetBackupProperty("backupEvery").Value);
            comboBox_backup_every_type.SelectedIndex = Convert.ToInt32(settings.GetBackupProperty("backupEveryType").Value);
            label_backup_last.Text = settings.GetBackupProperty("backupLastDate").Value.Replace("T", " ");
        }

        private void button_browse_backup_folder_Click(object sender, EventArgs e)
        {
            try
            {
                var sPath = textBox_backup_folder.Text;

                if (!Directory.Exists(sPath))
                {
                    while (sPath.Contains("\\"))
                    {
                        sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                        if (Directory.Exists(sPath))
                        {
                            break;
                        }
                    }
                }

                var fsd = new FolderSelectDialog
                {
                    Title = PluginResources.Select_Backup_Folder,
                    InitialDirectory = sPath
                };
                if (fsd.ShowDialog(IntPtr.Zero))
                {
                    if (fsd.FileName.Trim() != string.Empty)
                    {
                        sPath = fsd.FileName;


                        textBox_backup_folder.Text = sPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }

        private void textBox_backup_folder_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.BackUpSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.BackUpSettings, ChangedItem.ItemAction.Updated));
        }

        private void numericUpDown_backup_every_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.BackUpSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.BackUpSettings, ChangedItem.ItemAction.Updated));
        }


        private void comboBox_backup_every_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.BackUpSettings))
                ChangedItems.Add(new ChangedItem(-1, ChangedItem.ItemType.BackUpSettings, ChangedItem.ItemAction.Updated));
        }


        private void linkLabel_viewFoldersInWindowsExplorer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var sPath = textBox_backup_folder.Text;

                if (!Directory.Exists(sPath))
                {
                    while (sPath.Contains("\\"))
                    {
                        sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                        if (Directory.Exists(sPath))
                        {
                            break;
                        }
                    }
                }
                if (Directory.Exists(sPath))
                {
                    Process.Start(sPath);
                }
                else
                {
                    MessageBox.Show(this, PluginResources.Invalid_directory_, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void linkLabel_create_a_backup_now_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Helper.BackUpMyDatabasesNow(textBox_backup_folder.Text.Trim());

                Tracked.Settings.GetBackupProperty("backupLastDate").Value = Sdl.Community.TM.Database.Helper.DateTimeToSQLite(DateTime.Now);
                settings.GetBackupProperty("backupLastDate").Value = Tracked.Settings.GetBackupProperty("backupLastDate").Value;

                label_backup_last.Text = settings.GetBackupProperty("backupLastDate").Value.Replace("T", " ");

                var query = new Query();
                query.SaveBackupSettings(Tracked.Settings.ApplicationPaths.ApplicationMyDocumentsDatabaseSettingsPath, Tracked.Settings.BackupSettings.BackupProperties);

                MessageBox.Show(PluginResources.Backup_was_successful_);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        #endregion


        #region  |  language rates  |

        private void initialize_rate_groups()
        {
            #region  |  rate groups  |
            treeView_price_groups.Nodes.Clear();

            foreach (var priceGroup in settings.LanguageRateGroups)
            {
                var tn = treeView_price_groups.Nodes.Add(priceGroup.Name);
                tn.Tag = priceGroup;
            }

            if (settings.LanguageRateGroups.Count > 0)
                treeView_price_groups.SelectedNode = treeView_price_groups.Nodes[0];
            #endregion
        }

        private void AddPriceGroup()
        {
            var f = new LanguageRate.LanguageRateGroup
            {
                PriceGroup = new LanguageRateGroup
                {
                    Name = PluginResources.New_Language_Rate_Group,
                    Currency = "EUR"
                }
            };
            f.PriceGroup.GroupLanguages.Add(new LanguageRateGroupLanguage(-1, "*", -1, LanguageRateGroupLanguage.LanguageType.Source));

            var ci = new CultureInfo("en-US");
            f.PriceGroup.GroupLanguages.Add(new LanguageRateGroupLanguage(-1, ci.Name, -1, LanguageRateGroupLanguage.LanguageType.Source));


            f.PriceGroup.GroupLanguages.Add(new LanguageRateGroupLanguage(-1, "*", -1, LanguageRateGroupLanguage.LanguageType.Target));
            ci = new CultureInfo("it-IT");
            f.PriceGroup.GroupLanguages.Add(new LanguageRateGroupLanguage(-1, ci.Name, -1, LanguageRateGroupLanguage.LanguageType.Target));
            ci = new CultureInfo("de-DE");
            f.PriceGroup.GroupLanguages.Add(new LanguageRateGroupLanguage(-1, ci.Name, -1, LanguageRateGroupLanguage.LanguageType.Target));
            ci = new CultureInfo("es-ES");
            f.PriceGroup.GroupLanguages.Add(new LanguageRateGroupLanguage(-1, ci.Name, -1, LanguageRateGroupLanguage.LanguageType.Target));
            ci = new CultureInfo("fr-FR");
            f.PriceGroup.GroupLanguages.Add(new LanguageRateGroupLanguage(-1, ci.Name, -1, LanguageRateGroupLanguage.LanguageType.Target));

            f.IsEdit = false;

            f.ShowDialog();
            if (!f.Saved) return;
            if (f.PriceGroup.Name.Trim() == string.Empty) return;

            #region  |  check nameAlreadyUsed  |
            var nameAlreadyUsed = false;
            foreach (var priceGroup in settings.LanguageRateGroups)
            {
                if (string.Compare(priceGroup.Name.Trim(), f.PriceGroup.Name.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nameAlreadyUsed = true;
                }
            }
            if (nameAlreadyUsed)
            {
                var index = 0;
                string newName;

                while (true)
                {
                    index++;
                    newName = f.PriceGroup.Name + "_" + index.ToString().PadLeft(3, '0');
                    nameAlreadyUsed = settings.LanguageRateGroups.Any(priceGroup => string.Compare(priceGroup.Name.Trim(), newName, StringComparison.OrdinalIgnoreCase) == 0);
                    if (!nameAlreadyUsed)
                        break;

                }
                f.PriceGroup.Name = newName;
            }
            #endregion

            settings.LanguageRateGroups.Add(f.PriceGroup);

            var tn = treeView_price_groups.Nodes.Add(f.PriceGroup.Name);
            tn.Tag = f.PriceGroup;

            treeView_price_groups.SelectedNode = tn;

            if (!IsLoading && !ChangedItems.Exists(a => { return (a.Type == ChangedItem.ItemType.languageRateGroup) && (a.Id == f.PriceGroup.Id); }))
                ChangedItems.Add(new ChangedItem(f.PriceGroup.Id, ChangedItem.ItemType.languageRateGroup
                    , f.PriceGroup.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void DeletePriceGroup()
        {

            if (treeView_price_groups.SelectedNode != null)
            {

                var dr = MessageBox.Show(this, PluginResources.Do_you_want_to_delete_the_selected_PRM_Rate_ + "\r\n\r\n"
                    + PluginResources.Note__you_will_not_be_able_to_recover_this_information + "\r\n\r\n"
                    + PluginResources.Click__Yes__to_delete_the_selected_item + "\r\n"
                    + PluginResources.Click__No__to_cancel, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes) return;
                listView_price_groups.Items.Clear();

                var priceGroup = (LanguageRateGroup)treeView_price_groups.SelectedNode.Tag;


                if (!IsLoading && priceGroup.Id > -1)
                {
                    var existingItem = ChangedItems.Find(a => { return (a.Type == ChangedItem.ItemType.languageRateGroup) && (a.Id == priceGroup.Id); });
                    if (existingItem != null)
                        existingItem.Action = ChangedItem.ItemAction.Deleted;
                    else
                        ChangedItems.Add(new ChangedItem(priceGroup.Id, ChangedItem.ItemType.languageRateGroup, ChangedItem.ItemAction.Deleted));
                }

                foreach (var rateGroup in settings.LanguageRateGroups)
                {
                    if (string.Compare(priceGroup.Name, rateGroup.Name, StringComparison.OrdinalIgnoreCase) != 0)
                        continue;
                    settings.LanguageRateGroups.Remove(rateGroup);
                    break;
                }
                treeView_price_groups.SelectedNode.Remove();
            }

        }
        private void EditPriceGroup()
        {
            if (treeView_price_groups.SelectedNode == null) return;
            var tn = treeView_price_groups.SelectedNode;
            var priceGroupCurrent = (LanguageRateGroup)tn.Tag;


            var f = new LanguageRate.LanguageRateGroup
            {
                PriceGroup = priceGroupCurrent,
                IsEdit = true
            };



            f.ShowDialog();
            if (!f.Saved) return;
            if (f.PriceGroup.Name.Trim() == string.Empty) return;
            var i = settings.LanguageRateGroups.Count(priceGroup => string.Compare(priceGroup.Name.Trim(), f.PriceGroup.Name.Trim(), StringComparison.OrdinalIgnoreCase) == 0);

            if (i > 1)
                MessageBox.Show(this, PluginResources.Unable_to_save_group_settings__the_updated_name_is_already_used_, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                tn.Text = f.PriceGroup.Name;
                tn.Tag = f.PriceGroup;

                if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.languageRateGroup) && (a.Id == f.PriceGroup.Id)))
                    ChangedItems.Add(new ChangedItem(f.PriceGroup.Id, ChangedItem.ItemType.languageRateGroup
                        , f.PriceGroup.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));

                treeView_price_groups_AfterSelect(null, null);

            }
        }

        private void AddRate()
        {
            if (treeView_price_groups.SelectedNode == null) return;
            var rateGroup = (LanguageRateGroup)treeView_price_groups.SelectedNode.Tag;
            var f = new LanguageRate.LanguageRate(new Dictionary<string, List<string>>())
            {
                Rate = new Sdl.Community.Structures.Rates.LanguageRate(),
                RateGroup = rateGroup,
                IsEdit = false
            };


            f.ShowDialog();
            if (!f.Saved) return;
            var foundRate = rateGroup.LanguageRates.Any(price => string.Compare(price.SourceLanguage, f.Rate.SourceLanguage, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(price.TargetLanguage, f.Rate.TargetLanguage, true) == 0);
            if (foundRate) return;
            rateGroup.LanguageRates.Add(f.Rate);

            var itmx = listView_price_groups.Items.Add(f.Rate.SourceLanguage);
            itmx.SubItems.Add(f.Rate.TargetLanguage);

            itmx.SubItems.Add(f.Rate.BaseRate.ToString(CultureInfo.InvariantCulture));


            itmx.SubItems.Add(f.Rate.RndType.ToString());
            itmx.SubItems.Add(f.Rate.RatePm.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(f.Rate.RateCm.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(f.Rate.RateRep.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(f.Rate.Rate100.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(f.Rate.Rate95.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(f.Rate.Rate85.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(f.Rate.Rate75.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(f.Rate.Rate50.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(f.Rate.RateNew.ToString(CultureInfo.InvariantCulture));
            itmx.ImageIndex = 0;
            itmx.Tag = f.Rate;



            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.languageRateGroup) && (a.Id == f.RateGroup.Id)))
                ChangedItems.Add(new ChangedItem(f.RateGroup.Id, ChangedItem.ItemType.languageRateGroup
                    , f.RateGroup.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void AddPriceMultiple()
        {
            if (treeView_price_groups.SelectedNode == null) return;
            var priceGroup = (LanguageRateGroup)treeView_price_groups.SelectedNode.Tag;

            var f = new AppendMultipleRates();

            f.comboBox_sourceLanguages.Items.Clear();
            foreach (var language in priceGroup.GroupLanguages)
            {
                CultureInfo ci = null;
                if (language.LanguageIdCi != "*")
                    ci = new CultureInfo(language.LanguageIdCi);

                if (language.Type == LanguageRateGroupLanguage.LanguageType.Source)
                    f.comboBox_sourceLanguages.Items.Add(language.LanguageIdCi + " - " + (ci != null ? ci.EnglishName : "{all}"));
            }


            f.comboBox_sourceLanguages.SelectedIndex = 0;

            f.ShowDialog();
            if (!f.Saved) return;
            LanguageRateGroupLanguage languageSource = null;

            var id = f.comboBox_sourceLanguages.SelectedItem.ToString().Substring(0, f.comboBox_sourceLanguages.SelectedItem.ToString().IndexOf(" ", StringComparison.Ordinal));
            languageSource = id == "*" ? new LanguageRateGroupLanguage(-1, "*", -1, LanguageRateGroupLanguage.LanguageType.Source) : new LanguageRateGroupLanguage(-1, id, -1, LanguageRateGroupLanguage.LanguageType.Source);


            var languageComboIndex = (from ListViewItem itmx in listView_price_groups.Items select (Sdl.Community.Structures.Rates.LanguageRate) itmx.Tag into price select price.SourceLanguage + " - " + price.TargetLanguage).ToList();

            foreach (var languageTarget in priceGroup.GroupLanguages)
            {
                if (languageComboIndex.Contains(languageSource.LanguageIdCi + " - " + languageTarget.LanguageIdCi))
                    continue;
                languageComboIndex.Add(languageSource.LanguageIdCi + " - " + languageTarget.LanguageIdCi);

                var price = new Sdl.Community.Structures.Rates.LanguageRate
                {
                    SourceLanguage = languageSource.LanguageIdCi,
                    TargetLanguage = languageTarget.LanguageIdCi
                };

                var itmx = listView_price_groups.Items.Add(price.SourceLanguage);
                itmx.SubItems.Add(price.TargetLanguage);

                itmx.SubItems.Add(price.BaseRate.ToString(CultureInfo.InvariantCulture));

                itmx.SubItems.Add(price.RndType.ToString());
                itmx.SubItems.Add(price.RatePm.ToString(CultureInfo.InvariantCulture));
                itmx.SubItems.Add(price.RateCm.ToString(CultureInfo.InvariantCulture));
                itmx.SubItems.Add(price.RateRep.ToString(CultureInfo.InvariantCulture));
                itmx.SubItems.Add(price.Rate100.ToString(CultureInfo.InvariantCulture));
                itmx.SubItems.Add(price.Rate95.ToString(CultureInfo.InvariantCulture));
                itmx.SubItems.Add(price.Rate85.ToString(CultureInfo.InvariantCulture));
                itmx.SubItems.Add(price.Rate75.ToString(CultureInfo.InvariantCulture));
                itmx.SubItems.Add(price.Rate50.ToString(CultureInfo.InvariantCulture));
                itmx.SubItems.Add(price.RateNew.ToString(CultureInfo.InvariantCulture));
                itmx.ImageIndex = 0;
                itmx.Tag = price;

                priceGroup.LanguageRates.Add(price);
            }

            if (!IsLoading && !ChangedItems.Exists(a => { return (a.Type == ChangedItem.ItemType.languageRateGroup) && (a.Id == priceGroup.Id); }))
                ChangedItems.Add(new ChangedItem(priceGroup.Id, ChangedItem.ItemType.languageRateGroup
                    , priceGroup.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void EditPrice()
        {
            if (treeView_price_groups.SelectedNode == null) return;
            var priceGroup = (LanguageRateGroup)treeView_price_groups.SelectedNode.Tag;
            var f = new LanguageRate.LanguageRate(new Dictionary<string, List<string>>());

            if (listView_price_groups.SelectedItems.Count <= 0) return;
            var item = listView_price_groups.SelectedItems[0];


            var languageRate = (Sdl.Community.Structures.Rates.LanguageRate)item.Tag;

            f.Rate = (Sdl.Community.Structures.Rates.LanguageRate)languageRate.Clone();
            f.RateGroup = priceGroup;

            f.IsEdit = true;
            f.IsEditMultiple = listView_price_groups.SelectedItems.Count > 1;
            f.ShowDialog();
            if (!f.Saved) return;
            foreach (ListViewItem itmx in listView_price_groups.Items)
            {
                if (!itmx.Selected) continue;
                var price = (Sdl.Community.Structures.Rates.LanguageRate)itmx.Tag;

                price.BaseRate = f.Rate.BaseRate;
                price.RndType = f.Rate.RndType;
                price.RatePm = f.Rate.RatePm;
                price.RateCm = f.Rate.RateCm;
                price.RateRep = f.Rate.RateRep;
                price.Rate100 = f.Rate.Rate100;
                price.Rate95 = f.Rate.Rate95;
                price.Rate85 = f.Rate.Rate85;
                price.Rate75 = f.Rate.Rate75;
                price.Rate50 = f.Rate.Rate50;
                price.RateNew = f.Rate.RateNew;


                itmx.SubItems[2].Text = f.Rate.BaseRate.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[3].Text = f.Rate.RndType.ToString();
                itmx.SubItems[4].Text = f.Rate.RatePm.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[5].Text = f.Rate.RateCm.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[6].Text = f.Rate.RateRep.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[7].Text = f.Rate.Rate100.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[8].Text = f.Rate.Rate95.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[9].Text = f.Rate.Rate85.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[10].Text = f.Rate.Rate75.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[11].Text = f.Rate.Rate50.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[12].Text = f.Rate.RateNew.ToString(CultureInfo.InvariantCulture);

                itmx.Tag = price;
            }
            if (!IsLoading && !ChangedItems.Exists(a => { return (a.Type == ChangedItem.ItemType.languageRateGroup) && (a.Id == f.RateGroup.Id); }))
                ChangedItems.Add(new ChangedItem(f.RateGroup.Id, ChangedItem.ItemType.languageRateGroup
                    , f.RateGroup.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void DeletePrice()
        {
            if (treeView_price_groups.SelectedNode == null) return;
            var priceGroup = (LanguageRateGroup)treeView_price_groups.SelectedNode.Tag;

            foreach (ListViewItem itmx in listView_price_groups.Items)
            {
                if (!itmx.Selected) continue;
                var papd = (Sdl.Community.Structures.Rates.LanguageRate)itmx.Tag;

                for (var i = 0; i <= priceGroup.LanguageRates.Count; i++)
                {
                    if (papd.SourceLanguage != priceGroup.LanguageRates[i].SourceLanguage ||
                        papd.TargetLanguage != priceGroup.LanguageRates[i].TargetLanguage) continue;
                    priceGroup.LanguageRates.RemoveAt(i);
                    break;
                }
                itmx.Remove();
            }
            if (!IsLoading && !ChangedItems.Exists(a => { return (a.Type == ChangedItem.ItemType.languageRateGroup) && (a.Id == priceGroup.Id); }))
                ChangedItems.Add(new ChangedItem(priceGroup.Id, ChangedItem.ItemType.languageRateGroup
                    , priceGroup.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void EditAnalysisBandPercentages()
        {
            if (treeView_price_groups.SelectedNode == null) return;
            var priceGroup = (LanguageRateGroup)treeView_price_groups.SelectedNode.Tag;
            if (listView_price_groups.SelectedItems.Count <= 0) return;
            var _papd = (Sdl.Community.Structures.Rates.LanguageRate)listView_price_groups.SelectedItems[0].Tag;
         
            var f = new AppendAnlaysisBand();

            #region  |  price analysis band percentages  |
            if (_papd.RatePm > 0)
                f.numericUpDown_percentagePerfect.Value = _papd.RatePm / _papd.BaseRate * 100;
            else
                f.numericUpDown_percentagePerfect.Value = 0;

            if (_papd.RateCm > 0)
                f.numericUpDown_percentageContext.Value = _papd.RateCm / _papd.BaseRate * 100;
            else
                f.numericUpDown_percentageContext.Value = 0;

            if (_papd.RateRep > 0)
                f.numericUpDown_percentageRepetitions.Value = _papd.RateRep / _papd.BaseRate * 100;
            else
                f.numericUpDown_percentageRepetitions.Value = 0;

            if (_papd.Rate100 > 0)
                f.numericUpDown_percentageExact.Value = _papd.Rate100 / _papd.BaseRate * 100;
            else
                f.numericUpDown_percentageExact.Value = 0;

            if (_papd.Rate95 > 0)
                f.numericUpDown_percentageFuzzy99.Value = _papd.Rate95 / _papd.BaseRate * 100;
            else
                f.numericUpDown_percentageFuzzy99.Value = 0;

            if (_papd.Rate85 > 0)
                f.numericUpDown_percentageFuzzy94.Value = _papd.Rate85 / _papd.BaseRate * 100;
            else
                f.numericUpDown_percentageFuzzy94.Value = 0;

            if (_papd.Rate75 > 0)
                f.numericUpDown_percentageFuzzy84.Value = _papd.Rate75 / _papd.BaseRate * 100;
            else
                f.numericUpDown_percentageFuzzy84.Value = 0;

            if (_papd.Rate50 > 0)
                f.numericUpDown_percentageFuzzy74.Value = _papd.Rate50 / _papd.BaseRate * 100;
            else
                f.numericUpDown_percentageFuzzy74.Value = 0;

            if (_papd.RateNew > 0)
                f.numericUpDown_percentageNew.Value = _papd.RateNew / _papd.BaseRate * 100;
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
                f.Text += PluginResources.___default_percentages_;
                f.numericUpDown_percentagePerfect.Value = priceGroup.DefaultAnalysisBand.PercentPm;
                f.numericUpDown_percentageContext.Value = priceGroup.DefaultAnalysisBand.PercentCm;
                f.numericUpDown_percentageRepetitions.Value = priceGroup.DefaultAnalysisBand.PercentRep;
                f.numericUpDown_percentageExact.Value = priceGroup.DefaultAnalysisBand.Percent100;
                f.numericUpDown_percentageFuzzy99.Value = priceGroup.DefaultAnalysisBand.Percent95;
                f.numericUpDown_percentageFuzzy94.Value = priceGroup.DefaultAnalysisBand.Percent85;
                f.numericUpDown_percentageFuzzy84.Value = priceGroup.DefaultAnalysisBand.Percent75;
                f.numericUpDown_percentageFuzzy74.Value = priceGroup.DefaultAnalysisBand.Percent50;
                f.numericUpDown_percentageNew.Value = priceGroup.DefaultAnalysisBand.PercentNew;
            }


            f.ShowDialog();

            if (!f.Saved) return;
            foreach (ListViewItem itmx in listView_price_groups.SelectedItems)
            {
                var papd = (Sdl.Community.Structures.Rates.LanguageRate)itmx.Tag;
                papd.RatePm = 0;
                papd.RateCm = 0;
                papd.RateRep = 0;
                papd.Rate100 = 0;
                papd.Rate95 = 0;
                papd.Rate85 = 0;
                papd.Rate75 = 0;
                papd.Rate50 = 0;
                papd.RateNew = 0;


                if (papd.BaseRate > 0)
                {
                    switch (papd.RndType)
                    {
                        case RoundType.Roundup:

                            #region  |  round up  |

                            if (f.numericUpDown_percentagePerfect.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentagePerfect.Value * .01M), 3);

                                papd.RatePm = value <= papd.BaseRate ? value : papd.BaseRate;
                            }


                            if (f.numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentageContext.Value * .01M), 3);

                                papd.RateCm = value <= papd.BaseRate ? value : papd.BaseRate;
                            }


                            if (f.numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentageRepetitions.Value * .01M), 3);

                                papd.RateRep = value <= papd.BaseRate ? value : papd.BaseRate;
                            }


                            if (f.numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentageExact.Value * .01M), 3);

                                papd.Rate100 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }


                            if (f.numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentageFuzzy99.Value * .01M), 3);

                                papd.Rate95 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }


                            if (f.numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentageFuzzy94.Value * .01M), 3);

                                papd.Rate85 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }


                            if (f.numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentageFuzzy84.Value * .01M), 3);

                                papd.Rate75 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }


                            if (f.numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentageFuzzy74.Value * .01M), 3);

                                papd.Rate50 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }


                            if (f.numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Helper.RoundUp(papd.BaseRate, Convert.ToDecimal(f.numericUpDown_percentageNew.Value * .01M), 3);

                                papd.RateNew = value <= papd.BaseRate ? value : papd.BaseRate;
                            }
                            break;

                            #endregion

                        case RoundType.Rounddown:

                            #region  |  round down  |
                            var strDecimalLen = "10".PadRight(3 + 1, '0');
                            var decimalLen = Convert.ToInt32(strDecimalLen);

                            if (f.numericUpDown_percentagePerfect.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentagePerfect.Value * .01M) * decimalLen) / decimalLen;

                                papd.RatePm = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageContext.Value * .01M) * decimalLen) / decimalLen;

                                papd.RateCm = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageRepetitions.Value * .01M) * decimalLen) / decimalLen;

                                papd.RateRep = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageExact.Value * .01M) * decimalLen) / decimalLen;

                                papd.Rate100 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageFuzzy99.Value * .01M) * decimalLen) / decimalLen;

                                papd.Rate95 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageFuzzy94.Value * .01M) * decimalLen) / decimalLen;

                                papd.Rate85 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageFuzzy84.Value * .01M) * decimalLen) / decimalLen;

                                papd.Rate75 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageFuzzy74.Value * .01M) * decimalLen) / decimalLen;

                                papd.Rate50 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Math.Truncate(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageNew.Value * .01M) * decimalLen) / decimalLen;

                                papd.RateNew = value <= papd.BaseRate ? value : papd.BaseRate;
                            }
                            break;

                            #endregion

                        case RoundType.Round:

                            #region  |  round  |

                            if (f.numericUpDown_percentagePerfect.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentagePerfect.Value * .01M), 3);
                                papd.RatePm = value <= papd.BaseRate ? value : papd.BaseRate;
                            }




                            if (f.numericUpDown_percentageContext.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageContext.Value * .01M), 3);
                                papd.RateCm = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageRepetitions.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageRepetitions.Value * .01M), 3);
                                papd.RateRep = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageExact.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageExact.Value * .01M), 3);
                                papd.Rate100 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageFuzzy99.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageFuzzy99.Value * .01M), 3);
                                papd.Rate95 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }




                            if (f.numericUpDown_percentageFuzzy94.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageFuzzy94.Value * .01M), 3);
                                papd.Rate85 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }




                            if (f.numericUpDown_percentageFuzzy84.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageFuzzy84.Value * .01M), 3);
                                papd.Rate75 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageFuzzy74.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageFuzzy74.Value * .01M), 3);
                                papd.Rate50 = value <= papd.BaseRate ? value : papd.BaseRate;
                            }



                            if (f.numericUpDown_percentageNew.Value > 0)
                            {
                                var value = Helper.Round(papd.BaseRate * Convert.ToDecimal(f.numericUpDown_percentageNew.Value * .01M), 3);
                                papd.RateNew = value <= papd.BaseRate ? value : papd.BaseRate;
                            }
                            break;

                            #endregion
                    }
                }


                itmx.SubItems[2].Text = papd.BaseRate.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[3].Text = papd.RndType.ToString();
                itmx.SubItems[4].Text = papd.RatePm.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[5].Text = papd.RateCm.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[6].Text = papd.RateRep.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[7].Text = papd.Rate100.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[8].Text = papd.Rate95.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[9].Text = papd.Rate85.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[10].Text = papd.Rate75.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[11].Text = papd.Rate50.ToString(CultureInfo.InvariantCulture);
                itmx.SubItems[12].Text = papd.RateNew.ToString(CultureInfo.InvariantCulture);

                itmx.Tag = papd;


            }

            if (!IsLoading && !ChangedItems.Exists(a => (a.Type == ChangedItem.ItemType.languageRateGroup) && (a.Id == priceGroup.Id)))
                ChangedItems.Add(new ChangedItem(priceGroup.Id, ChangedItem.ItemType.languageRateGroup
                    , priceGroup.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }




        private void treeView_price_groups_AfterSelect(object sender, TreeViewEventArgs e)
        {

            listView_price_groups.Items.Clear();

            if (treeView_price_groups.SelectedNode != null)
            {

                var priceGroup = (LanguageRateGroup)treeView_price_groups.SelectedNode.Tag;
                foreach (var price in priceGroup.LanguageRates)
                {
                    var itmx = listView_price_groups.Items.Add(price.SourceLanguage);
                    itmx.SubItems.Add(price.TargetLanguage);
                    itmx.SubItems.Add(price.BaseRate.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.RndType.ToString());
                    itmx.SubItems.Add(price.RatePm.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.RateCm.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.RateRep.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.Rate100.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.Rate95.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.Rate85.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.Rate75.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.Rate50.ToString(CultureInfo.InvariantCulture));
                    itmx.SubItems.Add(price.RateNew.ToString(CultureInfo.InvariantCulture));
                    itmx.ImageIndex = 0;
                    itmx.Tag = price;
                }

                if (listView_price_groups.Items.Count > 0)
                    listView_price_groups.Items[0].Selected = true;



                toolStripButton_priceGroup_new.Enabled = true;
                toolStripButton_priceGroup_edit.Enabled = true;
                toolStripButton_priceGroup_remove.Enabled = true;


                newPriceGroupToolStripMenuItem.Enabled = true;
                editPriceGroupToolStripMenuItem.Enabled = true;
                removePriceGroupToolStripMenuItem.Enabled = true;
                setAsDefaultPriceGroupToolStripMenuItem.Enabled = true;

            }
            else
            {


                toolStripButton_priceGroup_new.Enabled = true;
                toolStripButton_priceGroup_edit.Enabled = false;
                toolStripButton_priceGroup_remove.Enabled = false;


                newPriceGroupToolStripMenuItem.Enabled = true;
                editPriceGroupToolStripMenuItem.Enabled = false;
                removePriceGroupToolStripMenuItem.Enabled = false;
                setAsDefaultPriceGroupToolStripMenuItem.Enabled = false;
            }





        }

        private void toolStripButton_priceGroup_remove_Click(object sender, EventArgs e)
        {
            DeletePriceGroup();

        }

        private void toolStripButton_priceGroup_edit_Click(object sender, EventArgs e)
        {
            EditPriceGroup();
        }

        private void treeView_price_groups_DoubleClick(object sender, EventArgs e)
        {
            EditPriceGroup();
        }

        private void toolStripButton_groupPrice_add_Click(object sender, EventArgs e)
        {
            AddRate();
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
            AddRate();
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
            AddPriceGroup();
        }

        private void editPriceGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditPriceGroup();
        }

        private void removePriceGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeletePriceGroup();
        }



        private void listView_price_groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_price_groups.SelectedItems.Count > 0)
            {
                toolStripButton_groupPrice_add.Enabled = true;
                toolStripButton_groupPrice_edit.Enabled = true;
                toolStripButton_groupPrice_remove.Enabled = true;
                toolStripButton_analysisBandPercentage.Enabled = true;

                addPriceToolStripMenuItem.Enabled = true;
                editPriceToolStripMenuItem.Enabled = true;
                removePriceToolStripMenuItem.Enabled = true;
                analysisBandToolStripMenuItem.Enabled = true;
            }
            else
            {
                toolStripButton_groupPrice_add.Enabled = true;
                toolStripButton_groupPrice_edit.Enabled = false;
                toolStripButton_groupPrice_remove.Enabled = false;
                toolStripButton_analysisBandPercentage.Enabled = false;

                addPriceToolStripMenuItem.Enabled = true;
                editPriceToolStripMenuItem.Enabled = false;
                removePriceToolStripMenuItem.Enabled = false;
                analysisBandToolStripMenuItem.Enabled = false;
            }
        }



        private void treeView_price_groups_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeletePriceGroup();
            }
        }



        private void toolStripButton_priceGroup_new_Click(object sender, EventArgs e)
        {
            AddPriceGroup();
        }
























        private void contextMenuStrip_price_groups_listview_Opening(object sender, CancelEventArgs e)
        {

        }

        private void contextMenuStrip_price_groups_treeview_Opening(object sender, CancelEventArgs e)
        {

        }



        private void setAsDefaultPriceGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }




        private void listView_price_groups_KeyUp(object sender, KeyEventArgs e)
        {

        }


        #endregion


        #region  |  DQF settings  |

        private void initialize_DQFSettings()
        {

            textBox_profile_user_dqf_key.Text = settings.DqfSettings.UserKey.Trim();
            textBox_profile_user_translator_dqf_key.Text = settings.DqfSettings.TranslatorKey.Trim();
        }

        private void RequestTranslatorApiKey()
        {
            if (textBox_userInternetEmail.Text.Trim() != string.Empty && textBox_userName.Text.Trim() != string.Empty)
            {
                try
                {
                    var processor = new Processor();
                    var success = processor.SetupTranslatorInfo(new Uri(Configuration.DqfServerRoot + Configuration.DqfApiVersion
                        + @"translator?name=" + textBox_userName.Text.Trim().Replace(" ", "+")
                        + @"&email=" + textBox_userInternetEmail.Text.Trim().Replace(" ", "+"))
                        , textBox_profile_user_dqf_key.Text.Trim());

                    MessageBox.Show(this,
                        success
                            ? "The translator was added correctly, please check your\r\ne-mail to recover the 'Translator API Key'."
                            : "An unidentified error occurred!", Application.ProductName, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "To generate the 'Transaltor API Key', you need to first specify the following:\r\n\r\n * User Name\r\n * User E-Mail address\r\n\r\nNote: you can specify these from the 'My Details' settings area", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void linkLabel_create_new_translator_api_key_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RequestTranslatorApiKey();
        }



        private void textBox_profile_user_dqf_key_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.DqfSettings))
                ChangedItems.Add(new ChangedItem(settings.DqfSettings.Id, ChangedItem.ItemType.DqfSettings
                    , settings.DqfSettings.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void textBox_profile_user_translator_dqf_key_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.DqfSettings))
                ChangedItems.Add(new ChangedItem(settings.DqfSettings.Id, ChangedItem.ItemType.DqfSettings
                    , settings.DqfSettings.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }
        private void checkBox_dqf_enable_reports_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoading && !ChangedItems.Exists(a => a.Type == ChangedItem.ItemType.DqfSettings))
                ChangedItems.Add(new ChangedItem(settings.DqfSettings.Id, ChangedItem.ItemType.DqfSettings
                    , settings.DqfSettings.Id == -1 ? ChangedItem.ItemAction.Added : ChangedItem.ItemAction.Updated));
        }

        private void linkLabel_taus_dqf_net_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://dqf.taus.net/");

        }

        private void linkLabel_dqf_taus_evaluate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://evaluate.taus.net/");
        }

        private void richTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
        #endregion


        private void button_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(PluginResources.no_help_file_found_);
        }




    }
}
