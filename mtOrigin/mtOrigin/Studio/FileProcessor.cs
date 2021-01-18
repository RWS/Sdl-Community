using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.mtOrigin.Studio
{
	public class FileProcessor : AbstractBilingualContentProcessor
	{
		private readonly string _newOrigin;

		public FileProcessor(string newOrigin)
		{
			_newOrigin = newOrigin;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				return;
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				ReplaceTranslationOrigin(segmentPair?.Properties?.TranslationOrigin, _newOrigin);
				ReplaceTranslationOrigin(segmentPair?.Properties?.TranslationOrigin.OriginBeforeAdaptation,_newOrigin);
			}
		}

		private void ReplaceTranslationOrigin(ITranslationOrigin translationOrigin, string newOrigin)
		{
			if (!string.IsNullOrEmpty(translationOrigin?.OriginType))
			{
				if (translationOrigin.OriginType.Equals("nmt") || translationOrigin.OriginType.Equals("mt"))
				{
					translationOrigin.OriginType = newOrigin;
				}
			}
		}
	}
}

