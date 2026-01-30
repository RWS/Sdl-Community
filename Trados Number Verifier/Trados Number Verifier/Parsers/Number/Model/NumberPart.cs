namespace Sdl.Community.NumberVerifier.Parsers.Number.Model
{
	public class NumberPart
	{
		public enum NumberType
		{
			None,
			GroupSeparator,
			DecimalSeparator,
			Number,
			Separator,
			Sign,
			Currency,
			WhiteSpace,
			Exponent,
			Invalid
		}

		public NumberType Type { get; set; }

		public string Value { get; set; }
		
		public string Message { get; set; }
	}
}
