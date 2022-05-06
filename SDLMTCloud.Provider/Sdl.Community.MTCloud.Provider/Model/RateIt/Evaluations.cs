using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Model.RateIt
{
	public class Evaluations
	{
		public QualityEstimation CurrentSegmentEvaluation { get; set; }
		public Dictionary<SegmentId, QualityEstimation> EvaluationPerSegment { get; set; } = new();
	}
}