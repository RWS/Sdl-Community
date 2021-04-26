using System.IO;
using System.Windows.Input;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Trados.TargetRenamer.Interfaces;
using Trados.TargetRenamer.Model;
using Trados.TargetRenamer.Services;

namespace Trados.TargetRenamer.ViewModel
{
    public class TargetRenamerSettingsViewModel : BaseModel, ISettingsAware<TargetRenamerSettings>
    {
	    public TargetRenamerSettings Settings { get; set; }

	    private bool _overwriteTargetFiles = true;
	    private bool _appendAsPrefix;
	    private bool _appendAsSuffix = true;
	    private bool _useRegularExpression;
	    private bool _useCustomLocation;
	    private string _customLocation = Path.GetTempPath();
	    private string _regularExpressionSearchFor;
	    private string _regularExpressionReplaceWith;
	    private string _delimitator = "_";
	    private bool _useShortLocales;
	    private bool _appendTargetLanguage = true;
	    private bool _appendCustomString;
	    private string _customString;
	    private readonly IFolderDialogService _folderDialogService;
	    private ICommand _selectTargetFolder;

	    public TargetRenamerSettingsViewModel(IFolderDialogService folderDialogService)
	    {
		    _folderDialogService = folderDialogService;
	    }

	    public ICommand SelectTargetFolder =>
		    _selectTargetFolder ?? (_selectTargetFolder = new CommandHandler(SelectFolder));

	    private void SelectFolder(object obj)
	    {
		    CustomLocation = _folderDialogService.ShowDialog();
	    }

	    public bool OverwriteTargetFiles
	    {
		    get => _overwriteTargetFiles;
		    set
		    {
			    if (_overwriteTargetFiles == value) return;
			    _overwriteTargetFiles = value;
				OnPropertyChanged(nameof(OverwriteTargetFiles));
		    }
	    }

	    public bool AppendAsPrefix
	    {
		    get => _appendAsPrefix;
		    set
		    {
			    if (_appendAsPrefix == value) return;
			    _appendAsPrefix = value;
			    OnPropertyChanged(nameof(AppendAsPrefix));
		    }
	    }

	    public bool AppendAsSuffix
	    {
		    get => _appendAsSuffix;
		    set
		    {
			    if (_appendAsSuffix == value) return;
			    _appendAsSuffix = value;
			    OnPropertyChanged(nameof(AppendAsSuffix));
		    }
	    }

	    public bool UseRegularExpression
	    {
		    get => _useRegularExpression;
		    set
		    {
				if(_useRegularExpression == value) return;
				_useRegularExpression = value;
				OnPropertyChanged(nameof(UseRegularExpression));
		    }
	    }

	    public bool UseCustomLocation
	    {
		    get => _useCustomLocation;
		    set
		    {
			    if (_useCustomLocation == value) return;
			    _useCustomLocation = value;
			    OnPropertyChanged(nameof(UseCustomLocation));
		    }
	    }

	    public string CustomLocation
	    {
		    get => _customLocation;
		    set
		    {
			    if (_customLocation == value) return;
			    _customLocation = value;
			    OnPropertyChanged(nameof(CustomLocation));
		    }
	    }

	    public string RegularExpressionSearchFor
	    {
		    get => _regularExpressionSearchFor;
		    set
		    {
			    if (_regularExpressionSearchFor == value) return;
			    _regularExpressionSearchFor = value;
			    OnPropertyChanged(nameof(RegularExpressionSearchFor));
		    }
	    }

	    public string RegularExpressionReplaceWith
	    {
		    get => _regularExpressionReplaceWith;
		    set
		    {
			    if (_regularExpressionReplaceWith == value) return;
			    _regularExpressionReplaceWith = value;
			    OnPropertyChanged(nameof(RegularExpressionReplaceWith));
		    }
	    }

	    public string Delimitator
	    {
		    get => _delimitator;
		    set
		    {
			    if (string.IsNullOrWhiteSpace(_delimitator))
			    {
				    _delimitator = "_";
			    }
			    if (_delimitator == value) return;
			    _delimitator = value;
			    OnPropertyChanged(nameof(Delimitator));
		    }
	    }

	    public bool UseShortLocales
	    {
		    get => _useShortLocales;
		    set
		    {
			    if (_useShortLocales == value) return;
			    _useShortLocales = value;
			    OnPropertyChanged(nameof(UseShortLocales));
		    }
	    }

	    public bool AppendTargetLanguage
	    {
		    get => _appendTargetLanguage;
		    set
		    {
			    if (_appendTargetLanguage == value) return;
			    _appendTargetLanguage = value;
			    OnPropertyChanged(nameof(AppendTargetLanguage));
		    }
	    }

	    public bool AppendCustomString
	    {
		    get => _appendCustomString;
		    set
		    {
			    if (_appendCustomString == value) return;
			    _appendCustomString = value;
			    OnPropertyChanged(nameof(AppendCustomString));
		    }
	    }

	    public string CustomString
	    {
		    get => _customString;
		    set
		    {
			    if (_customString == value) return;
			    _customString = value;
			    OnPropertyChanged(nameof(CustomString));
		    }
	    }
	}
}
