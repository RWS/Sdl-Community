using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.ApplyTMTemplate.Commands;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MahApps.Metro.Controls.Dialogs;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class MainWindowViewModel : ModelBase
	{
		private readonly TemplateLoader _templateLoader;
		private readonly TMLoader _tmLoader;
		private bool _abbreviationsChecked;
		private bool _ordinalFollowersChecked;
		private bool _segmentationRulesChecked;
		private bool _allTmsChecked;
		private bool _variablesChecked;

		private string _tmPath;
		private string _resourceTemplatePath;

		private ICommand _addFolderCommand;
		private ICommand _addTMsCommand;
		private ICommand _applyTemplateCommand;
		private ICommand _browseCommand;
		private ICommand _dragEnterCommand;
		private ICommand _removeTMsCommand;
		private readonly IDialogCoordinator _dialogCoordinator;

		private ObservableCollection<TranslationMemory> _tmCollection;

		public MainWindowViewModel(TemplateLoader templateLoader, TMLoader tmLoader, IDialogCoordinator dialogCoordinator)
		{
			_templateLoader = templateLoader;
			_tmLoader = tmLoader;
			_dialogCoordinator = dialogCoordinator;

			_tmPath = _tmPath == null ? _templateLoader.GetTmFolderPath() : Environment.CurrentDirectory;

			_variablesChecked = true;
			_abbreviationsChecked = true;
			_ordinalFollowersChecked = true;
			_segmentationRulesChecked = true;

			_tmCollection = new ObservableCollection<TranslationMemory>();

			var tmTemplatesFolder = _templateLoader.GetTmTemplateFolderPath();
			ResourceTemplatePath = Directory.Exists(tmTemplatesFolder) ? Directory.GetFiles(tmTemplatesFolder)[0] : "";
		}

		public bool AbbreviationsChecked
		{
			get => _abbreviationsChecked;
			set
			{
				_abbreviationsChecked = value;
				OnPropertyChanged();
			}
		}

		public bool OrdinalFollowersChecked
		{
			get => _ordinalFollowersChecked;
			set
			{
				_ordinalFollowersChecked = value;
				OnPropertyChanged();
			}
		}

		public bool SegmentationRulesChecked
		{
			get => _segmentationRulesChecked;
			set
			{
				_segmentationRulesChecked = value;
				OnPropertyChanged();
			}
		}

		public bool VariablesChecked
		{
			get => _variablesChecked;
			set
			{
				_variablesChecked = value;
				OnPropertyChanged();
			}
		}

		public string ResourceTemplatePath
		{
			get => _resourceTemplatePath;
			set
			{
				_resourceTemplatePath = value;
				OnPropertyChanged();
			}
		}

		public bool AllTmsChecked
		{
			get => _allTmsChecked;
			set
			{
				ToggleCheckAllTms(value);

				_allTmsChecked = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<TranslationMemory> TmCollection
		{
			get => _tmCollection;
			set
			{
				_tmCollection = value;
				OnPropertyChanged();
			}
		}

		private void Tm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (AreAllTmsSelectedOrUnselected())
			{
				AllTmsChecked = TmCollection[0].IsSelected;
			}
		}

		private bool AreAllTmsSelectedOrUnselected()
		{
			for (var i = 0; i < TmCollection.Count - 1; i++)
			{
				if (TmCollection[i].IsSelected != TmCollection[i + 1].IsSelected) return false;
			}

			return true;
		}

		public ICommand AddFolderCommand => _addFolderCommand ?? (_addFolderCommand = new CommandHandler(AddFolder, true));

		public ICommand AddTMCommand => _addTMsCommand ?? (_addTMsCommand = new CommandHandler(AddTMs, true));

		public ICommand ApplyTemplateCommand => _applyTemplateCommand ?? (_applyTemplateCommand = new CommandHandler(ApplyTmTemplate, true));

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new CommandHandler(Browse, true));

		public ICommand DragEnterCommand => _dragEnterCommand ??
											(_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

		public ICommand RemoveTMsCommand => _removeTMsCommand ?? (_removeTMsCommand = new CommandHandler(RemoveTMs, true));

		private void AddFolder()
		{
			var dlg = new FolderSelectDialog
			{
				Title = "Please select the folder containing the TMs",
				InitialDirectory = _tmPath ?? Environment.CurrentDirectory
			};

			if (!dlg.ShowDialog()) return;

			_tmPath = dlg.FileName;

			if (string.IsNullOrEmpty(_tmPath)) return;

			var files = Directory.GetFiles(_tmPath);

			AddRange(_tmLoader.GetTms(files, TmCollection));
		}

		private void AddRange(ObservableCollection<TranslationMemory> tmsCollection)
		{
			foreach (var tm in tmsCollection)
			{
				TmCollection.Add(tm);
				tm.PropertyChanged += Tm_PropertyChanged;
			}
		}

		private void AddTMs()
		{
			var dlg = new OpenFileDialog()
			{
				Filter = "Translation Memories|*.sdltm|All Files|*.*",
				InitialDirectory = string.IsNullOrEmpty(_tmPath) ? Environment.CurrentDirectory : _tmPath,
				Multiselect = true
			};

			dlg.ShowDialog();
			AddRange(_tmLoader.GetTms(dlg.FileNames, TmCollection));
		}

		private async void ApplyTmTemplate()
		{
			var langResBundlesList = _templateLoader.GetLanguageResourceBundlesFromFile(ResourceTemplatePath, out var message);

			if (langResBundlesList == null || langResBundlesList.Count == 0)
			{
				await _dialogCoordinator.ShowMessageAsync(this, "Warning", message);
				return;
			}

			var selectedTmList = TmCollection.Where(tm => tm.IsSelected).ToList();

			if (selectedTmList.Count == 0)
			{
				await _dialogCoordinator.ShowMessageAsync(this, "Warning", "Select at least one TM");
				return;
			}

			var template = new Template(langResBundlesList);

			var options = new Options
			{
				AbbreviationsChecked = AbbreviationsChecked,
				VariablesChecked = VariablesChecked,
				OrdinalFollowersChecked = OrdinalFollowersChecked,
				SegmentationRulesChecked = SegmentationRulesChecked
			};

			template.ApplyTmTemplate(selectedTmList, options);
		}

		private void Browse()
		{
			var dlg = new OpenFileDialog
			{
				Filter = "Language resource templates|*.resource|All Files|*.*",
				InitialDirectory = ResourceTemplatePath.Substring(0, ResourceTemplatePath.LastIndexOf('\\') + 1)
			};

			var result = dlg.ShowDialog();

			if (result == true)
			{
				ResourceTemplatePath = dlg.FileName;
			}
		}

		private void HandlePreviewDrop(object droppedFile)
		{
			if (droppedFile == null) return;

			AddRange(_tmLoader.GetTms(droppedFile as string[], TmCollection));
		}

		private void RemoveTMs()
		{
			TmCollection = new ObservableCollection<TranslationMemory>(TmCollection.Where(tm => !tm.IsSelected));

			if (TmCollection.Count == 0)
			{
				AllTmsChecked = false;
			}
		}

		private void ToggleCheckAllTms(bool onOff)
		{
			foreach (var translationMemory in TmCollection)
			{
				if (translationMemory.IsSelected != onOff)
				{
					translationMemory.IsSelected = onOff;
				}
			}
		}
	}
}