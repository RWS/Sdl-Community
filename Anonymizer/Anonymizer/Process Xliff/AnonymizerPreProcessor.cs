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
		private readonly List<RegexPattern> _patterns;
		private readonly string _encryptionKey;
		public AnonymizerPreProcessor(List<RegexPattern> patterns,string encryptionKey)
		{
			_patterns = patterns;
			_encryptionKey = encryptionKey;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }
			
			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var segmentVisitor = new SegmentVisitor(_patterns,_encryptionKey);
				segmentVisitor.ReplaceText(segmentPair.Source,ItemFactory,PropertiesFactory);
				if (segmentPair.Target != null)
				{
					segmentVisitor.ReplaceText(segmentPair.Target, ItemFactory, PropertiesFactory);
				}
			}
		}

		
	}
}
