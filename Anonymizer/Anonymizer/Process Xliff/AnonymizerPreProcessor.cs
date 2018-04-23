using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Anonymizer.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class AnonymizerPreProcessor: AbstractBilingualContentProcessor
	{
		private List<RegexPattern> _patterns= new List<RegexPattern>();
		public AnonymizerPreProcessor(List<RegexPattern> patterns)
		{
			_patterns = patterns;
		}
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }
			
			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var segmentVisitor = new SegmentVisitor(_patterns);
				segmentVisitor.ReplaceText(segmentPair.Source,ItemFactory,PropertiesFactory);
				if (segmentPair.Target != null)
				{
					segmentVisitor.ReplaceText(segmentPair.Target, ItemFactory, PropertiesFactory);
				}
				//segmentPair.Source.Add(ItemFactory.CreatePlaceholderTag(PropertiesFactory.CreatePlaceholderTagProperties("asda")));
			}
		}

		
	}
}
