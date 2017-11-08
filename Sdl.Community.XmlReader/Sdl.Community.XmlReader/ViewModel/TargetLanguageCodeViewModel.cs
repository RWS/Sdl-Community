using Sdl.Community.XmlReader.Data;
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

        public string TargetLanguageCode { get { return _targetLanguageCode.LanguageCode; } }

        protected override void LoadChildren()
        {
            foreach (AnalyzeFile analyzeFile in LocalDatabase.GetAnalyzeFilesByLanguageCode(_targetLanguageCode.LanguageCode))
            {
                base.Children.Add(new AnalyzeFileViewModel(analyzeFile, this));
            }
        }
    }
}
