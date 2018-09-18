using System.Collections.Generic;
using System.Linq;
using Sdl.Community.projectAnonymizer.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.projectAnonymizer.Process_Xliff
{
	public class AnonymizerPreProcessor: AbstractBilingualContentProcessor
	{
		private readonly List<RegexPattern> _patterns;
		private readonly string _encryptionKey;
		private readonly bool _arePatternsEncrypted;
		public AnonymizerPreProcessor(List<RegexPattern> patterns,string encryptionKey, bool arePatternsEncrypted)
		{
			_arePatternsEncrypted = arePatternsEncrypted;
			_patterns = patterns;
			_encryptionKey = encryptionKey;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure) { return; }
			
			foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
			{
				var segmentVisitor = new SegmentVisitor(_patterns,_encryptionKey, _arePatternsEncrypted);
				segmentVisitor.ReplaceText(segmentPair.Source,ItemFactory,PropertiesFactory);
				if (segmentPair.Target != null)
				{
					segmentVisitor.ReplaceText(segmentPair.Target, ItemFactory, PropertiesFactory);
				}
			}
		}

		
	}
}
