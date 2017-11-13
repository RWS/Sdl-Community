using Sdl.Community.XmlReader.WPF.Models;

namespace Sdl.Community.XmlReader.WPF.ViewModels
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
