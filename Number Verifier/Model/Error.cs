using Sdl.Community.NumberVerifier.Validator;

namespace Sdl.Community.NumberVerifier.Model
{
	public class Error
	{
		public NumberText.ErrorLevel ErrorLevel { get; set; }
		public string Message { get; set; }
	}
}