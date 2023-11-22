using System.Linq;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.Excel.FileType.Services
{
	public class ClearTranslationsContentProcessor : AbstractBilingualContentProcessor
	{
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				segmentPair.Target.Clear();
				segmentPair.Properties.ConfirmationLevel = ConfirmationLevel.Unspecified;
				segmentPair.Properties.TranslationOrigin = null;
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}
	}
}