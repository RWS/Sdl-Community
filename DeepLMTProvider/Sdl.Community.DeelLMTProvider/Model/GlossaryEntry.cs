using Sdl.Community.DeepLMTProvider.Command;
using System.Windows.Input;

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

        public bool IsEmpty() => string.IsNullOrWhiteSpace(SourceTerm) && string.IsNullOrWhiteSpace(TargetTerm);
    }
}