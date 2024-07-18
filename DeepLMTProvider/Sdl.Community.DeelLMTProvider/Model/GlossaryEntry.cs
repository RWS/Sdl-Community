using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryEntry : ViewModel.ViewModel, IDataErrorInfo
    {
        private readonly Dictionary<string, string> _validationErrors = new();
        private string _sourceTerm;
        private string _targetTerm;

        public string Error => _validationErrors.Count > 0 ? string.Join("\n", _validationErrors.Values) : null;

        public bool HasErrors => _validationErrors.Count > 0;

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
                _validationErrors.TryGetValue(columnName, out var error);
                return error;
            }
        }

        public void AddValidationError(string propertyName, string errorMessage)
        {
            _validationErrors[propertyName] = errorMessage;
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(Error));
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

        public bool Validate()
        {
            _validationErrors.Clear();
            var context = new ValidationContext(this, null, null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(this, context, results, true);

            foreach (var result in results)
            {
                foreach (var memberName in result.MemberNames)
                {
                    _validationErrors[memberName] = result.ErrorMessage;
                }
            }

            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(Error));

            return isValid;
        }
    }
}