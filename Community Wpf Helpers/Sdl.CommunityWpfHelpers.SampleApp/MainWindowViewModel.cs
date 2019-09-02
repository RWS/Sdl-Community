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
