using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class AnonymizerPreProcessor: AbstractBilingualContentProcessor
	{
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }
			
			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var segmentVisitor = new SegmentVisitor();
				segmentVisitor.ReplaceText(segmentPair.Source,ItemFactory,PropertiesFactory);
				//segmentPair.Source.Add(ItemFactory.CreatePlaceholderTag(PropertiesFactory.CreatePlaceholderTagProperties("asda")));
			}
		}

		
	}
}
