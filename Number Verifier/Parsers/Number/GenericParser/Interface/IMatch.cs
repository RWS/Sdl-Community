namespace Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Interface
{
	public interface IMatch
	{
		bool Success { get; set; }
		string Name{ get; set; }
		string Value { get; }
	}
}