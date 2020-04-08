using System.Globalization;
using System.IO;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.SDLBatchAnonymize.Service
{
	public class ResourceOriginsService : IResourceOriginsService
	{
		public void RemoveMt(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings)
		{
			var translationOrigin = segmentPair?.Properties?.TranslationOrigin;
			var originBefereAdaptation = segmentPair?.Properties?.TranslationOrigin.OriginBeforeAdaptation;

			if (translationOrigin != null && IsAutomatedTranslated(translationOrigin))
			{
				AnonymizeTranslationOrigin(segmentPair, anonymizerSettings, translationOrigin);
			}
			if (originBefereAdaptation != null && IsAutomatedTranslated(originBefereAdaptation))
			{
				AnonymizeTranslationOrigin(segmentPair, anonymizerSettings, originBefereAdaptation);
			}
		}

		public void RemoveTm(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings)
		{
			var translationOrigin = segmentPair?.Properties?.TranslationOrigin;
			var originBefereAdaptation = segmentPair?.Properties?.TranslationOrigin.OriginBeforeAdaptation;

			if (translationOrigin != null && IsTmTransaltion(translationOrigin))
			{
				AnonymizeTranslationOrigin(segmentPair, anonymizerSettings, translationOrigin);
			}
			if (originBefereAdaptation != null && IsTmTransaltion(originBefereAdaptation))
			{
				AnonymizeTranslationOrigin(segmentPair, anonymizerSettings, originBefereAdaptation);
			}
		}

		private void AnonymizeTranslationOrigin(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings,
			ITranslationOrigin translationOrigin)
		{
			var anonymizeCustomResources = ShouldAnonymizeWithCustomResources(anonymizerSettings);
			AnonymizeToDefaultValues(translationOrigin); // we need to set the origin to be interactive in order to have edied fuzzy

			if (anonymizeCustomResources)
			{
				if (segmentPair.Properties?.TranslationOrigin.OriginBeforeAdaptation == null)
				{
					var originClone = (ITranslationOrigin) translationOrigin.Clone();
					originClone.OriginBeforeAdaptation = null;
					segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation = originClone;
				}
				AnonymizeTmMatch(segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation, anonymizerSettings);
			}
		}

		private bool ShouldAnonymizeWithCustomResources(IBatchAnonymizerSettings anonymizerSettings)
		{
			var isSpecificResourceChecked = anonymizerSettings.SetSpecificResChecked;
			var isFuzzyChanged = anonymizerSettings.FuzzyScore > 0; // default value is 0
			var hasTmAdded = !string.IsNullOrEmpty(anonymizerSettings.TmName);

			return isSpecificResourceChecked && isFuzzyChanged && hasTmAdded;
		}

		private bool IsAutomatedTranslated(ITranslationOrigin translationOrigin)
		{
			var originType = translationOrigin?.OriginType;

			return !string.IsNullOrEmpty(originType) &&
			       (originType.Equals(DefaultTranslationOrigin.MachineTranslation) ||
			        originType.Equals(DefaultTranslationOrigin.NeuralMachineTranslation) ||
			        originType.Equals(DefaultTranslationOrigin.AdaptiveMachineTranslation));
		}

		private bool IsTmTransaltion(ITranslationOrigin translationOrigin)
		{
			var originType = translationOrigin?.OriginType;
			return !string.IsNullOrEmpty(originType) && originType.Equals(DefaultTranslationOrigin.TranslationMemory);
		}

		private void AnonymizeToDefaultValues(ITranslationOrigin translationOrigin)
		{
			translationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
			translationOrigin.OriginSystem = string.Empty;
			translationOrigin.MatchPercent = byte.Parse("0");
			translationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;
		}

		private void AnonymizeTmMatch(ITranslationOrigin translationOrigin, IBatchAnonymizerSettings anonymizerSettings)
		{
			translationOrigin.OriginType = DefaultTranslationOrigin.TranslationMemory;
			if (!string.IsNullOrEmpty(anonymizerSettings.TmName))
			{
				translationOrigin.OriginSystem = Path.GetFileNameWithoutExtension(anonymizerSettings.TmName);
			}
			if (anonymizerSettings.FuzzyScore > 0)
			{
				var fuzzy = anonymizerSettings.FuzzyScore.ToString(CultureInfo.InvariantCulture);
				translationOrigin.MatchPercent = byte.Parse(fuzzy);
			}
		}
	}
}
