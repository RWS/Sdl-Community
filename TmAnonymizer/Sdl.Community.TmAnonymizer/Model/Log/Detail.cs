using System;

namespace Sdl.Community.SdlTmAnonymizer.Model.Log
{
	[Serializable]
	public class Detail
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public string Previous { get; set; }
		public string Value { get; set; }
	}
}
