using Sdl.MultiSelectComboBox.API;

namespace InterpretBank.SettingsService.Model
{
	public class LanguageModel : IItemGroupAware
	{
		public int Index { get; set; }
		public string Name { get; set; }
		public IItemGroup Group { get; set; }

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			return ((LanguageModel)obj).Index == Index;
		}
	}
}