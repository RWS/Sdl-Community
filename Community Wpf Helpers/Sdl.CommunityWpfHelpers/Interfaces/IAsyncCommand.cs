using System.Threading.Tasks;
using System.Windows.Input;

namespace Sdl.CommunityWpfHelpers.Interfaces
{
	public interface IAsyncCommand : IAsyncCommand<object>
	{
	}
	public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged
	{
		Task ExecuteAsync(T obj);
		bool CanExecute(object obj);
		ICommand Command { get; }
	}
}
