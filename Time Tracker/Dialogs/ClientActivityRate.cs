using System;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Structures;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    public partial class ClientActivityRate : Form
    {

        public bool Saved { get; set; }
        public bool IsEdit { get; set; }
        public Structures.ActivityType Activity { get; set; }
        public ClientActivityType CompanyActivity { get; set; }


        public ClientActivityRate()
        {
            InitializeComponent();


        }

        private void Activity_Load(object sender, EventArgs e)
        {
            Text += IsEdit ? " (Edit)" : " (New)";

            textBox_name.Text = Activity.Name;
            numericUpDown_hourlyRate_client.Value = CompanyActivity.HourlyRateClient;
            numericUpDown_hourlyRate_default.Value = Activity.HourlyRate;

            CompanyActivity.HourlyRateAdjustment = CompanyActivity.HourlyRateClient - Activity.HourlyRate;

            if (CompanyActivity.HourlyRateAdjustment > 0)
            {
                label_adjustment.Text = "+" + CompanyActivity.HourlyRateAdjustment;
                label_adjustment.ForeColor = Color.DarkBlue;
            }
            else if (CompanyActivity.HourlyRateAdjustment < 0)
            {
                label_adjustment.Text = "" + CompanyActivity.HourlyRateAdjustment;
                label_adjustment.ForeColor = Color.DarkRed;
            }
            else
            {
                label_adjustment.Text = "";
                
            }

        }

        private void button_save_Click(object sender, EventArgs e)
        {

        
            Saved = true;
            Close();

        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void numericUpDown_hourlyRate_client_ValueChanged(object sender, EventArgs e)
        {
            CompanyActivity.HourlyRateClient = numericUpDown_hourlyRate_client.Value;

            CompanyActivity.HourlyRateAdjustment = CompanyActivity.HourlyRateClient - Activity.HourlyRate;

            if (CompanyActivity.HourlyRateAdjustment > 0)
            {
                label_adjustment.Text = "+" + CompanyActivity.HourlyRateAdjustment;
                label_adjustment.ForeColor = Color.DarkBlue;
            }
            else if (CompanyActivity.HourlyRateAdjustment < 0)
            {
                label_adjustment.Text = "" + CompanyActivity.HourlyRateAdjustment;
                label_adjustment.ForeColor = Color.DarkRed;
            }
            else
            {
                label_adjustment.Text = "";
            }

        }



    }
}
