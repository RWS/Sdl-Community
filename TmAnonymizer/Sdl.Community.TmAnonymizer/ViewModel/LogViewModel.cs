using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Model.Log;
using Sdl.Community.SdlTmAnonymizer.Services;
using SegmentComparer.Structure;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class LogViewModel : ViewModelBase, IDisposable
	{
		private readonly TranslationMemoryViewModel _model;
		private readonly SettingsService _settingsService;
		private readonly SerializerService _serializerService;
		private readonly ExcelImportExportService _excelImportExportService;
		private ObservableCollection<ReportFile> _reportFiles;
		private ICommand _openFolderContaining;
		private ICommand _exportToExcel;
		private ReportFile _selectedItem;
		private bool _isEnabled;

		public LogViewModel(TranslationMemoryViewModel model, SerializerService serializerService, ExcelImportExportService excelImportExportService)
		{
			_settingsService = model.SettingsService;
			_serializerService = serializerService;
			_excelImportExportService = excelImportExportService;

			// the amount of actions that are visble in the log report
			VisibleActionsLimit = 100;

			_model = model;
			_model.PropertyChanged += Model_PropertyChanged;
			_model.TmsCollection.CollectionChanged += TmsCollection_CollectionChanged;

			IsEnabled = _settingsService.GetSettings().Accepted;
		}

		public int VisibleActionsLimit { get; set; }

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

		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				_isEnabled = value;
				OnPropertyChanged(nameof(IsEnabled));
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

		public ICommand OpenFolderContainingCommand => _openFolderContaining ?? (_openFolderContaining = new RelayCommand(OpenFolderContaining));

		public ICommand ExportToExcelCommand => _exportToExcel ?? (_exportToExcel = new RelayCommand(ExportToExcel));

		private void OpenFolderContaining(object parameter)
		{
			if (SelectedItem != null && Directory.Exists(Path.GetDirectoryName(SelectedItem.FullPath)))
			{
				System.Diagnostics.Process.Start("explorer.exe", "\"" + Path.GetDirectoryName(SelectedItem.FullPath) + "\"");
			}
		}

		private void ExportToExcel(object parameter)
		{
			if (SelectedItem != null && Directory.Exists(Path.GetDirectoryName(SelectedItem.FullPath)))
			{
				_excelImportExportService.ExportLogReportToExcel(SelectedItem.FullPath + ".xlsx", Report);

				if (File.Exists(SelectedItem.FullPath + ".xlsx"))
				{
					System.Diagnostics.Process.Start("\"" + SelectedItem.FullPath + ".xlsx" + "\"");
				}
			}
		}

		public string SelectedReportHtml => SelectedItem != null ? SelectedItem.FullPath : string.Empty;

		public ReportFile SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;

				OnPropertyChanged(nameof(SelectedItem));
				OnPropertyChanged(nameof(Report));
			}
		}

		public string ReportCreated { get; set; }

		private Report _report;
		public Report Report
		{
			get
			{
				if (SelectedItem != null && File.Exists(SelectedItem.FullPath))
				{
					if (_report == null || _report.ReportFullPath != SelectedItem.FullPath)
					{
						
						_report = _serializerService.Read<Report>(SelectedItem.FullPath);
						ReportCreated = _report.Created.ToString(CultureInfo.InvariantCulture);

						if (_report.Actions.Count > VisibleActionsLimit)
						{
							MessageBox.Show("Only the first " + VisibleActionsLimit + " comparison differences are identified in view", "SDLTM Anonymizer");
						}

						var comparer = new SegmentComparer.Comparer();

						if (_report.Scope == Report.ReportScope.Content)
						{
							var i = 0;
							foreach (var action in _report.Actions)
							{								
								if (i++ < VisibleActionsLimit)
								{
									var comparison = comparer.CompareSegment(action.TmId.Id.ToString(), action.Previous, action.Value, true, 1);

									var previousSpan = new Span();
									var valueSpan = new Span();

									var toolTip = string.Empty;
									foreach (var unit in comparison.ComparisonUnits)
									{
										switch (unit.Type)
										{
											case ComparisonUnit.ComparisonType.New:

												if (unit.TextType != ComparisonUnit.ContentType.Tag)
												{
													valueSpan.Inlines.Add(new Run(unit.Text));
												}
												else
												{
													var valueNewText = new Run(unit.Text)
													{
														Background = new SolidColorBrush(Colors.Yellow),
														ToolTip = toolTip
													};

													valueSpan.Inlines.Add(valueNewText);
												}

												break;
											case ComparisonUnit.ComparisonType.Removed:
												var previousRemovedText = new Run(unit.Text)
												{
													Background = new SolidColorBrush(Colors.Pink),
												};

												toolTip = unit.Text;
												previousSpan.Inlines.Add(previousRemovedText);
												break;
											case ComparisonUnit.ComparisonType.Identical:
											case ComparisonUnit.ComparisonType.None:
												previousSpan.Inlines.Add(new Run(unit.Text));
												valueSpan.Inlines.Add(new Run(unit.Text));
												break;
										}
									}

									action.PreviousSpan = previousSpan;
									action.ValueSpan = valueSpan;
								}
								else
								{
									var previousSpan = new Span();
									var valueSpan = new Span();

									previousSpan.Inlines.Add(new Run(action.Previous));
									valueSpan.Inlines.Add(new Run(action.Value));

									action.PreviousSpan = previousSpan;
									action.ValueSpan = valueSpan;
								}
							}

							_report.Actions.RemoveAll(a => a.PreviousSpan == null);
						}
						else
						{
							foreach (var action in _report.Actions)
							{
								var previousSpan = new Span();
								var valueSpan = new Span();

								previousSpan.Inlines.Add(new Run(action.Previous));
								valueSpan.Inlines.Add(new Run(action.Value));

								action.PreviousSpan = previousSpan;
								action.ValueSpan = valueSpan;
							}
						}
					}
				}
				else
				{
					_report = new Report();
					ReportCreated = string.Empty;
				}

				OnPropertyChanged(nameof(ReportCreated));

				return _report;
			}
		}

		public ObservableCollection<ReportFile> ReportFiles
		{
			get => _reportFiles;
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