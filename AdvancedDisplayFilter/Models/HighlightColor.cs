using System.Drawing;

namespace Sdl.Community.AdvancedDisplayFilter.Models
{
	public class HighlightColor
	{		
		public HighlightColor(Color color, string displayName, string name, Image image)
		{
			Color = color;
			DisplayName = displayName;
			Name = name;
			Image = image;
		}

		public Color Color { get; set; }

		public string Name { get; set; }

		public string DisplayName { get; set; }

		public Image Image { get; set; }

		public string GetArgb()
		{
			var colorArgb = "0, " + Color.R + ", " + Color.G + ", " + Color.B;
			return colorArgb;
		}
	}
}
