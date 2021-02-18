﻿namespace Sdl.Community.DsiViewer.Model
{
	public class SdlMtCloudFilterSettings : ModelBase
	{
		private bool _byModel;
		private bool _byQualityEstimation;
		private string _model;
		private bool _qeAdequate;
		private bool _qeGood;
		private bool _qeNoneAvailable;
		private bool _qePoor;

		public bool ByModel
		{
			get => _byModel;
			set
			{
				_byModel = value;
				OnPropertyChanged(nameof(ByModel));
			}
		}

		public bool ByQualityEstimation
		{
			get => _byQualityEstimation;
			set
			{
				_byQualityEstimation = value;
				OnPropertyChanged(nameof(ByQualityEstimation));
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

		public bool QeAdequate
		{
			get => _qeAdequate;
			set
			{
				_qeAdequate = value;
				OnPropertyChanged(nameof(QeAdequate));
			}
		}

		public bool QeGood
		{
			get => _qeGood;
			set
			{
				_qeGood = value;
				OnPropertyChanged(nameof(QeGood));
			}
		}

		public bool QeNoneAvailable
		{
			get => _qeNoneAvailable;
			set
			{
				_qeNoneAvailable = value;
				OnPropertyChanged(nameof(QeNoneAvailable));
			}
		}

		public bool QePoor
		{
			get => _qePoor;
			set
			{
				_qePoor = value;
				OnPropertyChanged(nameof(QePoor));
			}
		}

		public void ClearFilter()
		{
			ByQualityEstimation = false;
			Model = null;
			QeNoneAvailable = false;
			QePoor = false;
			QeGood = false;
			QeAdequate = false;
			ByModel = false;
		}
	}
}