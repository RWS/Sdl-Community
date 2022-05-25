using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
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
	    private string _jsonFilePath;
	    private string _projectName;
	    private bool _v2Selected;

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
		    _v2Selected = true;
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
			    V2Selected = value.Version == Enums.Version.V2;
			    Message = string.Empty;
			    GoogleResponse = string.Empty;
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

	    public string JsonFilePath
	    {
		    get => _jsonFilePath;
		    set
		    {
			    if (_jsonFilePath == value) return;
			    _jsonFilePath = value;
			    OnPropertyChanged(nameof(JsonFilePath));
		    }
	    }

	    public string ProjectName
	    {
		    get => _projectName;
		    set
		    {
			    if (_projectName == value) return;
			    _projectName = value;
			    OnPropertyChanged(nameof(ProjectName));
		    }
	    }

	    public bool V2Selected
	    {
		    get => _v2Selected;
		    set
		    {
			    if (_v2Selected == value) return;
			    _v2Selected = value;
			    OnPropertyChanged(nameof(V2Selected));
		    }
	    }

	    public ICommand ValidateCommand => _validateCommand ?? (_validateCommand = new CommandHandler(ValidateKey, true));

	    private void ValidateKey()
	    {
		    Message = string.Empty;

		    if (SelectedVersion.Version == Enums.Version.V2)
		    {
			    ValidateV2();
		    }
		    else
		    {
			    ValidateV3();
		    }
	    }

	    private void ValidateV3()
	    {
		    if (IsWindowValid())
		    {
			    if (File.Exists(JsonFilePath))
			    {
					Environment.SetEnvironmentVariable(AppResources.GoogleApiEnvironmentVariableName, JsonFilePath);
				    try
				    {
					    var translationServiceClient = TranslationServiceClient.Create();
					    var request = new TranslateTextRequest
					    {
						    Contents =
						    {
							    "test"
						    },
						    TargetLanguageCode = "fr-FR",
						    Parent = new ProjectName(ProjectName).ToString()

					    };
					    var response = translationServiceClient.TranslateText(request);
					    if (response != null)
					    {
							GoogleResponse = AppResources.SuccessMsg;
						}
					}
				    catch (Exception e)
				    {
						var message = e.Message.Contains("Invalid resource name") ? AppResources.InvalidProjectName : e.Message;
					    GoogleResponse = AddEncriptionMeta(message);
				    }
				}
			    else
			    {
				    Message = AppResources.JsonFileMessage;
			    }
		    }
		    else
		    {
			    Message = AppResources.AllFieldsRequired;
		    }
	    }

	    private void ValidateV2()
	    {
		    if (string.IsNullOrEmpty(ApiKey))
		    {
			    Message = AppResources.EmptyKey;
		    }
		    else
		    {
			    try
			    {
				    var client = TranslationClient.CreateFromApiKey(ApiKey);
				    var response = client.TranslateText("", "ru", "en");
				    if (response != null)
				    {
					    GoogleResponse = AppResources.SuccessMsg;
				    }
			    }
			    catch (Exception e)
			    {
				    GoogleResponse = AddEncriptionMeta(e.Message);
			    }
		    }
	    }

	    private string AddEncriptionMeta(string response)
	    {
			var htmlStart = "<html> \n <meta http-equiv=\'Content-Type\' content=\'text/html;charset=UTF-8\'>\n <body style=\"font-family:Segoe Ui!important;font-size:13px!important\">\n";

		    var editedDescription = response.Insert(0, htmlStart);
		    editedDescription += "\n</body></html>";
		    return editedDescription;
		}

		private bool IsWindowValid()
	    {
		    return !(string.IsNullOrEmpty(JsonFilePath) || string.IsNullOrEmpty(ProjectName));
	    }
    }
}
