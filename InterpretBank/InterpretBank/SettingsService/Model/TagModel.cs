using Sdl.MultiSelectComboBox.API;

namespace InterpretBank.SettingsService.Model;

public class TagModel : IItemGroupAware
{
	public IItemGroup Group { get; set; }
	public string TagName { get; set; }

	public override string ToString()
	{
		return TagName;
	}
}