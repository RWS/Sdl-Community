using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sdl.Community.InSource.Models;

namespace Sdl.Community.InSource
{
    public partial class TimerControl : UserControl
    {
        private readonly TimerModel _timerSettings;
        private readonly Persistence _persistence = new Persistence();
        private readonly Lazy<InSourceViewControl> _control;

        public TimerControl(Lazy<InSourceViewControl> control)
        {
            InitializeComponent();

            _timerSettings = _persistence.LoadTimerSettings();
            _control = control;

            if (_timerSettings.HasTimer)
            {
                timerCheckBox.Checked = true;
                timeTextBox.Enabled = true;
                timeLbl.ForeColor = Color.Black;
                timeTextBox.Text = _timerSettings.Minutes.ToString();

            }
            else
            {
                timerCheckBox.Checked = false;
                timeTextBox.Enabled = false;
                timeLbl.ForeColor = Color.Gray;
                timeTextBox.Text = @"0";

            }

            timeTextBox.TextChanged += TimeTextBox_TextChanged;

        }


        private void TimeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(timeTextBox.Text, "^[0-9]*$"))
            {
                _timerSettings.Minutes = int.Parse(timeTextBox.Text);

                _persistence.SaveTimerSettings(_timerSettings);
                _control.Value.Value_TimeChanged();
            }
            else
            {
                 MessageBox.Show(@"Please set a number for timer ",@"Warning" , MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
          
        }

        private void timerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (timerCheckBox.Checked)
            {
                timeTextBox.Enabled = true;
                timeLbl.ForeColor = Color.Black;
                _timerSettings.HasTimer = true;

            }
            else
            {
                timeTextBox.Enabled = false;
                timeLbl.ForeColor = Color.Gray;
                _timerSettings.HasTimer = false;
                _timerSettings.Minutes = 0;
                timeTextBox.Text = @"0";
            }

        }

    }
}
