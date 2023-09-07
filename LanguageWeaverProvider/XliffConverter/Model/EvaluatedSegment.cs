﻿using System;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.XliffConverter.Model
{
	public class EvaluatedSegment
	{
		public Segment Segment { get; set; }

		public string QualityEstimation { get; set; }

		public QualityEstimations Estimation => Enum.TryParse(QualityEstimation, out QualityEstimations qualityEstimation) ? qualityEstimation : QualityEstimations.None;
	}
}