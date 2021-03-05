﻿namespace Sdl.Community.DsiViewer.Model
{
	public class TranslationOriginData : ModelBase
	{
		private string _colorCode;
		private string _model;
		private string _qualityEstimation;

		public string ColorCode
		{
			get => _colorCode;
			set
			{
				_colorCode = value;
				OnPropertyChanged(nameof(ColorCode));
			}
		}

		public string Model
		{
			get => _model;
			set
			{
				_model = value;
				OnPropertyChanged(nameof(Model));
			}
		}

		public string QualityEstimation
		{
			get => _qualityEstimation;
			set
			{
				_qualityEstimation = value;
				OnPropertyChanged(nameof(QualityEstimation));
			}
		}
	}
}