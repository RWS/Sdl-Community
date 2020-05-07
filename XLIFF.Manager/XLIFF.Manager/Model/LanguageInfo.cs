using System.Globalization;
using System.Windows.Media.Imaging;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class LanguageInfo: BaseModel
	{
		public CultureInfo CultureInfo { get; set; }

		public BitmapImage Image { get; set; }

		public string ImageName { get; set; }

		public override string ToString()
		{
			return CultureInfo.Name;
		}
	}
}
