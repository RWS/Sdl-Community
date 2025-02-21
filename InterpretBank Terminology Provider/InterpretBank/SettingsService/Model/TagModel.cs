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

	public override int GetHashCode()
	{
		return TagName.GetHashCode();
	}

	public override bool Equals(object obj)
    {
        if (obj is not TagModel model) return false;
		return model.TagName == TagName;
	}
}