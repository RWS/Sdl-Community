using System.ComponentModel;

namespace Sdl.Community.TMBackup.Helpers
{
	public class Enums
	{
		public enum TimeTypes
		{
			[Description("seconds")]
			Seconds = 0,
			[Description("minutes")]
			Minutes = 1,
			[Description("hours")]
			Hours = 2
		}
	}
}