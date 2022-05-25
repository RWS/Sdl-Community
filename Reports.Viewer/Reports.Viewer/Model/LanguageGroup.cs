using Sdl.MultiSelectComboBox.API;

namespace Reports.Viewer.Plus.Model
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
