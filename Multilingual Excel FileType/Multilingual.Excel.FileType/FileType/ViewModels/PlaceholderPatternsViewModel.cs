using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Multilingual.Excel.FileType.Commands;
using Multilingual.Excel.FileType.Controls;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.FileType.Views;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Providers.PlaceholderPatterns;
using DataFormats = System.Windows.Forms.DataFormats;
using DataGrid = System.Windows.Controls.DataGrid;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Multilingual.Excel.FileType.FileType.ViewModels
{
	public class PlaceholderPatternsViewModel : BaseModel, IDisposable
	{
		private readonly PlaceholderPatternsProvider _placeholderPatternsProvider;
		private bool _placeholderPatternsProcess;

		private PlaceholderPattern _selectedPlaceholderPattern;

		private ObservableCollection<PlaceholderPattern> _placeholderPatterns;
		private IList _selectedPlaceholderPatterns;
		private bool _selectAll;

		private ICommand _exportPlaceholderPatternsCommand;
		private ICommand _importPlaceholderPatternsCommand;
		private ICommand _selectAllCommand;
		private ICommand _movePlaceholderPatternUpCommand;
		private ICommand _movePlaceholderPatternDownCommand;
		private ICommand _addPlaceholderPatternCommand;
		private ICommand _editPlaceholderPatternCommand;
		private ICommand _removePlaceholderPatternCommand;
		private ICommand _dragDropCommand;
		private ICommand _mouseDoubleClick;

		public PlaceholderPatternsViewModel(PlaceholderPatternSettings settings, PlaceholderPatternsProvider placeholderPatternsProvider)
		{
			Settings = settings;
			_placeholderPatternsProvider = placeholderPatternsProvider;

			UpdatePlaceholderPatternOrder(Settings.PlaceablePatterns);

			PlaceholderPatternsProcess = Settings.PlaceablePatternsProcess;
			PlaceholderPatterns = new ObservableCollection<PlaceholderPattern>(Settings.PlaceablePatterns);

			UpdateCheckedAllState();
		}

		public ICommand ExportPlaceholderPatternsCommand => _exportPlaceholderPatternsCommand ?? (_exportPlaceholderPatternsCommand = new CommandHandler(ExportPlaceholderPatterns));

		public ICommand ImportPlaceholderPatternsCommand => _importPlaceholderPatternsCommand ?? (_importPlaceholderPatternsCommand = new CommandHandler(ImportPlaceholderPatterns));


		public ICommand MovePlaceholderPatternUpCommand => _movePlaceholderPatternUpCommand ?? (_movePlaceholderPatternUpCommand = new CommandHandler(MovePlaceholderPatternUp));

		public ICommand MovePlaceholderPatternDownCommand => _movePlaceholderPatternDownCommand ?? (_movePlaceholderPatternDownCommand = new CommandHandler(MovePlaceholderPatternDown));

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllPatterns));

		public ICommand AddPlaceholderPatternCommand => _addPlaceholderPatternCommand ?? (_addPlaceholderPatternCommand = new CommandHandler(AddPlaceholderPattern));

		public ICommand EditPlaceholderPatternCommand => _editPlaceholderPatternCommand ?? (_editPlaceholderPatternCommand = new CommandHandler(EditPlaceholderPattern));

		public ICommand RemovePlaceholderPatternCommand => _removePlaceholderPatternCommand ?? (_removePlaceholderPatternCommand = new CommandHandler(RemovePlaceholderPattern));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

		public PlaceholderPatternSettings Settings { get; }

		public bool PlaceholderPatternsProcess
		{
			get => _placeholderPatternsProcess;
			set
			{
				if (_placeholderPatternsProcess == value)
				{
					return;
				}

				_placeholderPatternsProcess = value;
				OnPropertyChanged(nameof(PlaceholderPatternsProcess));

				Settings.PlaceablePatternsProcess = _placeholderPatternsProcess;
			}
		}

		public ObservableCollection<PlaceholderPattern> PlaceholderPatterns
		{
			get => _placeholderPatterns;
			set
			{

				if (_placeholderPatterns != null)
				{
					foreach (var pattern in _placeholderPatterns)
					{
						pattern.PropertyChanged -= PatternOnPropertyChanged;
					}
				}

				_placeholderPatterns = value;

				if (_placeholderPatterns != null)
				{
					foreach (var pattern in _placeholderPatterns)
					{
						pattern.PropertyChanged += PatternOnPropertyChanged;
					}
				}

				Settings.PlaceablePatterns = _placeholderPatterns?.ToList();

				OnPropertyChanged(nameof(PlaceholderPatterns));
				OnPropertyChanged(nameof(PlaceholderPatternsStatusLabel));
			}
		}

		private void PatternOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateCheckedAllState();

			Settings.PlaceablePatterns = _placeholderPatterns?.ToList();
		}

		public PlaceholderPattern SelectedPlaceholderPattern
		{
			get => _selectedPlaceholderPattern;
			set
			{
				_selectedPlaceholderPattern = value;
				OnPropertyChanged(nameof(SelectedPlaceholderPattern));

				OnPropertyChanged(nameof(IsPlaceholderPatternSelected));
				OnPropertyChanged(nameof(IsPlaceholderPatternsSelected));
				OnPropertyChanged(nameof(PlaceholderPatternsStatusLabel));
			}
		}

		public IList SelectedPlaceholderPatterns
		{
			get => _selectedPlaceholderPatterns;
			set
			{
				_selectedPlaceholderPatterns = value;
				OnPropertyChanged(nameof(SelectedPlaceholderPatterns));


				_selectedPlaceholderPattern = _selectedPlaceholderPatterns?.Cast<PlaceholderPattern>().ToList().FirstOrDefault();

				OnPropertyChanged(nameof(IsPlaceholderPatternSelected));
				OnPropertyChanged(nameof(IsPlaceholderPatternsSelected));
				OnPropertyChanged(nameof(PlaceholderPatternsStatusLabel));
			}
		}

		public bool SelectAll
		{
			get => _selectAll;
			set
			{
				if (Equals(value, _selectAll))
				{
					return;
				}
				_selectAll = value;
				OnPropertyChanged(nameof(SelectAll));
			}
		}

		public string PlaceholderPatternsStatusLabel
		{
			get
			{
				var message = string.Format("Placeholder patterns: {0}, Selected: {1}",
					PlaceholderPatterns?.Count ?? 0,
					SelectedPlaceholderPatterns?.Count ?? 0);
				return message;
			}
		}

		public bool IsPlaceholderPatternsSelected => SelectedPlaceholderPatterns?.Cast<PlaceholderPattern>().ToList().Count > 0;

		public bool IsPlaceholderPatternSelected => SelectedPlaceholderPatterns?.Cast<PlaceholderPattern>().ToList().Count == 1;

		public PlaceholderPatternSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();

			PlaceholderPatternsProcess = Settings.PlaceablePatternsProcess;
			PlaceholderPatterns = new ObservableCollection<PlaceholderPattern>(Settings.PlaceablePatterns);

			return Settings;
		}

		private void ExportPlaceholderPatterns()
		{
			if (SelectedPlaceholderPatterns?.Count > 0)
			{
				var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					Sdl.Versioning.Versions.StudioDocumentsFolderName, "Placeholder Patterns");
				if (!Directory.Exists(defaultPath))
				{
					Directory.CreateDirectory(defaultPath);
				}

				var selectedRules = new List<PlaceholderPattern>();
				var fileDialog = new SaveFileDialog
				{
					Title = PluginResources.InformationMessage_ExortSelectedPlaceholders,
					Filter = @"Excel |*.xlsx",
					InitialDirectory = defaultPath,
					FileName = Path.Combine(defaultPath, "Placeholder Patterns.xlsx")
				};


				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{

					foreach (var rule in SelectedPlaceholderPatterns.OfType<PlaceholderPattern>())
					{
						selectedRules.Add(rule);
					}

					if (!fileDialog.FileName.ToLower().EndsWith(".xlsx"))
					{
						fileDialog.FileName += ".xlsx";
					}

					_placeholderPatternsProvider.SavePlaceholderPatterns(selectedRules, fileDialog.FileName);

					MessageBox.Show(PluginResources.InformationMessage_FileWasExportedSuccessfully, PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Information);

					if (SelectedPlaceholderPattern != null && File.Exists(fileDialog.FileName))
					{
						System.Diagnostics.Process.Start("\"" + fileDialog.FileName + "\"");
					}
				}
			}
			else
			{
				MessageBox.Show(PluginResources.InformationMessage_PleaseSelectAtLeastOnePlaceholder, PluginResources.Plugin_Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void ImportPlaceholderPatterns()
		{
			var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				Sdl.Versioning.Versions.StudioDocumentsFolderName, "Placeholder Patterns");
			if (!Directory.Exists(defaultPath))
			{
				Directory.CreateDirectory(defaultPath);
			}

			var fileDialog = new OpenFileDialog
			{
				Title = PluginResources.InformationMessage_SelectTheImportFiles,
				Filter = @"Excel |*.xlsx",
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = "xlsx",
				Multiselect = true,
				InitialDirectory = defaultPath
			};

			var result = fileDialog.ShowDialog();
			if (result == DialogResult.OK && fileDialog.FileNames.Length > 0)
			{
				ImportPlaceholderPatternsFromFiles(fileDialog.FileNames.ToList());
			}
		}

		private void ImportPlaceholderPatternsFromFiles(List<string> fileNames)
		{
			var patterns = new List<PlaceholderPattern>();

			foreach (var fileName in fileNames)
			{
				var importedRules = _placeholderPatternsProvider.GetPlaceholderPatterns(fileName);
				if (importedRules == null)
				{
					return;
				}

				patterns.AddRange(PlaceholderPatterns.Select(rule => rule.Clone() as PlaceholderPattern));

				foreach (var pattern in importedRules)
				{
					var existingPattern = patterns.FirstOrDefault(s =>
						s.Pattern.Equals(pattern.Pattern, StringComparison.InvariantCultureIgnoreCase));
					if (existingPattern == null)
					{
						patterns.Add(new PlaceholderPattern
						{
							Pattern = pattern.Pattern,
							Description = pattern.Description,
							SegmentationHint = pattern.SegmentationHint,
							Order = patterns.Count
						});
					}
					else
					{
						existingPattern.Pattern = pattern.Pattern;
						existingPattern.Description = pattern.Description;
						existingPattern.SegmentationHint = pattern.SegmentationHint;
					}
				}
			}

			UpdatePlaceholderPatternOrder(patterns);
			PlaceholderPatterns = new ObservableCollection<PlaceholderPattern>(patterns.OrderBy(a => a.Order));

			Settings.PlaceablePatterns = PlaceholderPatterns.ToList();
		}

		private void ExportPlaceholderPatterns(object paramter)
		{
			ExportPlaceholderPatterns();
		}

		private void ImportPlaceholderPatterns(object paramter)
		{
			ImportPlaceholderPatterns();
		}

		private void OpenAppendPlaceholderPattern(PlaceholderPattern placeholderPattern, bool isEditMode)
		{
			var viewModel = new AppendPlaceablePatternViewModel(placeholderPattern,
				_placeholderPatterns.ToList(), isEditMode);

			var window = new AppendPlaceablePatternWindow(viewModel, null);

			var result = window.ShowDialog();
			if (result != null && (bool)result)
			{
				if (isEditMode)
				{
					var pattern = _placeholderPatterns.FirstOrDefault(a => a.Pattern == placeholderPattern.Pattern);
					if (pattern != null)
					{
						pattern.Pattern = viewModel.Pattern;
						pattern.Description = viewModel.Description;
						pattern.SegmentationHint = viewModel.SegmentationHintItem.Item;
					}
				}
				else
				{
					_placeholderPatterns.Add(new PlaceholderPattern
					{
						Pattern = viewModel.Pattern,
						Description = viewModel.Description,
						SegmentationHint = viewModel.SegmentationHintItem.Item
					});
				}

				UpdatePlaceholderPatternOrder(_placeholderPatterns);
				PlaceholderPatterns = new ObservableCollection<PlaceholderPattern>(PlaceholderPatterns);

				OnPropertyChanged(nameof(SelectedPlaceholderPattern));
				OnPropertyChanged(nameof(PlaceholderPatternsStatusLabel));

				Settings.PlaceablePatterns = PlaceholderPatterns.ToList();
			}
		}

		private void UpdateCheckedAllState()
		{
			if (PlaceholderPatterns.Count > 0)
			{
				SelectAll = PlaceholderPatterns.Count(a => !a.Selected) <= 0;
			}
			else
			{
				SelectAll = false;
			}
		}

		private static void UpdatePlaceholderPatternOrder(IReadOnlyCollection<PlaceholderPattern> patterns)
		{
			var orders = new List<int>();
			foreach (var pattern in patterns.OrderBy(a => a.Order))
			{
				if (!orders.Contains(pattern.Order))
				{
					orders.Add(pattern.Order);
				}
				else
				{
					var i = 0;
					while (orders.Contains(i))
					{
						i++;
					}

					pattern.Order = i;
					orders.Add(i);
				}
			}

			var orderIndex = 0;
			foreach (var pattern in patterns.OrderBy(a => a.Order))
			{
				pattern.Order = orderIndex;
				orderIndex++;
			}
		}

		private void MovePlaceholderPatternUp(object paramter)
		{
			var selectedRule = SelectedPlaceholderPattern;
			var selectedIndex = PlaceholderPatterns.IndexOf(SelectedPlaceholderPattern);

			if (selectedIndex > 0)
			{
				var previousRule = PlaceholderPatterns[selectedIndex - 1];
				previousRule.Order = selectedIndex;

				selectedRule.Order = selectedIndex - 1;

				_placeholderPatterns.Move(selectedIndex, selectedIndex - 1);
				OnPropertyChanged(nameof(PlaceholderPatterns));

				if (paramter is SortAwareDataGrid dataGrid)
				{
					dataGrid.Focus();
					dataGrid.ForceCursor = true;

					dataGrid.UpdateLayout();

					dataGrid.SelectedIndex = selectedRule.Order;
					dataGrid.ScrollIntoView(dataGrid.Items[selectedRule.Order]);

					dataGrid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

					if (dataGrid.ItemContainerGenerator.ContainerFromIndex(selectedRule.Order) is DataGridRow row)
					{
						row.IsSelected = true;
						dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);

						row.Focus();
						row.ForceCursor = true;
						Keyboard.Focus(row);
					}
				}
			}
		}

		private void MovePlaceholderPatternDown(object paramter)
		{
			var selectedRule = SelectedPlaceholderPattern;
			var selectedIndex = PlaceholderPatterns.IndexOf(SelectedPlaceholderPattern);

			if (selectedIndex < PlaceholderPatterns.Count - 1)
			{
				var nextRule = PlaceholderPatterns[selectedIndex + 1];
				nextRule.Order = selectedIndex;

				selectedRule.Order = selectedIndex + 1;

				_placeholderPatterns.Move(selectedIndex, selectedIndex + 1);
				OnPropertyChanged(nameof(PlaceholderPatterns));

				if (paramter is SortAwareDataGrid dataGrid)
				{
					dataGrid.Focus();
					dataGrid.ForceCursor = true;

					dataGrid.UpdateLayout();

					dataGrid.SelectedIndex = selectedRule.Order;
					dataGrid.ScrollIntoView(dataGrid.Items[selectedRule.Order]);

					dataGrid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

					if (dataGrid.ItemContainerGenerator.ContainerFromIndex(selectedRule.Order) is DataGridRow row)
					{
						row.IsSelected = true;
						dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);

						row.Focus();
						row.ForceCursor = true;
						Keyboard.Focus(row);
					}
				}
			}
		}

		private void SelectAllPatterns(object paramter)
		{
			var value = SelectAll;
			foreach (var rule in PlaceholderPatterns)
			{
				rule.Selected = value;
			}
		}

		private void AddPlaceholderPattern(object paramter)
		{
			OpenAppendPlaceholderPattern(new PlaceholderPattern(), false);
			if (paramter is DataGrid dataGrid)
			{
				dataGrid.ForceCursor = true;
				Keyboard.Focus(dataGrid);

				var selectedIndex = dataGrid.Items.Count - 1;

				dataGrid.UpdateLayout();

				if (selectedIndex > -1)
				{
					dataGrid.SelectedIndex = selectedIndex;
					dataGrid.ScrollIntoView(dataGrid.Items[selectedIndex]);

					if (dataGrid.ItemContainerGenerator.ContainerFromIndex(selectedIndex) is DataGridRow row)
					{
						row.IsSelected = true;
						row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
					}
				}
			}
		}

		private void EditPlaceholderPattern(object paramter)
		{
			if (SelectedPlaceholderPattern != null)
			{
				OpenAppendPlaceholderPattern(SelectedPlaceholderPattern, true);
				if (paramter is DataGrid dataGrid)
				{
					var selectedIndex = dataGrid.SelectedIndex;

					dataGrid.ForceCursor = true;
					Keyboard.Focus(dataGrid);

					dataGrid.UpdateLayout();

					if (selectedIndex > -1)
					{
						dataGrid.SelectedIndex = selectedIndex;
						dataGrid.ScrollIntoView(dataGrid.Items[selectedIndex]);

						if (dataGrid.ItemContainerGenerator.ContainerFromIndex(selectedIndex) is DataGridRow row)
						{
							row.IsSelected = true;
							row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
						}
					}
				}
			}
		}

		private void RemovePlaceholderPattern(object paramter)
		{
			var dataGrid = paramter as DataGrid;
			if (dataGrid == null)
			{
				return;
			}

			var selectedIndex = dataGrid.SelectedIndex;

			var selectedIds = _selectedPlaceholderPatterns.Cast<PlaceholderPattern>().ToList().Select(a => a.Pattern);
			foreach (var id in selectedIds)
			{
				var pattern = _placeholderPatterns.FirstOrDefault(a => a.Pattern == id);
				if (pattern != null)
				{
					pattern.PropertyChanged -= PatternOnPropertyChanged;
					_placeholderPatterns.Remove(pattern);
				}
			}

			UpdatePlaceholderPatternOrder(_placeholderPatterns);

			Settings.PlaceablePatterns = _placeholderPatterns?.ToList();

			OnPropertyChanged(nameof(PlaceholderPatterns));
			OnPropertyChanged(nameof(PlaceholderPatternsStatusLabel));


			dataGrid.ForceCursor = true;
			Keyboard.Focus(dataGrid);

			dataGrid.UpdateLayout();

			if (dataGrid.Items.Count > 0)
			{
				if (selectedIndex > 0)
				{
					while (selectedIndex > -1 && selectedIndex > dataGrid.Items.Count - 1)
					{
						selectedIndex--;
					}
				}

				dataGrid.SelectedIndex = selectedIndex;
				dataGrid.ScrollIntoView(dataGrid.Items[selectedIndex]);

				if (dataGrid.ItemContainerGenerator.ContainerFromIndex(selectedIndex) is DataGridRow row)
				{
					row.IsSelected = true;
					row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
				}
			}
		}

		private void DragDrop(object parameter)
		{
			if (!(parameter is System.Windows.DragEventArgs eventArgs))
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] files && files.Length > 0 && files.Length <= 2)
			{
				var filePaths = new List<string>();
				foreach (var fullPath in files)
				{
					var fileAttributes = File.GetAttributes(fullPath);
					if (!fileAttributes.HasFlag(FileAttributes.Directory))
					{
						if (fullPath.ToLower().EndsWith(".xlsx"))
						{
							filePaths.Add(fullPath);
						}
					}
				}

				if (filePaths.Count > 0)
				{
					ImportPlaceholderPatternsFromFiles(filePaths);
				}
			}
		}

		private void MouseDoubleClick(object parameter)
		{
			if (SelectedPlaceholderPattern != null)
			{
				OpenAppendPlaceholderPattern(SelectedPlaceholderPattern, true);
			}
		}

		public void Dispose()
		{
			if (_placeholderPatterns != null)
			{
				foreach (var pattern in _placeholderPatterns)
				{
					pattern.PropertyChanged -= PatternOnPropertyChanged;
				}
			}
		}
	}
}
