using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Community.DeepLMTProvider
{
	public class ElementVisitor : ISegmentElementVisitor
	{
		private readonly StringBuilder _textBuilder;
		private string _plainText;

		public string PlainText
		{
			get
			{
				if (_plainText == null)
				{
					_plainText = "";
				}
				return _plainText;
			}
			set
			{
				_plainText = value;
			}
		}


		public ElementVisitor()
		{
			_textBuilder = new StringBuilder();
		}

		public void VisitDateTimeToken(DateTimeToken token)
		{
			_plainText += token.Text;
		}

		public void VisitMeasureToken(MeasureToken token)
		{
			_plainText += token.Text;
		}

		public void VisitNumberToken(NumberToken token)
		{
			_plainText += token.Text;
		}

		public void VisitSimpleToken(SimpleToken token)
		{
			_plainText += token.Text;
		}

		public void VisitTag(Tag tag)
		{
			//_plainText += tag.Duplicate(); this will send tag text to _visitor.PlainTextGT in Language Direction -- useful for debugging
			var test = new Text(tag.TextEquivalent);
			VisitText(test);
			//_plainText += tag.TextEquivalent;

		}

		public void VisitTagToken(TagToken token)
		{
			_plainText += token.Text;

		}

		public void VisitText(Text text)
		{

			_plainText += text;
		}
		private void VisitChildren(ISegmentElementVisitor container)
		{
			//container.
			//if (container == null)
			//	return;

			//foreach (var item in container)
			//{
			//	item.AcceptVisitor(this);
			//}
		}
	}
}
