using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace CustomViewExample.Model
{
	public class CustomViewLanguage : ICloneable
	{
		public CultureInfo CultureInfo { get; set; }

		public BitmapImage Image { get; set; }

		public override string ToString()
		{
			return CultureInfo.Name;
		}

		public object Clone()
		{
			return new CustomViewLanguage
			{
				CultureInfo = new CultureInfo(CultureInfo.Name),
				Image = Image.Clone()
			};
		}
	}
}
