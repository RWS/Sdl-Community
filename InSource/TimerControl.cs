using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Sdl.Community.InSource.Models;
using Timer = System.Timers.Timer;

namespace Sdl.Community.InSource
{
    public partial class TimerControl : UserControl
    {
        private  TimerModel _timerSettings;
        private readonly Persistence _persistence = new Persistence();
        private readonly Timer _timer;
        private int _timeLeft;
        public event EventHandler CheckForProjectsRequestEvent;

        public TimerControl()
        {
            InitializeComponent();

            _timer = new Timer();
            _timer.Interval = 60000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;

            intervalTextBox.TextChanged += TimeTextBox_TextChanged;

        }

        protected override void OnLoad(EventArgs e)
        {
            _timerSettings = _persistence.LoadRequest().Timer;

            if (_timerSettings.HasTimer)
            {
                timerCheckBox.Checked = true;
                intervalTextBox.Enabled = true;
                refreshIntervalLbl.ForeColor = Color.Black;
                intervalTextBox.Text = _timerSettings.Minutes.ToString();

                _timeLeft = _timerSettings.Minutes;
                _timer.Enabled = true;
                remainingTime.Text = _timerSettings.Minutes +@" minutes until project request is checked. ";
            }
            else
            {
                timerCheckBox.Checked = false;
                intervalTextBox.Enabled = false;
                refreshIntervalLbl.ForeColor = Color.Gray;
                intervalTextBox.Text = @"0";
                remainingTime.Text = @"Timer is disabled";
                remainingTime.ForeColor = Color.Gray;
                remainingTime.ForeColor = Color.Gray;

                _timer.Enabled = false;

            }
            if (_persistence.LoadRequest().DeleteFolders)
            {
                deleteBtn.Checked = true;
            }
            else
            {
                archiveBtn.Checked = true;
            }
            archiveBtn.CheckedChanged += ArchiveBtn_CheckedChanged;

        }

        private void ArchiveBtn_CheckedChanged(object sender, EventArgs e)
        {
            bool deleteFolders;
            if (!archiveBtn.Checked && deleteBtn.Checked)
            {
                deleteFolders = true;
            }
            else
            {
                deleteFolders = false;
            }
          
            _persistence.UpdateDelete(deleteFolders);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_timeLeft > 0)
            {
                _timeLeft--;
                remainingTime.Text = _timeLeft + @" minutes until project request is checked. ";


            }
            if (_timeLeft == 0)
            {
                OnCheckForProjectsRequestEvent();
                _timeLeft = _timerSettings.Minutes;
                remainingTime.Text = _timeLeft + @" minutes until project request is checked. ";
            }
               
            
        }


        private void TimeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(intervalTextBox.Text, "^[0-9]*$"))
            {
                _timerSettings.Minutes = int.Parse(intervalTextBox.Text);

                _persistence.SaveTimerSettings(_timerSettings);

                _timeLeft = _timerSettings.Minutes;

                remainingTime.Text = _timeLeft + @" minutes until project request is checked. ";
                _timer.Enabled = true;
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
                intervalTextBox.Enabled = true;
                refreshIntervalLbl.ForeColor = Color.Black;
                remainingTime.ForeColor = Color.Black;
                remainingTime.ForeColor = Color.Black;
                _timerSettings.HasTimer = true;
               

            }
            else
            {
                intervalTextBox.Enabled = false;
                refreshIntervalLbl.ForeColor = Color.Gray;
                _timerSettings.HasTimer = false;
                _timerSettings.Minutes = 0;
                intervalTextBox.Text = @"0";

                _timer.Enabled = false;
               remainingTime.Text = @"Timer is disabled";
                remainingTime.ForeColor = Color.Gray;
                remainingTime.ForeColor = Color.Gray;
            }

        }

       private void OnCheckForProjectsRequestEvent()
        {
            
            if (CheckForProjectsRequestEvent != null)
            {
                CheckForProjectsRequestEvent(this, EventArgs.Empty);
            }
        }
    }
}
