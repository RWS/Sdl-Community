using System.Threading.Tasks;
using System.Windows.Input;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IAsyncCommand : ICommand
	{
		bool CanExecute();

		Task ExecuteAsync();
	}

	public interface IAsyncCommand<T> : ICommand
	{
		bool CanExecute(T parameter);

		Task ExecuteAsync(T parameter);
	}
}