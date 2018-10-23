using System;
using System.Collections.Generic;

namespace Sdl.Community.SdlTmAnonymizer.Model.Log
{
	public class Report
	{
		public TmFile TmFile { get; set; }

		public DateTime Created { get; set; }

		public TimeSpan ElapsedTime { get; set; }

		public List<Action> Actions { get; set; }
	}
}
