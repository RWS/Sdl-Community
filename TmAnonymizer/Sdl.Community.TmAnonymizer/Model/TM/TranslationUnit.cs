using System;

namespace Sdl.Community.SdlTmAnonymizer.Model.TM
{
	public class TranslationUnit
	{
		public int Id { get; set; }
		public string Guid { get; set; }
		public string SourceSegment { get; set; }
		public string TargetSegment { get; set; }
		public DateTime CreationDate { get; set; }
		public string CreationUser { get; set; }
		public DateTime ChangeDate { get; set; }
		public string ChangeUser { get; set; }
		public DateTime LastUsedDate { get; set; }
		public string LastUsedUser { get; set; }
		public int UsageCounter { get; set; }
	}
}
