using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Multilingual.XML.FileType.Commands;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.FileType.Views;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services;
using Rws.MultiSelectComboBox.API;
using DataFormats = System.Windows.Forms.DataFormats;
using DataGrid = System.Windows.Controls.DataGrid;
using DragEventArgs = System.Windows.Forms.DragEventArgs;

namespace Multilingual.XML.FileType.FileType.ViewModels
{
	public sealed class LanguageMappingViewModel : BaseModel
	{
		private readonly ImageService _imageService;
		private string _languagesXPath;
		private bool _monoLanguage;
		private bool _monoLanguageIsEnabled;
		private bool _addLanguaeIsEnabled;

		private LanguageMapping _selectedLanguageMapping;

		private ObservableCollection<LanguageMapping> _languageMappings;
		private IList _selectedLanguageMappings;

		private ICommand _addLanguageCommand;
		private ICommand _editLanguageCommand;
		private ICommand _removeLanguageCommand;
		private ICommand _dragDropCommand;
		private ICommand _mouseDoubleClick;

		public LanguageMappingViewModel(LanguageMappingSettings settings, ImageService imageService, IFilterService filterService)
		{
			Settings = settings;
			_imageService = imageService;
			FilterService = filterService;

			MonoLanguage = Settings.LanguageMappingMonoLanguage;
			LanguagesXPath = Settings.LanguageMappingLanguagesXPath;
			LanguageMappings = new ObservableCollection<LanguageMapping>(Settings.LanguageMappingLanguages);

			CheckAddLanguageIsEnabled();
			CheckAddLanguageIsEnabled();
		}

		public ICommand AddLanguageCommand => _addLanguageCommand ?? (_addLanguageCommand = new CommandHandler(AddLanguage));

		public ICommand EditLanguageCommand => _editLanguageCommand ?? (_editLanguageCommand = new CommandHandler(EditLanguage));

		public ICommand RemoveLanguageCommand => _removeLanguageCommand ?? (_removeLanguageCommand = new CommandHandler(RemoveLangauge));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

		public IFilterService FilterService { get; set; }

		public LanguageMappingSettings Settings { get; }

		public string LanguagesXPath
		{
			get => _languagesXPath;
			set
			{
				if (_languagesXPath == value)
				{
					return;
				}

				_languagesXPath = value;
				OnPropertyChanged(nameof(LanguagesXPath));

				Settings.LanguageMappingLanguagesXPath = _languagesXPath;
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

				CheckMonoLanguageIsEnabled();
				CheckAddLanguageIsEnabled();
			}
		}

		public bool MonoLanguage
		{
			get => _monoLanguage;
			set
			{
				if (_monoLanguage == value)
				{
					return;
				}

				_monoLanguage = value;
				OnPropertyChanged(nameof(MonoLanguage));
				OnPropertyChanged(nameof(MonoLanguageIsEnabled));
				CheckAddLanguageIsEnabled();

				Settings.LanguageMappingMonoLanguage = _monoLanguage;
			}
		}

		public bool MonoLanguageIsEnabled
		{
			get => _monoLanguageIsEnabled;
			set
			{
				_monoLanguageIsEnabled = value;
				OnPropertyChanged(nameof(MonoLanguageIsEnabled));

				if (!_monoLanguageIsEnabled)
				{
					MonoLanguage = false;
				}

				CheckAddLanguageIsEnabled();
			}
		}

		private void CheckMonoLanguageIsEnabled()
		{
			MonoLanguageIsEnabled = LanguageMappings.Count <= 1;
		}

		private void CheckAddLanguageIsEnabled()
		{
			AddLanguaeIsEnabled = !MonoLanguage || _languageMappings?.Count <= 0;
		}

		public bool AddLanguaeIsEnabled
		{
			get => _addLanguaeIsEnabled;
			set
			{
				if (_addLanguaeIsEnabled == value)
				{
					return;
				}

				_addLanguaeIsEnabled = value;
				OnPropertyChanged(nameof(AddLanguaeIsEnabled));
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

			LanguagesXPath = Settings.LanguageMappingLanguagesXPath;
			LanguageMappings = new ObservableCollection<LanguageMapping>(Settings.LanguageMappingLanguages);

			MonoLanguage = Settings.LanguageMappingMonoLanguage;

			return Settings;
		}

		private void OpenAppendLangauge(LanguageMapping languageMapping, bool isEditMode)
		{
			var viewModel = new AppendLanguageViewModel(languageMapping,
				_languageMappings.ToList(), _imageService, FilterService, isEditMode);

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
							mapping.XPath = viewModel.XPath;
							//mapping.CommentXPath = viewModel.CommentXPath;
							mapping.LanguageId = cultureInfo.Name;
							mapping.DisplayName = cultureInfo.DisplayName;
							mapping.Image = _imageService.GetImage(cultureInfo.Name);
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
							XPath = viewModel.XPath,
							//CommentXPath = viewModel.CommentXPath,
							LanguageId = cultureInfo.Name,
							DisplayName = cultureInfo.DisplayName,
							Image = _imageService.GetImage(cultureInfo.Name)
						});
					}
				}

				LanguageMappings = new ObservableCollection<LanguageMapping>(LanguageMappings);

				OnPropertyChanged(nameof(SelectedLanguageMapping));
				OnPropertyChanged(nameof(LanguageMappingsStatusLabel));
				CheckMonoLanguageIsEnabled();

				Settings.LanguageMappingLanguages = LanguageMappings.ToList();
			}
		}

		private void AddLanguage(object paramter)
		{
			if (!AddLanguaeIsEnabled)
			{
				return;
			}

			OpenAppendLangauge(new LanguageMapping(), false);
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

		private void EditLanguage(object paramter)
		{
			if (SelectedLanguageMapping != null)
			{
				OpenAppendLangauge(SelectedLanguageMapping, true);
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

		private void RemoveLangauge(object paramter)
		{
			var dataGrid = paramter as DataGrid;
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

			LanguageMappings = new ObservableCollection<LanguageMapping>(LanguageMappings);

			OnPropertyChanged(nameof(LanguageMappings));
			OnPropertyChanged(nameof(SelectedLanguageMapping));
			OnPropertyChanged(nameof(LanguageMappingsStatusLabel));
			CheckMonoLanguageIsEnabled();

			Settings.LanguageMappingLanguages = LanguageMappings.ToList();


			CheckMonoLanguageIsEnabled();
			CheckAddLanguageIsEnabled();

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
							xsltFilePaths.Add(fullPath);
						}
					}
				}

				if (xsltFilePaths.Count > 0)
				{
					//TODO
					//_window.Dispatcher.Invoke(DispatcherPriority.Input, new System.Action(delegate
					//{
					//	OpenAppendLangauge(new LanguageMapping
					//	{
					//		Group = string.Empty,
					//		Language = string.Empty,
					//		Scope = ReportTemplate.TemplateScope.All,
					//		PlaceableRuleText = xsltFilePaths[0]
					//	}, false);
					//}));
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
	}
}
