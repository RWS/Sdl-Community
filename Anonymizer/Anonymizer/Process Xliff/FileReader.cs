using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public delegate bool ValidateSegmentOverwriteDelegate(ISegment targetSegment);
	public class FileReader : AbstractBilingualContentProcessor
	{
		private readonly DataAnonymizer _dataAnonymizer;
		private readonly AnonymizerSettings _anonymizerSettings;
		private readonly string _filePath;

		public FileReader(DataAnonymizer dataAnonymizer,AnonymizerSettings anonymizerSettings,string filePath)
		{
			_dataAnonymizer = dataAnonymizer;
			_anonymizerSettings = anonymizerSettings;
			_filePath = filePath;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				//_dataAnonymizer.Process(segmentPair);
				var segmentVisitor = new SegmentVisitor();
				//segmentVisitor.ReplaceText(segmentPair.Target);
				//segmentPair.Target.Add(ItemFactory.CreateText(
				//	PropertiesFactory.CreateTextProperties("Andrea")));

				segmentPair.Target.Insert(0, ItemFactory.CreateText(
					PropertiesFactory.CreateTextProperties("Andrea")));
			}
			base.ProcessParagraphUnit(paragraphUnit);
		}

	}
}
