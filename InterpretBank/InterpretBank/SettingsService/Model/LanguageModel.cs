using Sdl.MultiSelectComboBox.API;

namespace InterpretBank.SettingsService.Model
{
	public class LanguageModel : IItemGroupAware
	{
		public IItemGroup Group { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			return ((LanguageModel)obj).Name == Name;
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}