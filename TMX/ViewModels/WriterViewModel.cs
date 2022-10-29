using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.FileType.TMX.Settings;

namespace Sdl.Community.FileType.TMX.ViewModels
{
	public class WriterViewModel : INotifyPropertyChanged
	{
		private WriterSettings _settings;
		private string _test;

		public WriterSettings Settings => _settings;

		public WriterViewModel(WriterSettings settings)
		{
			_settings = settings;
		}

		public string Test
		{
			get => _test;
			set
			{
				if (value == _test) return;
				_test = value;
				OnPropertyChanged();
			}
		}

		public WriterSettings ResetToDefaults()
		{
			// FIXME
			return Settings;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
