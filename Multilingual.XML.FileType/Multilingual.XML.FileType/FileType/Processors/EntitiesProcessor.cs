using Multilingual.XML.FileType.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.XML.FileType.FileType.Processors
{
	public class EntitiesProcessor: AbstractBilingualContentProcessor
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly BilingualParser _parser;

		public EntitiesProcessor(SegmentBuilder segmentBuilder, BilingualParser parser)
		{
			_segmentBuilder = segmentBuilder;
			_parser = parser;
		}
	}
}
