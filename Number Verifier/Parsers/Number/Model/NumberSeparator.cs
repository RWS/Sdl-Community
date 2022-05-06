namespace Sdl.Community.NumberVerifier.Parsers.Number.Model
{
	public class NumberSeparator
	{
		public enum SeparatorType
		{
			GroupSeparator,
			DecimalSeparator
		}

		public SeparatorType Type { get; set; }

		public string Value { get; set; }
	}
}
