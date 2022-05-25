namespace Sdl.Community.AdvancedDisplayFilter.Models
{
	public class BackReference
	{
		public BackReference(int number, string name, bool isNamed = false)
		{
			Number = number;
			Name = name;
			IsNamed = isNamed;
		}

		public int Number { get; set; }
		public string Name { get; set; }
		public bool IsNamed { get; set; }
	}
}
