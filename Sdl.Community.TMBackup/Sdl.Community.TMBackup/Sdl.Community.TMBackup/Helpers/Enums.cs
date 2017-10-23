using System;
using System.ComponentModel.DataAnnotations;

namespace Sdl.Community.TMBackup.Helpers
{
	public class Enums
	{
		[Flags]
		public enum TimeTypes
		{
			[Display(Name = "seconds")]
			Seconds = 0,

			[Display(Name = "minutes")]
			Minutes = 1,

			[Display(Name = "horus")]
			Hours = 2
		}
	}
}