namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryItem : ViewModel.ViewModel
    {
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
            get;
            set => SetField(ref field, value);
        }

        public string SourceLanguage
        {
            get;
            set => SetField(ref field, value);
        }

        public string TargetLanguage
        {
            get;
            set => SetField(ref field, value);
        }
    }
}