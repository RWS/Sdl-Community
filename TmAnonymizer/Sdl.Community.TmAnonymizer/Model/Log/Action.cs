using System;
using System.Collections.Generic;

namespace Sdl.Community.SdlTmAnonymizer.Model.Log
{
	[Serializable]
	public class Action
	{
		public enum ActionScope
		{
			All = 0,
			Content = 1,
			SystemFields = 2,
			CustomFields = 3
		}

		public enum ActionType
		{
			All = 0,
			Backup = 1,
			Update = 2
		}
		public string Id { get; set; }

		public ActionType Type { get; set; }

		public ActionScope Scope { get; set; }

		public List<Detail> Details { get; set; }

		public string Result { get; set; }
	}
}
