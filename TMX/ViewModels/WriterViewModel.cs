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
		private bool _writeChangeDate = false;
		private bool _writeUserId = false;

		public WriterSettings Settings => _settings;
		private bool _loaded;

		public WriterViewModel(WriterSettings settings)
		{
			_settings = settings;
			Load();
			_loaded = true;
		}

		public bool WriteChangeDate
		{
			get => _writeChangeDate;
			set
			{
				if (value == _writeChangeDate) return;
				_writeChangeDate = value;
				OnPropertyChanged();
			}
		}

		public bool WriteUserID
		{
			get => _writeUserId;
			set
			{
				if (value == _writeUserId) return;
				_writeUserId = value;
				OnPropertyChanged();
			}
		}

		private void Load()
		{
			WriteChangeDate = Settings.WriteChangeDate;
			WriteUserID = Settings.WriteUserID;
		}

		private void Save()
		{
			Settings.WriteChangeDate = WriteChangeDate;
			Settings.WriteUserID = WriteUserID;
		}

		public WriterSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();
			return Settings;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (_loaded)
				Save();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
