using Sdl.MultiSelectComboBox.API;

namespace InterpretBank.SettingsService.Model
{
	public class LanguageGroup : IItemGroup
	{
		public LanguageGroup(int index, string name)
		{
			Order = index;
			Name = name;
		}

		public string Name { get; }
		public int Order { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}