namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryItem : ViewModel.ViewModel
    {
        private string _name;
        private string _sourceLanguage;
        private string _targetLanguage;

        public GlossaryItem(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(Path);
            Delimiter = "\\t";

        }

        public string Path { get; set; }

        public string Delimiter { get; set; }

        public string GetDelimiter() => Delimiter.Length > 0 ? Delimiter : null;

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string SourceLanguage
        {
            get => _sourceLanguage;
            set => SetField(ref _sourceLanguage, value);
        }

        public string TargetLanguage
        {
            get => _targetLanguage;
            set => SetField(ref _targetLanguage, value);
        }
    }
}