using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Sdl.Community.InSource.Interfaces;
using Sdl.Community.InSource.Models;
using Sdl.Community.InSource.Service;
using Timer = System.Timers.Timer;

namespace Sdl.Community.InSource
{
	public partial class TimerControl : UserControl, IDisposable
	{
		private TimerModel _timerSettings;
		private readonly Persistence _persistence = new Persistence();
		private int _timeLeft;
		private IMessageBoxService _messageBoxService;

		public event EventHandler CheckForProjectsRequestEvent;
		public Timer Timer { get; set; }

		public TimerControl()
		{
			InitializeComponent();
			_messageBoxService = new MessageBoxService();

			Timer = new Timer();
			Timer.Interval = 60000;
			Timer.Elapsed += _timer_Elapsed;
			Timer.AutoReset = true;

			intervalTextBox.TextChanged += TimeTextBox_TextChanged;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			Timer.Dispose();
			base.Dispose(disposing);
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
				Timer.Enabled = true;
				remainingTime.Text = _timerSettings.Minutes + @" minutes until project request is checked. ";
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

				Timer.Enabled = false;

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
				Timer.Enabled = true;
			}
			else
			{
				_messageBoxService.ShowWarningMessage(PluginResources.TimerNumberSetup_Message, string.Empty);
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

				Timer.Enabled = false;
				remainingTime.Text = @"Timer is disabled";
				remainingTime.ForeColor = Color.Gray;
				remainingTime.ForeColor = Color.Gray;
			}
		}

		private void OnCheckForProjectsRequestEvent()
		{
			CheckForProjectsRequestEvent?.Invoke(this, EventArgs.Empty);
		}
	}
}