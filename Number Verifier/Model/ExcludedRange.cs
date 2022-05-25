namespace Sdl.Community.NumberVerifier.Model
{
	public class ExcludedRange
	{
		public int LeftLimit { get; set; } = -1;
		public int RightLimit { get; set; } = -1;

		public bool Contains(int left, int right) => Contains(new ExcludedRange
		{
			LeftLimit = left,
			RightLimit = right
		});

		public bool Contains(ExcludedRange rangeInCase)
			=> LeftLimit <= rangeInCase.LeftLimit && RightLimit >= rangeInCase.RightLimit;
	}
}