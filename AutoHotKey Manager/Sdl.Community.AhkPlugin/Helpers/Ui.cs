using System.Collections.ObjectModel;
using Sdl.Community.AhkPlugin.Model;

namespace Sdl.Community.AhkPlugin.Helpers
{
	public static class Ui
	{
		public static void Select(ObservableCollection<Script> scripts, bool shouldSelect)
		{
			foreach (var script in scripts)
			{
				script.IsSelected = shouldSelect;
			}
		}

		public static Script SetStateColors(Script script)
		{
			if (script.ScriptStateAction.Equals("Disable"))
			{
				script.Active = false;
				script.ScriptStateAction = "Enable";
				script.RowColor = "DarkGray";
			}
			else
			{
				script.Active = true;
				script.ScriptStateAction = "Disable";
				script.RowColor = "Black";
			}
			return script;
		}
	}

}
