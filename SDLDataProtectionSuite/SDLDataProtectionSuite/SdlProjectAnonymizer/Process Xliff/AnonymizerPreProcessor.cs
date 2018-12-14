using System.Collections.Generic;
using System.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff
{
	public class AnonymizerPreProcessor : AbstractBilingualContentProcessor
	{
		private readonly List<RegexPattern> _patterns;
		private readonly string _encryptionKey;
		private readonly bool _arePatternsEncrypted;

		public AnonymizerPreProcessor(List<RegexPattern> patterns, string encryptionKey, bool arePatternsEncrypted)
		{
			_arePatternsEncrypted = arePatternsEncrypted;
			_patterns = patterns;
			_encryptionKey = encryptionKey;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure)
			{
				return;
			}

			var segmentPairs = paragraphUnit.SegmentPairs.ToList();

			var segmentVisitor = new SegmentVisitor(ItemFactory, PropertiesFactory, _patterns, _encryptionKey, _arePatternsEncrypted);

			if (segmentPairs.Count == 0 && paragraphUnit.Source != null)
			{
				segmentVisitor.ReplaceText(paragraphUnit.Source);
			}
			else
			{
				foreach (var segmentPair in segmentPairs)
				{
					segmentVisitor.ReplaceText(segmentPair.Source);
					if (segmentPair.Target != null)
					{
						segmentVisitor.ReplaceText(segmentPair.Target);
					}
				}
			}
		}
	}
}