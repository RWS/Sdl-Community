using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Sdl.Community.InSource.Interfaces;
using Sdl.Community.InSource.Models;
using Sdl.Community.InSource.Service;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Timer = System.Timers.Timer;

namespace Sdl.Community.InSource
{
	public partial class TimerControl : UserControl, IUIControl, IDisposable
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

			ConfigureTimer();
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
				ConfigureLabel(refreshIntervalLbl, string.Empty, Color.Black);
				SetIntervalTextBox(true, _timerSettings.Minutes.ToString());

				_timeLeft = _timerSettings.Minutes;
				Timer.Enabled = true;
				ConfigureLabel(remainingTimeLbl, $"{_timerSettings.Minutes}  {PluginResources.RemainingMinutes_Message}", Color.Empty);
			}
			else
			{
				timerCheckBox.Checked = false;
				SetIntervalTextBox(false, @"0");
				ConfigureLabel(refreshIntervalLbl, string.Empty, Color.Gray);
				ConfigureLabel(remainingTimeLbl, PluginResources.DisabledTimer_Message, Color.Gray);

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
				remainingTimeLbl.Text = _timeLeft + @" minutes until project request is checked. ";
			}
			if (_timeLeft == 0)
			{
				OnCheckForProjectsRequestEvent();
				_timeLeft = _timerSettings.Minutes;
				remainingTimeLbl.Text = _timeLeft + PluginResources.RemainingMinutes_Message;
			}
		}

		private void TimeTextBox_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(intervalTextBox.Text) && Regex.IsMatch(intervalTextBox.Text, "^[0-9]*$"))
			{
				_timerSettings.Minutes = int.Parse(intervalTextBox.Text);

				_persistence.SaveTimerSettings(_timerSettings);

				_timeLeft = _timerSettings.Minutes;

				ConfigureLabel(remainingTimeLbl, $"{_timeLeft} {PluginResources.RemainingMinutes_Message}", Color.Empty);
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
				ConfigureLabel(refreshIntervalLbl, string.Empty, Color.Black);
				ConfigureLabel(remainingTimeLbl, string.Empty, Color.Black);

				_timerSettings.HasTimer = true;
				if(intervalTextBox.Text.Equals("0"))
				{
					remainingTimeLbl.Text = string.Empty;
				}
			}
			else
			{
				SetIntervalTextBox(false, @"0");
				ConfigureLabel(refreshIntervalLbl, string.Empty, Color.Gray);

				_timerSettings.HasTimer = false;
				_timerSettings.Minutes = 0;
				Timer.Enabled = false;
				
				ConfigureLabel(remainingTimeLbl, PluginResources.DisabledTimer_Message, Color.Gray);
			}
		}

		private void SetIntervalTextBox(bool isEnabled, string text)
		{
			intervalTextBox.Enabled = isEnabled;
			intervalTextBox.Text = text;
		}

		private void ConfigureLabel(Label label, string text, Color color)
		{
			if (!string.IsNullOrEmpty(text))
			{
				label.Text = text;
			}
			if (!color.IsEmpty)
			{
				label.ForeColor = color;
			}
		}

		private void OnCheckForProjectsRequestEvent()
		{
			CheckForProjectsRequestEvent?.Invoke(this, EventArgs.Empty);
		}

		private void ConfigureTimer()
		{
			Timer = new Timer();
			Timer.Interval = 60000;
			Timer.Elapsed += _timer_Elapsed;
			Timer.AutoReset = true;
		}
	}
}