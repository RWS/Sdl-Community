using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.SDLBatchAnonymize
{
	public class AnonymizerProcessor : AbstractBilingualContentProcessor
	{
		public static readonly Log Log = Log.Instance;
		private readonly IBatchAnonymizerSettings _settings;
		private readonly IUserNameService _usernameService;

		public AnonymizerProcessor(IBatchAnonymizerSettings settings, IUserNameService usernameService)
		{
			_settings = settings;
			_usernameService = usernameService;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure)
			{
				return;
			}
			try
			{
				foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
				{
					if (_settings.CreatedByChecked || _settings.ModifyByChecked)
					{
						_usernameService.AnonymizeCreatedByAndEdited(segmentPair, _settings);
					}
				}
			}
			catch (Exception exception)
			{
				Log.Logger.Error($"{exception.Message}\n {exception.StackTrace}");
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
