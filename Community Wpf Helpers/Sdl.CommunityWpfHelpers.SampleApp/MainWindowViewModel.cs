using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.CommunityWpfHelpers.Commands;
using Sdl.CommunityWpfHelpers.Services;
using Sdl.CommunityWpfHelpers.Interfaces;


namespace Sdl.CommunityWpfHelpers.SampleApp
{
	public class MainWindowViewModel:BaseModel
	{
		private ICommand _parameterCommand;
		private ICommand _sampleCommand;
		private ICommand _awaitableCommand;
		private ICommand _windowClosingCommand;
		private readonly IMessageBoxService _messageBoxService;
		private string _text;

		public MainWindowViewModel(IMessageBoxService messageBoxService)
		{
			_text = "Source";
			_messageBoxService = messageBoxService;
		}

		public string Text
		{
			get => _text;
			set
			{
				_text = value;
				OnPropertyChanged(nameof(Text));
			}
		}

		public ICommand ParameterCommand =>_parameterCommand ?? (_parameterCommand = new RelayCommand(ParameterCommandAction));
		public ICommand SampleCommand => _sampleCommand ?? (_sampleCommand = new CommandHandler(ShowWarningMessage, true));
		public ICommand AwaitableCommand => _awaitableCommand ?? (_awaitableCommand = new AwaitableCommand(AwaitableAction));
		public ICommand WindowClosingCommand => _windowClosingCommand ??(_windowClosingCommand = new CommandHandler(OnWindowClosing, true));

		private async Task AwaitableAction()
		{
			
		}

		private void ShowWarningMessage()
		{
			//_messageBoxService.ShowWarningMessage("Message text","Warning");
			//_messageBoxService.ShowInformationMessage("Message text", "information");
			var confirmation = _messageBoxService.AskForConfirmation("Are you sure?","Please confirm");
			if (confirmation)
			{
				// User clicked ok
			}
			else
			{
				// user clicked cancel
			}
		}

		private void ParameterCommandAction(object commandParameter)
		{
			var text = (string) commandParameter;
		}

		private void OnWindowClosing()
		{
		}
	}
}
