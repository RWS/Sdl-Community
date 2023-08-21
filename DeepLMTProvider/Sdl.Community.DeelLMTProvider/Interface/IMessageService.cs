using System.Runtime.CompilerServices;

namespace Sdl.Community.DeepLMTProvider.Interface
{
	public interface IMessageService
	{
		void ShowWarning(string message, [CallerMemberName] string failingMethod = null);
	}
}