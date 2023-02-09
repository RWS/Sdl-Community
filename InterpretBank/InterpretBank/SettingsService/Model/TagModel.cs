using Sdl.MultiSelectComboBox.API;

namespace InterpretBank.SettingsService.Model;

public class TagModel : IItemGroupAware
{
	public string TagName { get; set; }
	public IItemGroup Group { get; set; }

	public override string ToString()
	{
		return TagName;
	}
}