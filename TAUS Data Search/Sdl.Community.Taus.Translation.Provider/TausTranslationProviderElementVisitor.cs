using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.Taus.Translation.Provider
{
    class TausTranslationProviderElementVisitor : ISegmentElementVisitor
    {
        private TausTranslationOptions _options;
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

        public void Reset()
        {
            _plainText = "";
        }

        public TausTranslationProviderElementVisitor(TausTranslationOptions options)
        {
            _options = options;
        }

        #region ISegmentElementVisitor Members

        public void VisitDateTimeToken(global::Sdl.LanguagePlatform.Core.Tokenization.DateTimeToken token)
        {            
            //_plainText += token.Text;
        }

        public void VisitMeasureToken(global::Sdl.LanguagePlatform.Core.Tokenization.MeasureToken token)
        {
            _plainText += token.Text;
        }

        public void VisitNumberToken(global::Sdl.LanguagePlatform.Core.Tokenization.NumberToken token)
        {
            _plainText += token.Text;
        }

        public void VisitSimpleToken(global::Sdl.LanguagePlatform.Core.Tokenization.SimpleToken token)
        {
            _plainText += token.Text;
        }

        public void VisitTag(Tag tag)
        {            
            _plainText += " ";
            //_plainText += tag.TextEquivalent;
        }

        public void VisitTagToken(global::Sdl.LanguagePlatform.Core.Tokenization.TagToken token)
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
