using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.CommunityWpfHelpers.Commands;
using Sdl.CommunityWpfHelpers.Services;

namespace Sdl.CommunityWpfHelpers.SampleApp
{
	public class MainWindowViewModel:BaseModel
	{
		private ICommand _parameterCommand;
		private ICommand _sampleCommand;
		private ICommand _awaitableCommand;
		private ICommand _windowClosingCommand;
		private string _text;

		public MainWindowViewModel()
		{
			_text = "Source";
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
		public ICommand SampleCommand => _sampleCommand ?? (_sampleCommand = new CommandHandler(SampleAction, true));
		public ICommand AwaitableCommand => _awaitableCommand ?? (_awaitableCommand = new AwaitableCommand(AwaitableAction));
		public ICommand WindowClosingCommand => _windowClosingCommand ??(_windowClosingCommand = new CommandHandler(OnWindowClosing, true));

		private async Task AwaitableAction()
		{
			
		}

		private void SampleAction()
		{
			
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
