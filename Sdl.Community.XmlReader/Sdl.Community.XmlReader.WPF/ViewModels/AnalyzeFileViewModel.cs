using Sdl.Community.XmlReader.WPF.Models;

namespace Sdl.Community.XmlReader.WPF.ViewModels
{
    public class AnalyzeFileViewModel : TreeViewItemViewModel
    {
        readonly AnalyzeFile _analyzeFile;
        public AnalyzeFileViewModel(AnalyzeFile analyzeFile, TargetLanguageCodeViewModel parent) : base(parent, false)
        {
            _analyzeFile = analyzeFile;
        }

        public string AnalyzeFileName
        {
            get { return _analyzeFile.Name; }
        }
        public string AnalyzeFilePath
        {
            get { return _analyzeFile.Path; }
        }
    }
}
