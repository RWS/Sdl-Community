using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Google.Cloud.Translation.V2;
using Sdl.Community.GoogleApiValidator.Commands;
using Sdl.Community.GoogleApiValidator.Model;
using Sdl.Community.GoogleApiValidator.Utils;

namespace Sdl.Community.GoogleApiValidator.ViewModel
{
    public class MainWindowViewModel:ModelBase
    {
	    private ObservableCollection<GoogleApiVersion> _apiVersions;
	    private GoogleApiVersion _selectedVersion;
	    private ICommand _validateCommand;
	    private string _apyKey;
	    private string _message;
	    private string _googleResponse;

		public MainWindowViewModel()
	    {
		    _apiVersions = new ObservableCollection<GoogleApiVersion>
		    {
			    new GoogleApiVersion
			    {
				    Name = "V2 - Basic Translation",
					Version = Enums.Version.V2
			    },
			    new GoogleApiVersion
			    {
				    Name = "V3 - Advanced Translation",
				    Version = Enums.Version.V3
				}
			};

		    _selectedVersion = _apiVersions[0];
	    }

	    public ObservableCollection<GoogleApiVersion> ApiVersions
	    {
		    get => _apiVersions;
		    set
		    {
			    _apiVersions = value;
			    OnPropertyChanged(nameof(ApiVersions));
		    }
	    }

	    public GoogleApiVersion SelectedVersion
	    {
		    get => _selectedVersion;
		    set
		    {
			    _selectedVersion = value;
			    OnPropertyChanged(nameof(SelectedVersion));
		    }
	    }

	    public string ApiKey
	    {
		    get => _apyKey;
		    set
		    {
			    if (_apyKey == value) return;
			    _apyKey = value;
			    GoogleResponse = string.Empty;
			    OnPropertyChanged(nameof(ApiKey));
		    }
	    }

	    public string Message
	    {
		    get => _message;
		    set
		    {
			    if (_message == value) return;
			    _message = value;
			    OnPropertyChanged(nameof(Message));
		    }
	    }

	    public string GoogleResponse
	    {
		    get => _googleResponse;
		    set
		    {
			    if (_googleResponse == value) return;
			    _googleResponse = value;
			    OnPropertyChanged(nameof(GoogleResponse));
		    }
	    }
	    public ICommand ValidateCommand => _validateCommand ?? (_validateCommand = new CommandHandler(ValidateKey, true));

	    private void ValidateKey()
	    {
		    Message = string.Empty;

		    if (string.IsNullOrEmpty(ApiKey))
		    {
			    Message = AppResources.EmptyKey;
		    }
		    else
		    {
			    if (SelectedVersion.Version == Enums.Version.V2)
			    {
				    try
				    {
					    var client = TranslationClient.CreateFromApiKey(ApiKey);
					    var response = client.TranslateText("","ru","en");
					    if (response != null)
					    {
						    GoogleResponse = AppResources.SuccessMsg;
					    }
				    }
				    catch (Exception e)
				    {
					    GoogleResponse = e.Message;
				    }
			    }
		    }

	    }
    }
}
