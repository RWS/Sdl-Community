using Sdl.Community.NumberVerifier.Validator;

namespace Sdl.Community.NumberVerifier.Model
{
	public class ComparisonItem
	{
		public Comparer Comparer { get; set; }
		public int SourceIndex { get; set; }
		public int TargetIndex { get; set; }
	}
}