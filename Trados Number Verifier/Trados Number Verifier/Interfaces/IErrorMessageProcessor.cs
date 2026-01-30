namespace Sdl.Community.NumberVerifier.Interfaces
{
	public interface IErrorMessageProcessor
    {
        string GenerateMessage(INumberResults numberResult, string errorMessage);
    }
}