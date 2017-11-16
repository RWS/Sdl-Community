using Sdl.Community.XmlReader.WPF.Models;

namespace Sdl.Community.XmlReader.WPF.ViewModels
{
    public class TargetLanguageCodeViewModel : TreeViewItemViewModel
    {
        readonly TargetLanguageCode _targetLanguageCode;
        readonly string _iconUri;

        public TargetLanguageCodeViewModel(TargetLanguageCode targetLanguageCode, string iconUri) : base(null, true)
        {
            _targetLanguageCode = targetLanguageCode;
            _iconUri = iconUri;
        }

        public string TargetLanguageCode
        {
            get { return _targetLanguageCode.LanguageCode; }
        }

        public string IconUri
        {
            get { return _iconUri; }
        }

        public void AddChild(AnalyzeFile analyzeFile)
        {
            Children.Add(new AnalyzeFileViewModel(analyzeFile, this));
        }
    }
}
