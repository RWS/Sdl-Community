namespace Sdl.Community.SdlTmAnonymizer.Model.Comparison
{
	public class Part
	{
		public enum Type
		{
			Text,
			End,
			LockedContent,
			Standalone,
			Start,
			TextPlaceholder,
			Undefined,
			UnmatchedEnd,
			UnmatchedStart
		}

		public Type ContentType { get; set; }

		public string Content { get; set; }

		public int StartPosition { get; set; }

		public int EndPosition { get; set; }
	}
}
