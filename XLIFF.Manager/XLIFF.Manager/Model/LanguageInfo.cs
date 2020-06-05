using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class LanguageInfo : BaseModel, ICloneable
	{
		public CultureInfo CultureInfo { get; set; }

		public BitmapImage Image { get; set; }

		public override string ToString()
		{
			return CultureInfo.Name;
		}

		public object Clone()
		{
			return new LanguageInfo
			{
				CultureInfo = new CultureInfo(CultureInfo.Name),
				Image = Image.CloneCurrentValue()
			};
		}
	}
}
