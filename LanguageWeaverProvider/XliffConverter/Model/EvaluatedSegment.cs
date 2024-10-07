using System;
using LanguageWeaverProvider.Model.Interface;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.XliffConverter.Model
{
	public class EvaluatedSegment
	{
		public Segment Translation { get; set; }

		public string QualityEstimation { get; set; }

		public QualityEstimations Estimation => Enum.TryParse(QualityEstimation, out QualityEstimations qualityEstimation) ? qualityEstimation : QualityEstimations.None;
	}
}