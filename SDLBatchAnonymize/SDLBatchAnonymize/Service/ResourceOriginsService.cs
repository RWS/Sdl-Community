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
			var originBeforeAdaptation = segmentPair?.Properties?.TranslationOrigin?.OriginBeforeAdaptation;

			if (translationOrigin != null && IsAutomatedTranslated(translationOrigin))
			{
				AnonymizeMtTranslationOrigin(segmentPair, anonymizerSettings, translationOrigin);
			}
			if (originBeforeAdaptation != null && IsAutomatedTranslated(originBeforeAdaptation))
			{
				AnonymizeMtTranslationOrigin(segmentPair, anonymizerSettings, originBeforeAdaptation);
			}
		}

		public void RemoveQe(ISegmentPair segmentPair)
		{
			var translationOrigin = segmentPair.Properties.TranslationOrigin;
			if (translationOrigin is null) return;
			if (translationOrigin.MetaDataContainsKey("quality_estimation"))
			{
				translationOrigin.RemoveMetaData("quality_estimation");
			}
			if (translationOrigin.MetaDataContainsKey("model"))
			{
				segmentPair.Properties.TranslationOrigin.RemoveMetaData("model");
			}
		}

		public void RemoveTm(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings)
		{
			var translationOrigin = segmentPair?.Properties?.TranslationOrigin;
			var originBefereAdaptation = segmentPair?.Properties?.TranslationOrigin.OriginBeforeAdaptation;

			if (translationOrigin != null && IsTmTransaltion(translationOrigin))
			{
				AnonymizeTmTransaltionOrigin(anonymizerSettings, translationOrigin);
			}
			if (originBefereAdaptation != null && IsTmTransaltion(originBefereAdaptation))
			{
				AnonymizeTmTransaltionOrigin(anonymizerSettings, originBefereAdaptation);
			}
		}

		private void AnonymizeMtTranslationOrigin(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings,
			ITranslationOrigin translationOrigin)
		{
			var anonymizeCustomResources = ShouldAnonymizeAtWithCustomResources(anonymizerSettings);
			AnonymizeToDefaultValues(translationOrigin); // we need to set the origin to be interactive in order to have edied fuzzy

			if (anonymizeCustomResources)
			{
				if (segmentPair.Properties?.TranslationOrigin.OriginBeforeAdaptation == null)
				{
					var originClone = (ITranslationOrigin)translationOrigin.Clone();
					originClone.OriginBeforeAdaptation = null;
					segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation = originClone;
				}
				AnonymizeTmMatch(segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation, anonymizerSettings, false);
			}
		}

		private void AnonymizeTmMatch(ITranslationOrigin translationOrigin, IBatchAnonymizerSettings anonymizerSettings, bool tmAnonymization)
		{
			translationOrigin.OriginType = DefaultTranslationOrigin.TranslationMemory;
			if (!string.IsNullOrEmpty(anonymizerSettings.TmName))
			{
				translationOrigin.OriginSystem = Path.GetFileNameWithoutExtension(anonymizerSettings.TmName);
			}
			if (anonymizerSettings.FuzzyScore > 0 && !tmAnonymization)
			{
				var fuzzy = anonymizerSettings.FuzzyScore.ToString(CultureInfo.InvariantCulture);
				translationOrigin.MatchPercent = byte.Parse(fuzzy);
			}
		}

		private void AnonymizeTmTransaltionOrigin(IBatchAnonymizerSettings anonymizerSettings, ITranslationOrigin translationOrigin)
		{
			var anonymizeWithCustomRes = ShouldAnonymizeTmWithCustomResources(anonymizerSettings);
			if (!anonymizeWithCustomRes)
			{
				AnonymizeToDefaultValues(translationOrigin);
			}
			else
			{
				AnonymizeTmMatch(translationOrigin, anonymizerSettings, true);
			}
		}

		private void AnonymizeToDefaultValues(ITranslationOrigin translationOrigin)
		{
			translationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
			translationOrigin.OriginSystem = string.Empty;
			translationOrigin.MatchPercent = byte.Parse("0");
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

		private bool ShouldAnonymizeAtWithCustomResources(IBatchAnonymizerSettings anonymizerSettings)
		{
			var isSpecificResourceChecked = anonymizerSettings.SetSpecificResChecked;
			var isFuzzyChanged = anonymizerSettings.FuzzyScore > 0; // default value is 0
			var hasTmAdded = !string.IsNullOrEmpty(anonymizerSettings.TmName);

			return isSpecificResourceChecked && (isFuzzyChanged || hasTmAdded);
		}

		private bool ShouldAnonymizeTmWithCustomResources(IBatchAnonymizerSettings anonymizerSettings)
		{
			var isSpecificResourceChecked = anonymizerSettings.SetSpecificResChecked;
			var hasTmAdded = !string.IsNullOrEmpty(anonymizerSettings.TmName);

			return isSpecificResourceChecked && hasTmAdded;
		}
	}
}