using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Multilingual.XML.FileType.Commands;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services;
using Rws.MultiSelectComboBox.API;
using Rws.MultiSelectComboBox.EventArgs;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.Core.Settings;

namespace Multilingual.XML.FileType.FileType.ViewModels
{
	public class AppendLanguageViewModel : INotifyPropertyChanged
	{
		private string _windowTitle;
		private ICommand _selectedLanguagesChangedCommand;
		private string _xPath;
		//private string _commentXPath;
		private bool _isEditMode;
		private readonly List<LanguageMapping> _languageMappings;
		private readonly ImageService _imageService;
		private readonly LanguageMapping _languageMapping;
		private readonly bool _processComments;
		private ISettingsBundle _settingsBundle;

		private LanguageMapping _previousLanguageMapping;

		private List<LanguageItem> _languageItems;
		private List<LanguageItem> _selectedLanguageItems;

		public AppendLanguageViewModel(LanguageMapping languageMapping,
			List<LanguageMapping> languageMappings, ImageService imageService,
			IFilterService filterService, bool isEditMode)
		{
			_languageMapping = languageMapping ?? new LanguageMapping();
			_languageMappings = languageMappings ?? new List<LanguageMapping>();

			_previousLanguageMapping = _languageMapping.Clone() as LanguageMapping;

			_imageService = imageService;
			IsEditMode = isEditMode;
			FilterService = filterService;

			XPath = languageMapping?.XPath;
			//CommentXPath = languageMapping?.CommentXPath;
			
			LanguageItems = LanguageRegistryApi.Instance.GetAllLanguages()
				.Select(language => new LanguageItem
				{
					Id = language.CultureInfo.Name,
					Name = language.DisplayName,
					CultureInfo = language.CultureInfo,
					Image = _imageService.GetImage(language.CultureInfo.Name)
				})
				.OrderBy(a => a.Name).ToList();

			SelectedLanguageItems = new List<LanguageItem> {
				LanguageItems.FirstOrDefault(a=>
					string.Compare(a.CultureInfo.Name, languageMapping?.LanguageId, StringComparison.CurrentCultureIgnoreCase)==0) };

			WindowTitle = IsEditMode ? PluginResources.WindowTitle_Edit_Language : PluginResources.WindowTitle_Add_Language;
		}

		public ICommand SelectedLanguagesChangedCommand => _selectedLanguagesChangedCommand ?? (_selectedLanguagesChangedCommand = new CommandHandler(SelectedLanguagesChanged));

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public IFilterService FilterService { get; set; }

		public bool IsEditMode
		{
			get => _isEditMode;
			set
			{
				if (_isEditMode == value)
				{
					return;
				}

				_isEditMode = value;
				OnPropertyChanged(nameof(IsEditMode));
			}
		}


		public List<LanguageItem> LanguageItems
		{
			get => _languageItems;
			set
			{
				_languageItems = value;
				OnPropertyChanged(nameof(LanguageItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<LanguageItem> SelectedLanguageItems
		{
			get => _selectedLanguageItems;
			set
			{
				_selectedLanguageItems = value;
				OnPropertyChanged(nameof(SelectedLanguageItems));
				OnPropertyChanged(nameof(IsValid));

				UpdateLanguage();
			}
		}

		private void UpdateLanguage()
		{
			var selectedLanguage = _selectedLanguageItems?.FirstOrDefault();
			var selectedLanguageId = selectedLanguage?.CultureInfo.Name ?? string.Empty;
			var selectedLanguageDisplayName = selectedLanguage?.CultureInfo.DisplayName ?? string.Empty;
			var selectedLanguageImage = _imageService.GetImage(selectedLanguageId);

			if (string.Compare(selectedLanguageId, _previousLanguageMapping.LanguageId,
				StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				if (string.IsNullOrEmpty(XPath))
				{
					XPath = selectedLanguageId;
				}
				else if (!string.IsNullOrEmpty(_previousLanguageMapping.LanguageId) && XPath.Contains(_previousLanguageMapping.LanguageId))
				{
					XPath = XPath.Replace(_previousLanguageMapping.LanguageId, selectedLanguageId);
				}
			}

			_previousLanguageMapping = new LanguageMapping
			{
				LanguageId = selectedLanguageId,
				XPath = XPath,
				//CommentXPath = CommentXPath,
				DisplayName = selectedLanguageDisplayName,
				Image = selectedLanguageImage
			};
		}


		public string XPath
		{
			get => _xPath;
			set
			{
				if (_xPath == value)
				{
					return;
				}

				_xPath = value;
				OnPropertyChanged(nameof(XPath));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		//public string CommentXPath
		//{
		//	get => _commentXPath;
		//	set
		//	{
		//		if (_commentXPath == value)
		//		{
		//			return;
		//		}

		//		_commentXPath = value;
		//		OnPropertyChanged(nameof(CommentXPath));
		//		OnPropertyChanged(nameof(IsValid));
		//	}
		//}

		public bool IsValid
		{
			get
			{
				var languageId = SelectedLanguageItems?.FirstOrDefault()?.CultureInfo?.Name;
				if (string.IsNullOrEmpty(languageId) || string.IsNullOrEmpty(XPath?.Trim('\\')))
				{
					return false;
				}

				if (IsEditMode)
				{

					if (_languageMapping.LanguageId != languageId)
					{
						if (_languageMappings.Exists(a => a.LanguageId == languageId))
						{
							return false;
						}
					}

					if (_languageMapping.XPath != XPath)
					{
						if (_languageMappings.Exists(a => a.XPath == XPath))
						{
							return false;
						}
					}
				}
				else
				{
					if (_languageMappings.Exists(a => a.LanguageId == languageId || a.XPath == XPath))
					{
						return false;
					}
				}

				return true;
			}
		}

		private void SelectedLanguagesChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{
				UpdateLanguage();
				OnPropertyChanged(nameof(SelectedLanguageItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
