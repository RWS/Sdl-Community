using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Multilingual.Excel.FileType.Commands;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.FileType.Views;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Services;
using Rws.MultiSelectComboBox.API;
using DataFormats = System.Windows.Forms.DataFormats;
using DataGrid = System.Windows.Controls.DataGrid;
using DragEventArgs = System.Windows.Forms.DragEventArgs;

namespace Multilingual.Excel.FileType.FileType.ViewModels
{
	public class LanguageMappingViewModel : BaseModel
	{
		private readonly ImageService _imageService;
		private readonly ColorService _colorService;
		private int _firstRowIndex;
		private bool _firstRowIsHeading;
		private bool _readAllWorkSheets;

		private LanguageMapping _selectedLanguageMapping;

		private ObservableCollection<LanguageMapping> _languageMappings;
		private IList _selectedLanguageMappings;

		private ICommand _addLanguageCommand;
		private ICommand _editLanguageCommand;
		private ICommand _removeLanguageCommand;
		private ICommand _setAsDefaultLanguageCommand;
		private ICommand _dragDropCommand;
		private ICommand _mouseDoubleClick;

		public LanguageMappingViewModel(LanguageMappingSettings settings, ImageService imageService, 
			ColorService colorService, IFilterService languageLanguageFilterService)
		{
			Settings = settings;
			_imageService = imageService;
			_colorService = colorService;
			LanguageFilterService = languageLanguageFilterService;

			FirstRowIndex = Settings.LanguageMappingFirstRowIndex;
			FirstRowIsHeading = Settings.LanguageMappingFirstRowIsHeading;
			ReadAllWorkSheets = Settings.LanguageMappingReadAllWorkSheets;

			LanguageMappings = new ObservableCollection<LanguageMapping>(Settings.LanguageMappingLanguages);
		}

		public ICommand AddLanguageCommand => _addLanguageCommand ?? (_addLanguageCommand = new CommandHandler(AddLanguage));

		public ICommand EditLanguageCommand => _editLanguageCommand ?? (_editLanguageCommand = new CommandHandler(EditLanguage));

		public ICommand RemoveLanguageCommand => _removeLanguageCommand ?? (_removeLanguageCommand = new CommandHandler(RemoveLangauge));

		public ICommand SetAsDefaultLanguageCommand => _setAsDefaultLanguageCommand ?? (_setAsDefaultLanguageCommand = new CommandHandler(SetAsDefaultLanguage));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

		public IFilterService LanguageFilterService { get; set; }

		public LanguageMappingSettings Settings { get; }

		public int FirstRowIndex
		{
			get => _firstRowIndex;
			set
			{
				if (_firstRowIndex == value)
				{
					return;
				}

				_firstRowIndex = value;
				OnPropertyChanged(nameof(FirstRowIndex));

				Settings.LanguageMappingFirstRowIndex = _firstRowIndex;
			}
		}

		public bool FirstRowIsHeading
		{
			get => _firstRowIsHeading;
			set
			{
				if (_firstRowIsHeading == value)
				{
					return;
				}

				_firstRowIsHeading = value;
				OnPropertyChanged(nameof(FirstRowIsHeading));

				Settings.LanguageMappingFirstRowIsHeading = _firstRowIsHeading;
			}
		}

		public bool ReadAllWorkSheets
		{
			get => _readAllWorkSheets;
			set
			{
				if (_readAllWorkSheets == value)
				{
					return;
				}

				_readAllWorkSheets = value;
				OnPropertyChanged(nameof(ReadAllWorkSheets));

				Settings.LanguageMappingReadAllWorkSheets = _readAllWorkSheets;
			}
		}

		public ObservableCollection<LanguageMapping> LanguageMappings
		{
			get => _languageMappings;
			set
			{
				_languageMappings = value;
				OnPropertyChanged(nameof(LanguageMappings));
				OnPropertyChanged(nameof(LanguageMappingsStatusLabel));
			}
		}

		public LanguageMapping SelectedLanguageMapping
		{
			get => _selectedLanguageMapping;
			set
			{
				_selectedLanguageMapping = value;
				OnPropertyChanged(nameof(SelectedLanguageMapping));

				OnPropertyChanged(nameof(IsLanguageMappingSelected));
				OnPropertyChanged(nameof(IsLanguageMappingsSelected));
				OnPropertyChanged(nameof(LanguageMappingsStatusLabel));
			}
		}

		public IList SelectedLanguageMappings
		{
			get => _selectedLanguageMappings;
			set
			{
				_selectedLanguageMappings = value;
				OnPropertyChanged(nameof(SelectedLanguageMappings));


				_selectedLanguageMapping = _selectedLanguageMappings?.Cast<LanguageMapping>().ToList().FirstOrDefault();

				OnPropertyChanged(nameof(IsLanguageMappingSelected));
				OnPropertyChanged(nameof(IsLanguageMappingsSelected));
				OnPropertyChanged(nameof(LanguageMappingsStatusLabel));
			}
		}

		public string LanguageMappingsStatusLabel
		{
			get
			{
				var message = string.Format("Languages: {0}, Selected: {1}",
					LanguageMappings?.Count ?? 0,
					SelectedLanguageMappings?.Count ?? 0);
				return message;
			}
		}

		public bool IsLanguageMappingsSelected => SelectedLanguageMappings?.Cast<LanguageMapping>().ToList().Count > 0;

		public bool IsLanguageMappingSelected => SelectedLanguageMappings?.Cast<LanguageMapping>().ToList().Count == 1;

		public LanguageMappingSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();

			FirstRowIndex = Settings.LanguageMappingFirstRowIndex;
			FirstRowIsHeading = Settings.LanguageMappingFirstRowIsHeading;
			ReadAllWorkSheets = Settings.LanguageMappingReadAllWorkSheets;
			LanguageMappings = new ObservableCollection<LanguageMapping>(Settings.LanguageMappingLanguages);

			return Settings;
		}

		private void OpenAppendLangauge(LanguageMapping languageMapping, bool isEditMode)
		{
			if (!isEditMode)
			{
				var defaultLanguageMapping = _languageMappings.FirstOrDefault(a => a.IsDefault);
				if (defaultLanguageMapping != null && defaultLanguageMapping != languageMapping)
				{
					languageMapping.ContextColumn = defaultLanguageMapping.ContextColumn;
					languageMapping.CommentColumn = defaultLanguageMapping.CommentColumn;
					languageMapping.CharacterLimitationColumn = defaultLanguageMapping.CharacterLimitationColumn;
					languageMapping.PixelLimitationColumn = defaultLanguageMapping.PixelLimitationColumn;
					languageMapping.PixelFontFamilyColumn = defaultLanguageMapping.PixelFontFamilyColumn;
					languageMapping.PixelFontSizeColumn = defaultLanguageMapping.PixelFontSizeColumn;
					languageMapping.PixelFontFamilyName = defaultLanguageMapping.PixelFontFamilyName;
					languageMapping.PixelFontSize = defaultLanguageMapping.PixelFontSize;
					languageMapping.FilterFillColor = defaultLanguageMapping.FilterFillColor;
					languageMapping.FilterFillColorChecked = defaultLanguageMapping.FilterFillColorChecked;
					languageMapping.FilterScope = defaultLanguageMapping.FilterScope;
				}
			}

			var languageMappingClone = languageMapping.Clone() as LanguageMapping;

			var viewModel = new AppendLanguageViewModel(languageMapping,
				_languageMappings.ToList(), _imageService, LanguageFilterService, isEditMode);

			var window = new AppendLanguageWindow(viewModel, null);

			var result = window.ShowDialog();
			if (result != null && (bool)result)
			{
				if (isEditMode)
				{
					var mapping = _languageMappings.FirstOrDefault(a => a.LanguageId == languageMapping.LanguageId);
					if (mapping != null)
					{
						var cultureInfo = viewModel.SelectedLanguageItems?.FirstOrDefault()?.CultureInfo;
						if (cultureInfo != null)
						{
							mapping.ContentColumn = viewModel.ContentColumn.ToUpper();
							mapping.ExcludeTranslations = viewModel.ExcludeTranslations;
							mapping.ContextColumn = viewModel.ContextColumn?.ToUpper().Trim(';');
							mapping.CommentColumn = viewModel.CommentColumn?.ToUpper().Trim(';');
							mapping.CharacterLimitationColumn = viewModel.CharacterLimitationColumn?.ToUpper();
							mapping.PixelLimitationColumn = viewModel.PixelLimitationColumn?.ToUpper();
							mapping.PixelFontFamilyColumn = viewModel.PixelFontFamilyColumn?.ToUpper();
							mapping.PixelFontSizeColumn = viewModel.PixelFontSizeColumn?.ToUpper();
							mapping.PixelFontFamilyName = viewModel.SelectedFontFamilies?.FirstOrDefault()?.Name;
							mapping.PixelFontSize = viewModel.PixelFontSize;
							mapping.LanguageId = cultureInfo.Name;
							mapping.DisplayName = cultureInfo.DisplayName;
							mapping.Image = _imageService.GetImage(cultureInfo.Name);
							mapping.FilterFillColor = viewModel.SelectedFilterFillColors;
							mapping.FilterFillColorChecked = viewModel.FilterFillColorChecked;
							mapping.FilterScope = viewModel.SelectedFilterScope;
						}
					}
				}
				else
				{
					var cultureInfo = viewModel.SelectedLanguageItems?.FirstOrDefault()?.CultureInfo;
					if (cultureInfo != null)
					{
						_languageMappings.Add(new LanguageMapping
						{
							ContentColumn = viewModel.ContentColumn.ToUpper(),
							ExcludeTranslations = viewModel.ExcludeTranslations,
							ContextColumn = viewModel.ContextColumn?.ToUpper().Trim(';'),
							CommentColumn = viewModel.CommentColumn?.ToUpper().Trim(';'),
							CharacterLimitationColumn = viewModel.CharacterLimitationColumn?.ToUpper(),
							PixelLimitationColumn = viewModel.PixelLimitationColumn?.ToUpper(),
							PixelFontFamilyColumn = viewModel.PixelFontFamilyColumn?.ToUpper(),
							PixelFontSizeColumn = viewModel.PixelFontSizeColumn?.ToUpper(),
							PixelFontFamilyName = viewModel.SelectedFontFamilies?.FirstOrDefault()?.Name,
							PixelFontSize = viewModel.PixelFontSize,
							LanguageId = cultureInfo.Name,
							DisplayName = cultureInfo.DisplayName,
							Image = _imageService.GetImage(cultureInfo.Name),
							FilterFillColor = viewModel.SelectedFilterFillColors,
							FilterFillColorChecked = viewModel.FilterFillColorChecked,
							FilterScope = viewModel.SelectedFilterScope
					});
					}
				}


				EnsureDefaultLanguageMappingIsSet();
				RefreshLanguageMappings();
			}
			else
			{
				// undo any temporary changes to the referenced instance from the edit window
				languageMapping.ContentColumn = languageMappingClone.ContentColumn;
				languageMapping.ExcludeTranslations = languageMappingClone.ExcludeTranslations;
				languageMapping.ContextColumn = languageMappingClone.ContextColumn;
				languageMapping.CommentColumn = languageMappingClone.CommentColumn;
				languageMapping.CharacterLimitationColumn = languageMappingClone.CharacterLimitationColumn;
				languageMapping.PixelLimitationColumn = languageMappingClone.PixelLimitationColumn;
				languageMapping.PixelFontFamilyColumn = languageMappingClone.PixelFontFamilyColumn;
				languageMapping.PixelFontSizeColumn = languageMappingClone.PixelFontSizeColumn;
				languageMapping.PixelFontFamilyName = languageMappingClone.PixelFontFamilyName;
				languageMapping.PixelFontSize = languageMappingClone.PixelFontSize;
				languageMapping.LanguageId = languageMappingClone.LanguageId;
				languageMapping.DisplayName = languageMappingClone.DisplayName;
				languageMapping.Image = languageMappingClone.Image;
				languageMapping.FilterFillColor = languageMappingClone.FilterFillColor;
				languageMapping.FilterFillColorChecked = languageMappingClone.FilterFillColorChecked;
				languageMapping.FilterScope = languageMappingClone.FilterScope;
			}
		}

		private void RefreshLanguageMappings()
		{
			LanguageMappings = new ObservableCollection<LanguageMapping>(LanguageMappings);

			OnPropertyChanged(nameof(SelectedLanguageMapping));
			OnPropertyChanged(nameof(LanguageMappingsStatusLabel));

			Settings.LanguageMappingLanguages = LanguageMappings.ToList();
		}

		private void EnsureDefaultLanguageMappingIsSet()
		{
			if (_languageMappings.Count > 0)
			{
				// ensure one of the language mappings has a default marker
				var defaultLanguageMapping = _languageMappings.FirstOrDefault(a => a.IsDefault);
				if (defaultLanguageMapping == null)
				{
					LanguageMappings.FirstOrDefault().IsDefault = true;

					RefreshLanguageMappings();
				}
			}
		}

		private void AddLanguage(object parameter)
		{
			OpenAppendLangauge(new LanguageMapping(), false);
			if (parameter is DataGrid dataGrid)
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

		private void EditLanguage(object parameter)
		{
			if (SelectedLanguageMapping != null)
			{
				OpenAppendLangauge(SelectedLanguageMapping, true);
				if (parameter is DataGrid dataGrid)
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

		private void RemoveLangauge(object parameter)
		{
			var dataGrid = parameter as DataGrid;
			if (dataGrid == null)
			{
				return;
			}

			var selectedIndex = dataGrid.SelectedIndex;

			var selectedIds = _selectedLanguageMappings.Cast<LanguageMapping>().ToList().Select(a => a.LanguageId);
			foreach (var id in selectedIds)
			{
				var template = _languageMappings.FirstOrDefault(a => a.LanguageId == id);
				if (template != null)
				{
					_languageMappings.Remove(template);
				}
			}

			EnsureDefaultLanguageMappingIsSet();

			OnPropertyChanged(nameof(LanguageMappings));
			OnPropertyChanged(nameof(LanguageMappingsStatusLabel));


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

		private void SetAsDefaultLanguage(object parameter)
		{
			foreach (var languageMapping in _languageMappings)
			{
				languageMapping.IsDefault = languageMapping == SelectedLanguageMapping;
			}

			RefreshLanguageMappings();

			if (parameter is DataGrid dataGrid)
			{
				var selectedIndex = dataGrid.SelectedIndex;
				if (selectedIndex > -1)
				{
					dataGrid.SelectedIndex = selectedIndex;
					dataGrid.ScrollIntoView(dataGrid.Items[selectedIndex]);

					var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
					row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

					var newDataGridCellsPresenter = GetVisualChild<DataGridCellsPresenter>(row);
					if (newDataGridCellsPresenter != null)
					{
						if (newDataGridCellsPresenter.ItemContainerGenerator.ContainerFromIndex(0) is DataGridCell newDataGridCell)
						{
							newDataGridCell.Focus();
						}
					}
				}
			}
		}

		public static void Send(Key key)
		{
			if (Keyboard.PrimaryDevice != null)
			{
				if (Keyboard.PrimaryDevice.ActiveSource != null)
				{
					var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
					{
						RoutedEvent = Keyboard.PreviewKeyUpEvent
					};
					InputManager.Current.ProcessInput(e);
				}
			}
		}

		private void DragDrop(object parameter)
		{
			if (!(parameter is DragEventArgs eventArgs))
			{
				return;
			}

			var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
			if (fileDrop is string[] files && files.Length > 0 && files.Length <= 2)
			{
				var xsltFilePaths = new List<string>();
				foreach (var fullPath in files)
				{
					var fileAttributes = File.GetAttributes(fullPath);
					if (!fileAttributes.HasFlag(FileAttributes.Directory))
					{
						if (fullPath.ToLower().EndsWith(".xml"))
						{
							// Do something
						}
					}
				}
			}
		}

		private void MouseDoubleClick(object parameter)
		{
			if (SelectedLanguageMapping != null)
			{
				OpenAppendLangauge(SelectedLanguageMapping, true);
			}
		}

		static T GetVisualChild<T>(DependencyObject parent) where T : Visual
		{
			var child = default(T);
			var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
			for (var i = 0; i < numVisuals; i++)
			{
				var v = (Visual)VisualTreeHelper.GetChild(parent, i);
				child = v as T ?? GetVisualChild<T>(v);
				if (child != null)
				{
					break;
				}
			}
			return child;
		}
	}
}
