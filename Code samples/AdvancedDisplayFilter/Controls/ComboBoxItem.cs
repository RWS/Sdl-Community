namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
	public class ComboBoxItem
	{
		public string Name { get; set; }
		public object Type { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
