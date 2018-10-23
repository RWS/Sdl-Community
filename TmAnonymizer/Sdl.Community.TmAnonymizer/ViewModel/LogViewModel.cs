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

		public LogViewModel(TranslationMemoryViewModel model, SerializerService serializerService)
		{			
			_settingsService = model.SettingsService;
			_serializerService = serializerService;

			_model = model;
			_model.TmsCollection.CollectionChanged += TmsCollection_CollectionChanged;
		}

		private void TmsCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var logsFullPath = _settingsService.GetLogReportPath();

			var logReports = new List<Report>();
			var logFiles = Directory.GetFiles(logsFullPath, "*.xml").ToList();
			
			foreach (var logFile in logFiles)
			{
				var logFileName = Path.GetFileName(logFile);
				foreach (var tmFile in _model.TmsCollection)
				{
					if (tmFile.IsSelected && logFileName?.IndexOf(tmFile.Name, StringComparison.CurrentCultureIgnoreCase) > 0)
					{
						var report = _serializerService.Read<Report>(logFile);

						logReports.Add(report);
					}
				}
			}

			Reports = new ObservableCollection<Report>(logReports);
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