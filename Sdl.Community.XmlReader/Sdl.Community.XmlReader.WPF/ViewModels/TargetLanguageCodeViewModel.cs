using Sdl.Community.XmlReader.WPF.Models;

namespace Sdl.Community.XmlReader.WPF.ViewModels
{
    public class TargetLanguageCodeViewModel : TreeViewItemViewModel
    {
        readonly TargetLanguageCode _targetLanguageCode;

	    public TargetLanguageCodeViewModel(TargetLanguageCode targetLanguageCode, string iconUri) : base(null, true)
        {
            _targetLanguageCode = targetLanguageCode;
            IconUri = iconUri;
        }

        public string TargetLanguageCode => _targetLanguageCode.LanguageCode;

	    public string IconUri { get; }

	    public void AddChild(AnalyzeFile analyzeFile)
        {
            Children.Add(new AnalyzeFileViewModel(analyzeFile, this));
        }
    }
}
