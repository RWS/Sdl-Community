using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Multilingual.Excel.FileType.Commands;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Services;
using Rws.MultiSelectComboBox.API;
using Rws.MultiSelectComboBox.EventArgs;
using Sdl.Core.Globalization.LanguageRegistry;
using static Multilingual.Excel.FileType.Common.Enumerators;

namespace Multilingual.Excel.FileType.FileType.ViewModels
{
	public class AppendLanguageViewModel : INotifyPropertyChanged, IDataErrorInfo
	{
		private string _windowTitle;
		private ICommand _selectedLanguagesChangedCommand;
		private ICommand _selectedFontsChangedCommand;
		private ICommand _getDefaultLanguageFontCommand;
		private string _contentColumn;
		public string _contextColumn;
		private string _commentColumn;
		private string _characterLimitationColumn;
		private string _lineLimitationColumn;
		private string _pixelLimitationColumn;
		private string _pixelFontFamilyColumn;
		private string _pixelFontSizeColumn;
		private float _pixelFontSize;
		private bool _isDefault;
		private bool _excludeTranslations;
		private bool _isEditMode;
		private string _selectedFilterFillColors;
		private bool _filterFillColorChecked;
		private string _filterScope;

		private List<string> _filterScopes;
		private readonly List<LanguageMapping> _languageMappings;
		private readonly ImageService _imageService;
		private readonly LanguageMapping _languageMapping;

		private List<LanguageItem> _languageItems;
		private List<LanguageItem> _selectedLanguageItems;


		private List<FontFamilyItem> _fontFamilies;
		private List<FontFamilyItem> _selectedFontFamilies;

		public AppendLanguageViewModel(LanguageMapping languageMapping,
			List<LanguageMapping> languageMappings, ImageService imageService,
			IFilterService languageLanguageFilterService,  bool isEditMode)
		{
			_languageMapping = languageMapping ?? new LanguageMapping();
			_languageMappings = languageMappings ?? new List<LanguageMapping>();
			
			_imageService = imageService;
			IsEditMode = isEditMode;
			LanguageFilterService = languageLanguageFilterService;

			ContentColumn = languageMapping?.ContentColumn;

			ExcludeTranslations = languageMapping?.ExcludeTranslations ?? false;
			ContextColumn = languageMapping?.ContextColumn;
			CommentColumn = languageMapping?.CommentColumn;
			CharacterLimitationColumn = languageMapping?.CharacterLimitationColumn;
			LineLimitationColumn = languageMapping?.LineLimitationColumn;
			PixelLimitationColumn = languageMapping?.PixelLimitationColumn;
			PixelFontFamilyColumn = languageMapping?.PixelFontFamilyColumn;
			PixelFontSizeColumn = languageMapping?.PixelFontSizeColumn;
			PixelFontSize = languageMapping?.PixelFontSize ?? 0;

			SelectedFilterFillColors = languageMapping?.FilterFillColor;
			FilterFillColorChecked = languageMapping?.FilterFillColorChecked ?? false;

			FilterScopes = Enum.GetNames(typeof(FilterScope)).ToList();
			SelectedFilterScope = string.IsNullOrEmpty(languageMapping?.FilterScope) ? FilterScope.Import.ToString() : languageMapping.FilterScope;

			IsDefault = languageMapping?.IsDefault ?? false;

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

			FontFamilies = GetInstalledFonts();
			SelectedFontFamilies = new List<FontFamilyItem> {
				FontFamilies.FirstOrDefault(a=>
					string.Compare(a.Name, languageMapping?.PixelFontFamilyName, StringComparison.CurrentCultureIgnoreCase)==0) };


			WindowTitle = IsEditMode ? PluginResources.WindowTitle_Edit_Language : PluginResources.WindowTitle_Add_Language;
		}

		private List<FontFamilyItem> GetInstalledFonts()
		{
			var fonts = new List<FontFamilyItem>();

			using (var fontsCollection = new InstalledFontCollection())
			{
				var fontFamilies = fontsCollection.Families;
				foreach (var font in fontFamilies)
				{
					var fontFamily = new FontFamilyItem
					{
						Name = font.Name,
						Id = font.Name
					};
					fonts.Add(fontFamily);
				}
			}

			return fonts;
		}

		public ICommand SelectedLanguagesChangedCommand => _selectedLanguagesChangedCommand ?? (_selectedLanguagesChangedCommand = new CommandHandler(SelectedLanguagesChanged));

		public ICommand SelectedFontsChangedCommand => _selectedFontsChangedCommand ?? (_selectedFontsChangedCommand = new CommandHandler(SelectedFontsChanged));

		public ICommand GetDefaultLanguageFontCommand => _getDefaultLanguageFontCommand ?? (_getDefaultLanguageFontCommand = new CommandHandler(GetDefaultLanguageFont));

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public IFilterService LanguageFilterService { get; set; }

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

		public string SelectedLanguageItemErrorToolTip { get; set; }

		public List<LanguageItem> SelectedLanguageItems
		{
			get => _selectedLanguageItems;
			set
			{
				_selectedLanguageItems = value;
				OnPropertyChanged(nameof(SelectedLanguageItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<FontFamilyItem> FontFamilies
		{
			get => _fontFamilies;
			set
			{
				_fontFamilies = value;
				OnPropertyChanged(nameof(FontFamilies));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<FontFamilyItem> SelectedFontFamilies
		{
			get => _selectedFontFamilies;
			set
			{
				_selectedFontFamilies = value;
				OnPropertyChanged(nameof(SelectedLanguageItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string ContentColumn
		{
			get => _contentColumn;
			set
			{
				if (_contentColumn == value)
				{
					return;
				}

				_contentColumn = NormalizeColumn(value);
				_languageMapping.ContentColumn = _contentColumn;

				OnPropertyChanged(nameof(ContentColumn));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string ContentColumnErrorToolTip { get; set; }

		public string ContextColumn
		{
			get => _contextColumn;
			set
			{
				if (_contextColumn == value)
				{
					return;
				}

				_contextColumn = NormalizeColumn(value);
				_languageMapping.ContextColumn = _contextColumn;

				OnPropertyChanged(nameof(ContextColumn));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string ContextColumnErrorToolTip { get; set; }

		public string CommentColumn
		{
			get => _commentColumn;
			set
			{
				if (_commentColumn == value)
				{
					return;
				}

				_commentColumn = NormalizeColumn(value);
				_languageMapping.CommentColumn = _commentColumn;

				OnPropertyChanged(nameof(CommentColumn));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string CommentColumnErrorToolTip { get; set; }

		public string CharacterLimitationColumn
		{
			get => _characterLimitationColumn;
			set
			{
				if (_characterLimitationColumn == value)
				{
					return;
				}

				_characterLimitationColumn = NormalizeColumn(value);
				_languageMapping.CharacterLimitationColumn = _characterLimitationColumn;

				OnPropertyChanged(nameof(CharacterLimitationColumn));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string CharacterLimitationColumnErrorToolTip { get; set; }

		public string LineLimitationColumn
		{
			get => _lineLimitationColumn;
			set
			{
				if (_lineLimitationColumn == value)
				{
					return;
				}

				_lineLimitationColumn = NormalizeColumn(value);
				_languageMapping.LineLimitationColumn = _lineLimitationColumn;

				OnPropertyChanged(nameof(LineLimitationColumn));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string LineLimitationColumnErrorToolTip { get; set; }

		public string PixelLimitationColumn
		{
			get => _pixelLimitationColumn;
			set
			{
				if (_pixelLimitationColumn == value)
				{
					return;
				}

				_pixelLimitationColumn = NormalizeColumn(value);
				_languageMapping.PixelLimitationColumn = _pixelLimitationColumn;

				OnPropertyChanged(nameof(PixelControlIsEnabled));
				OnPropertyChanged(nameof(PixelLimitationColumn));
				OnPropertyChanged(nameof(SelectedFontFamilies));
				OnPropertyChanged(nameof(PixelFontSize));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool PixelControlIsEnabled => !string.IsNullOrEmpty(PixelLimitationColumn);

		public float PixelFontSize
		{
			get => _pixelFontSize;
			set
			{
				if (_pixelFontSize == value)
				{
					return;
				}

				_pixelFontSize = value;
				OnPropertyChanged(nameof(PixelFontSize));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string PixelFontFamilyColumn
		{
			get => _pixelFontFamilyColumn;
			set
			{
				if (_pixelFontFamilyColumn == value)
				{
					return;
				}

				_pixelFontFamilyColumn = NormalizeColumn(value);
				_languageMapping.PixelFontFamilyColumn = _pixelFontFamilyColumn;


				OnPropertyChanged(nameof(PixelFontFamilyColumn));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string PixelFontSizeColumn
		{
			get => _pixelFontSizeColumn;
			set
			{
				if (_pixelFontSizeColumn == value)
				{
					return;
				}

				_pixelFontSizeColumn = NormalizeColumn(value);
				_languageMapping.PixelFontSizeColumn = _pixelFontSizeColumn;


				OnPropertyChanged(nameof(PixelFontSizeColumn));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string PixelLimitationColumnErrorToolTip { get; set; }

		public string PixelFontFamilyColumnErrorToolTip { get; set; }

		public string PixelFontSizeColumnErrorToolTip { get; set; }

		public string PixelFontFamilyErrorToolTip { get; set; }

		public string PixelSizeErrorToolTip { get; set; }

		public bool ExcludeTranslations
		{
			get => _excludeTranslations;
			set
			{
				if (_excludeTranslations == value)
				{
					return;
				}

				_excludeTranslations = value;
				OnPropertyChanged(nameof(ExcludeTranslations));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string SelectedFilterFillColors
		{
			get => _selectedFilterFillColors;
			set
			{
				if (Equals(_selectedFilterFillColors, value))
				{
					return;
				}

				_selectedFilterFillColors = value;
				OnPropertyChanged(nameof(SelectedFilterFillColors));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool FilterFillColorChecked
		{
			get => _filterFillColorChecked;
			set
			{
				if (_filterFillColorChecked == value)
				{
					return;
				}

				_filterFillColorChecked = value;
				OnPropertyChanged(nameof(FilterFillColorChecked));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<string> FilterScopes
		{
			get => _filterScopes;
			set
			{
				_filterScopes = value;
				OnPropertyChanged(nameof(FilterScopes));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string SelectedFilterScope
		{
			get => _filterScope;
			set
			{
				if (Equals(_filterScope, value))
				{
					return;
				}

				_filterScope = value;
				OnPropertyChanged(nameof(SelectedFilterScope));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool IsDefault
		{
			get => _isDefault;
			set
			{
				if (_isDefault == value)
				{
					return;
				}

				_isDefault = value;
				OnPropertyChanged(nameof(IsDefault));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool IsValid
		{
			get
			{
				var languageId = SelectedLanguageItems?.FirstOrDefault()?.CultureInfo?.Name;
				if (string.IsNullOrEmpty(languageId) || string.IsNullOrEmpty(ContentColumn?.Trim('\\')))
				{
					return false;
				}

				if (!Validate(nameof(SelectedLanguageItems))
					|| !Validate(nameof(ContentColumn))
					|| !Validate(nameof(ContextColumn))
					|| !Validate(nameof(CommentColumn))
					|| !Validate(nameof(CharacterLimitationColumn))
					|| !Validate(nameof(LineLimitationColumn))
					|| !Validate(nameof(PixelLimitationColumn))
					|| !Validate(nameof(PixelFontFamilyColumn))
					|| !Validate(nameof(PixelFontSizeColumn))
					|| !Validate(nameof(PixelFontSize))
					|| !Validate(nameof(SelectedFontFamilies)))
				{
					return false;
				}

				return true;
			}
		}

		private string NormalizeColumn(string column)
		{
			var text = column.ToUpper().Trim();
			text = text.Replace(";;", ";");
			return text;
		}

		private void GetDefaultLanguageFont(object paramter)
		{
			// TODO
		}

		private void SelectedLanguagesChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{
				OnPropertyChanged(nameof(SelectedLanguageItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		private void SelectedFontsChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{
				OnPropertyChanged(nameof(SelectedFontFamilies));
				OnPropertyChanged(nameof(IsValid));
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public string this[string columnName]
		{
			get
			{
				Validate(columnName, out var validationMessage);

				switch (columnName)
				{
					case nameof(SelectedLanguageItems):
						SelectedLanguageItemErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(ContentColumn):
						ContentColumnErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(ContextColumn):
						ContextColumnErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(CommentColumn):
						CommentColumnErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(CharacterLimitationColumn):
						CharacterLimitationColumnErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(LineLimitationColumn):
						LineLimitationColumnErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(PixelLimitationColumn):
						PixelLimitationColumnErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(PixelFontFamilyColumn):
						PixelFontFamilyColumnErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(PixelFontSizeColumn):
						PixelFontSizeColumnErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(PixelFontSize):
						PixelSizeErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
					case nameof(SelectedFontFamilies):
						PixelFontFamilyErrorToolTip = validationMessage == string.Empty ? null : validationMessage;
						break;
				}

				OnPropertyChanged(nameof(SelectedLanguageItemErrorToolTip));
				OnPropertyChanged(nameof(ContentColumnErrorToolTip));
				OnPropertyChanged(nameof(ContextColumnErrorToolTip));
				OnPropertyChanged(nameof(CommentColumnErrorToolTip));
				OnPropertyChanged(nameof(CharacterLimitationColumnErrorToolTip));
				OnPropertyChanged(nameof(LineLimitationColumnErrorToolTip));
				OnPropertyChanged(nameof(PixelLimitationColumnErrorToolTip));
				OnPropertyChanged(nameof(PixelFontFamilyColumnErrorToolTip));
				OnPropertyChanged(nameof(PixelFontSizeColumnErrorToolTip));
				OnPropertyChanged(nameof(PixelSizeErrorToolTip));
				OnPropertyChanged(nameof(PixelFontFamilyErrorToolTip));

				return validationMessage;
			}
		}

		private bool Validate(string propertyName)
		{
			return Validate(propertyName, out _);
		}

		private List<string> GetColumnNames(string columnName)
		{
			var columns = new List<string>();
			if (columnName == null)
			{
				return columns;
			}

			columns.AddRange(columnName.Split(';').Where(column => !string.IsNullOrEmpty(column)));

			return columns;
		}

		private bool Validate(string propertyName, out string validationMessage)
		{
			validationMessage = string.Empty;
			switch (propertyName)
			{
				case nameof(SelectedLanguageItems):

					var selectedLanguageItem = SelectedLanguageItems.FirstOrDefault();

					if (selectedLanguageItem != null && _languageMapping.LanguageId != selectedLanguageItem.CultureInfo.Name)
					{
						if (_languageMappings.Exists(a => a.LanguageId == selectedLanguageItem.CultureInfo.Name))
						{
							validationMessage = PluginResources.ValidationMessage_Language_Already_Defined;
						}
					}
					break;
				case nameof(ContentColumn):
					if (!string.IsNullOrEmpty(ContentColumn))
					{
						var mapping = _languageMappings.FirstOrDefault(a =>
							a.LanguageId != _languageMapping.LanguageId
							&& (a.ContentColumn == ContentColumn ||
								GetColumnNames(a.ContextColumn).Contains(ContentColumn) ||
								GetColumnNames(a.CommentColumn).Contains(ContentColumn) ||
								a.CharacterLimitationColumn == ContentColumn ||
								a.LineLimitationColumn == ContentColumn ||
								a.PixelLimitationColumn == ContentColumn ||
								a.PixelFontFamilyColumn == ContentColumn ||
								a.PixelFontSizeColumn == ContentColumn)
						);

						if (mapping != null)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Language, ContentColumn, mapping.LanguageId);
						}

						if (GetColumnNames(ContextColumn).Contains(ContentColumn) ||
							GetColumnNames(CommentColumn).Contains(ContentColumn) ||
							ContentColumn == CharacterLimitationColumn ||
							ContentColumn == LineLimitationColumn ||
							ContentColumn == PixelLimitationColumn ||
							ContentColumn == PixelFontFamilyColumn ||
							ContentColumn == PixelFontSizeColumn)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Current_Language, ContentColumn);
						}

						var invalidChars = GetInvalidChars(ContentColumn);
						if (!string.IsNullOrEmpty(invalidChars))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Found_Invalid_Characters, invalidChars);
						}
					}
					break;
				case nameof(ContextColumn):
					if (!string.IsNullOrEmpty(ContextColumn))
					{
						foreach (var contextColumn in GetColumnNames(ContextColumn))
						{
							var mapping = _languageMappings.FirstOrDefault(a =>
								a.ContentColumn == contextColumn ||
								GetColumnNames(a.CommentColumn).Contains(contextColumn) ||
								a.CharacterLimitationColumn == contextColumn ||
								a.LineLimitationColumn == contextColumn ||
								a.PixelLimitationColumn == contextColumn ||
								a.PixelFontFamilyColumn == contextColumn ||
								a.PixelFontSizeColumn == contextColumn);

							if (mapping != null)
							{
								validationMessage =
									string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Language, contextColumn, mapping.LanguageId);
							}

							if (contextColumn == ContentColumn ||
								GetColumnNames(CommentColumn).Contains(contextColumn) ||
								contextColumn == CharacterLimitationColumn ||
								contextColumn == LineLimitationColumn ||
								contextColumn == PixelLimitationColumn ||
								contextColumn == PixelFontFamilyColumn ||
								contextColumn == PixelFontSizeColumn)
							{
								validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Current_Language, contextColumn);
							}
						}

						if (HasDuplicateColumn(GetColumnNames(ContextColumn), out var duplicateColumn))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Already_Defined, duplicateColumn);
						}

						var invalidChars = GetInvalidChars(ContextColumn, new List<string> { ";", " " });
						if (!string.IsNullOrEmpty(invalidChars))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Found_Invalid_Characters, invalidChars);
						}
					}
					break;
				case nameof(CommentColumn):
					if (!string.IsNullOrEmpty(CommentColumn))
					{
						foreach (var commentColumn in GetColumnNames(CommentColumn))
						{
							var mapping = _languageMappings.FirstOrDefault(a =>
								a.ContentColumn == commentColumn ||
								GetColumnNames(a.ContextColumn).Contains(commentColumn) ||
								a.CharacterLimitationColumn == commentColumn ||
								a.LineLimitationColumn == commentColumn ||
								a.PixelLimitationColumn == commentColumn ||
								a.PixelFontFamilyColumn == commentColumn ||
								a.PixelFontSizeColumn == commentColumn);
							if (mapping != null)
							{
								validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Language, commentColumn, mapping.LanguageId);
							}

							if (commentColumn == ContentColumn ||
								GetColumnNames(ContextColumn).Contains(commentColumn) ||
								commentColumn == CharacterLimitationColumn ||
								commentColumn == LineLimitationColumn ||
								commentColumn == PixelLimitationColumn ||
								commentColumn == PixelFontFamilyColumn ||
								commentColumn == PixelFontSizeColumn)
							{
								validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Current_Language, commentColumn);
							}
						}

						if (HasDuplicateColumn(GetColumnNames(ContentColumn), out var duplicateColumn))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Already_Defined, duplicateColumn);
						}

						var invalidChars = GetInvalidChars(CommentColumn, new List<string> { ";", " " });
						if (!string.IsNullOrEmpty(invalidChars))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Found_Invalid_Characters, invalidChars);
						}
					}
					break;
				case nameof(CharacterLimitationColumn):
					if (!string.IsNullOrEmpty(CharacterLimitationColumn))
					{
						var mapping = _languageMappings.FirstOrDefault(a =>
							a.ContentColumn == CharacterLimitationColumn ||
							GetColumnNames(a.ContextColumn).Contains(CharacterLimitationColumn) ||
							GetColumnNames(a.CommentColumn).Contains(CharacterLimitationColumn) ||
							a.LineLimitationColumn == CharacterLimitationColumn ||
							a.PixelLimitationColumn == CharacterLimitationColumn ||
							a.PixelFontFamilyColumn == CharacterLimitationColumn ||
							a.PixelFontSizeColumn == CharacterLimitationColumn);
						if (mapping != null)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Language, CharacterLimitationColumn, mapping.LanguageId);
						}

						if (CharacterLimitationColumn == ContentColumn ||
							GetColumnNames(ContextColumn).Contains(CharacterLimitationColumn) ||
							GetColumnNames(CommentColumn).Contains(CharacterLimitationColumn) ||
							CharacterLimitationColumn == LineLimitationColumn ||
							CharacterLimitationColumn == PixelLimitationColumn ||
							CharacterLimitationColumn == PixelFontFamilyColumn ||
							CharacterLimitationColumn == PixelFontSizeColumn)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Current_Language, CharacterLimitationColumn);
						}

						var invalidChars = GetInvalidChars(CharacterLimitationColumn);
						if (!string.IsNullOrEmpty(invalidChars))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Found_Invalid_Characters, invalidChars);
						}
					}
					break;
				case nameof(LineLimitationColumn):
					if (!string.IsNullOrEmpty(LineLimitationColumn))
					{
						var mapping = _languageMappings.FirstOrDefault(a =>
							a.ContentColumn == LineLimitationColumn ||
							GetColumnNames(a.ContextColumn).Contains(LineLimitationColumn) ||
							GetColumnNames(a.CommentColumn).Contains(LineLimitationColumn) ||
							a.CharacterLimitationColumn == LineLimitationColumn ||
							a.PixelLimitationColumn == LineLimitationColumn ||
							a.PixelFontFamilyColumn == LineLimitationColumn ||
							a.PixelFontSizeColumn == LineLimitationColumn);
						if (mapping != null)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Language, LineLimitationColumn, mapping.LanguageId);
						}

						if (LineLimitationColumn == ContentColumn ||
						    GetColumnNames(ContextColumn).Contains(LineLimitationColumn) ||
						    GetColumnNames(CommentColumn).Contains(LineLimitationColumn) ||
						    LineLimitationColumn == CharacterLimitationColumn ||
						    LineLimitationColumn == PixelLimitationColumn ||
						    LineLimitationColumn == PixelFontFamilyColumn ||
						    LineLimitationColumn == PixelFontSizeColumn)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Current_Language, LineLimitationColumn);
						}

						var invalidChars = GetInvalidChars(LineLimitationColumn);
						if (!string.IsNullOrEmpty(invalidChars))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Found_Invalid_Lines, invalidChars);
						}
					}
					break;
				case nameof(PixelLimitationColumn):
					if (!string.IsNullOrEmpty(PixelLimitationColumn))
					{
						var mapping = _languageMappings.FirstOrDefault(a =>
							a.ContentColumn == PixelLimitationColumn ||
							GetColumnNames(a.ContextColumn).Contains(PixelLimitationColumn) ||
							GetColumnNames(a.CommentColumn).Contains(PixelLimitationColumn) ||
							a.CharacterLimitationColumn == PixelLimitationColumn ||
							a.LineLimitationColumn == PixelLimitationColumn ||
							a.PixelFontFamilyColumn == PixelLimitationColumn ||
							a.PixelFontSizeColumn == PixelLimitationColumn);

						if (mapping != null)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Language, PixelLimitationColumn, mapping.LanguageId);
						}

						if (PixelLimitationColumn == ContentColumn ||
							GetColumnNames(ContextColumn).Contains(PixelLimitationColumn) ||
							GetColumnNames(CommentColumn).Contains(PixelLimitationColumn) ||
							PixelLimitationColumn == CharacterLimitationColumn ||
							PixelLimitationColumn == LineLimitationColumn ||
							PixelLimitationColumn == PixelFontFamilyColumn ||
							PixelLimitationColumn == PixelFontSizeColumn)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Current_Language, PixelLimitationColumn);
						}

						var invalidChars = GetInvalidChars(PixelLimitationColumn);
						if (!string.IsNullOrEmpty(invalidChars))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Found_Invalid_Characters, invalidChars);
						}
					}
					break;

				case nameof(PixelFontFamilyColumn):
					if (!string.IsNullOrEmpty(PixelFontFamilyColumn))
					{
						var mapping = _languageMappings.FirstOrDefault(a =>
							a.ContentColumn == PixelFontFamilyColumn ||
							GetColumnNames(a.ContextColumn).Contains(PixelFontFamilyColumn) ||
							GetColumnNames(a.CommentColumn).Contains(PixelFontFamilyColumn) ||
							a.PixelLimitationColumn == PixelFontFamilyColumn ||
							a.CharacterLimitationColumn == PixelFontFamilyColumn ||
							a.LineLimitationColumn == PixelFontFamilyColumn ||
							a.PixelFontSizeColumn == PixelFontFamilyColumn);

						if (mapping != null)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Language, PixelFontFamilyColumn, mapping.LanguageId);
						}

						if (PixelFontFamilyColumn == ContentColumn ||
							GetColumnNames(ContextColumn).Contains(PixelFontFamilyColumn) ||
							GetColumnNames(CommentColumn).Contains(PixelFontFamilyColumn) ||
							PixelFontFamilyColumn == PixelLimitationColumn ||
							PixelFontFamilyColumn == CharacterLimitationColumn ||
							PixelFontFamilyColumn == LineLimitationColumn ||
							PixelFontFamilyColumn == PixelFontSizeColumn)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Current_Language, PixelFontFamilyColumn);
						}

						var invalidChars = GetInvalidChars(PixelFontFamilyColumn);
						if (!string.IsNullOrEmpty(invalidChars))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Found_Invalid_Characters, invalidChars);
						}
					}
					break;

				case nameof(PixelFontSizeColumn):
					if (!string.IsNullOrEmpty(PixelFontSizeColumn))
					{
						var mapping = _languageMappings.FirstOrDefault(a =>
							a.ContentColumn == PixelFontSizeColumn ||
							GetColumnNames(a.ContextColumn).Contains(PixelFontSizeColumn) ||
							GetColumnNames(a.CommentColumn).Contains(PixelFontSizeColumn) ||
							a.PixelLimitationColumn == PixelFontSizeColumn ||
							a.CharacterLimitationColumn == PixelFontSizeColumn ||
							a.LineLimitationColumn == PixelFontSizeColumn ||
							a.PixelFontFamilyColumn == PixelFontSizeColumn);

						if (mapping != null)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Language, PixelFontSizeColumn, mapping.LanguageId);
						}

						if (PixelFontSizeColumn == ContentColumn ||
							GetColumnNames(ContextColumn).Contains(PixelFontSizeColumn) ||
							GetColumnNames(CommentColumn).Contains(PixelFontSizeColumn) ||
							PixelFontSizeColumn == PixelLimitationColumn ||
							PixelFontSizeColumn == CharacterLimitationColumn ||
							PixelFontSizeColumn == LineLimitationColumn ||
							PixelFontSizeColumn == PixelFontFamilyColumn)
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Column_Is_Defined_For_Current_Language, PixelFontSizeColumn);
						}

						var invalidChars = GetInvalidChars(PixelFontSizeColumn);
						if (!string.IsNullOrEmpty(invalidChars))
						{
							validationMessage = string.Format(PluginResources.ValidationMessage_Found_Invalid_Characters, invalidChars);
						}
					}
					break;

				case nameof(PixelFontSize):
					if (!string.IsNullOrEmpty(PixelLimitationColumn))
					{
						if (PixelFontSize <= 0)
						{
							validationMessage = "The pixel size must be greater than zero";
						}
					}
					break;
				case nameof(SelectedFontFamilies):

					if (!string.IsNullOrEmpty(PixelLimitationColumn))
					{
						if (!SelectedFontFamilies.Any() || SelectedFontFamilies.FirstOrDefault() == null)
						{
							validationMessage = "The pixel font family cannot be null";
						}
					}

					break;
			}

			return string.IsNullOrEmpty(validationMessage);
		}

		private bool HasDuplicateColumn(List<string> columnNames, out string duplicateColumn)
		{
			duplicateColumn = null;
			var uniqueList = new List<string>();
			foreach (var column in columnNames)
			{
				if (!uniqueList.Contains(column))
				{
					uniqueList.Add(column);
				}
				else
				{
					duplicateColumn = column;
					return true;
				}
			}

			return false;
		}

		private string GetInvalidChars(string text, List<string> exclude = null)
		{
			var nonLettersRegex = new Regex("[^A-Z]", RegexOptions.IgnoreCase);

			var invalidChars = string.Empty;
			var matches = nonLettersRegex.Matches(text);
			foreach (Match match in matches)
			{
				var excluded = exclude?.Any(item => string.Compare(item, match.Value, StringComparison.InvariantCultureIgnoreCase) == 0) ?? false;
				if (!excluded)
				{
					invalidChars += match.Value;
				}
			}

			return invalidChars;
		}

		public string Error { get; }
	}
}
