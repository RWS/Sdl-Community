using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.FailSafeTask
{
	internal class ClearTarget : AbstractBilingualContentHandler
	{
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure) { return; }

			foreach (var pair in paragraphUnit.SegmentPairs)
			{
				pair.Target.Clear();
			}
		}
	}
}