using System;

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
			None = 0,
			Backup = 1,
			Update = 2
		}

		public ActionType Type { get; set; }

		public ActionScope Scope { get; set; }

		public string Details { get; set; }

		public string Result { get; set; }
	}
}
