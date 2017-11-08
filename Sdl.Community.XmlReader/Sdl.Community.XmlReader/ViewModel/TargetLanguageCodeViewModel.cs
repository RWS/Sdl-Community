using Sdl.Community.XmlReader.Model;

namespace Sdl.Community.XmlReader
{
    public class TargetLanguageCodeViewModel : TreeViewItemViewModel
    {
        readonly TargetLanguageCode _targetLanguageCode;

        public TargetLanguageCodeViewModel(TargetLanguageCode targetLanguageCode) : base(null, true)
        {
            _targetLanguageCode = targetLanguageCode;
        }

        public string TargetLanguageCode
        {
            get { return _targetLanguageCode.LanguageCode; }
        }

        public void AddChild(AnalyzeFile analyzeFile)
        {
            Children.Add(new AnalyzeFileViewModel(analyzeFile, this));
        }
    }
}
