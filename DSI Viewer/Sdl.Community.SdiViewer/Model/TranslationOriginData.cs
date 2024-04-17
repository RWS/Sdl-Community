namespace Sdl.Community.DsiViewer.Model
{
	public class TranslationOriginData : ModelBase
	{
		private string _model;
		private string _qualityEstimation;

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

		public string AccessibilityQELabel => $"Quality Estimation: {QualityEstimation}";

		public string AccessibilityModelLabel => $"Model: {Model}";
	}
}