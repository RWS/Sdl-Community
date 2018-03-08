using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	public class FSTGenerator
	{
		public static FST.FST GenerateNumberFST(System.Globalization.CultureInfo ci)
		{
			return NumberFSTRecognizer.CreateFST(ci, Core.CultureInfoExtensions.UseBlankAsWordSeparator(ci));
		}

		public static FST.FST GenerateMeasurementFST(System.Globalization.CultureInfo ci)
		{
			return MeasureFSTRecognizer.CreateFST(ci, Core.CultureInfoExtensions.UseBlankAsWordSeparator(ci));
		}
	}
}
