using System;
using System.Windows.Forms;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class TimedTextBox : ModelBase
	{
		private string _path;

		public TimedTextBox()
		{
			Timer.Tick += StartValidation;
			PropertyChanged += StartValidationTimer;
		}

		public event EventHandler ShouldStartValidation;
		public string Path
		{
			get => _path;
			set
			{
				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

		public Timer Timer { get; } = new Timer { Interval = 500 };

		private void StartValidationTimer(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Timer.Stop();
			Timer.Start();
		}
		private void StartValidation(object sender, EventArgs e)
		{
			Timer.Stop();
			ShouldStartValidation?.Invoke(sender, e);
		}
	}
}