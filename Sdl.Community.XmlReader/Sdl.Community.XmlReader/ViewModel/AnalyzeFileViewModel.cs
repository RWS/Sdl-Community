using Sdl.Community.XmlReader.Model;

namespace Sdl.Community.XmlReader
{
    public class AnalyzeFileViewModel : TreeViewItemViewModel
    {
        readonly AnalyzeFile _analyzeFile;
        public AnalyzeFileViewModel(AnalyzeFile analyzeFile, TargetLanguageCodeViewModel parent) : base(parent, false)
        {
            _analyzeFile = analyzeFile;
        }

        public string AnalyzeFileName {  get { return _analyzeFile.Name; } }
        //public string AnalyzeFilePath {  get { return _analyzeFile.Path; } }
    }
}
