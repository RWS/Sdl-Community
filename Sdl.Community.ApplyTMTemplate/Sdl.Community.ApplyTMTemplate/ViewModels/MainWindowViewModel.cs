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
using Sdl.Community.ApplyTMTemplate.Commands;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class MainWindowViewModel : ModelBase
	{
		private readonly TemplateLoader _templateLoader;
		private bool _abbreviationsChecked;
		private ICommand _addFolderCommand;
		private ICommand _addTMsCommand;
		private ICommand _applyTemplateCommand;
		private ICommand _browseCommand;
		private ICommand _dragEnterCommand;
		private bool _ordinalFollowersChecked;
		private ICommand _removeTMsCommand;
		private string _resourceTemplatePath;
		private bool _segmentationRulesChecked;
		private bool _selectedProjectChecked;
		private ObservableCollection<TranslationMemory> _tmCollection;
		private string _tmPath;
		private bool _variablesChecked;

		public MainWindowViewModel()
		{
			_templateLoader = new TemplateLoader();
			_tmPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				@"Studio 2019\Translation Memories");

			_variablesChecked = true;
			_abbreviationsChecked = true;
			_ordinalFollowersChecked = true;
			_segmentationRulesChecked = true;

			_tmCollection = new ObservableCollection<TranslationMemory>();

			var tmTemplatesFolder = _templateLoader.GetTmTemplateFolderPath();
			if (Directory.Exists(tmTemplatesFolder))
			{
				ResourceTemplatePath = Directory.GetFiles(tmTemplatesFolder)[0];
			}
			else
			{
				ResourceTemplatePath = "";
			}
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

		public ICommand AddFolderCommand => _addFolderCommand ?? (_addFolderCommand = new CommandHandler(AddFolder, true));

		public ICommand AddTMCommand => _addTMsCommand ?? (_addTMsCommand = new CommandHandler(AddTMs, true));

		public bool AllTmsChecked
		{
			get => _selectedProjectChecked;
			set
			{
				ToggleCheckAllTms(value);

				_selectedProjectChecked = value;
				OnPropertyChanged();
			}
		}

		public ICommand ApplyTemplateCommand => _applyTemplateCommand ?? (_applyTemplateCommand = new CommandHandler(ApplyTmTemplate, true));

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new CommandHandler(Browse, true));

		public ICommand DragEnterCommand => _dragEnterCommand ??
											(_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

		public bool OrdinalFollowersChecked
		{
			get => _ordinalFollowersChecked;
			set
			{
				_ordinalFollowersChecked = value;
				OnPropertyChanged();
			}
		}

		public ICommand RemoveTMsCommand => _removeTMsCommand ?? (_removeTMsCommand = new CommandHandler(RemoveTMs, true));

		public string ResourceTemplatePath
		{
			get => _resourceTemplatePath;
			set
			{
				_resourceTemplatePath = value;
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

		public ObservableCollection<TranslationMemory> TmCollection
		{
			get => _tmCollection;
			set
			{
				_tmCollection = value;
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

		private void AddFolder()
		{
			var tmPath = "";
			var dlg = new FolderBrowserDialog
			{
				SelectedPath = _tmPath ?? Environment.CurrentDirectory,
				Description = "Select TMs' Folder"
			};

			var result = dlg.ShowDialog();

			if (result == DialogResult.OK)
			{
				tmPath = dlg.SelectedPath;
			}

			if (!string.IsNullOrEmpty(tmPath))
			{
				var files = Directory.GetFiles(tmPath);
				ValidateAndAddTms(files);
			}
		}

		private void AddLanguageResourceToBundle(LanguageResourceBundle langResBundle, XmlNode resource)
		{
			if (resource.Attributes["Type"].Value == "Variables" && VariablesChecked)
			{
				var vars = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

				langResBundle.Variables = new Wordlist();

				foreach (Match s in Regex.Matches(vars, @"([^\s]+)"))
				{
					langResBundle.Variables.Add(s.ToString());
				}

				return;
			}

			if (resource.Attributes["Type"].Value == "Abbreviations" && AbbreviationsChecked)
			{
				var abbrevs = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

				langResBundle.Abbreviations = new Wordlist();

				foreach (Match s in Regex.Matches(abbrevs, @"([^\s]+)"))
				{
					langResBundle.Abbreviations.Add(s.ToString());
				}

				return;
			}

			if (resource.Attributes["Type"].Value == "OrdinalFollowers" && OrdinalFollowersChecked)
			{
				var ordFollowers = Encoding.UTF8.GetString(Convert.FromBase64String(resource.InnerText));

				langResBundle.OrdinalFollowers = new Wordlist();

				foreach (Match s in Regex.Matches(ordFollowers, @"([^\s]+)"))
				{
					langResBundle.OrdinalFollowers.Add(s.ToString());
				}

				return;
			}

			if (resource.Attributes["Type"].Value == "SegmentationRules" && SegmentationRulesChecked)
			{
				var segRules = Convert.FromBase64String(resource.InnerText);

				var stream = new MemoryStream(segRules);

				var segmentRules = SegmentationRules.Load(stream,
					CultureInfo.GetCultureInfo(langResBundle.Language.LCID), null);

				langResBundle.SegmentationRules = segmentRules;
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
			ValidateAndAddTms(dlg.FileNames);
		}

		private void ApplyTmTemplate()
		{
			if (string.IsNullOrEmpty(ResourceTemplatePath))
			{
				MessageBox.Show("Please select a Resource Template", "Resource Template", MessageBoxButtons.OK);
				return;
			}

			var lrt = _templateLoader.LoadDataFromFile(ResourceTemplatePath, "LanguageResource");

			var langResBundlesList = new List<LanguageResourceBundle>();
			var defaultLangResProvider = new DefaultLanguageResourceProvider();

			foreach (XmlNode res in lrt)
			{
				var lr = langResBundlesList.FirstOrDefault(lrb => lrb.Language.LCID == Int32.Parse(res.Attributes["Lcid"].Value));

				if (lr == null)
				{
					lr = defaultLangResProvider.GetDefaultLanguageResources(CultureInfo.GetCultureInfo(Int32.Parse(res.Attributes["Lcid"].Value)));
					langResBundlesList.Add(lr);
				}

				AddLanguageResourceToBundle(lr, res);
			}

			var selectedTmList = TmCollection.Where(tm => tm.IsSelected).ToList();

			foreach (var languageResourceBundle in langResBundlesList)
			{
				foreach (var translationMemory in selectedTmList)
				{
					if (translationMemory.Tm.LanguageResourceBundles.FirstOrDefault(lrb => lrb.Language.Equals(languageResourceBundle.Language)) != null)
					{
						translationMemory.ToggleCheckedUnchecked(true);
						translationMemory.Tm.LanguageResourceBundles.Add(languageResourceBundle);
						translationMemory.Tm.Save();
					}
					else
					{
						translationMemory.ToggleCheckedUnchecked(false);
					}
				}
			}
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

			ValidateAndAddTms(droppedFile as string[]);
		}
		private void RemoveTMs()
		{
			TmCollection = new ObservableCollection<TranslationMemory>(TmCollection.Where(tm => !tm.IsSelected));
		}

		private void ToggleCheckAllTms(bool onOff)
		{
			foreach (var translationMemory in TmCollection)
			{
				translationMemory.IsSelected = onOff;
			}
		}

		private void ValidateAndAddTms(string[] files)
		{
			foreach (var file in files)
			{
				var fileBasedTM = new FileBasedTranslationMemory(file);
				if (Path.GetExtension(file) == ".sdltm" && TmCollection.All(tm => tm.Name != fileBasedTM.Name))
				{
					TmCollection.Add(new TranslationMemory(fileBasedTM));
				}
			}
		}
	}
}