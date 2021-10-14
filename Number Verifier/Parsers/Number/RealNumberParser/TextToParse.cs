namespace Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser
{
	public class TextToParse
	{
		public TextToParse(string text, int index = 0)
		{
			Text = text;
			CurrentIndex = index;
		}

		public char Current => Text[CurrentIndex];

		public int CurrentIndex { get; set; }

		public string Text { get; }

		public void Advance() => CurrentIndex++;

		public bool IsAtEnd()
		{
			return CurrentIndex == Text.Length || Text.Length == 0;
		}
	}
}