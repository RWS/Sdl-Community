using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.SdlTmAnonymizer.Model.Log;
using Sdl.Community.SdlTmAnonymizer.Services;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class LogViewModel : ViewModelBase, IDisposable
	{
		private readonly TranslationMemoryViewModel _model;
		private readonly SettingsService _settingsService;
		private readonly SerializerService _serializerService;
		private ObservableCollection<ReportFile> _reportFiles;
		private ReportFile _selectedItem;

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

		private void TmsCollection_CollectionChanged(object sender,
			System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			Refresh();
		}

		private void Refresh()
		{
			var selectedFullPath = SelectedItem?.FullPath;

			var logsFullPath = _settingsService.GetLogReportPath();

			var reportFiles = new List<ReportFile>();
			var logFiles = Directory.GetFiles(logsFullPath, "*.xml").ToList();
			var regexFileName = new Regex(@"^(?<type>\d)\.(?<date>[^\.]+)\.(?<name>.*)$", RegexOptions.None);

			foreach (var logFile in logFiles)
			{
				var logFileName = Path.GetFileName(logFile);
				foreach (var tmFile in _model.TmsCollection.Where(a => a.IsSelected))
				{
					if (logFileName?.IndexOf(tmFile.Name, StringComparison.CurrentCultureIgnoreCase) > 0)
					{

						var reportFile = new ReportFile
						{
							FullPath = logFile,
							Name = Path.GetFileName(logFile),
							Created = DateTime.Now,
							Scope = Report.ReportScope.All
						};


						var match = regexFileName.Match(reportFile.Name);
						if (match.Success)
						{
							var type = match.Groups["type"].Value;
							var date = match.Groups["date"].Value;
							var name = match.Groups["name"].Value;

							reportFile.Scope = (Report.ReportScope)Convert.ToInt32(type);
							reportFile.Created = _settingsService.GetDateTimeFromString(date);
							reportFile.Name = name;
						}

						reportFiles.Add(reportFile);
					}
				}
			}

			ReportFiles = new ObservableCollection<ReportFile>(reportFiles);
			OnPropertyChanged(nameof(ReportFiles));

			if (ReportFiles?.Count > 0)
			{
				SelectedItem = ReportFiles.FirstOrDefault(a => a.FullPath == selectedFullPath) ?? ReportFiles[0];
			}
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
			get { return SelectedItem != null ? SelectedItem.FullPath : string.Empty; }
		}

		public ReportFile SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;

				OnPropertyChanged(nameof(SelectedItem));
				OnPropertyChanged(nameof(Report));
			}
		}

		public string ReportCreated { get; set; }

		public Report Report
		{
			get
			{
				Report report;
				if (SelectedItem != null && File.Exists(SelectedItem.FullPath))
				{
					report = _serializerService.Read<Report>(SelectedItem.FullPath);
					ReportCreated = report.Created.ToString(CultureInfo.InvariantCulture);									
				}
				else
				{
					report = new Report();
					ReportCreated = string.Empty;
				}
				
				OnPropertyChanged(nameof(ReportCreated));

				return report;
			}
		}

		public ObservableCollection<ReportFile> ReportFiles
		{
			get
			{
				return _reportFiles;

			}
			set
			{
				_reportFiles = value;
				OnPropertyChanged(nameof(ReportFiles));
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