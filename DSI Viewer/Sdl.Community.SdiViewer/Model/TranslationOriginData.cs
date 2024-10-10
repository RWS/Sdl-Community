namespace Sdl.Community.DsiViewer.Model
{
    public class TranslationOriginData : ModelBase
    {
        private string _description;
        private string _model;
        private string _qualityEstimation;
        private string _score;
        private string _system;
        public string AccessibilityModelLabel => $"Model: {Model}";

        public string AccessibilityQELabel => $"Quality Estimation: {QualityEstimation}";

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged(nameof(Model));
                OnPropertyChanged(nameof(AccessibilityModelLabel));
            }
        }

        public string QualityEstimation
        {
            get => _qualityEstimation;
            set
            {
                _qualityEstimation = value;
                OnPropertyChanged(nameof(QualityEstimation));
                OnPropertyChanged(nameof(AccessibilityQELabel));
            }
        }

        public string Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged();
            }
        }

        public string System
        {
            get => _system;
            set
            {
                _system = value;
                OnPropertyChanged();
            }
        }
    }
}