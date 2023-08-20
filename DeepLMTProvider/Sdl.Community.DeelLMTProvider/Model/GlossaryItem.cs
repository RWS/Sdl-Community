namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryItem : ViewModel.ViewModel
    {
        private string _sourceLanguage;
        private string _targetLanguage;
        public string Path { get; set; }

        public string Name => System.IO.Path.GetFileName(Path);

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