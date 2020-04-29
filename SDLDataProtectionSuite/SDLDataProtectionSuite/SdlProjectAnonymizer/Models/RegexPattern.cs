using System;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models
{
	public class RegexPattern
	{
		public RegexPattern()
		{
			Id = Guid.NewGuid().ToString();
		}

		public string Pattern { get; set; }
		public string Description { get; set; }
		public bool ShouldEncrypt { get; set; }
		public bool ShouldEnable { get; set; }
		public bool IsDefaultPath { get; set; }
		public string Id { get; set; }
	}
}