using Sdl.MultiSelectComboBox.API;

namespace Sdl.Community.Reports.Viewer.Model
{
	public class LanguageGroup: IItemGroup
	{
		public string Name { get; set; }

		public int Order { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
