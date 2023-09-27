namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryEntry : ViewModel.ViewModel
    {
        private string _sourceTerm;
        private string _targetTerm;

        public string SourceTerm
        {
            get => _sourceTerm;
            set => SetField(ref _sourceTerm, value);
        }

        public string TargetTerm
        {
            get => _targetTerm;
            set => SetField(ref _targetTerm, value);
        }

        public void CleanTerm()
        {
            SourceTerm = "";
            TargetTerm = "";
        }

        public bool IsDummyTerm() => SourceTerm == "new entry" && TargetTerm == "new entry";

        public bool IsEmpty() => string.IsNullOrWhiteSpace(SourceTerm) && string.IsNullOrWhiteSpace(TargetTerm);

        public bool IsInvalid() => string.IsNullOrWhiteSpace(SourceTerm) || string.IsNullOrWhiteSpace(TargetTerm);

        public override string ToString() => nameof(GlossaryEntry);
    }
}