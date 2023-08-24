namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryItem : ViewModel.ViewModel
    {
        public GlossaryItem(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(Path);
        }

        private string _sourceLanguage;
        private string _targetLanguage;
        private string _name;
        public string Path { get; set; }

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