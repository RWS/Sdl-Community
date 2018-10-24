using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Model.Log;
using Sdl.Community.SdlTmAnonymizer.Services;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class LogViewModel : ViewModelBase, IDisposable
	{
		private readonly TranslationMemoryViewModel _model;
		private readonly SettingsService _settingsService;
		private readonly SerializerService _serializerService;
		private ObservableCollection<Report> _reports;
		private Report _selectedItem;

		public LogViewModel(TranslationMemoryViewModel model, SerializerService serializerService)
		{
			_settingsService = model.SettingsService;
			_serializerService = serializerService;

			_model = model;
			_model.PropertyChanged += Model_PropertyChanged;
			_model.TmsCollection.CollectionChanged += TmsCollection_CollectionChanged;
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(_model.TmsCollection))
			{
				Refresh();
			}
		}

		private void TmsCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Refresh();
		}

		private void Refresh()
		{
			var logsFullPath = _settingsService.GetLogReportPath();

			var logReports = new List<Report>();
			var logFiles = Directory.GetFiles(logsFullPath, "*.xml").ToList();

			foreach (var logFile in logFiles)
			{
				var logFileName = Path.GetFileName(logFile);
				foreach (var tmFile in _model.TmsCollection.Where(a => a.IsSelected))
				{
					if (logFileName?.IndexOf(tmFile.Name, StringComparison.CurrentCultureIgnoreCase) > 0)
					{
						var report = _serializerService.Read<Report>(logFile);

						logReports.Add(report);
					}
				}
			}

			Reports = new ObservableCollection<Report>(logReports);
			OnPropertyChanged(nameof(Reports));
		}

		private static string GetSafeFileName(string name)
		{
			var invalid = new string(Path.GetInvalidFileNameChars());

			foreach (var c in invalid)
			{
				name = name.Replace(c.ToString(), string.Empty);
			}

			return name;
		}

		public string SelectedReportHtml
		{
			get { return SelectedItem != null ? SelectedItem.ReportFullPath : string.Empty; }
		}

		public Report SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				_selectedItem = value;

				OnPropertyChanged(nameof(SelectedItem));
				OnPropertyChanged(nameof(SelectedReportHtml));
			}
		}

		public ObservableCollection<Report> Reports
		{
			get
			{
				return _reports;

			}
			set
			{
				_reports = value;
				OnPropertyChanged(nameof(Reports));
			}
		}

		public void Dispose()
		{
			if (_model != null)
			{
				_model.TmsCollection.CollectionChanged -= TmsCollection_CollectionChanged;
				_model.Dispose();
			}
		}
	}
}