using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryEntry : ViewModel.ViewModel, IDataErrorInfo
    {
        private string _sourceTerm;
        private string _targetTerm;

        public string Error => null;

        [Required(ErrorMessage = "Source term required.")]
        public string SourceTerm
        {
            get => _sourceTerm;
            set
            {
                SetField(ref _sourceTerm, value);

            }
        }

        [Required(ErrorMessage = "Target term required.")]
        public string TargetTerm
        {
            get => _targetTerm;
            set => SetField(ref _targetTerm, value);
        }

        public string this[string columnName]
        {
            get
            {
                var validationContext = new ValidationContext(this) { MemberName = columnName };
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateProperty(
                    GetType().GetProperty(columnName).GetValue(this),
                    validationContext,
                    results
                );

                return results.Count > 0 ? results.First().ErrorMessage : null;
            }
        }

        public void CleanTerm()
        {
            SourceTerm = "";
            TargetTerm = "";
        }

        public bool IsDummyTerm() => SourceTerm == "new entry" && TargetTerm == "new entry";

        public bool IsDuplicate(GlossaryEntry other)
        {
            return SourceTerm == other.SourceTerm && TargetTerm == other.TargetTerm;
        }

        public bool IsEmpty() => string.IsNullOrWhiteSpace(SourceTerm) && string.IsNullOrWhiteSpace(TargetTerm);

        public bool IsInvalid() => string.IsNullOrWhiteSpace(SourceTerm) || string.IsNullOrWhiteSpace(TargetTerm);

        public override string ToString() => nameof(GlossaryEntry);

        public void Trim()
        {
            SourceTerm = SourceTerm.Trim();
            TargetTerm = TargetTerm.Trim();
        }
    }
}