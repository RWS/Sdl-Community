using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class AnonymizerPreProcessor: AbstractBilingualContentProcessor
	{
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }
			
			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				var segmentVisitor = new SegmentVisitor();
				segmentVisitor.ReplaceText(segmentPair.Source,ItemFactory);
				//segmentPair.Source.Add(ItemFactory.CreateText(PropertiesFactory.CreateTextProperties("Andrea1")));
			}
		}

		
	}
}
