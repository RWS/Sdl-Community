using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Custom;
using Sdl.Community.Studio.Time.Tracker.Structures;
using Sdl.Community.Studio.Time.Tracker.Tracking;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    public partial class Settings : Form
    {

        public bool Saved { get; set; }
        public Structures.Settings settings { get; set; }


        public Settings()
        {
            InitializeComponent();

            treeView_clients.MouseDown += (sender, args) =>
                treeView_clients.SelectedNode = treeView_clients.GetNodeAt(args.X, args.Y);

            panel_general.Dock = DockStyle.Fill;
            panel_client_rates.Dock = DockStyle.Fill;
            panel_activities.Dock = DockStyle.Fill;
            panel_my_info.Dock = DockStyle.Fill;
            panel_backup.Dock = DockStyle.Fill;

            treeView_main.SelectedNode = treeView_main.Nodes[0];
        }

        private void treeView_main_AfterSelect(object sender, TreeViewEventArgs e)
        {
            pictureBox_header.Image = imageList_settings_navigation.Images[e.Node.ImageIndex];
            textBox_header.Text = e.Node.Text;

            switch (e.Node.Name)
            {
                case "Node_general": panel_general.BringToFront(); break;
                case "Node_client_rates": panel_client_rates.BringToFront(); break;    
                case "Node_activities": panel_activities.BringToFront(); break;
                case "Node_my_info": panel_my_info.BringToFront(); break;
                case "Node_backup": panel_backup.BringToFront(); break;
             
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            initialize_general();
            initialize_activities();            
            initialize_company();
            initialize_user();
            initialize_backup();
            
        }



        private void button_Save_Click(object sender, EventArgs e)
        {

            if (textBox_backup_folder.Text.Trim() != string.Empty && Directory.Exists(textBox_backup_folder.Text.Trim()))
            {
                
                #region  |  general  |

                settings.DefaultFilterProjectStatus = comboBox_default_project_status.SelectedItem.ToString().Trim();
                settings.DefaultFilterGroupBy = comboBox_default_project_group_by.SelectedItem.ToString().Trim();


                var comboboxItem = (ComboboxItem)comboBox_default_currency.Items[comboBox_default_currency.SelectedIndex];
                var itemValue = (Currency)comboboxItem.Value;
                settings.DefaultCurrency = itemValue.Name;

                settings.TrackerConfirmActivities = checkBox_trackerConfirmActivities.Checked;

                #endregion

                #region  |  activities  |



                #endregion

                #region  |  user  |

                if (settings.User.Id.Trim() == string.Empty)
                    settings.User.Id = Guid.NewGuid().ToString();

                settings.User.CompanyName = textBox_userCompanyName.Text.Trim();
                settings.User.TaxCode = textBox_userTaxCode.Text.Trim();
                settings.User.VatCode = textBox_userVatCode.Text.Trim();
                settings.User.Email = textBox_userInternetEmail.Text.Trim();
                settings.User.WebPage = textBox_userInternetWebPageAddress.Text.Trim();
                settings.User.PhoneNumber = textBox_userPhoneNumber.Text.Trim();
                settings.User.FaxNumber = textBox_userFaxNumber.Text.Trim();

                #endregion

                #region  |  backup  |

                settings.BackupFolder = textBox_backup_folder.Text.Trim();
                settings.BackupEvery = Convert.ToInt32(numericUpDown_backup_every.Value);
                settings.BackupEveryType = comboBox_backup_every_type.SelectedIndex;
               
                #endregion


                Saved = true;
                Close();
            }
            else
            {
                MessageBox.Show(this, @"Unable to save settings; unable to locate the backup folder specified!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

      

        #region  |  general  settings  |


        private void initialize_general()
        {


            #region  |  default project status  |

          
            if (settings.DefaultFilterProjectStatus.Trim() != string.Empty)
            {
                if (settings.DefaultFilterProjectStatus.Trim().IndexOf("Show all projects", StringComparison.Ordinal) > -1)
                    comboBox_default_project_status.SelectedIndex = 0;
                else if (settings.DefaultFilterProjectStatus.Trim().IndexOf("In progress", StringComparison.Ordinal) > -1)
                    comboBox_default_project_status.SelectedIndex = 1;
                else if (settings.DefaultFilterProjectStatus.Trim().IndexOf("Completed", StringComparison.Ordinal) > -1)
                    comboBox_default_project_status.SelectedIndex = 2;
                else
                    comboBox_default_project_status.SelectedIndex = 0;
            }
            else
            {
                comboBox_default_project_status.SelectedIndex = 0;
            }

            #endregion

            #region  | default project grouped by  |

         
            if (settings.DefaultFilterGroupBy.Trim() != string.Empty)
            {
                if (settings.DefaultFilterGroupBy.Trim().IndexOf("Client name", StringComparison.Ordinal) > -1)
                    comboBox_default_project_group_by.SelectedIndex = 0;
                else if (settings.DefaultFilterGroupBy.Trim().IndexOf("Project name", StringComparison.Ordinal) > -1)
                    comboBox_default_project_group_by.SelectedIndex = 1;
                else if (settings.DefaultFilterGroupBy.Trim().IndexOf("Date (year/month)", StringComparison.Ordinal) > -1)
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

            comboBox_default_currency.BeginUpdate();
            foreach (var currency in Tracked.Currencies)
            {
                comboBox_default_currency.Items.Add(new ComboboxItem(currency.Name + "  (" + currency.Country + ")", currency));
            }
            comboBox_default_currency.EndUpdate();


            var iSelectedIndex = -1;
            const int iDefaultIndex = 0;
            for (var i = 0; i < comboBox_default_currency.Items.Count; i++)
            {
                var comboboxItem = (ComboboxItem)comboBox_default_currency.Items[i];

                var itemValue = (Currency)comboboxItem.Value;
                if (string.Compare(itemValue.Name, settings.DefaultCurrency, StringComparison.OrdinalIgnoreCase) != 0) continue;
                iSelectedIndex = i;
                break;
            }
            comboBox_default_currency.SelectedIndex = iSelectedIndex > -1 ? iSelectedIndex : iDefaultIndex;
            #endregion

            checkBox_trackerConfirmActivities.Checked = settings.TrackerConfirmActivities;
        }

        private void comboBox_default_currency_SelectedIndexChanged(object sender, EventArgs e)
        {

            var comboboxItem = (ComboboxItem)comboBox_default_currency.Items[comboBox_default_currency.SelectedIndex];
            var itemValue = (Currency)comboboxItem.Value;
            settings.DefaultCurrency = itemValue.Name;

        }

        #endregion


        #region  |  activities  |


        private void initialize_activities()
        {
            listView_activities.BeginUpdate();
            foreach (var activity in settings.ActivitiesTypes)
            {
                AddActivityToList(activity);
            }
            listView_activities.EndUpdate();
            if (listView_activities.Items.Count > 0)
                listView_activities.Items[0].Selected = true;

            label_activities_count.Text = @"Activities: " + listView_activities.Items.Count;
        }


        private void addActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_addActivity_Click(sender, e);
        }

        private void editActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_eidtActivity_Click(sender, e);
        }

        private void removeActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_deleteActivity_Click(sender, e);
        }

        private void button_userAddress_Click(object sender, EventArgs e)
        {
            var addressDetails = new AddressDetails
            {
                textBox_addressStreet = {Text = settings.User.AddressStreet.Trim()},
                textBox_addressCity = {Text = settings.User.AddressCity.Trim()},
                textBox_addressState = {Text = settings.User.AddressState.Trim()},
                textBox_addressZip = {Text = settings.User.AddressZip.Trim()},
                textBox_addressCountry = {Text = settings.User.AddressCountry.Trim()}
            };


            addressDetails.ShowDialog();
            if (!addressDetails.Saved) return;
            settings.User.AddressStreet = addressDetails.textBox_addressStreet.Text.Trim();
            settings.User.AddressCity = addressDetails.textBox_addressCity.Text.Trim();
            settings.User.AddressState = addressDetails.textBox_addressState.Text.Trim();
            settings.User.AddressZip = addressDetails.textBox_addressZip.Text.Trim();
            settings.User.AddressCountry = addressDetails.textBox_addressCountry.Text.Trim();


            textBox_userAddress.Text = settings.User.AddressStreet + "\r\n"
                                       + settings.User.AddressZip + " "
                                       + settings.User.AddressCity
                                       + (settings.User.AddressState != string.Empty ? " (" + settings.User.AddressState + ")" : string.Empty) + "\r\n"
                                       + settings.User.AddressCountry;
        }

        private void toolStripButton_addActivity_Click(object sender, EventArgs e)
        {

            var activityType = new ActivityType {IsEdit = false};
            var activityNew = new Structures.ActivityType {Id = Guid.NewGuid().ToString()};
            activityType.Activity = activityNew;
            activityType.DefaultCurrency = settings.DefaultCurrency;

            activityType.ShowDialog();

            if (activityType.Saved)
            {
                var foundName = (from ListViewItem itmx in listView_activities.Items 
                                 select (Structures.ActivityType) itmx.Tag).Any(activity => 
                                     string.Compare(activity.Name, activityType.Activity.Name, StringComparison.OrdinalIgnoreCase) == 0);
                if (foundName)
                {
                    MessageBox.Show(this, @"Unable to save the Activity; name already exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    settings.ActivitiesTypes.Add(activityType.Activity);

                    AddActivityToList(activityType.Activity);



                    foreach (var profileInfo in settings.Clients)
                    {
                        var clientActivityType = new ClientActivityType
                        {
                            Id = Guid.NewGuid().ToString(),
                            IdActivity = activityType.Activity.Id,
                            Activated = activityType.checkBox_activateForAllClients.Checked,
                            HourlyRateClient = activityType.Activity.HourlyRate,
                            HourlyRateAdjustment = 0
                        };



                        profileInfo.ClientActivities.Add(clientActivityType);
                    }

                    treeView_clients_AfterSelect(null, null);



                }

            }
            label_activities_count.Text = @"Activities: " + listView_activities.Items.Count;
        }

        private void AddActivityToList(Structures.ActivityType activity)
        {
            var itmx = listView_activities.Items.Add(activity.Name,0);            
            itmx.SubItems.Add(activity.Description);
            itmx.SubItems.Add(activity.Billable.ToString());
            itmx.SubItems.Add(activity.HourlyRate.ToString(CultureInfo.InvariantCulture));
            itmx.SubItems.Add(activity.Currency);
            itmx.Tag = activity;

        }
       


        private void toolStripButton_eidtActivity_Click(object sender, EventArgs e)
        {
            if (listView_activities.SelectedIndices.Count > 0)
            {
                var selectedItem = listView_activities.SelectedItems[0];



                var activityType = new ActivityType {IsEdit = true};

                var activityEdit = (Structures.ActivityType)selectedItem.Tag;
                activityType.Activity = (Structures.ActivityType)activityEdit.Clone();

                activityType.ShowDialog();

                if (activityType.Saved)
                {
                    var foundName = listView_activities.Items.Cast<ListViewItem>().Select(itmx => 
                        (Structures.ActivityType) itmx.Tag).Where(activity => 
                            string.Compare(activity.Name, activityType.Activity.Name, StringComparison.OrdinalIgnoreCase) == 0).Any(activity => activity.Id != activityType.Activity.Id);
                    if (foundName)
                    {
                        MessageBox.Show(this, @"Unable to save the Activity; name already exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        activityEdit.Name = activityType.Activity.Name;
                        activityEdit.Description = activityType.Activity.Description;
                        activityEdit.Billable = activityType.Activity.Billable;
                        activityEdit.Currency = activityType.Activity.Currency;
                        activityEdit.HourlyRate = activityType.Activity.HourlyRate;

                        selectedItem.SubItems[0].Text = activityEdit.Name;
                        selectedItem.SubItems[1].Text = activityEdit.Description;
                        selectedItem.SubItems[2].Text = activityEdit.Billable.ToString();
                        selectedItem.SubItems[3].Text = activityEdit.HourlyRate.ToString(CultureInfo.InvariantCulture);
                        selectedItem.SubItems[4].Text = activityEdit.Currency;
                        selectedItem.Tag = activityEdit;



                        foreach (var clientProfileInfo in settings.Clients)
                        {

                            foreach (var clientActivityType in clientProfileInfo.ClientActivities)
                            {
                                if (clientActivityType.IdActivity != activityEdit.Id) continue;
                                clientActivityType.HourlyRateAdjustment = clientActivityType.HourlyRateClient - activityEdit.HourlyRate;
                                   
                                break;
                            }
                        }

                        treeView_clients_AfterSelect(null, null);
                    }

                }

                
            }

            label_activities_count.Text = @"Activities: " + listView_activities.Items.Count;
        }

        private void listView_activities_DoubleClick(object sender, EventArgs e)
        {
            toolStripButton_eidtActivity_Click(sender, e);
        }

        private void toolStripButton_deleteActivity_Click(object sender, EventArgs e)
        {
            var continueDelete = true;
            var message = string.Empty;
            if (listView_activities.SelectedIndices.Count > 0)
            {
                foreach (ListViewItem itmx in listView_activities.SelectedItems)
                {
                    var activityType = (Structures.ActivityType)itmx.Tag;

                    foreach (var trackerProject in settings.TrackerProjects)
                    {
                        foreach (var trackerProjectActivity in trackerProject.ProjectActivities)
                        {
                            if (trackerProjectActivity.ActivityTypeId != activityType.Id) continue;
                            continueDelete = false;
                            message = "Unable to delete the activity '" + activityType.Name + "'\r\n";
                            message += "Note: it is made reference to from the project activities '" + trackerProjectActivity.Name + "'";
                        }
                    }
                }
            }

            if (continueDelete)
            {
                if (listView_activities.SelectedIndices.Count > 0)
                {
                    foreach (ListViewItem itmx in listView_activities.SelectedItems)
                    {
                        var activity = (Structures.ActivityType)itmx.Tag;

                        foreach (var clientProfileInfo in settings.Clients)
                        {
                            foreach (var clientActivityType in clientProfileInfo.ClientActivities)
                            {
                                if (clientActivityType.IdActivity != activity.Id) continue;
                                clientProfileInfo.ClientActivities.Remove(clientActivityType);
                                break;
                            }
                        }

                        settings.ActivitiesTypes.Remove(activity);

                        itmx.Remove();
                    }
                    treeView_clients_AfterSelect(null, null);
                }
            }
            else
            {
                MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            label_activities_count.Text = @"Activities: " + listView_activities.Items.Count;
        }

        private void listView_activities_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                toolStripButton_deleteActivity_Click(settings, e);
        }


        private void listView_activities_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView_activities.SelectedItems.Count == 1)
            {
                toolStripButton_addActivity.Enabled = true;
                toolStripButton_eidtActivity.Enabled = true;
                toolStripButton_deleteActivity.Enabled = true;

                addActivityToolStripMenuItem.Enabled = true;
                editActivityToolStripMenuItem.Enabled = true;
                removeActivityToolStripMenuItem.Enabled = true;

            }
            else if (listView_activities.SelectedItems.Count > 1)
            {
                toolStripButton_addActivity.Enabled = true;
                toolStripButton_eidtActivity.Enabled = false;
                toolStripButton_deleteActivity.Enabled = true;

                addActivityToolStripMenuItem.Enabled = true;
                editActivityToolStripMenuItem.Enabled = false;
                removeActivityToolStripMenuItem.Enabled = true;
            }
            else
            {
                toolStripButton_addActivity.Enabled = true;
                toolStripButton_eidtActivity.Enabled = false;
                toolStripButton_deleteActivity.Enabled = false;

                addActivityToolStripMenuItem.Enabled = true;
                editActivityToolStripMenuItem.Enabled = false;
                removeActivityToolStripMenuItem.Enabled = false;
            }
        }

        #endregion


        #region  |  company profile info  |

        private void initialize_company()
        {
            treeView_clients.Nodes.Clear();

            foreach (var clientProfileInfo in settings.Clients)
            {
                AddCompanyToList(clientProfileInfo);              
            }

            if (treeView_clients.Nodes.Count > 0)
                treeView_clients.SelectedNode = treeView_clients.Nodes[0];

            label_clientCount.Text = @"Clients: " + treeView_clients.Nodes.Count;
        }


        private void addClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_addClient_Click(sender, e);
        }

        private void editClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_editClient_Click(sender, e);
        }

        private void removeClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_deleteClient_Click(sender, e);
        }


        private void toolStripButton_addClient_Click(object sender, EventArgs e)
        {
            var clientProfile = new ClientProfile {IsEdit = false};
            var cpiNew = new ClientProfileInfo {Id = Guid.NewGuid().ToString()};



            clientProfile.ClientProfileInfo = cpiNew;

            clientProfile.ShowDialog();

            if (clientProfile.Saved)
            {
                var foundName = (from TreeNode treeNode in treeView_clients.Nodes 
                                 select (ClientProfileInfo) treeNode.Tag).Any(clientProfileInfo => 
                                     string.Compare(clientProfileInfo.ClientName, clientProfile.ClientProfileInfo.ClientName, StringComparison.OrdinalIgnoreCase) == 0 
                                     && clientProfileInfo.Id != clientProfile.ClientProfileInfo.Id);
                if (foundName)
                {
                    MessageBox.Show(this, @"Unable to save the company profile; name already exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {


                    foreach (var activityType in settings.ActivitiesTypes)
                    {


                        var clientActivityType = new ClientActivityType
                        {
                            Id = Guid.NewGuid().ToString(),
                            IdActivity = activityType.Id,
                            Activated = true,
                            HourlyRateClient = activityType.HourlyRate,
                            HourlyRateAdjustment = 0
                        };



                        clientProfile.ClientProfileInfo.ClientActivities.Add(clientActivityType);
                    }


                    settings.Clients.Add(clientProfile.ClientProfileInfo);


                    AddCompanyToList(clientProfile.ClientProfileInfo);
                }

            }
            label_clientCount.Text = @"Clients: " + treeView_clients.Nodes.Count;
        }

        private void AddCompanyToList(ClientProfileInfo clientProfileInfo)
        {
            var treeNode = treeView_clients.Nodes.Add(clientProfileInfo.ClientName);
            treeNode.Tag = clientProfileInfo;
           
        }



        private void toolStripButton_editClient_Click(object sender, EventArgs e)
        {
            if (treeView_clients.SelectedNode == null) return;
            var tnEdit = treeView_clients.SelectedNode;
            var cpiEdit = (ClientProfileInfo)tnEdit.Tag;

            var clientProfile = new ClientProfile
            {
                IsEdit = true,
                ClientProfileInfo = (ClientProfileInfo) cpiEdit.Clone()
            };

            clientProfile.ShowDialog();

            if (!clientProfile.Saved) return;
            var foundName = (from TreeNode tn in treeView_clients.Nodes 
                             select (ClientProfileInfo) tn.Tag).Any(clientProfileInfo => 
                                 string.Compare(clientProfileInfo.ClientName, clientProfile.ClientProfileInfo.ClientName, StringComparison.OrdinalIgnoreCase) == 0 
                                 && clientProfileInfo.Id != clientProfile.ClientProfileInfo.Id);
            if (foundName)
            {
                MessageBox.Show(this, @"Unable to save the company profile; name already exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                cpiEdit.ClientName = clientProfile.ClientProfileInfo.ClientName;

                cpiEdit.TaxCode = clientProfile.ClientProfileInfo.TaxCode;
                cpiEdit.VatCode = clientProfile.ClientProfileInfo.VatCode;

                cpiEdit.Email = clientProfile.ClientProfileInfo.Email;
                cpiEdit.WebPage = clientProfile.ClientProfileInfo.WebPage;
                cpiEdit.PhoneNumber = clientProfile.ClientProfileInfo.PhoneNumber;
                cpiEdit.FaxNumber = clientProfile.ClientProfileInfo.FaxNumber;


                cpiEdit.AddressStreet = clientProfile.ClientProfileInfo.AddressStreet;
                cpiEdit.AddressCity = clientProfile.ClientProfileInfo.AddressCity;
                cpiEdit.AddressState = clientProfile.ClientProfileInfo.AddressState;
                cpiEdit.AddressZip = clientProfile.ClientProfileInfo.AddressZip;
                cpiEdit.AddressCountry = clientProfile.ClientProfileInfo.AddressCountry;


                tnEdit.Text = cpiEdit.ClientName;


                tnEdit.Tag = cpiEdit;                        
            }
        }

        private void toolStripButton_deleteClient_Click(object sender, EventArgs e)
        {
            if (treeView_clients.SelectedNode != null)
            {

                var selectedNode = treeView_clients.SelectedNode;
                var cpiEdit = (ClientProfileInfo)selectedNode.Tag;

                var continueDelete = true;
                var message = string.Empty;
                foreach (var cpi in settings.Clients)
                {
                    foreach (var tp in settings.TrackerProjects)
                    {
                        if (tp.ClientId != cpiEdit.Id) continue;
                        continueDelete = false;
                        message = "Unable to delete the client '" + cpi.ClientName + "'\r\n";
                        message += "Note: it is made reference to from the project '" + tp.Name + "'";
                        break;
                    }
                    if (!continueDelete)
                        break;
                }
                if (continueDelete)
                {
                    for (var i = 0; i < settings.Clients.Count; i++)
                    {
                        var cpi = settings.Clients[i];

                        if (cpi.Id != cpiEdit.Id) continue;
                        settings.Clients.RemoveAt(i);
                        break;
                    }

                    selectedNode.Remove();

                    treeView_clients_AfterSelect(null, null);
                }
                else
                {

                    MessageBox.Show(this, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            label_clientCount.Text = @"Clients: " + treeView_clients.Nodes.Count;
        }

        private void toolStripButton_editActivityRate_Click(object sender, EventArgs e)
        {
            if (treeView_clients.SelectedNode != null)
            {
                if (listView_clientActivityRates.SelectedItems.Count <= 0 ||
                    listView_clientActivityRates.SelectedItems.Count != 1) 
                    return;

                var itmx = listView_clientActivityRates.SelectedItems[0];
                itmx.UseItemStyleForSubItems = true;
                var clientActivityType = (ClientActivityType)itmx.Tag;
                var activityType = settings.ActivitiesTypes.FirstOrDefault(type => clientActivityType.IdActivity == type.Id);
                
                if (activityType == null) 
                    return;

                var clientActivityRate = new ClientActivityRate
                {
                    Activity = activityType,
                    CompanyActivity = clientActivityType
                };

                clientActivityRate.ShowDialog();

                if (!clientActivityRate.Saved) 
                    return;

                clientActivityType.HourlyRateClient = clientActivityRate.CompanyActivity.HourlyRateClient;
                clientActivityType.HourlyRateAdjustment = clientActivityRate.CompanyActivity.HourlyRateAdjustment;

                itmx.SubItems[3].Text = clientActivityType.HourlyRateClient.ToString(CultureInfo.InvariantCulture);


                itmx.UseItemStyleForSubItems = false;
                if (clientActivityType.HourlyRateAdjustment > 0)
                {
                    itmx.SubItems[4].Text = "+" + clientActivityType.HourlyRateAdjustment;
                    itmx.SubItems[4].ForeColor = Color.DarkBlue;
                    itmx.SubItems[4].BackColor = Color.LightGoldenrodYellow;
                }
                else if (clientActivityType.HourlyRateAdjustment < 0)
                {
                    itmx.SubItems[4].Text = "" + clientActivityType.HourlyRateAdjustment;
                    itmx.SubItems[4].ForeColor = Color.DarkRed;
                    itmx.SubItems[4].BackColor = Color.LightGoldenrodYellow;
                }
                else
                {
                    itmx.SubItems[4].Text = "" + clientActivityType.HourlyRateAdjustment;
                    itmx.SubItems[4].ForeColor = Color.Black;
                    itmx.SubItems[4].BackColor = listView_clientActivityRates.BackColor;
                }
            }
        }

        private void listView_clientActivityRates_DoubleClick(object sender, EventArgs e)
        {
            toolStripButton_editActivityRate_Click(sender, e);
        }
        private void listView_clientActivityRates_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var itmx = listView_clientActivityRates.Items[e.Index];

            var clientActivityType = (ClientActivityType)itmx.Tag;

            clientActivityType.Activated = e.NewValue == CheckState.Checked;

            itmx.ImageIndex = clientActivityType.Activated ? 1 : 2;

        }

        private void treeView_clients_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView_clientActivityRates.Items.Clear();
            if (treeView_clients.SelectedNode != null)
            {

                var selectedNode = treeView_clients.SelectedNode;
                var clientProfileInfo = (ClientProfileInfo)selectedNode.Tag;

                try
                {
                    listView_clientActivityRates.BeginUpdate();
                    foreach (var ca in clientProfileInfo.ClientActivities)
                    {
                        var activityType = settings.ActivitiesTypes.FirstOrDefault(type => ca.IdActivity == type.Id);
                        
                        if (activityType == null) 
                            continue;
                        var itmx = listView_clientActivityRates.Items.Add(activityType.Name);
                        itmx.SubItems.Add(activityType.Description);
                        itmx.SubItems.Add(activityType.Billable.ToString());
                        itmx.SubItems.Add(ca.HourlyRateClient.ToString(CultureInfo.InvariantCulture));

                        itmx.UseItemStyleForSubItems = false;
                        if (ca.HourlyRateAdjustment > 0)
                        {
                            itmx.SubItems.Add("+" + ca.HourlyRateAdjustment);
                            itmx.SubItems[4].ForeColor = Color.DarkBlue;
                            itmx.SubItems[4].BackColor = Color.LightGoldenrodYellow;
                        }
                        else if (ca.HourlyRateAdjustment < 0)
                        {
                            itmx.SubItems.Add(ca.HourlyRateAdjustment.ToString(CultureInfo.InvariantCulture));
                            itmx.SubItems[4].ForeColor = Color.DarkRed;
                            itmx.SubItems[4].BackColor = Color.LightGoldenrodYellow;
                        }
                        else
                        {
                            itmx.SubItems.Add(ca.HourlyRateAdjustment.ToString(CultureInfo.InvariantCulture));
                            itmx.SubItems[4].ForeColor = Color.Black;
                            itmx.SubItems[4].BackColor = listView_clientActivityRates.BackColor;
                        }

                        itmx.SubItems.Add(activityType.Currency);


                        itmx.Tag = ca;
                        itmx.Checked = ca.Activated;

                        itmx.ImageIndex = itmx.Checked ? 1 : 2;
                    }
                }
                finally
                {
                    listView_clientActivityRates.EndUpdate();
                }
                if (listView_clientActivityRates.Items.Count > 0)
                    listView_clientActivityRates.Items[0].Selected = true;
            }
            listView_clientActivityRates_ItemSelectionChanged(null, null);
        }

        private void treeView_clients_DoubleClick(object sender, EventArgs e)
        {
            toolStripButton_editClient_Click(sender, e);
        }

        private void listView_clientActivityRates_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView_clientActivityRates.SelectedItems.Count == 1)
            {
                toolStripButton_editActivityRate.Enabled = true;
                editActivityRateToolStripMenuItem.Enabled = true;
            }
            else
            {
                toolStripButton_editActivityRate.Enabled = false;
                editActivityRateToolStripMenuItem.Enabled = false;
            }
        }


        private void linkLabel_client_rates_activities_checkall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (ListViewItem itmx in listView_clientActivityRates.Items)
            {
                var clientActivityType = (ClientActivityType)itmx.Tag;
                clientActivityType.Activated = true;
                itmx.Checked = true;

                itmx.ImageIndex = itmx.Checked ? 1 : 2;
            }
        }

        private void linkLabel_client_rates_activities_uncheckall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (ListViewItem itmx in listView_clientActivityRates.Items)
            {
                var clientActivityType = (ClientActivityType)itmx.Tag;
                clientActivityType.Activated = false;
                itmx.Checked = false;

                itmx.ImageIndex = itmx.Checked ? 1 : 2;
            }
        }

        private void linkLabel_client_resetDefaultRates_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (ListViewItem itmx in listView_clientActivityRates.Items)
            {
                var clientActivityType = (ClientActivityType)itmx.Tag;
                var activityType = settings.ActivitiesTypes.FirstOrDefault(type => clientActivityType.IdActivity == type.Id);
                if (activityType == null) continue;
                clientActivityType.HourlyRateClient = activityType.HourlyRate;
                clientActivityType.HourlyRateAdjustment = 0;

                itmx.SubItems[3].Text = clientActivityType.HourlyRateClient.ToString(CultureInfo.InvariantCulture);


                itmx.UseItemStyleForSubItems = false;
                if (clientActivityType.HourlyRateAdjustment > 0)
                {
                    itmx.SubItems[4].Text = "+" + clientActivityType.HourlyRateAdjustment;
                    itmx.SubItems[4].ForeColor = Color.DarkBlue;
                    itmx.SubItems[4].BackColor = Color.LightGoldenrodYellow;
                }
                else if (clientActivityType.HourlyRateAdjustment < 0)
                {
                    itmx.SubItems[4].Text = "" + clientActivityType.HourlyRateAdjustment;
                    itmx.SubItems[4].ForeColor = Color.DarkRed;
                    itmx.SubItems[4].BackColor = Color.LightGoldenrodYellow;
                }
                else
                {
                    itmx.SubItems[4].Text = "" + clientActivityType.HourlyRateAdjustment;
                    itmx.SubItems[4].ForeColor = Color.Black;
                    itmx.SubItems[4].BackColor = listView_clientActivityRates.BackColor;
                }
            }
        }

        private void editActivityRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_editActivityRate_Click(sender, e);
        }

        #endregion


        #region  |  my info  |

        private void initialize_user()
        {
            #region  |  user  |

            textBox_userCompanyName.Text = settings.User.CompanyName.Trim();
            textBox_userTaxCode.Text = settings.User.TaxCode.Trim();
            textBox_userVatCode.Text = settings.User.VatCode.Trim();
            textBox_userInternetEmail.Text = settings.User.Email.Trim();
            textBox_userInternetWebPageAddress.Text = settings.User.WebPage.Trim();
            textBox_userPhoneNumber.Text = settings.User.PhoneNumber.Trim();
            textBox_userFaxNumber.Text = settings.User.FaxNumber.Trim();


            textBox_userAddress.Text = settings.User.AddressStreet + "\r\n"
                     + settings.User.AddressZip + " "
                     + settings.User.AddressCity
                     + (settings.User.AddressState != string.Empty ? " (" + settings.User.AddressState + ")" : string.Empty) + "\r\n"
                     + settings.User.AddressCountry;

            #endregion
        }

        #endregion


        #region  |  backup  |

        private void initialize_backup()
        {
            textBox_backup_folder.Text = settings.BackupFolder;
            numericUpDown_backup_every.Value = settings.BackupEvery;
            comboBox_backup_every_type.SelectedIndex = settings.BackupEveryType;
            label_backup_last.Text = settings.BackupLastDate.ToString(CultureInfo.InvariantCulture);
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

                var folderSelectDialog = new FolderSelectDialog
                {
                    Title = "Select Backup Folder",
                    InitialDirectory = sPath
                };
                if (!folderSelectDialog.ShowDialog(IntPtr.Zero)) return;
                if (folderSelectDialog.FileName.Trim() == string.Empty) return;
                sPath = folderSelectDialog.FileName;


                textBox_backup_folder.Text = sPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
           
        }

        private void textBox_backup_folder_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown_backup_every_ValueChanged(object sender, EventArgs e)
        {

        }


        private void comboBox_backup_every_type_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                    MessageBox.Show(this, @"Invalid directory!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }


        #endregion

        private void button_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"no help file found!");
        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            panel_client_rates_bottom.Width = treeView_clients.Width;
        }

       

      

        

       

      

   

       

 

    
        

      

       

     

      

        



    }
}
