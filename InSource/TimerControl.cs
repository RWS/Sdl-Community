using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Sdl.Community.InSource.Models;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Timer = System.Timers.Timer;

namespace Sdl.Community.InSource
{
    public partial class TimerControl : UserControl
    {
        private readonly TimerModel _timerSettings;
        private readonly Persistence _persistence = new Persistence();
        private Timer _timer;
        private int _timeLeft=0;
        private readonly Lazy<InSourceViewControl> _control;

        public TimerControl(Lazy<InSourceViewControl>  control)
        {
            InitializeComponent();

            _timerSettings = _persistence.LoadTimerSettings();
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = 60000;
               
            _timer.AutoReset = true;
            
            _timer.Start();

             _control = control;

            if (_timerSettings.HasTimer)
            {
                timerCheckBox.Checked = true;
                timeTextBox.Enabled = true;
                timeLbl.ForeColor = Color.Black;
                timeTextBox.Text = _timerSettings.Minutes.ToString();

                _timeLeft = _timerSettings.Minutes;
                _timer.Enabled = true;
                _timer.Elapsed += _timer_Elapsed;
                remainingTimeLbl.Text = _timerSettings.Minutes.ToString();
            }
            else
            {
                timerCheckBox.Checked = false;
                timeTextBox.Enabled = false;
                timeLbl.ForeColor = Color.Gray;
                timeTextBox.Text = @"0";             
                remainingTimeLbl.Text = @"Timer is disabled";
                remainingTimeLbl.ForeColor = Color.Gray;
                timerLbl.ForeColor = Color.Gray;

                _timer.Stop();
             
            }

            timeTextBox.TextChanged += TimeTextBox_TextChanged;

        }

     
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_timeLeft > 0)
            {
                _timeLeft--;
                remainingTimeLbl.Text = _timeLeft + @" min";
                
                
            }
            else
            {
                _control.Value.Controller.CheckForProjects();
                _timeLeft = _timerSettings.Minutes;
            }
        }


        private void TimeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(timeTextBox.Text, "^[0-9]*$"))
            {
                _timerSettings.Minutes = int.Parse(timeTextBox.Text);

                _persistence.SaveTimerSettings(_timerSettings);

                _timeLeft = _timerSettings.Minutes;
                _timer.Enabled = true;
                _timer.Elapsed += _timer_Elapsed;

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
                remainingTimeLbl.ForeColor = Color.Black;
                timerLbl.ForeColor = Color.Black;
                _timerSettings.HasTimer = true;

            }
            else
            {
                timeTextBox.Enabled = false;
                timeLbl.ForeColor = Color.Gray;
                _timerSettings.HasTimer = false;
                _timerSettings.Minutes = 0;
                timeTextBox.Text = @"0";

                _timer.Stop();
               // _timer.Dispose();
                remainingTimeLbl.Text = @"Timer is disabled";
                remainingTimeLbl.ForeColor = Color.Gray;
                timerLbl.ForeColor = Color.Gray;
            }

        }

    }
}
