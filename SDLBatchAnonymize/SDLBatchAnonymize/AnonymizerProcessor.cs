using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.SDLBatchAnonymize
{
	public class AnonymizerProcessor : AbstractBilingualContentProcessor
	{
		private readonly BatchAnonymizerSettings _settings;

		public AnonymizerProcessor(BatchAnonymizerSettings settings)
		{
			_settings = settings;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure)
			{
				return;
			}
			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var translationOrigin = segmentPair?.Properties?.TranslationOrigin;

				if (IsAutomatedTranslated(translationOrigin))
				{
					AnonymizeComplete(translationOrigin);
					var translationOriginBeforeAdaptation = segmentPair?.Properties?.TranslationOrigin?.OriginBeforeAdaptation;
					if (_settings.AnonymizeComplete)
					{
						AnonymizeComplete(translationOriginBeforeAdaptation);
					}
					else
					{
						AnonymizeTmMatch(translationOriginBeforeAdaptation);
					}
				}
			}
		}

		private bool IsAutomatedTranslated(ITranslationOrigin translationOrigin)
		{
			var originType = translationOrigin?.OriginType;

			return !string.IsNullOrEmpty(originType) &&
			       (originType.Equals(DefaultTranslationOrigin.MachineTranslation) ||
			        originType.Equals(DefaultTranslationOrigin.NeuralMachineTranslation) ||
			        originType.Equals(DefaultTranslationOrigin.AdaptiveMachineTranslation));
		}

		private void AnonymizeComplete(ITranslationOrigin translationOrigin)
		{
			translationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
			translationOrigin.OriginSystem = string.Empty;
			translationOrigin.MatchPercent = byte.Parse("0");
		}

		private void AnonymizeTmMatch(ITranslationOrigin translationOrigin)
		{
			translationOrigin.OriginType = DefaultTranslationOrigin.TranslationMemory;
			if (!string.IsNullOrEmpty(_settings.TmName))
			{
				translationOrigin.OriginSystem = Path.GetFileNameWithoutExtension(_settings.TmName);
			}
			var fuzzy = _settings.FuzzyScore.ToString(CultureInfo.InvariantCulture);
			translationOrigin.MatchPercent= byte.Parse(fuzzy);
		}
	}
}
