using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.SDLBatchAnonymize
{
	public class AnonymizerProcessor : AbstractBilingualContentProcessor
	{
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure)
			{
				return;
			}
			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				Anonymize(segmentPair?.Properties?.TranslationOrigin);
				Anonymize(segmentPair?.Properties?.TranslationOrigin?.OriginBeforeAdaptation);
			}
		}

		private void Anonymize(ITranslationOrigin translationOrigin)
		{
			var originType = translationOrigin?.OriginType;
			if (!string.IsNullOrEmpty(originType) && (originType.Equals(DefaultTranslationOrigin.MachineTranslation) 
				|| originType.Equals(DefaultTranslationOrigin.NeuralMachineTranslation) || originType.Equals(DefaultTranslationOrigin.AdaptiveMachineTranslation)))
			{
				translationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
				translationOrigin.OriginSystem = string.Empty;
			}
		}
	}
}
