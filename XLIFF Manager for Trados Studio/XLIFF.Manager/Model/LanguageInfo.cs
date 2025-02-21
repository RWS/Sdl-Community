using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class LanguageInfo : BaseModel, ICloneable
	{
		[XmlIgnore]
		public CultureInfo CultureInfo { get; set; }

		[XmlIgnore]
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
