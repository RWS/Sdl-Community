using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class SettingsViewModel : INotifyPropertyChanged
	{
		private readonly Window _window;
		private readonly Settings _settings;
		private readonly PathInfo _pathInfo;
		private string _windowTitle;
		private ICommand _saveCommand;
		private ICommand _resetCommand;
		private bool _displayDateSuffixWithReportName;
		private GroupType _groupType;
		private List<GroupType> _groupTypes;


		public SettingsViewModel(Window window, Settings settings, PathInfo pathInfo)
		{
			_window = window;
			_settings = settings;
			_pathInfo = pathInfo;

			WindowTitle = "Settings";

			DisplayDateSuffixWithReportName = settings.DisplayDateSuffixWithReportName;
			GroupType = GroupTypes.FirstOrDefault(a => a.Type == settings.GroupByType) ?? GroupTypes.First();
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(SaveChanges));

		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(Reset));


		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public bool DisplayDateSuffixWithReportName
		{
			get => _displayDateSuffixWithReportName;
			set
			{
				if (_displayDateSuffixWithReportName == value)
				{
					return;
				}

				_displayDateSuffixWithReportName = value;
				OnPropertyChanged(nameof(DisplayDateSuffixWithReportName));
			}
		}

		public GroupType GroupType
		{
			get => _groupType;
			set
			{
				if (_groupType == value)
				{
					return;
				}

				_groupType = value;
				OnPropertyChanged(nameof(GroupType));
			}
		}

		public List<GroupType> GroupTypes
		{
			get
			{
				return _groupTypes ?? (_groupTypes = new List<GroupType>
				{
					new GroupType
					{
						Name = "Group Name",
						Type = "Group"
					},
					new GroupType
					{
						Name = "Language",
						Type = "Language"
					},
				});
			}
			set
			{
				_groupTypes = value;
				OnPropertyChanged(nameof(GroupType));
			}
		}

		private void SaveChanges(object parameter)
		{
			_settings.DisplayDateSuffixWithReportName = DisplayDateSuffixWithReportName;
			_settings.GroupByType = GroupType.Type;

			File.WriteAllText(_pathInfo.SettingsFilePath, JsonConvert.SerializeObject(_settings));

			_window.DialogResult = true;
			_window?.Close();
		}

		private void Reset(object paramter)
		{
			var settings = new Settings();

			DisplayDateSuffixWithReportName = settings.DisplayDateSuffixWithReportName;
			GroupType = GroupTypes.FirstOrDefault(a => a.Type == settings.GroupByType) ?? GroupTypes.First();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
