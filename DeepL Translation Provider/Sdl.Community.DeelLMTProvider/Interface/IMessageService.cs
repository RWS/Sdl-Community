using System.Runtime.CompilerServices;

namespace Sdl.Community.DeepLMTProvider.Interface
{
	public interface IMessageService
	{
        bool ShowDialog(string message, [CallerMemberName] string method = null);
        void ShowWarning(string message, [CallerMemberName] string failingMethod = null);
	}
}