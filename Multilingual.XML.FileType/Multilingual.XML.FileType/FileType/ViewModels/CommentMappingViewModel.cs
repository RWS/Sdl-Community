using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Multilingual.XML.FileType.Commands;
using Multilingual.XML.FileType.Common;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.FileType.Views;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Providers;
using Rws.MultiSelectComboBox.API;
using DataFormats = System.Windows.Forms.DataFormats;
using DragEventArgs = System.Windows.Forms.DragEventArgs;

namespace Multilingual.XML.FileType.FileType.ViewModels
{
	public sealed class CommentMappingViewModel : BaseModel
	{
		private readonly StudioCommentPropertyProvider _studioCommentPropertyProvider;
		private string _commentElementName;
		private bool _processComments;

		private CommentPropertyMapping _selectedCommentPropertyMapping;

		private ObservableCollection<CommentPropertyMapping> _commentPropertyMappings;
		private IList _selectedCommentPropertyMappings;

		private ICommand _addLanguageCommand;
		private ICommand _editLanguageCommand;
		private ICommand _removeLanguageCommand;
		private ICommand _dragDropCommand;
		private ICommand _mouseDoubleClick;

		public CommentMappingViewModel(CommentMappingSettings settings, StudioCommentPropertyProvider studioCommentPropertyProvider)
		{
			Settings = settings;
			_studioCommentPropertyProvider = studioCommentPropertyProvider;

			ProcessComments = settings.CommentsProcess;
			CommentElementName = Settings.CommentElementName;
			CommentPropertyMappings = new ObservableCollection<CommentPropertyMapping>(Settings.CommentMappings);

			OnPropertyChanged(nameof(AddCommentPropertyIsEnabled));
		}

		public ICommand AddCommentPropertyCommand => _addLanguageCommand ?? (_addLanguageCommand = new CommandHandler(AddPropertyMapping));

		public ICommand EditCommentPropertyCommand => _editLanguageCommand ?? (_editLanguageCommand = new CommandHandler(EditPropertyMapping));

		public ICommand RemoveCommentPropertyCommand => _removeLanguageCommand ?? (_removeLanguageCommand = new CommandHandler(RemovePropertyMappings));

		public ICommand DragDropCommand => _dragDropCommand ?? (_dragDropCommand = new CommandHandler(DragDrop));

		public ICommand MouseDoubleClickCommand => _mouseDoubleClick ?? (_mouseDoubleClick = new CommandHandler(MouseDoubleClick));

		public IFilterService FilterService { get; set; }

		public CommentMappingSettings Settings { get; }

		public string CommentElementName
		{
			get => _commentElementName;
			set
			{
				if (_commentElementName == value)
				{
					return;
				}

				_commentElementName = value;
				OnPropertyChanged(nameof(CommentElementName));

				Settings.CommentElementName = _commentElementName;
			}
		}

		public bool ProcessComments
		{
			get => _processComments;
			set
			{
				if (_processComments == value)
				{
					return;
				}

				_processComments = value;
				OnPropertyChanged(nameof(ProcessComments));

				Settings.CommentsProcess = _processComments;
			}
		}

		public ObservableCollection<CommentPropertyMapping> CommentPropertyMappings
		{
			get => _commentPropertyMappings;
			set
			{
				_commentPropertyMappings = value;
				OnPropertyChanged(nameof(CommentPropertyMappings));
				OnPropertyChanged(nameof(CommentPropertyMappingsStatusLabel));
			}
		}

		public CommentPropertyMapping SelectedCommentPropertyMapping
		{
			get => _selectedCommentPropertyMapping;
			set
			{
				_selectedCommentPropertyMapping = value;
				OnPropertyChanged(nameof(SelectedCommentPropertyMapping));

				OnPropertyChanged(nameof(IsCommentPropertyMappingSelected));
				OnPropertyChanged(nameof(IsCommentPropertyMappingsSelected));
				OnPropertyChanged(nameof(CommentPropertyMappingsStatusLabel));
			}
		}

		public IList SelectedCommentMappings
		{
			get => _selectedCommentPropertyMappings;
			set
			{
				_selectedCommentPropertyMappings = value;
				OnPropertyChanged(nameof(SelectedCommentMappings));

				_selectedCommentPropertyMapping = _selectedCommentPropertyMappings?.Cast<CommentPropertyMapping>().ToList().FirstOrDefault();

				OnPropertyChanged(nameof(IsCommentPropertyMappingSelected));
				OnPropertyChanged(nameof(IsCommentPropertyMappingsSelected));
				OnPropertyChanged(nameof(CommentPropertyMappingsStatusLabel));
			}
		}

		public string CommentPropertyMappingsStatusLabel
		{
			get
			{
				var message = string.Format("Attributes: {0}, Selected: {1}",
					CommentPropertyMappings?.Count ?? 0,
					SelectedCommentMappings?.Count ?? 0);
				return message;
			}
		}

		public bool IsCommentPropertyMappingsSelected => SelectedCommentMappings?.Cast<CommentPropertyMapping>().ToList().Count > 0;

		public bool IsCommentPropertyMappingSelected => SelectedCommentMappings?.Cast<CommentPropertyMapping>().ToList().Count == 1;

		public bool AddCommentPropertyIsEnabled => _commentPropertyMappings.Count <
		                                           _studioCommentPropertyProvider.DefaultCommentProperties.Count;

		public CommentMappingSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();

			CommentElementName = Settings.CommentElementName;
			CommentPropertyMappings = new ObservableCollection<CommentPropertyMapping>(Settings.CommentMappings);

			return Settings;
		}

		private void OpenAppendLangauge(CommentPropertyMapping commentPropertyMapping, bool isEditMode)
		{
			var viewModel = new AppendCommentMappingViewModel(commentPropertyMapping,
				_commentPropertyMappings.ToList(), _studioCommentPropertyProvider, FilterService, isEditMode);

			var window = new AppendCommentMappingWindow(viewModel, null);

			var result = window.ShowDialog();
			if (result != null && (bool)result)
			{
				if (isEditMode)
				{
					var mapping = _commentPropertyMappings.FirstOrDefault(a => a.StudioPropertyName == commentPropertyMapping.StudioPropertyName);
					if (mapping != null)
					{
						if (!string.IsNullOrEmpty(viewModel.SelectedCommentPropertyType))
						{
							mapping.PropertyName = viewModel.CommentPropertyName;
							mapping.PropertyType = Enum.Parse(typeof(Enumerators.CommentPropertyType), viewModel.SelectedCommentPropertyType, true).ToString();
							mapping.StudioPropertyName = viewModel.SelectedStudioCommentProperty.Name;
						}
					}
				}
				else
				{
					_commentPropertyMappings.Add(new CommentPropertyMapping
					{
						PropertyName = viewModel.CommentPropertyName,
						PropertyType = Enum.Parse(typeof(Enumerators.CommentPropertyType), viewModel.SelectedCommentPropertyType, true).ToString(),
						StudioPropertyName = viewModel.SelectedStudioCommentProperty.Name
					});
				}

				CommentPropertyMappings = new ObservableCollection<CommentPropertyMapping>(CommentPropertyMappings);

				OnPropertyChanged(nameof(SelectedCommentPropertyMapping));
				OnPropertyChanged(nameof(CommentPropertyMappingsStatusLabel));
				OnPropertyChanged(nameof(AddCommentPropertyIsEnabled));

				Settings.CommentMappings = CommentPropertyMappings.ToList();
			}
		}

		private void AddPropertyMapping(object paramter)
		{
			if (!AddCommentPropertyIsEnabled)
			{
				return;
			}

			OpenAppendLangauge(new CommentPropertyMapping(), false);
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

		private void EditPropertyMapping(object paramter)
		{
			if (SelectedCommentPropertyMapping != null)
			{
				OpenAppendLangauge(SelectedCommentPropertyMapping, true);
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

		private void RemovePropertyMappings(object paramter)
		{
			var dataGrid = paramter as DataGrid;
			if (dataGrid == null)
			{
				return;
			}

			var selectedIndex = dataGrid.SelectedIndex;

			var selectedIds = _selectedCommentPropertyMappings.Cast<CommentPropertyMapping>().ToList().Select(a => a.StudioPropertyName);
			foreach (var id in selectedIds)
			{
				var template = _commentPropertyMappings.FirstOrDefault(a => a.StudioPropertyName == id);
				if (template != null)
				{
					_commentPropertyMappings.Remove(template);
				}
			}

			Settings.CommentMappings = CommentPropertyMappings.ToList();

			OnPropertyChanged(nameof(CommentPropertyMappings));
			OnPropertyChanged(nameof(CommentPropertyMappingsStatusLabel));
			OnPropertyChanged(nameof(AddCommentPropertyIsEnabled));

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
			if (SelectedCommentPropertyMapping != null)
			{
				OpenAppendLangauge(SelectedCommentPropertyMapping, true);
			}
		}
	}
}
