using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace UpdateServerTm
{
	public class TextVisitor	: ISegmentElementVisitor
	{ 
		public void VisitText(Text text)
		{
			text.Value = "updated from API";
		}

		public void VisitTag(Tag tag)
		{
		}

		public void VisitDateTimeToken(DateTimeToken token)
		{
		}

		public void VisitNumberToken(NumberToken token)
		{
		}

		public void VisitMeasureToken(MeasureToken token)
		{
		}

		public void VisitSimpleToken(SimpleToken token)
		{
		}

		public void VisitTagToken(TagToken token)
		{
		}
	}
}
