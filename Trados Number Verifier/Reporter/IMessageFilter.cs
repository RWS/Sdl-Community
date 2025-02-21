namespace Sdl.Community.NumberVerifier.Reporter
{
	public interface IMessageFilter
	{
		bool IsAllowed(string message);
	}
}