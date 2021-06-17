﻿using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider
{
	//TODO PACH (06/04/2021): Confirm if this is still required/ remove if obsolete code
	
	class MtTranslationProviderElementVisitor : ISegmentElementVisitor
	{
		private MtTranslationOptions _options;
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
			set => _plainText = value;
		}

		public void Reset()
		{
			_plainText = "";
		}

		public MtTranslationProviderElementVisitor(MtTranslationOptions options)
		{
			_options = options;
		}

		#region ISegmentElementVisitor Members

		public void VisitDateTimeToken(Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken token)
		{
			_plainText += token.Text;
		}

		public void VisitMeasureToken(Sdl.LanguagePlatform.Core.Tokenization.MeasureToken token)
		{
			_plainText += token.Text;
		}

		public void VisitNumberToken(Sdl.LanguagePlatform.Core.Tokenization.NumberToken token)
		{
			_plainText += token.Text;
		}

		public void VisitSimpleToken(LanguagePlatform.Core.Tokenization.SimpleToken token)
		{
			_plainText += token.Text;
		}

		public void VisitTag(Tag tag)
		{
			//_plainText += tag.Duplicate(); this will send tag text to _visitor.PlainTextGT in Language Direction -- useful for debugging
			_plainText += tag.TextEquivalent;
		}

		public void VisitTagToken(LanguagePlatform.Core.Tokenization.TagToken token)
		{
			_plainText += token.Text;
		}

		public void VisitText(Text text)
		{
			_plainText += text;
		}
		#endregion
	}
}