using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Custom;
using Sdl.Community.Studio.Time.Tracker.Structures;
using Sdl.Community.Studio.Time.Tracker.Tracking;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    public partial class ActivityType : Form
    {

        public bool Saved { get; set; }
        public bool IsEdit { get; set; }
        public Structures.ActivityType Activity { get; set; }
        public string DefaultCurrency { get; set; }

        public ActivityType()
        {
            InitializeComponent();

            foreach (var c in Tracked.Currencies)
            {
                comboBox_currency.Items.Add(new ComboboxItem(c.Name + "  (" + c.Country + ")", c));
            }            
        }

        private void Activity_Load(object sender, EventArgs e)
        {
            Text += IsEdit ? " (Edit)" : " (New)";
            checkBox_activateForAllClients.Visible = !IsEdit;

            textBox_name.Text = Activity.Name;
            textBox_description.Text = Activity.Description;
            numericUpDown_hourlyRate.Value = Activity.HourlyRate;

            var iSelectedIndex = -1;
            var iDefaultIndex = 0;
            for (var i = 0; i < comboBox_currency.Items.Count; i++)
            {
                var comboboxItem = (ComboboxItem)comboBox_currency.Items[i];

                var itemValue = (Currency)comboboxItem.Value;
                if (string.Compare(itemValue.Name, Activity.Currency, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    iSelectedIndex = i;
                    break;
                }
                if (string.Compare(itemValue.Name, DefaultCurrency, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    iDefaultIndex = i;
                }
            }
            comboBox_currency.SelectedIndex = iSelectedIndex > -1 ? iSelectedIndex : iDefaultIndex;

            checkBox_billableHours.Checked = Activity.Billable;

            textBox_name_TextChanged(sender, e);

        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (textBox_name.Text.Trim() == string.Empty)
                return;

            var foundName = Tracked.Preferences.ActivitiesTypes.Any(a => 
                string.Compare(textBox_name.Text.Trim(), a.Name, StringComparison.OrdinalIgnoreCase) == 0 && a.Id != Activity.Id);
            if (foundName)
            {
                MessageBox.Show(this, @"Unable to save the Activity; name already exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {


                Activity.Name = textBox_name.Text;
                Activity.Description = textBox_description.Text;
                Activity.HourlyRate = numericUpDown_hourlyRate.Value;

                var comboboxItem = (ComboboxItem)comboBox_currency.Items[comboBox_currency.SelectedIndex];
                var itemValue = (Currency)comboboxItem.Value;
                Activity.Currency = itemValue.Name;

                Activity.Billable = checkBox_billableHours.Checked;

                Saved = true;
                Close();
            }
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            button_save.Enabled = textBox_name.Text.Trim() != string.Empty;
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }
        

      
    }
}
