using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.Model;

namespace Sdl.Community.AhkPlugin.ViewModels
{
    public class AddScriptViewModel: ViewModelBase
    {
	    private static  MainWindowViewModel _mainWindowViewModel;
	    private ICommand _backCommand;
	    private ICommand _insertCommand;
	    private  string _scriptName;
	    private string _scriptDescription;
	    private string _scriptContent;
	    private string _message;
	    private string _messageVisibility;
	    private bool _formIsValid;
	    private readonly DbContext _dbContext;
		private string _messageColor;
	    private bool _isDisabled;

		public static readonly Log Log = Log.Instance;
		
		public AddScriptViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}

	    public AddScriptViewModel()
	    {
			_scriptName = string.Empty;
		    _scriptDescription = string.Empty;
		    _scriptContent = string.Empty;
		    _message = string.Empty;
		    _messageVisibility = "Hidden";
		    _messageColor = string.Empty;
			_dbContext = new DbContext();
		    _isDisabled = false;
	    }

		public string ScriptName
		{
			get => _scriptName;

			set
			{
				if (Equals(value, _scriptName))
				{
					return;
				}
				_scriptName = value;
				OnPropertyChanged(nameof(ScriptName));
			}
		}

		public string Message
		{
			get => _message;

			set
			{
				if (Equals(value, _message))
				{
					return;
				}
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public string MessageColor
		{
			get => _messageColor;

			set
			{
				if (Equals(value, _messageColor))
				{
					return;
				}
				_messageColor = value;
				OnPropertyChanged(nameof(MessageColor));
			}
		}

		public string ScriptDescription
		{
			get => _scriptDescription;

			set
			{
				if (Equals(value, _scriptDescription))
				{
					return;
				}
				_scriptDescription = value;
				OnPropertyChanged(nameof(ScriptDescription));
			}
		}

		public string ScriptContent
		{
			get => _scriptContent;

			set
			{
				if (Equals(value, _scriptContent))
				{
					return;
				}
				_scriptContent = value;
				OnPropertyChanged(nameof(ScriptContent));
			}
		}

		public bool FormIsValid
		{
			get => _formIsValid;

			set
			{
				if (Equals(value, _formIsValid))
				{
					return;
				}
				_formIsValid = value;
				OnPropertyChanged(nameof(FormIsValid));
			}
		}

		public string MessageVisibility
		{
			get => _messageVisibility;

			set
			{
				if (Equals(value, _messageVisibility))
				{
					return;
				}
				_messageVisibility = value;
				OnPropertyChanged(nameof(MessageVisibility));
			}
		}

		public bool IsDisabled
		{
			get => _isDisabled;

			set
			{
				if (Equals(value, _isDisabled))
				{
					return;
				}
				_isDisabled = value;
				OnPropertyChanged(nameof(IsDisabled));
			}
		}

		public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));
	    public ICommand InsertCommand => _insertCommand ?? (_insertCommand = new CommandHandler(InsertScript, true));

	    private async void InsertScript()
	    {
			try
			{
				ValidateForm();
				MessageVisibility = "Visible";
				if (FormIsValid)
				{
					var script = new Script
					{
						ScriptId = Guid.NewGuid().ToString(),
						Active = !IsDisabled,
						Description = ScriptDescription,
						Name = ScriptName,
						Text = ScriptContent
					};
					script.ScriptStateAction = script.Active ? "Disable" : "Enable";
					script.RowColor = script.Active ? "Black" : "DarkGray";

					//add the script in master script 
					var masterScript = await _dbContext.GetMasterScript();
					masterScript.Scripts.Add(script);
					await _dbContext.UpdateScript(masterScript);

					//write masterscript on the disk
					ProcessScript.ExportScript(Path.Combine(masterScript.Location, masterScript.Name), masterScript.Scripts);

					Message = "Script added successfully.";
					MessageColor = "#3EA691";

					ClearForm();
				}
				else
				{
					Message = "Please fill all the fields.";
					MessageColor = Color.DarkRed.Name;
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.InsertScript}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}

	    private void ClearForm()
	    {
		    ScriptContent = string.Empty;
			ScriptDescription = string.Empty;
		    ScriptName = string.Empty;
		    IsDisabled = false;
	    }

		private void ValidateForm()
	    {
		    FormIsValid = !string.IsNullOrEmpty(ScriptName) && !string.IsNullOrEmpty(ScriptDescription) &&
		                  !string.IsNullOrEmpty(ScriptContent);
	    }

		private void BackToScriptsList()
	    {
		    _mainWindowViewModel.LoadScriptsPage();
	    }
	}
}