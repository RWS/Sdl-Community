using System;
using System.Windows.Forms;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class TimedTextBox : ModelBase
	{
		private string _path;
		public event EventHandler ShouldStartValidation;

		public TimedTextBox()
		{
			Timer.Tick += StartValidation;
			PropertyChanged += StartValidationTimer;
		}

		private void StartValidationTimer(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Timer.Stop();
			Timer.Start();
		}

		public string Path
		{
			get => _path;
			set
			{
				_path = value;
				OnPropertyChanged();
			}
		}

		public Timer Timer { get; } = new Timer { Interval = 500 };

		private void StartValidation(object sender, EventArgs e)
		{
			Timer.Stop();
			ShouldStartValidation?.Invoke(sender, e);
		}
	}
}
