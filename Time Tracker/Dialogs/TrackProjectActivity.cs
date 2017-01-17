using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Custom;
using Sdl.Community.Studio.Time.Tracker.Structures;
using Sdl.Community.Studio.Time.Tracker.Tracking;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    public partial class TrackProjectActivity : Form
    {

        public bool Saved { get; set; }
        public bool IsEdit { get; set; }
        public bool IsMerge { get; set; }
        private bool IsLoading { get; set; }

        public TrackerProject Project { get; set; }
        public TrackerProjectActivity Activity { get; set; }
        public List<TrackerProject> Projects { get; set; }
        private ClientProfileInfo Client { get; set; }


        private readonly ErrorProvider _activityName;
        private readonly ErrorProvider _dateStart;
        private readonly ErrorProvider _dateEnd;
        private readonly ErrorProvider _activityType;

        private readonly ErrorProvider _hoursQuantity;


        public TrackProjectActivity()
        {
            InitializeComponent();

            _hoursQuantity = new ErrorProvider();
            _hoursQuantity.SetIconAlignment(numericUpDown_quantity, ErrorIconAlignment.MiddleRight);
            _hoursQuantity.SetIconPadding(numericUpDown_quantity, 2);
            _hoursQuantity.BlinkRate = 1000;
            _hoursQuantity.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;

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
            

            _activityType = new ErrorProvider();
            _activityType.SetIconAlignment(comboBox_activity_type, ErrorIconAlignment.MiddleRight);
            _activityType.SetIconPadding(comboBox_activity_type, 2);
            _activityType.BlinkRate = 1000;
            _activityType.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;
        }

        private void SaveButtonActive()
        {
            var enabled = true;


            var start = new DateTime(dateTimePicker_start_date.Value.Year, dateTimePicker_start_date.Value.Month, dateTimePicker_start_date.Value.Day
                , dateTimePicker_start_hours.Value.Hour, dateTimePicker_start_hours.Value.Minute, dateTimePicker_start_hours.Value.Second);

            var end = new DateTime(dateTimePicker_end_date.Value.Year, dateTimePicker_end_date.Value.Month, dateTimePicker_end_date.Value.Day
                , dateTimePicker_end_hours.Value.Hour, dateTimePicker_end_hours.Value.Minute, dateTimePicker_end_hours.Value.Second);



            if (textBox_name.Text.Trim() == string.Empty)
            {
                enabled = false;
                _activityName.SetError(textBox_name, @"The activity name cannot be empty!");
            }
            else
            {
                _activityName.SetError(textBox_name, string.Empty);
            }

            if (end < start)
            {
                enabled = false;
                _dateEnd.SetError(dateTimePicker_end_hours, @"The activity end date cannot precede the start date!");
            }
            else if (end > Project.DateDue)
            {               
                _dateEnd.SetError(dateTimePicker_end_hours, @"The activity end date cannot supersede the project due date!");
            }
            else
            {
                _dateEnd.SetError(dateTimePicker_end_hours, string.Empty);
            }

            _dateStart.SetError(dateTimePicker_start_hours,
                start > Project.DateDue
                    ? @"The activity start date cannot supersede the project due date!"
                    : string.Empty);


            var selectedItem = (ComboboxItem)comboBox_activity_type.SelectedItem;
            if (selectedItem.Value == null)
            {
                enabled = false;
                _activityType.SetError(comboBox_activity_type, @"The activity type cannot be null!");                
            }
            else
            {
                _activityType.SetError(comboBox_activity_type, string.Empty);
            }


            if (end < start)
            {
                label_total_elapsed_hours.Text = @"0.000  total hours";
                enabled = false;
                _hoursQuantity.SetError(numericUpDown_quantity, @"The number of hours exceed the allowable time comparing the activity start and end date/time");
            }
            else
            {
                var @decimal = Convert.ToDecimal(end.Subtract(start).TotalHours);

                label_total_elapsed_hours.Text = Math.Round(@decimal, 3) + @"  total hours";

                if (numericUpDown_quantity.Value > @decimal + 0.001m)
                {

                    enabled = false;
                    _hoursQuantity.SetError(numericUpDown_quantity, "The number of hours exceed the allowable time comparing the activity start and end date/time");
                }
                else
                {                    
                    _hoursQuantity.SetError(numericUpDown_quantity, string.Empty);
                }
            }

            button_save.Enabled = enabled;
        }

        private void CheckActivatedControls()
        {

            richTextBox_activity_type_note.Clear();


             var comboboxItem = (ComboboxItem)comboBox_projects.SelectedItem;
             if (comboboxItem.Value != null)
             {

                 var trackerProject = (TrackerProject)comboboxItem.Value;

                 Activity.TrackerProjectId = trackerProject.Id;
                 Activity.TrackerProjectName = trackerProject.Name;
                 Activity.TrackerProjectStatus = trackerProject.ProjectStatus; 
             }

            var selectedItem = (ComboboxItem)comboBox_activity_type.SelectedItem;


            if (selectedItem.Value != null)
            {
                SaveButtonActive();       
                
                var objs = (object[])selectedItem.Value;

                Structures.ActivityType activityType = null;
                if (objs[0] != null)
                    activityType = (Structures.ActivityType)objs[0];
                ClientActivityType clientActivityType = null;
                if (objs[1] != null)
                    clientActivityType = (ClientActivityType)objs[1];

                richTextBox_activity_type_note.SelectionFont = new Font(richTextBox_activity_type_note.Font.FontFamily.Name, richTextBox_activity_type_note.Font.Size, FontStyle.Bold);
                richTextBox_activity_type_note.ForeColor = Color.DarkGray;
                richTextBox_activity_type_note.SelectedText += "Note: ";

                if (activityType != null)
                {
                    Activity.ActivityTypeId = activityType.Id;
                    Activity.ActivityTypeName = activityType.Name;

                    Activity.Currency = activityType.Currency;

                    if (clientActivityType != null)
                    {
                    
                        Activity.ActivityTypeClientId = clientActivityType.Id;

                        if (!IsLoading)
                            numericUpDown_hourly_rate_client.Value = clientActivityType.HourlyRateClient;

                        richTextBox_activity_type_note.SelectionFont = new Font(richTextBox_activity_type_note.Font.FontFamily.Name, richTextBox_activity_type_note.Font.Size, FontStyle.Regular);
                        richTextBox_activity_type_note.ForeColor = Color.DarkGray;
                        richTextBox_activity_type_note.SelectedText += "default hourly rate (client): " + clientActivityType.HourlyRateClient + "; billable: " + activityType.Billable;
                    }
                    else
                    {
                        if (!IsLoading)
                            numericUpDown_hourly_rate_client.Value = activityType.HourlyRate;

                        richTextBox_activity_type_note.SelectionFont = new Font(richTextBox_activity_type_note.Font.FontFamily.Name, richTextBox_activity_type_note.Font.Size, FontStyle.Regular);
                        richTextBox_activity_type_note.ForeColor = Color.DarkGray;
                        richTextBox_activity_type_note.SelectedText += "default hourly rate: " + activityType.HourlyRate + "; billable: " + activityType.Billable;
                    }
                    checkBox_billable.Checked = activityType.Billable;
                }


                checkBox_billable.Enabled = true;
                numericUpDown_hourly_rate_client.Enabled = true;
                numericUpDown_quantity.Enabled = true;

                
            }
            else
            {
                SaveButtonActive();    

                checkBox_billable.Checked = false;
                numericUpDown_hourly_rate_client.Value = 0;

                checkBox_billable.Enabled = false;
                numericUpDown_hourly_rate_client.Enabled = false;
                numericUpDown_quantity.Enabled = false;

              
            }


        }

        private void CheckAdjustment()
        {
            var comboboxItem = (ComboboxItem)comboBox_activity_type.SelectedItem;
            if (comboboxItem.Value != null)
            {
                var objs = (object[])comboboxItem.Value;

                Structures.ActivityType activityType = null;
                if (objs[0] != null)
                    activityType = (Structures.ActivityType)objs[0];
                ClientActivityType clientActivityType = null;
                if (objs[1] != null)
                    clientActivityType = (ClientActivityType)objs[1];

                
                Activity.HourlyRate = numericUpDown_hourly_rate_client.Value;

                if (activityType == null) 
                    return;

                Activity.Currency = activityType.Currency;

                if (clientActivityType!=null)
                    Activity.HourlyRateAdjustment = Activity.HourlyRate - clientActivityType.HourlyRateClient;
                else
                    Activity.HourlyRateAdjustment = Activity.HourlyRate - activityType.HourlyRate;

                if (Activity.HourlyRateAdjustment > 0)
                {
                    label_adjustment.Text = "+" + Activity.HourlyRateAdjustment;
                    label_adjustment.ForeColor = Color.DarkBlue;
                }
                else if (Activity.HourlyRateAdjustment < 0)
                {
                    label_adjustment.Text =  Activity.HourlyRateAdjustment.ToString(CultureInfo.InvariantCulture);
                    label_adjustment.ForeColor = Color.DarkRed;
                }
                else
                {
                    label_adjustment.Text = string.Empty;
                }

                label_total.Text = Math.Round(numericUpDown_hourly_rate_client.Value * numericUpDown_quantity.Value, 2).ToString(CultureInfo.InvariantCulture);

                label_currency_hourly_rate.Text = activityType.Currency;
                label_currency_total.Text = activityType.Currency;
            }
            else
            {
                label_total.Text = @"0";
                label_adjustment.Text = string.Empty;
                label_currency_hourly_rate.Text = string.Empty;
                label_currency_total.Text = string.Empty;
            }
        }

        private void TrackProjectActivity_Load(object sender, EventArgs e)
        {
            IsLoading = true;

            if (IsMerge)
                Text += @" (Merge)";
            else
                Text += IsEdit ? " (Edit)" : " (New)";

            #region  |  get company profile  |
            foreach (var clientProfileInfo in Tracked.Preferences.Clients)
            {
                if (clientProfileInfo.Id != Project.ClientId) 
                    continue;

                Client = clientProfileInfo;
                break;
            }
            #endregion


            textBox_client.Text = Client != null ? Client.ClientName : "[no client]";

           
            initialize_projects_combobox();

            #region  |  set project  |

         
            var selectedProjectIndex = 0;
            for (var i = 0; i < comboBox_projects.Items.Count; i++)
            {
                var comboboxItem = (ComboboxItem)comboBox_projects.Items[i];

                if (comboboxItem.Value == null) 
                    continue;

                var trackerProject = (TrackerProject)comboboxItem.Value;

                if (trackerProject.Id != Project.Id)
                    continue;

                selectedProjectIndex = i;
                break;
            }

            comboBox_projects.SelectedIndex = selectedProjectIndex;
            #endregion
            

            initialize_activity_types_combobox();

            #region  |  set activity type  |

            var selectedTypeIndex = 0;
            for (var i = 0; i < comboBox_activity_type.Items.Count; i++)
            {
                var comboboxItem = (ComboboxItem)comboBox_activity_type.Items[i];

                if (comboboxItem.Value == null) 
                    continue;

                var objs = (object[])comboboxItem.Value;

                Structures.ActivityType activityType = null;
                if (objs[0] != null)
                    activityType = (Structures.ActivityType)objs[0];
                ClientActivityType clientActivityType = null;
                if (objs[1] != null)
                    clientActivityType = (ClientActivityType)objs[1];


                if (Client != null)
                {
                    if (clientActivityType == null) 
                        continue;

                    if (clientActivityType.Id != Activity.ActivityTypeClientId) 
                        continue;

                    selectedTypeIndex = i;
                    break;
                }
                if (activityType != null && activityType.Id != Activity.ActivityTypeId)
                    continue;

                selectedTypeIndex = i;
                break;
            }
            comboBox_activity_type.SelectedIndex = selectedTypeIndex;
            #endregion


            textBox_name.Text = Activity.Name;
            textBox_description.Text = Activity.Description;


            #region  |  dates/times  |
            if (!IsEdit)
            {
                dateTimePicker_start_date.Value = DateTime.Now;
                dateTimePicker_start_hours.Value = DateTime.Now;

                dateTimePicker_end_date.Value = DateTime.Now;
                dateTimePicker_end_hours.Value = DateTime.Now;
            }
            else
            {
                dateTimePicker_start_date.Value = Activity.DateStart;
                dateTimePicker_start_hours.Value = Activity.DateStart;

                dateTimePicker_end_date.Value = Activity.DateEnd;
                dateTimePicker_end_hours.Value = Activity.DateEnd;
            }
            #endregion


            comboBox_status.SelectedItem = Activity.Status;

            checkBox_invoiced.Checked = Activity.Invoiced;
            dateTimePicker_date_invoiced.Value = Activity.InvoicedDate != Common.DateNull ? Activity.InvoicedDate : DateTime.Now;

            checkBox_billable.Checked = Activity.Billable;

            numericUpDown_hourly_rate_client.Value = Activity.HourlyRate;
            numericUpDown_quantity.Value = Activity.Quantity;

            label_total.Text = Math.Round(numericUpDown_quantity.Value * numericUpDown_hourly_rate_client.Value, 2).ToString(CultureInfo.InvariantCulture);


            textBox_name.Select();

            checkBox_invoiced_CheckedChanged(null, null);
            

            CheckActivatedControls();

            IsLoading = false;
            CheckAdjustment();
        }

        private void initialize_projects_combobox()
        {
            comboBox_projects.BeginUpdate();

            foreach (var project in Projects)
            {
                comboBox_projects.Items.Add(new ComboboxItem(project.Name, project));
            }

            comboBox_projects.EndUpdate();
        }

        private void initialize_activity_types_combobox()
        {
            comboBox_activity_type.BeginUpdate();

            comboBox_activity_type.Items.Clear();
            comboBox_activity_type.Items.Add(new ComboboxItem("(none)", null));

            foreach (var activityType in Tracked.Preferences.ActivitiesTypes)
            {
                ClientActivityType clientActivityType = null;
                #region  | get client activity type  |

                if (Client != null)
                {
                    foreach (var type in Client.ClientActivities)
                    {
                        if (activityType.Id != type.IdActivity) continue;
                        if (type.Activated)
                        {
                            clientActivityType = type;
                        }
                        break;
                    }
                    if (clientActivityType != null)
                    {
                        comboBox_activity_type.Items.Add(new ComboboxItem(activityType.Name, new object[] { activityType, clientActivityType }));
                    }
                }
                else
                {
                    comboBox_activity_type.Items.Add(new ComboboxItem(activityType.Name, new object[] { activityType, null }));
                }
                #endregion
            }


            comboBox_activity_type.EndUpdate();
        }


        private void button_save_Click(object sender, EventArgs e)
        {


            Activity.Name = textBox_name.Text.Trim();
            Activity.Description = textBox_description.Text;

            Activity.Status = comboBox_status.SelectedItem.ToString();

            var start = new DateTime(dateTimePicker_start_date.Value.Year, dateTimePicker_start_date.Value.Month, dateTimePicker_start_date.Value.Day
                , dateTimePicker_start_hours.Value.Hour, dateTimePicker_start_hours.Value.Minute, dateTimePicker_start_hours.Value.Second);

            var end = new DateTime(dateTimePicker_end_date.Value.Year, dateTimePicker_end_date.Value.Month, dateTimePicker_end_date.Value.Day
                , dateTimePicker_end_hours.Value.Hour, dateTimePicker_end_hours.Value.Minute, dateTimePicker_end_hours.Value.Second);


            Activity.DateStart = start;
            Activity.DateEnd = end;


            Activity.Billable = checkBox_billable.Checked;
            Activity.Quantity = numericUpDown_quantity.Value;

            Activity.Invoiced = checkBox_invoiced.Checked;
            Activity.InvoicedDate = Activity.Invoiced ? dateTimePicker_date_invoiced.Value : Common.DateNull;

            Activity.Total = Math.Round(Activity.HourlyRate * Activity.Quantity, 2);

            Saved = true;
            Close();
        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void comboBox_activity_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                CheckActivatedControls();
        }

        private void numericUpDown_hourly_rate_client_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                CheckAdjustment();
        }

        private void numericUpDown_quantity_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
            {
                CheckAdjustment();
                SaveButtonActive();
            }
        }

        private void dateTimePicker_start_date_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                SaveButtonActive();
        }

        private void dateTimePicker_start_hours_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                SaveButtonActive();
        }

        private void dateTimePicker_end_date_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                SaveButtonActive();
        }

        private void dateTimePicker_end_hours_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                SaveButtonActive();
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoading)
                SaveButtonActive();
        }

        private void checkBox_invoiced_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker_date_invoiced.Enabled = checkBox_invoiced.Checked;
        }

       




    }
}
