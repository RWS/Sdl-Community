namespace Sdl.Community.DsiViewer.Model
{
	public class SdlMtCloudFilterSettings : ModelBase
	{
		private bool _byModel;
		private bool _qePoor;
		private bool _qeNoneAvailable;
		private bool _qeGood;
		private bool _qeAdequate;
		private string _model;
		private bool _byQualityEstimation;

		public string Model
		{
			get => _model;
			set
			{
				_model = value;
				OnPropertyChanged();
			}
		}

		public bool QeAdequate
		{
			get => _qeAdequate;
			set 
			{
				_qeAdequate = value;
				OnPropertyChanged();
			}
		}

		public bool QeGood
		{
			get => _qeGood;
			set
			{
				_qeGood = value;
				OnPropertyChanged();
			}
		}

		public bool QeNoneAvailable
		{
			get => _qeNoneAvailable;
			set
			{
				_qeNoneAvailable = value;
				OnPropertyChanged();
			}
		}

		public bool QePoor
		{
			get => _qePoor;
			set
			{
				_qePoor = value;
				OnPropertyChanged();
			}
		}

		public bool ByModel
		{
			get => _byModel;
			set
			{
				_byModel = value;
				OnPropertyChanged();
			}
		}

		public bool ByQualityEstimation
		{
			get => _byQualityEstimation;
			set
			{
				_byQualityEstimation = value;
				OnPropertyChanged();
			}
		}

		public void ClearFilter()
		{
			ByQualityEstimation = false;
			ByModel = false;
		}
	}
}