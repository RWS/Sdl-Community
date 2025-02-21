using System.Collections.Generic;
using System.Linq;

namespace LanguageWeaverProvider.Model
{
	public class PairModel
	{
		public string Active { get; set; }
		
		public string Name { get; set; }
		
		public string DisplayName { get; set; }
		
		public string SourceLanguageId { get; set; }
		
		public string TargetLanguageId { get; set; }
		
		public string Model { get; set; }
		
		public string Type { get; set; }
		
		public bool AutoAdaptiveSupport { get; set; }
		
		public bool AutoAdaptiveStatus { get; set; }
		
		public bool AdaptationSupport { get; set; }

		public bool QeSupport { get; set; }
		
		public List<LinguisticOption> LinguisticOptions { get; set; }

		public PairModel Clone()
		{
			var clonedPair = MemberwiseClone() as PairModel;
			clonedPair.LinguisticOptions = LinguisticOptions?.Select(lo => lo.Clone())?.ToList();

			return clonedPair;
		}
	}
}